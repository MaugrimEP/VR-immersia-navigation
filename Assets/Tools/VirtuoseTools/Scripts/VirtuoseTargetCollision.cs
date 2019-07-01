using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtuoseTargetCollision : MonoBehaviour
{
    public enum HapticLoopMode
    {
        Update,
        FixUpdate,
        HighResolutionThread,
        HaptionPeriodicFunction
    }

    public VirtuoseManager vm;

    public GameObject Target;

    public bool AutoSimulation = false;
    public bool FixFps = false;
    public HapticLoopMode LoopMode;

    /// <summary>
    /// 0 mean no compensation.
    /// </summary>
    [Range(0, 30)]
    public float Compensation = 10;

    /// <summary>
    /// Carefull with high value as it's lead to instability.
    /// </summary>
    [Range(0, 1.2f)]
    public float SpeedMultiplier = 1;

    public float GripperPosition = 0;
    public float GripperSpeed = 0;

    Rigidbody TargetRigidbody;

    Vector3 LastFramePosition;
    Quaternion LastFrameRotation;

    public float[] forces = { 0, 0, 0, 0, 0, 0 };

    (Vector3 position, Quaternion rotation) VirtuoseCurrentPose = (Vector3.zero, Quaternion.identity);
    (Vector3 position, Quaternion rotation) virtuoseTargetCurrentPose = (Vector3.zero, Quaternion.identity);
    (Vector3 position, Quaternion rotation) TargetRigidbodyCurrentPose = (Vector3.zero, Quaternion.identity);

    private readonly object balanceLock = new object();

    Tools.Timer.HighResolutionTimer Timer;

    static VirtuoseTargetCollision instance;

    void Reset()
    {
        vm = GetComponent<VirtuoseManager>();
    }

    private void Start()
    {
        if (Target)
        {
            TargetRigidbody = Target.GetComponentInChildren<Rigidbody>();
            TargetRigidbody.LogErrorIfNull();

            Physics.autoSimulation = AutoSimulation;
            if (FixFps)
            {
                Application.targetFrameRate = (int)(1f / vm.Timestep);
                VRTools.Log("[VirtuoseTargetCollision] Target framerate " + (1f / vm.Timestep));
            }

            StartCoroutine(vm.WaitVirtuoseConnexion(Init));

            if (LoopMode == HapticLoopMode.HaptionPeriodicFunction)
                instance = this;
        }
    }

    private void Init()
    {
        VirtuoseCurrentPose = vm.Virtuose.Pose;
        TargetRigidbody.position = VirtuoseCurrentPose.position;
        TargetRigidbody.rotation = VirtuoseCurrentPose.rotation;

        LastFramePosition = VirtuoseCurrentPose.position;
        LastFrameRotation = VirtuoseCurrentPose.rotation;

        if (LoopMode == HapticLoopMode.HighResolutionThread)
        {
            Timer = new Tools.Timer.HighResolutionTimer(vm.Timestep * 1000f);
           // Timer.UseHighPriorityThread = true;
            Timer.Elapsed += new EventHandler<Tools.Timer.TimerElapsedEventArgs>(OnTimedEvent);
            Timer.Start();
        }

        //Haptic loop do not seem to work properly with Unity. Got an null error after a while which crash the thread.
        //UnityEngine.UnhandledExceptionHandler:< RegisterUECatcher > m__0(Object, UnhandledExceptionEventArgs)
        else if (LoopMode == HapticLoopMode.HaptionPeriodicFunction)
        {
            vm.Virtuose.SetPeriodicFunction(PeriodicFunction, vm.Timestep);
            vm.Virtuose.StartLoop();
        }
    }

    private void OnTimedEvent(object sender,
         Tools.Timer.TimerElapsedEventArgs timerEventArgs)
    {
        PeriodicFunction(timerEventArgs.Delay);
    }

    void Update()
    {
        //if (vm.Arm.IsConnected && 
        //    vm.CommandType == VirtuoseAPI.VirtCommandType.COMMAND_TYPE_IMPEDANCE)
        //    SetForce();

        lock (balanceLock)
        {
            TargetRigidbodyCurrentPose.position = TargetRigidbody.position;
            TargetRigidbodyCurrentPose.rotation = TargetRigidbody.rotation;

            virtuoseTargetCurrentPose.position = Target.transform.position;
            virtuoseTargetCurrentPose.rotation = Target.transform.rotation;
        }

        if(!AutoSimulation)
            Physics.Simulate(VRTools.GetDeltaTime());

        if (LoopMode == HapticLoopMode.Update)
            PeriodicFunction(VRTools.GetDeltaTime());
    }

    void FixedUpdate()
    {
        MovePhysic();

        if (LoopMode == HapticLoopMode.FixUpdate)
            PeriodicFunction(VRTools.GetDeltaTime());
    }

    void OnApplicationQuit()
    {
        if (LoopMode == HapticLoopMode.HighResolutionThread)
        {
            Debug.Log("StopTimer"  + Timer.Running);
            if(Timer.Running)
                Timer.Stop();
            Debug.Log("StopTimer<" + Timer.Running);

        }
        else if (LoopMode == HapticLoopMode.HaptionPeriodicFunction)
            vm.Virtuose.StopLoop();
    }

    void MovePhysic()
    {
        lock (balanceLock)
        {
            TargetRigidbody.MovePosition(VirtuoseCurrentPose.position);
            TargetRigidbody.MoveRotation(VirtuoseCurrentPose.rotation);
        }
    }

    /// <summary>
    /// Haption intern Loop system.
    /// </summary>
    /// <param name="param0"></param>
    /// <param name="param1"></param>
    void PeriodicFunction(System.IntPtr param0, System.IntPtr param1)
    {
        PeriodicFunction(0);
    }

    void PeriodicFunction(double delay)
    {
   
        lock (balanceLock)
        {
            //if (this == null)
            //{
            //    Debug.Log("NULL");
            //}
            //else
            //{
            //    Debug.Log("P");
            //}

            if (vm.Initialized && Target != null)
            {
                VirtuoseCurrentPose = vm.Virtuose.Pose;

                float distance = 0;
                float dot = 0;

                Vector3 normal = virtuoseTargetCurrentPose.position - TargetRigidbodyCurrentPose.position;
                //When there is a collision the rigidbody position is at the virtuose arm position but the transform.position is impacted by the scene physic.
                Vector3 newPosition = TargetRigidbodyCurrentPose.position + Compensation * normal;
                Quaternion newRotation = TargetRigidbodyCurrentPose.rotation;

                distance = Vector3.Distance(VirtuoseCurrentPose.position, newPosition);
                dot = Quaternion.Dot(VirtuoseCurrentPose.rotation, newRotation);

                //Add extra protection to avoid high velocity movement.
                if (distance > VirtuoseAPIHelper.MAX_DISTANCE_PER_FRAME)
                {
                    VRTools.LogWarning("[Warning][VirtuoseTargetCollision] Haption arm new position is aboved the authorized threshold distance (" + distance + ">" + VirtuoseAPIHelper.MAX_DISTANCE_PER_FRAME + "). Power off.");
                    vm.Virtuose.Power = false;
                }

                if (dot < 1 - VirtuoseAPIHelper.MAX_DOT_DIFFERENCE)
                {
                    VRTools.LogWarning("[Warning][VirtuoseManager] Haption arm new rotation is aboved authorized the threshold dot (" + (1 - dot) + " : " + VirtuoseAPIHelper.MAX_DOT_DIFFERENCE + "). Power off.");
                    vm.Virtuose.Power = false;
                }

                vm.Virtuose.Pose = (newPosition, newRotation);
                float[] speed = vm.Virtuose.Speed;
                for (int s = 0; s < speed.Length; s++)
                {
                    speed[s] *= SpeedMultiplier;
                }
                vm.Virtuose.Speed = speed;

                //vm.Virtuose.ArticularPositionOfAdditionalAxe = gripperPosition;
                //vm.Virtuose.ArticularSpeedOfAdditionalAxe = gripperSpeed;
                forces = vm.Virtuose.Force;

                GripperPosition = vm.Virtuose.ArticularPositionOfAdditionalAxe;
                GripperSpeed = vm.Virtuose.ArticularSpeedOfAdditionalAxe;

                LastFramePosition = VirtuoseCurrentPose.position;
                LastFrameRotation = VirtuoseCurrentPose.rotation;
            }
        }
    }

    /// <summary>
    /// BAK
    /// </summary>

    void SetRigidbodyPositions()
    {
        if (Target != null)
        {
            (Vector3 position, Quaternion rotation) = vm.Virtuose.Pose;
            TargetRigidbody.MovePosition(position);
            TargetRigidbody.MoveRotation(rotation);

            float distance = 0;
            float dot = 0;

            Vector3 normal = Target.transform.position - TargetRigidbody.position;
            //When there is a collision the rigidbody position is at the virtuose arm position but the transform.position is impacted by the scene physic.
            Vector3 newPosition = /*infoCollision.IsCollided ? target.transform.position + compensation * normal : */ TargetRigidbody.position;
            Quaternion newRotation = /* infoCollision.IsCollided ? target.transform.rotation :*/ TargetRigidbody.rotation;

            distance = Vector3.Distance(position, newPosition);
            dot = Quaternion.Dot(rotation, newRotation);

            //Add extra protection to avoid high velocity movement.
            if (distance > VirtuoseAPIHelper.MAX_DISTANCE_PER_FRAME)
            {
                VRTools.LogWarning("[Warning][VirtuoseTargetCollision] Haption arm new position is aboved the authorized threshold distance (" + distance + ">" + VirtuoseAPIHelper.MAX_DISTANCE_PER_FRAME + "). Power off.");
                vm.Virtuose.Power = false;
            }

            if (dot < 1 - VirtuoseAPIHelper.MAX_DOT_DIFFERENCE)
            {
                VRTools.LogWarning("[Warning][VirtuoseManager] Haption arm new rotation is aboved authorized the threshold dot (" + (1 - dot) + " : " + VirtuoseAPIHelper.MAX_DOT_DIFFERENCE + "). Power off.");
                vm.Virtuose.Power = false;
            }

            vm.Virtuose.Pose = (newPosition, newRotation);
            float[] speed = vm.Virtuose.Speed;
            for(int s = 0; s < speed.Length; s++)   
                speed[s] *= SpeedMultiplier;

            vm.Virtuose.Speed = speed;
            forces = vm.Virtuose.Force;

            vm.Virtuose.ArticularPositionOfAdditionalAxe = GripperPosition;
            vm.Virtuose.ArticularSpeedOfAdditionalAxe = GripperSpeed;

            GripperPosition = vm.Virtuose.ArticularPositionOfAdditionalAxe;
            GripperSpeed = vm.Virtuose.ArticularSpeedOfAdditionalAxe;

            LastFramePosition = position;
            LastFrameRotation = rotation;
        }
    }

}
