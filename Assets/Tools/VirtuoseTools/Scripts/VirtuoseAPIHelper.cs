using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Helper functions for Virtuose API that work with VRTools library.
/// Manage error and log when needed.
/// Documentation is copy pasta from Virtuose official documentation.
/// </summary>
public class VirtuoseAPIHelper
{
    /// <summary>
    /// Vector3(x, y, z) + Quaternion(x, y, z, w)
    /// </summary>
    const int POSE_COMPONENTS_NUMBER = 7;

    /// <summary>
    /// At the moment for Virtuose 6HF.
    /// </summary>
    const int AXES_NUMBER = 6;

    bool[] buttonsPressed = new bool[4];
    bool[] buttonsToggled = new bool[4];

    //Cached value.
    float[] pose = new float[POSE_COMPONENTS_NUMBER];
    float[] axes = new float[AXES_NUMBER];

    VirtuoseArm arm;


    /// <summary>
    /// Vector3(x, y, z) + Quaternion(x, y, z, w)
    /// </summary>
    public const int POSITIONS_COMPONENTS = 7;

    /// <summary>
    /// In newton.
    /// </summary>
    public const float MAX_FORCE = 70;
    public const float MAX_TORQUE = 3.1f;


    /// <summary>
    /// In kg.
    /// </summary>
    public const float MIN_MASS = 0.001f;
    public const float MAX_MASS = 5;

    /// <summary>
    /// In Kg / m
    /// </summary>
    public const float MIN_INERTIE = 0;
    public const float MAX_INERTIE = 1;

    /// <summary>
    /// In meters.
    /// </summary>
    public const float MAX_DISTANCE_PER_FRAME = 2f;

    public const float MAX_DOT_DIFFERENCE = 0.1f;

    IntPtr ptr;

    /// <summary>
    /// Y       
    /// |   Z   
    /// | /
    /// |/___X
    /// </summary>
    enum ArticularScaleOne
    {
        Z = 0,
        Neg_X,
        Y,
        Base,
        Turret,
        Arm,
        ForeArm_Pitch,
        ForeArm_Roll,
        Handle_Pitch,
        Handle_Roll
    }

    public enum InBoundHF6Axe
    {
        LEFT_AXE_1, // corresponds to the left bound of axis 1, 
        RIGHT_AXE_1, // corresponds to the right bound of axis 1,
        SUP_AXE_2, // corresponds to the upper bound of axis 2,
        INF_AXE_2, // corresponds to the lower bound of axis 2,
        SUP_AXE_3, // corresponds to the upper bound of axis 3,
        INF_AXE_3, // corresponds to the lower bound of axis 3,
        LEFT_AXE_4, // corresponds to the left bound of axis 4,
        RIGHT_AXE_4, // corresponds to the right bound of axis 4,
        SUP_AXE_5, // corresponds to the upper bound of axis 5,
        INF_AXE_5, // corresponds to the lower bound of axis 5,
        LEFT_AXE_6, // corresponds to the left bound of axis 6,
        RIGHT_AXE_6 // corresponds to the right bound of axis 6. 
    }

    public enum InBoundScale1Axe
    {
        RIGHT, // right side in Scale1.
        LEFT,  // left side in Scale1.
        BACK, // back side in Scale1.
        FORWARD, // front side in Scale1.
        BOTTOM, // Not used but still present (Scale1 only)
        UP, // Not used but still present (Scale1 only)
        LEFT_AXE_0, // corresponds to the left bound of axis 0, 
        RIGHT_AXE_0, // corresponds to the right bound of axis 0, 
        LEFT_AXE_1, // corresponds to the left bound of axis 1, 
        RIGHT_AXE_1, // corresponds to the right bound of axis 1,
        SUP_AXE_2, // corresponds to the upper bound of axis 2,
        INF_AXE_2, // corresponds to the lower bound of axis 2,
        SUP_AXE_3, // corresponds to the upper bound of axis 3,
        INF_AXE_3, // corresponds to the lower bound of axis 3,
        LEFT_AXE_4, // corresponds to the left bound of axis 4,
        RIGHT_AXE_4, // corresponds to the right bound of axis 4,
        SUP_AXE_5, // corresponds to the upper bound of axis 5,
        INF_AXE_5, // corresponds to the lower bound of axis 5,
        LEFT_AXE_6, // corresponds to the left bound of axis 6,
        RIGHT_AXE_6 // corresponds to the right bound of axis 6. 
    }

    enum ArticularHF6
    {
        Base,
        Turret,
        Arm,
        ForeArm_Pitch,
        ForeArm_Roll,
    }

    public enum DeviceType
    {
        DEVICE_VIRTUOSE_3D = 1,
        DEVICE_VIRTUOSE_3D_DESKTOP = 2,
        DEVICE_VIRTUOSE_6D = 3,
        DEVICE_VIRTUOSE_6D_DESKTOP = 4,
        DEVICE_VIRTUOSE_7D = 5,
        DEVICE_MAT6D = 6,
        DEVICE_MAT7D = 7,
        DEVICE_INCA_6D = 8,
        DEVICE_INCA_3D = 9,
        DEVICE_ORTHESE = 10,
        DEVICE_SCALE1 = 11,
        DEVICE_1AXE = 12,
        DEVICE_OTHER = 13
    }

    /// <param name="arm"></param>
    public VirtuoseAPIHelper(VirtuoseArm arm)
    {
        this.arm = arm;
    }

    /// <summary>
    ///  Virtuose library version.
    ///  Major and minor index of the software version.  
    /// </summary>
    public (int major, int minor) APIVersion
    {
        get
        {
            int major = 0, minor = 0;
            ExecLogOnError(
                VirtuoseAPI.virtAPIVersion, ref major, ref minor);

            return (major, minor);
        }
    }

    /// <summary>
    /// Version of the embedded software.
    /// Major and minor index of the controller.
    /// </summary>
    public (int major, int minor) ControllerVersion
    {
        get
        {
            int major = 0, minor = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetControlerVersion, ref major, ref minor);

            return (major, minor);
        }
    }


    /// <summary>
    /// Open a connection to the controller of the Virtuose.
    /// The host parameter corresponds to the URL (Uniform Ressource Locator) of the Virtuose controller to connect to.
    /// In the current version of the API, only one type of communication protocol is available, therefore the URL is always in the form: 
    /// "udpxdr://identification:port_number+interface" 
    /// udpxdr is the only protocol available in the current version of the API.
    /// identification should be replaced by the host name of the Virtuose controller if it can be resolved by a DNS,
    /// or else by its IP address in dotted form (e.g. "192.168.0.1").
    /// port_number should be replaced by the port number to be used by the API to connect to the Virtuose controller.
    /// The default value is 0, and in that case the API looks for a free port number starting from 3131.
    /// interface designates the physical interface to be used by the API(ignored in the case of udpxdr). 
    /// In case only identification is given, the URL is completed as follows: 
    /// ... "udpxdr://identification:0" 
    /// Note: the automatic completion is limited to udpxdr only.The initial prefix "url:" defined in the URL standard is supported but ignored.
    /// </summary>
    /// <param name="ip">ip#port (127.0.0.1#5125).</param>
    public bool Open(string ip)
    {
        arm.Ip = ip;
        arm.Context = VirtuoseAPI.virtOpen(arm.Ip);
        if (arm.Context.ToInt32() == 0)
            VRTools.LogError("[Error][VirtuoseAPIHelper] Connection error with the arm " + arm.Ip + ErrorMessage);

        else
        {
            VRTools.Log("[VirtuoseAPIHelper] Connection successful with the arm " + arm.Ip);
            arm.IsConnected = true;
        }
        return arm.IsConnected;
    }

    /// <summary>
    ///  Closing of connection to the controller of the Virtuose.
    /// </summary>
    /// <returns></returns>
    public bool Close()
    {
        int errorCode = VirtuoseAPI.virtClose(arm.Context);
        if (errorCode == 0)
            VRTools.Log("[VirtuoseAPIHelper] Disconnection successful with the arm " + arm.Ip);
        else
            VRTools.LogError("[Error][VirtuoseAPIHelper] Disconnection error with arm " + arm.Ip + ErrorMessage);

        return errorCode == 0;
    }


    /// <summary>
    /// Initialize arm with default value.
    /// </summary>
    public void InitDefault()
    {
        IndexingMode = VirtuoseAPI.VirtIndexingType.INDEXING_ALL_FORCE_FEEDBACK_INHIBITION;
        GripperCommand = VirtuoseAPI.VirtGripperCommandType.GRIPPER_COMMAND_TYPE_POSITION;
        ForceFactor = 1f;
        SpeedFactor = 1;
        Timestep = 0.01f;
        BaseFrame = (Vector3.zero, Quaternion.identity);
        ObservationFrame = (Vector3.zero, Quaternion.identity);
        ObservationFrameSpeed = (Vector3.zero, Quaternion.identity);


        VirtuoseAPI.virtSaturateTorque(arm.Context, 35f, 3.3f);
    }

    /// <summary>
    /// Control mode of the Virtuose device.
    /// COMMAND_TYPE_NONE No possible movement,
    /// COMMAND_TYPE_IMPEDANCE Force/position control,
    /// COMMAND_TYPE_VIRTMECH Position/force control with virtual mechanism.
    /// </summary>
    public VirtuoseAPI.VirtCommandType CommandType
    {
        get
        {
            int commandType = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetCommandType, ref commandType);

            return (VirtuoseAPI.VirtCommandType) commandType;
        }
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetCommandType, (ushort) value);
        }
    }

    public VirtuoseAPI.VirtGripperCommandType GripperCommand
    {
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetGripperCommandType, (ushort) value);
        }
    }

    /// <summary>
    /// Mode of indexing (also called offset). 
    /// INDEXING_ALL Indexing is active for both translation and rotation movements, whenever the offset button is pushed or the power is off (power button or deadman sensor off). 
    /// INDEXING_TRANS Indexing is active only on the translation movements.When power is turned on, the device is constrained along a line segment going back to the orientation it had before switching off.
    /// INDEXING_NONE Indexing is inactive.When power is turned on, the device is constrained along a line segment going back to the position it had before switching off.
    /// Other values are implemented, which correspond to the same modes but forcefeedback is inhibited during indexing.
    /// </summary>
    public VirtuoseAPI.VirtIndexingType IndexingMode
    {
        get
        {
            ushort indexingMode = 0;
            ExecLogOnError(
                 VirtuoseAPI.virtGetIndexingMode, ref indexingMode);
            return IndexingMode;
        }
        set
        {
            ExecLogOnError(
                 VirtuoseAPI.virtSetIndexingMode, (ushort) value);
        }
    }

    /// <summary>
    /// Device type stored in its embedded variator card.
    /// </summary>
    public DeviceType DeviceID
    {
        get
        {
            int deviceId = 0;
            int serialNumber = 0;
            ExecLogOnError(
                 VirtuoseAPI.virtGetDeviceID, ref deviceId, ref serialNumber);
            return (DeviceType)deviceId;
        }
    }

    /// <summary>
    /// Check if device is a Scale1.
    /// </summary>
    public bool IsScaleOne
    {
        get
        {
            return DeviceID == VirtuoseAPIHelper.DeviceType.DEVICE_SCALE1;
        }
    }


    /// <summary>
    /// Serial number stored in its embedded variator card.
    /// </summary>
    public int SerialNumber
    {
        get
        {
            int deviceId = 0;
            int serialNumber = 0;
            ExecLogOnError(
                 VirtuoseAPI.virtGetDeviceID, ref deviceId, ref serialNumber);
            return serialNumber;
        }
    }

    /// <summary>
    /// State of the motor power supply
    /// </summary>
    public bool Power
    {
        get
        {
            int power = 0;
            ExecLogOnError(
                 VirtuoseAPI.virtGetPowerOn, ref power);
            return power == 1;
        }
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetPowerOn, value ? 1 : 0);
        }
    }

    /// <summary>
    /// State of the safety sensor.
    /// A value of 1 means that the safety sensor is active (user present), a value of 0 means that the sensor is inactive (user absent). 
    /// </summary>
    public bool DeadMan
    {
        get
        {
            int deadMan = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetDeadMan, ref deadMan);

            return deadMan == 1;
        }
    }

    /// <summary>
    /// State of the emergency stop.
    /// A value of 1 means that the chain is closed (the system is operational), a value of that it is open (the system is stopped). 
    /// </summary>
    public bool EmergencyStop
    {
        get
        {
            int emergencyStop = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetEmergencyStop, ref emergencyStop);

            return emergencyStop == 1;
        }
    }

    /// <summary>
    /// Encoder failure code.
    /// In case of success, the virtGetFailure function returns 0. 
    /// Otherwise, it returns 1 and the virtGetErrorCode function gives access to an error code. 
    /// </summary>
    public uint Failure
    {
        get
        {
            uint failure = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetFailure, ref failure);

            return failure;
        }
    }

    /// <summary>
    /// Current alarm status.
    /// VIRT_ALARM_OVERHEAT means that one motor is overheated. Forcefeedback is automatically reduced, until the motor has cooled down to an acceptable temperature.
    /// VIRT_ALARM_SATURATE means that the motor currents have reached their maximum and are saturated.
    /// VIRT_ALARM_CALLBACK_OVERRUN means that the execution time of the callback function defined with virtSetPeriodicFunction is greater than the timestep value. 
    /// In that case, the real-time execution of the simulation cannot be guaranteed. 
    /// </summary>
    public uint Alarm
    {
        get
        {
            uint alarm = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetAlarm, ref alarm);

            return alarm;
        }
    }

    /// <summary>
    /// Status of indexing.
    /// </summary>
    public bool IsInShiftPosition
    {
        get
        {
            //A value of 1 if the offset push-button is pressed or the power if off - a value of 0 otherwise. 
            int indexing = 0;
            ExecLogOnError(
                VirtuoseAPI.virtIsInShiftPosition, ref indexing);

            return indexing == 1;
        }
    }

    /// <summary>
    /// Force scale factor which corresponds to a scaling between the forces exerted at the tip of the VIRTUOSE and those computed in the simulation.
    /// A value smaller than 1 corresponds to an amplification of the forces from the Virtuose towards the simulation.
    /// The function must be called before the selection of the control mode. 
    /// </summary>
    public float ForceFactor
    {
        get
        {
            float forceFactor = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetForceFactor, ref forceFactor);

            return forceFactor;
        }
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetForceFactor, value);
        }
    }

    /// <summary>
    /// Movement scale factor which corresponds to a scaling of the workspace of the haptic device. 
    ///  A value larger than 1.0 means that the movements of the Virtuose are amplified inside the simulation. 
    /// </summary>
    public float SpeedFactor
    {
        get
        {
            float speedFactor = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetSpeedFactor, ref speedFactor);

            return speedFactor;
        }
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetSpeedFactor, value);
        }
    }

    /// <summary>
    /// Modify the current control speed.
    /// Function modifies the current value of the control speed. If an object is attached to the VIRTUOSE(virtAttachVO called before),
    /// then the control point is the center of the object, otherwise it is the center of the virtuose endeffector.
    /// Parameter corresponds to the speed with respect to the control point expressed in the coordinates of the environment reference frame.
    /// </summary>
    public float[] Speed
    {
        get
        {
            ExecLogOnError(
                VirtuoseAPI.virtGetSpeed, axes);
            return axes;
        }
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetSpeed, value);
        }
    }

    /// <summary>
    /// Simulation timestep.
    /// Virtuose controller of the simulation timestep. This value is used in order to guarantee the stability of the system.
    /// The function must be called before the selection of the type of control mode.
    /// Expressed in seconds. 
    /// </summary>
    public float Timestep
    {
        get
        {
            float timestep = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetTimeStep, ref timestep);

            return timestep;
        }
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetTimeStep, value);
        }
    }

    /// <summary>
    /// Timeout value used in communications with the Virtuose controller. 
    /// Expressed in seconds.
    /// </summary>
    public float Timeout
    {
        get
        {
            float timeout = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetTimeoutValue, ref timeout);
            return timeout;
        }
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetTimeoutValue, value);
        }
    }

    /// <summary>
    /// function disables (or enables) the watchdog control on the communication with the Virtuose controller. 
    /// The role of the watchdog control is to stop force-feedback in case of a software failure on the simulation side. 
    /// In practice, if the simulation does not update the control values during a time period of 2 seconds, 
    /// the Virtuose considers that the simulation is dead and cuts off the forcefeedback and the connection to the API. 
    /// For debugging purposes, it can be useful to disable the watchdog control, so that the simulation can be executed step-by-step without loosing the connection with the Virtuose.
    /// In that case, it is essential to call the virtClose function at the end of the simulation, 
    /// otherwise the haptic device will not be reset, and it could be impossible to establish a new connection.
    /// </summary>
    /// <param name="connexionState">state of the wanted watchdog control.</param>
    public bool ControlConnexion
    {
        set
        {
            //The disable parameter should be set to 1 to disable the watchdog control, and to 0 to re-enable it. 
            int disable = value ? 0 : 1;
            ExecLogOnError(
                VirtuoseAPI.virtDisableControlConnexion, disable);
        }
    }

    /// <summary>
    /// Number of axis of the Virtuose that can be controlled.
    /// </summary>
    public int AxesNumber
    {
        get
        {
            int nbAxes = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetNbAxes, ref nbAxes);
            return nbAxes;
        }
    }


    /// <summary>
    /// Test whether the mechanical limits of the device workspace have been reached.
    /// The bounds parameter is set by the function, as a field of bit with the following meaning:
    /// VIRT_BOUND_LEFT_AXE_1 corresponds to the left bound of axis 1, 
    /// VIRT_BOUND_RIGHT_AXE_1 corresponds to the right bound of axis 1,
    /// VIRT_BOUND_SUP_AXE_2 corresponds to the upper bound of axis 2,
    /// VIRT_BOUND_INF_AXE_2 corresponds to the lower bound of axis 2,
    /// VIRT_BOUND_SUP_AXE_3 corresponds to the upper bound of axis 3,
    /// VIRT_BOUND_INF_AXE_3 corresponds to the lower bound of axis 3,
    /// VIRT_BOUND_LEFT_AXE_4 corresponds to the left bound of axis 4,
    /// VIRT_BOUND_RIGHT_AXE_4 corresponds to the right bound of axis 4,
    /// VIRT_BOUND_SUP_AXE_5 corresponds to the upper bound of axis 5,
    /// VIRT_BOUND_INF_AXE_5 corresponds to the lower bound of axis 5,
    /// VIRT_BOUND_LEFT_AXE_6 corresponds to the left bound of axis 6,
    /// VIRT_BOUND_RIGHT_AXE_6 corresponds to the right bound of axis 6. 
    /// </summary>
    public uint IsInBound
    {
        get
        {
            uint bounds = 0;
            ExecLogOnError(
                VirtuoseAPI.virtIsInBounds, ref bounds);
            return bounds;
        }
    }

    /// <summary>
    /// Is the bit in given position set to 1.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool IsAxeInBound(int index)
    {
        uint bounds = IsInBound;
        uint axeBit = (uint) 1 << index;
        uint mask = bounds & axeBit;
        return mask != 0;
    }

    /// <summary>
    /// Time elapsed since the last update of the Virtuose state vector.
    /// Value in CPU ticks of the time elapsed since the last update of the state vector received from the Virtuose controller. 
    /// </summary>
    public float TimeLastUpdate
    {
        get
        {
            uint time = 0;
            ExecLogOnError(
               VirtuoseAPI.virtGetTimeLastUpdate, ref time);
            return time;
        }
    }

    /// <summary>
    /// Force tensor to be applied to the object attached to the Virtuose, allowing the dynamic simulation of the scene.
    /// It corresponds to the force applied by the user to the Virtuose, in the form of a 6 component force tensor.
    /// This force is expressed with respect to the center of the object, in the coordinates of the environment reference frame
    /// </summary>
    public float[] Force
    {
        get
        {
            ExecLogOnError(
                VirtuoseAPI.virtGetForce, axes);
            return axes;
        }
        set
        {
            Assert.AreEqual(value.Length, 6);

            for (int f = 0; f < 3; f++)
            {
                if (Mathf.Abs(value[f]) > MAX_FORCE)
                    VRTools.LogError("[Error][VirtuoseAPIHelper] Force clamped because it's outside of the defined limit |" + value[f] + "| > " + MAX_FORCE);

                value[f] = Mathf.Clamp(value[f], -MAX_FORCE, MAX_FORCE);
            }

            for (int f = 3; f < value.Length; f++)
            {
                if (Mathf.Abs(value[f]) > MAX_TORQUE)
                    VRTools.LogError("[Error][VirtuoseAPIHelper] Torque clamped because it's outside of limit |" + value[f] + "| > " + MAX_TORQUE);

                value[f] = Mathf.Clamp(value[f], -MAX_TORQUE, MAX_TORQUE);
            }


                ExecLogOnError(
                VirtuoseAPI.virtSetForce, value);
        }
    }

    /// <summary>
    /// Force tensor to be applied to the object attached to the Virtuose, allowing the dynamic simulation of the scene.
    /// It corresponds to the force applied by the user to the Virtuose, in the form of a 6 component force tensor.
    /// This force is expressed with respect to the center of the object, in the coordinates of the environment reference frame.
    /// This force can be applied regardless of the control mode used.
    /// </summary>
    public float[] AddForce
    {
        set
        {
            Assert.AreEqual(value.Length, 6);

            for (int f = 0; f < (value.Length / 2) - 1; f++)
            {
                if (Mathf.Abs(value[f]) > MAX_FORCE)
                    VRTools.LogError("[Error][VirtuoseAPIHelper] Force clamped because outside of limit |" + value[f] + "| > " + MAX_FORCE);

                value[f] = Mathf.Clamp(value[f], -MAX_FORCE, MAX_FORCE);
            }

            for (int f = (value.Length / 2); f < value.Length; f++)
            {
                if (Mathf.Abs(value[f]) > MAX_TORQUE)
                    VRTools.LogError("[Error][VirtuoseAPIHelper] Torque clamped because outside of limit |" + value[f] + "| > " + MAX_TORQUE);

                value[f] = Mathf.Clamp(value[f], -MAX_TORQUE, MAX_TORQUE);
            }

            ExecLogOnError(
                VirtuoseAPI.virtAddForce, value);
        }
    }

    /// <summary>
    /// Convert vecteur from Unity base to Virtuose base using to set force.
    /// </summary>
    public Vector3 UnitytoVirtuose(Vector3 value)
    {
        {
            ExecLogOnError(VirtuoseAPI.virtGetArticularPosition, pose);
            //ExecLogOnError(VirtuoseAPI.virtGetPosition, pose);
            //float a = pose[6];
            //float b = pose[3];
            //float c = pose[4];
            //float d = pose[5];

            //float M11 = a * a + b * b - c * c - d * d;
            //float M21 = 2 * a * d + 2 * b * c;
            //float M31 = 2 * b * d - 2 * a * c;
            //float M12 = 2 * b * c + 2 * a * d;
            //float M22 = a * a - b * b + c * c - d * d;
            //float M32 = 2 * a * b + 2 * c * d;
            //float M13 = 2 * a * c + 2 * b * d;
            //float M23 = 2 * c * d - 2 * a * b;
            //float M33 = a * a - b * b - c * c + d * d;

            //Vector3 vect = new Vector3(M11 * value[0] + M12 * value[1] + M13 * value[2], M21 * value[0] + M22 * value[1] + M23 * value[2], M31 * value[0] + M32 * value[1] + M33 * value[2]);
            Vector3 vect = new Vector3(-value[2] * Mathf.Cos(pose[0]) + value[0] * Mathf.Sin(pose[0]), value[0] * Mathf.Cos(pose[0]) + value[2] * Mathf.Sin(pose[0]), value[1]);
            return vect;
        }
    }

    /// <summary>
    /// Convert vecteur from wirst base to Virtuose base.
    /// </summary>
    public Vector3 VirtuosetoUnity(Vector3 value)
    {
        {
            ExecLogOnError(VirtuoseAPI.virtGetPosition, pose);
            float a = pose[6];
            float b = -pose[3];
            float c = -pose[4];
            float d = -pose[5];

            float M11 = a * a + b * b - c * c - d * d; //M11
            float M12 = 2 * a * d + 2 * b * c; //M21
            float M13 = 2 * b * d - 2 * a * c; //M31
            float M21 = 2 * b * c + 2 * a * d; //M12
            float M22 = a * a - b * b + c * c - d * d; //M22
            float M23 = 2 * a * b + 2 * c * d; //M32
            float M31 = 2 * a * c + 2 * b * d; //M13
            float M32 = 2 * c * d - 2 * a * b; //M23
            float M33 = a * a - b * b - c * c + d * d; //M33

            Vector3 vect = new Vector3(M11 * value[0] + M12 * value[1] + M13 * value[2], M21 * value[0] + M22 * value[1] + M23 * value[2], M31 * value[0] + M32 * value[1] + M33 * value[2]); //vecteur dans la base du virtuose
            //Vector3 vect = new Vector3(M21 * value[0] + M22 * value[1] + M23 * value[2], M31 * value[0] + M32 * value[1] + M33 * value[2], - M11 * value[0] + M12 * value[1] - M13 * value[2]);
            return vect;
        }
    }

    public void SetPeriodicFunction(VirtuoseCallbackFn callback, float period)
    {

        ptr = new IntPtr(0);
        ExecLogOnError(
            VirtuoseAPI.virtSetPeriodicFunction, callback, ref period, arm.Context);
    }

    public void StartLoop()
    {
        ExecLogOnError(
            VirtuoseAPI.virtStartLoop);
    }

    public void StopLoop()
    {
        ExecLogOnError(
            VirtuoseAPI.virtStopLoop);
    }

    /// <summary>
    /// Current value of the control position and sends it to the Virtuose controller.
    /// If an object is attached to the Virtuose (virtAttachVO called before),
    /// then the control point is the center of the object,
    /// otherwise it is the center of the Virtuose end-effector.
    /// </summary>
    public (Vector3 position, Quaternion rotation) Pose
    {
        get
        {
            ExecLogOnError(
                VirtuoseAPI.virtGetPosition, pose);
            return VirtuoseToUnityPose(pose);
        }
        set
        {
            pose = ConvertUnityToVirtuose(value.position, value.rotation);
            ExecLogOnError(
                VirtuoseAPI.virtSetPosition, pose);
        }
    }

    /// <summary>
    /// Indexed position of the end-effector.
    /// </summary>
    public (Vector3 position, Quaternion rotation) AvatarPose
    {
        get
        {
            ExecLogOnError(
                VirtuoseAPI.virtGetAvatarPosition, pose);
            return VirtuoseToUnityPose(pose);
        }
    }

    /// <summary>
    /// Physical position of the Virtuose with respect to its base.
    /// </summary>
    public (Vector3 position, Quaternion rotation) PhysicalPose
    {
        get
        {
            ExecLogOnError(
                VirtuoseAPI.virtGetPhysicalPosition, pose);
            return (VirtuosePhysicalToUnityPosition(pose), VirtuoseToUnityRotation(pose));
        }
    }


    /// <summary>
    /// Return articular value for each axe.
    /// For scale1 first three value are the position of the carrier : 0 -> Z, 1 -> -X, 2 -> Fixed Y value in meter.
    /// Then : For each axes the current angles in degree.
    /// </summary>
    public float[] Articulars
    {
        get
        {
            float[] articularValues = new float[SafeAxeNumber];
            ExecLogOnError(
                VirtuoseAPI.virtGetArticularPosition, articularValues);

            //Scale1 has translation on first 3 axes.
            int startRotationIndex = IsScaleOne ? 3 : 0;
            for (int a = 3; a < articularValues.Length; a++)
                articularValues[a] = Mathf.Rad2Deg * articularValues[a];
            return articularValues;
        }
        set
        {
            Assert.AreEqual(AxesNumber, value.Length, "Array length must have the same size as axes number.");

            for (int a = 3; a < value.Length; a++)
                value[a] = Mathf.Deg2Rad * value[a];

            ExecLogOnError(
                VirtuoseAPI.virtSetArticularPosition, value);
        }
    }

    /// <summary>
    /// Corresponds to the articular speed. It is an array of float, with length the number of axes of the Virtuose.
    /// </summary>
    public float[] ArticularsSpeed
    {
        get
        {
            float[] articularsSpeed = new float[SafeAxeNumber];
            ExecLogOnError(
                VirtuoseAPI.virtGetArticularSpeed, articularsSpeed);

            return articularsSpeed;
        }
        set
        {
            Assert.AreEqual(AxesNumber, value.Length, "Array length must have the same size as axes number.");
            ExecLogOnError(
                VirtuoseAPI.virtSetArticularSpeed, value);
        }
    }


    public float ArticularPositionOfAdditionalAxe
    {
        get
        {
            float gripperPosition = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetArticularPositionOfAdditionalAxe, ref gripperPosition);
            return gripperPosition;
        }
        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetArticularPositionOfAdditionalAxe, ref value);
        }
    }

    public float ArticularSpeedOfAdditionalAxe
    {
        get
        {
            float gripperSpeed = 0;
            ExecLogOnError(
                VirtuoseAPI.virtGetArticularSpeedOfAdditionalAxe, ref gripperSpeed);
            return gripperSpeed;
        }

        set
        {
            ExecLogOnError(
                VirtuoseAPI.virtSetArticularSpeedOfAdditionalAxe, ref value);
        }
    }

    /// <summary>
    /// Use this if you don't want to check if the AxeNumber is 0 because there is no connexion or other fancy fantasy.
    /// </summary>
    public int SafeAxeNumber
    {
        get
        {
            int axesNumber = AxesNumber;
            //Some haption function may crash if 0 size array is given as input.
            return axesNumber <= 0 ? POSE_COMPONENTS_NUMBER : axesNumber;
        }
    }

    public Vector2 Scale1CarrierPosition
    {
        get
        {
            float[] articulars = Articulars;
            return new Vector2(- articulars[1], articulars[0]);
        }
        set
        {
            if (Power)
            {
                float[] articulars = Articulars;
                articulars[0] = value[1];
                articulars[1] = - value[0];
                Articulars = articulars;
            }
            else
            {
                //Refresh position if no power.
                Articulars = Articulars;
            }
        }
    }

    public Vector3 LocalPhysicalPosition
    {
        get
        { 
            return PhysicalPose.position - ArticularsPosition; 
        }
    }

    /// <summary>
    /// Get base position which match power button.
    /// </summary>
    /// <param name="offset">Offset to match absolute tracking position.</param>
    /// <returns></returns>
    public (Vector3 position, Quaternion rotation) ComputeBasePose(Vector3 offset = new Vector3())
    {
        float[] articulars = Articulars;
        Vector3 position = VirtuoseAPIHelper.VirtuoseToUnityPosition(articulars);
        position.x = -position.x;
        position.z = -position.z;
        position += offset;
        Quaternion rotation = Quaternion.AngleAxis(- articulars[(int)ArticularScaleOne.Base], Vector3.up);
        return (position, rotation);
    }



    /// <summary>
    /// Get base position which match power button.
    /// </summary>
    /// <param name="offset">Offset to match absolute tracking position.</param>
    /// <returns></returns>
    public (Vector3 position, Quaternion rotation) ComputePhysicalPose(Vector3 offset = new Vector3())
    {
        (Vector3 position, Quaternion rotation) = PhysicalPose;
        position.x = -position.x;
        position.z = -position.z;
        position += offset;
        return (position, rotation);
    }

    /// <summary>
    /// Get base position which match power button.
    /// </summary>
    /// <param name="offset">Offset to match absolute tracking position.</param>
    /// <returns></returns>
    public (Vector3 position, Quaternion rotation) PhysicalBasePose
    {
        get
        {
            Vector3 position = ArticularsPosition;
            Quaternion rotation = Quaternion.AngleAxis(-Articulars[(int)ArticularScaleOne.Base], Vector3.up);
            return (position, rotation);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset">Offset to match absolute tracking position.</param>
    /// <param name="distance">Distance from the base. This distance should be read from the virtuose configuration file.</param>
    /// <returns></returns>
    public Vector3 BubblePosition(float distance = 0.60f)
    {
        (Vector3 position, Quaternion rotation) = PhysicalBasePose;
        return position + rotation * Vector3.forward * distance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offset">Offset to match absolute tracking position.</param>
    /// <param name="distance">Distance from the base. This distance should be read from the virtuose configuration file.</param>
    /// <returns></returns>
    public Vector3 ComputeBubblePosition(Vector3 offset = new Vector3(), float distance = 0.60f)
    {
        (Vector3 position, Quaternion rotation) = ComputeBasePose(offset);
        return position + rotation * Vector3.forward * distance;
    }


    /// <summary>
    /// Current position and orientation of the base reference frame.
    /// </summary>
    public (Vector3 position, Quaternion rotation) BaseFrame
    {
        get
        {
            ExecLogOnError(
               VirtuoseAPI.virtGetBaseFrame, pose);
            return VirtuoseToUnityPose(pose);
        }
        set
        {
            pose = ConvertUnityToVirtuose(value.Item1, value.Item2);
            ExecLogOnError(
               VirtuoseAPI.virtSetBaseFrame, pose);
        }
    }

    /// <summary>
    /// Observation frame with respect to the reference of environment.
    /// </summary>
    public (Vector3 position, Quaternion rotation) ObservationFrame
    {
        get
        {
            ExecLogOnError(
               VirtuoseAPI.virtGetObservationFrame, pose);
            return VirtuoseToUnityPose(pose);
        }
        set
        {
            pose = ConvertUnityToVirtuose(value.Item1, value.Item2);
            ExecLogOnError(
               VirtuoseAPI.virtSetObservationFrame, pose);
        }
    }

   

    /// <summary>
    /// Speed of the observation reference frame. 
    /// Speed of motion of the observation reference frame with respect to the environment reference frame. 
    /// </summary>
    public (Vector3 position, Quaternion rotation) ObservationFrameSpeed
    {
        set
        {
            pose = ConvertUnityToVirtuose(value.Item1, value.Item2);
            ExecLogOnError(
               VirtuoseAPI.virtSetObservationFrameSpeed, pose);
        }
    }

    /// <summary>
    /// Carries out the attachment, taking into account the mass and inertia of the object as well as the integration timestep of the simulation,
    /// in order to compute optimal values for the control parameters, guaranteeing the stability.
    /// The object is defined by the position of its center (reduction point of the force tensor),
    /// which has to be provided by using the virtSetPosition function before calling virtAttachVO. 
    /// Only accessible in control modes COMMAND_TYPE_VIRTMECH.
    /// </summary>
    /// <param name="mass">Corresponds to the mass of the object, expressed in kg.</param>
    /// <param name="inertie">Transform inertie into diagonal matrix.</param>
    public void AttachVO(float mass, float inertie)
    {
        if (inertie > MAX_INERTIE)
            VRTools.LogWarning("[Warning][VirtuoseAPIHelper] Inertie is above authorized threshold (" + inertie + ">" + MAX_INERTIE + ")");
        else if (inertie < 0)
            VRTools.LogWarning("[Warning][VirtuoseAPIHelper] Inertie must be >= 0.");

        inertie = Mathf.Clamp(inertie, MIN_INERTIE, MAX_INERTIE);

        float[] inerties = 
            {
                inertie, 0, 0,
                0, inertie, 0,
                0, 0, inertie
            };

        AttachVO(mass, inerties);
    }

    /// <summary>
    /// Carries out the attachment, taking into account the mass and inertia of the object as well as the integration timestep of the simulation,
    /// in order to compute optimal values for the control parameters, guaranteeing the stability.
    /// The object is defined by the position of its center (reduction point of the force tensor),
    /// which has to be provided by using the virtSetPosition function before calling virtAttachVO. 
    /// Only accessible in control modes COMMAND_TYPE_VIRTMECH. 
    /// </summary>
    /// <param name="mass">Corresponds to the mass of the object, expressed in kg.</param>
    /// <param name="inertie">Corresponds to the inertia matrix of the object, stored in lines as a vector with 9 values.</param>
    public void AttachVO(float mass, float[] inerties)
    {
        Assert.AreEqual(9, inerties.Length, "Array need to 9 components (Matrix 3x3).");

        Pose = (Vector3.zero, Quaternion.identity);
        Speed = new float[] { 0, 0, 0, 0, 0, 0 };

        if (mass > MAX_MASS)
            VRTools.LogWarning("[Warning][VirtuoseAPIHelper] Mass is aboved authorized threshold (" + mass + ">" + MAX_MASS + ")");
        else if (mass < 0)
            VRTools.LogWarning("[Warning][VirtuoseAPIHelper] Mass must be > 0.");

        mass = Mathf.Clamp(mass, MIN_MASS, MAX_MASS); //Use 1g as minimum, completely arbitrary.

        for(int i = 0; i < inerties.Length; i++)
        {
            if (inerties[i] > MAX_INERTIE)
                VRTools.LogWarning("[Warning][VirtuoseAPIHelper] Inertie is above authorized threshold (" + inerties[i] + "(" + i + ")>" + MAX_INERTIE + ")");
            else if (inerties[i] < 0)
                VRTools.LogWarning("[Warning][VirtuoseAPIHelper] Inertie must be >= 0.");

            inerties[i] = Mathf.Clamp(inerties[i], MIN_INERTIE, MAX_INERTIE);
        }
        ExecLogOnError(
            VirtuoseAPI.virtAttachVO, mass, inerties);
    }

    public void DetachVO()
    {
        ExecLogOnError(
            VirtuoseAPI.virtDetachVO);
    }

    public void UpdateArm()
    {       
        int buttonState = 0;
        for(int b = 0; b < 3; b++)
        {
            ExecLogOnError(
                VirtuoseAPI.virtGetButton, b, ref buttonState);

            buttonsToggled[b] = buttonsPressed[b] != GetButtonState(buttonState);
            buttonsPressed[b] = GetButtonState(buttonState);
        }
    }

    public bool Button(int button)
    {
        int state = 0;
        ExecLogOnError(
            VirtuoseAPI.virtGetButton, button, ref state);
        return state == 1;
    }

    public bool IsButtonPressed(int button = 2)
    {
        return buttonsPressed[button];
    }

    public bool IsButtonToggled(int button = 2)
    {
        return buttonsToggled[button];
    }

    public Vector2 Joystick(float[] referencearticulars, bool clamped = false)
    {
        float[] articulars = Articulars;
        float x = (referencearticulars[(int)ArticularScaleOne.Handle_Roll] - articulars[(int)ArticularScaleOne.Handle_Roll]) / 80;
        float y = (referencearticulars[(int)ArticularScaleOne.Handle_Pitch] - articulars[(int)ArticularScaleOne.Handle_Pitch]) / - 70;
        if (clamped)
        { 
            x = Mathf.Clamp(x, -1, 1);
            y = Mathf.Clamp(y, -1, 1);
        }
        return new Vector2(x, y);
    }



    /// <summary>
    /// 20/21 Ref.haptic devices - installation_manual_en_rev4.docx
    /// ID Definition State
    ///00 system OK Normal
    ///IP Reset factory IP setting(push on the reset
    ///button when the haptic device boot) Normal
    ///11 error DSP axe 1 System failed
    ///12 error DSP axe 2 System failed
    ///13 error DSP axe 3 System failed
    ///14 error DSP axe 4 System failed
    ///15 error DSP axe 5 System failed
    ///16 error DSP axe 6 System failed
    ///21 error coder axe 1 System failed
    ///22 error coder axe 2 System failed
    ///23 error coder axe 3 System failed
    ///24 error coder axe 4 System failed
    ///25 error coder axe 5 System failed
    ///26 error coder axe 6 System failed
    ///31 error read signal axe 1 System failed
    ///32 error read signal axe 2 System failed
    ///33 error read signal axe 3 System failed
    ///34 error read signal axe 4 System failed
    ///35 error read signal axe 5 System failed
    ///36 error read signal axe 6 System failed
    ///41 alarm T° PWM axe 1 Non-Fatal
    ///42 alarm T° PWM axe 2 Non-Fatal
    ///43 alarm T° PWM axe 3 Non-Fatal
    ///44 alarm T° PWM axe 4 Non-Fatal
    ///45 alarm T° PWM axe 5 Non-Fatal
    ///46 alarm T° PWM axe 6 Non-Fatal
    ///51 error PWM axe 1 System failed
    ///52 error PWM axe 2 System failed
    ///53 error PWM axe 3 System failed
    ///54 error PWM axe 4 System failed
    ///55 error PWM axe 5 System failed
    ///56 error PWM axe 6 System failed
    ///70 error relay System failed
    ///71 watchdog tram reception Non-Fatal
    /// </summary>
    public int ErrorCode
    {
        get
        {
            return VirtuoseAPI.virtGetErrorCode(arm.Context);
        }
    }

    public string ErrorMessage
    {
        get
        {
            int errorCode = VirtuoseAPI.virtGetErrorCode(arm.Context);
            return " (error " + errorCode + " : " + System.Runtime.InteropServices.Marshal.PtrToStringAnsi(VirtuoseAPI.virtGetErrorMessage(errorCode)) + ")";
        }
    }


    /// <summary>
    /// Unity
    /// Y       
    /// |   Z   
    /// | /
    /// |/___X
    /// 
    /// Virtuose
    /// Z       
    /// |   Y   
    /// | /
    /// |/___X
    /// </summary>
    /// <param name="positions"></param>
    /// <returns></returns>
    public static Vector3 VirtuoseToUnityPosition(float[] positions)
    {
        //Need to check the size of the array.
        if (positions.Length >= POSE_COMPONENTS_NUMBER)
            return new Vector3
                (
                    positions[1],
                    positions[2],
                   -positions[0]
                );

        VRTools.LogError("[Error][VirtuoseManager] Wrong array length for the pose: " + positions.Length + ".");
        return Vector3.zero;
    }

    public static Quaternion VirtuoseToUnityRotation(float[] pose)
    {
        if(pose.Length >= POSE_COMPONENTS_NUMBER)
            return new Quaternion(
               - pose[4], //x 3
               - pose[5], //y 4
                pose[3], //Z 5
                pose[6]);

        VRTools.LogError("[Error][VirtuoseManager] Wrong pose length: " + pose.Length + ".");
        return Quaternion.identity;
    }

    public static (Vector3, Quaternion) VirtuoseToUnityPose(float[] pose)
    {
        return (VirtuoseToUnityPosition(pose),  VirtuoseToUnityRotation(pose));
    }

    public static Vector3 VirtuosePhysicalToUnityPosition(float[] position)
    {
        //Need to check the size of the array.
        if (position.Length >= 3)
            return new Vector3
                (
                  - position[1],
                    position[2],
                    position[0]
                );

        VRTools.LogError("[Error][VirtuoseManager] Wrong array length for the articulars values: " + position.Length + ".");
        return Vector3.zero;
    }

    public Vector3 ArticularsPosition
    {
        get
        {
            return VirtuosePhysicalToUnityPosition(Articulars);
        }
    }

    /// <summary>
    /// [ x y z qx qy qz qw ] 
    /// /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static float[] ConvertUnityToVirtuose(Vector3 position, Quaternion rotation)
    {
        float[] positions = { 0, 0, 0,  0, 0, 0, 0 };
        positions[0] = - position.z;
        positions[1] = position.x;
        positions[2] = position.y;

        positions[3] = rotation.z;
        positions[4] = - rotation.x;
        positions[5] = - rotation.y;
        positions[6] = rotation.w;

        return positions;
    }

    /// <summary>
    /// Transform int button state into boolean button state.
    /// </summary>
    /// <param name="state">0: button released, 1: button pushed</param>
    /// <returns>True if pushed, False if released</returns>
    bool GetButtonState(int state)
    {
        return state == 1;
    }

    public delegate int virtDelegate(IntPtr context);
    public delegate int virtDelegateGen<T>(IntPtr context, T value);
    public delegate int virtDelegateRefGen<T>(IntPtr context, ref T value);
    public delegate int virtDelegateGenGen<T, U>(IntPtr context, T value1, U value2);
    public delegate int virtDelegateGenRefGen<T, U>(IntPtr context, T value1, ref U value2);
    public delegate int virtDelegateRefGenRefGen<T, U>(ref T value1, ref U value2);
    public delegate int virtDelegateContextRefGenRefGen<T, U>(IntPtr context, ref T value1, ref U value2);
    public delegate int virtDelegatePeriodFunction(IntPtr context, VirtuoseCallbackFn fn, ref float period, IntPtr arg);

    /// <summary>
    ///    errorCode = VirtuoseAPI.virtSetPosition(arm.Context, positions);
    ///    if (errorCode == -1)
    ///        VRTools.Log("[VirtuoseManager][Error] virtSetPosition error " + GetError());
    ///        =>
    ///     ExecLogOnError(
    ///         VirtuoseAPI.virtSetPosition, positions);
    /// </summary>
    /// <param name="virtMethod"></param>
    public void ExecLogOnError(virtDelegate virtMethod, IntPtr context)
    {
        int errorCode = virtMethod(context);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError(virtDelegate virtMethod)
    {
        int errorCode = virtMethod(arm.Context);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T>(virtDelegateGen<T> virtMethod, T value)
    {
        int errorCode = virtMethod(arm.Context, value);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T>(virtDelegateGen<T> virtMethod, IntPtr context, T value)
    {
        int errorCode = virtMethod(context, value);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T>(virtDelegateRefGen<T> virtMethod, ref T value)
    {
        int errorCode = virtMethod(arm.Context, ref value);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T>(virtDelegateRefGen<T> virtMethod, IntPtr context, ref T value)
    {
        int errorCode = virtMethod(context, ref value);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T, U>(virtDelegateGenGen<T, U> virtMethod, IntPtr context, T value1, U value2)
    {
        int errorCode = virtMethod(context, value1, value2);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T, U>(virtDelegateGenGen<T, U> virtMethod, T value1, U value2)
    {
        int errorCode = virtMethod(arm.Context, value1, value2);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T, U>(virtDelegateRefGenRefGen<T, U> virtMethod, ref T value1, ref U value2)
    {
        int errorCode = virtMethod(ref value1, ref value2);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T, U>(virtDelegateGenRefGen<T, U> virtMethod, T value1, ref U value2)
    {
        int errorCode = virtMethod(arm.Context, value1, ref value2);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T, U>(virtDelegateGenRefGen<T, U> virtMethod, IntPtr context, T value1, ref U value2)
    {
        int errorCode = virtMethod(context, value1, ref value2);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T, U>(virtDelegateContextRefGenRefGen<T, U> virtMethod, ref T value1, ref U value2)
    {
        int errorCode = virtMethod(arm.Context, ref value1, ref value2);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError<T, U>(virtDelegateContextRefGenRefGen<T, U> virtMethod, IntPtr context, ref T value1, ref U value2)
    {
        int errorCode = virtMethod(context, ref value1, ref value2);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError(virtDelegatePeriodFunction virtMethod, VirtuoseCallbackFn callback, ref float period, IntPtr arg)
    {
        int errorCode = virtMethod(arm.Context, callback, ref period, arg);
        LogError(errorCode, virtMethod.Method.Name);
    }

    public void ExecLogOnError(virtDelegatePeriodFunction virtMethod, IntPtr context, VirtuoseCallbackFn callback, ref float period, IntPtr arg)
    {
        int errorCode = virtMethod(context, callback, ref period, arg);
        LogError(errorCode, virtMethod.Method.Name);
    }


    public void LogError(int errorCode, string methodName)
    {
        if (errorCode == -1)
        {
            VRTools.LogError("[Error][VirtuoseManager] " + methodName + " error " + errorCode + "(" + ErrorMessage + ")");
            arm.HasError = true;
        }
    }
}

static class Extension
{
    public static string ToFlattenString(this Array array)
    {
        string flattenString = "[" + array.Length + "](";
        for (int i = 0; i < array.Length - 1; i++)
            flattenString += array.GetValue(i).ToString() + ", ";
        flattenString += array.GetValue(array.Length - 1) + "]";
        return flattenString;
    }

    public static int Next<T>(this ICollection<T> array, int currentIndex)
    {
        Assert.IsTrue(currentIndex >= 0);
        Assert.IsTrue(currentIndex < array.Count);
        return ++currentIndex % array.Count;
    }

    public static int Previous<T>(this ICollection<T> array, int currentIndex)
    {
        Assert.IsTrue(currentIndex >= 0);
        Assert.IsTrue(currentIndex < array.Count);
        return --currentIndex < 0 ? array.Count - 1 : currentIndex;
    }

    public static void LogErrorIfNull(this Component component)
    {
        if (!component)
            VRTools.LogError("[Error] Couldn't find component " + component + ".");
    }

    public static T GetComponentLogIfNull<T>(this Behaviour behaviour) where T : Component
    {
        T component = behaviour.GetComponent<T>();
        if (!component)
            VRTools.LogError("[Error] Couldn't find component " + typeof(T) + " in " + behaviour.name + " gameObject.");
        return component;
    }

    public static T GetComponentInChildrenLogIfNull<T>(this Behaviour behaviour) where T : Component
    {
        T component = behaviour.GetComponentInChildren<T>();
        if (!component)
            VRTools.LogError("[Error] Couldn't find component " + typeof(T) + " in " + behaviour.name + " children gameObject.");
        return component;
    }

    public static T GetComponentInParentLogIfNull<T>(this Behaviour behaviour) where T : Component
    {
        T component = behaviour.GetComponentInParent<T>();
        if (!component)
            VRTools.LogError("[Error] Couldn't find component " + typeof(T) + " in " + behaviour.name + " parent gameObject.");
        return component;
    }

    public static void SetPose(this Transform transform, (Vector3 position, Quaternion rotation) pose)
    {
        transform.position = pose.position;
        transform.rotation = pose.rotation;
    }

    public static (Vector3 position, Quaternion rotation) GetPose(this Transform transform)
    {
        return (transform.position, transform.rotation);
    }

    public static void SetLocalPose(this Transform transform, (Vector3 position, Quaternion rotation) pose)
    {
        transform.localPosition = pose.position;
        transform.localRotation = pose.rotation;
    }

    public static (Vector3 position, Quaternion rotation) GetLocalPose(this Transform transform)
    {
        return (transform.localPosition, transform.localRotation);
    }
}
