using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtuoseElasticNavigation : MonoBehaviour
{
    public VirtuoseManager vm;
    public TranslationNavigationController nav;

    public GameObject objectDirectionToFollow;

    Vector3 attachedPosition;

    public float TranslationMultiplier = 0.1f;
    public float RotationMultiplier = 0.1f;
    public float RotationTreshold = 0.05f;

    public float MovementMultiplier = 0.1f;

    float[] referenceArticulars;

    public CharacterMotor characterMotor;
    float initialGravity;
    float initialMaxFallSpeed;
    float initialMaxGroundAcceleration;
    float intialMaxAirAcceleration;

    void Start()
    {
        //REMOVE LATER UGLY HACK
        //   JoystickNavigationController joy = FindObjectOfType<JoystickNavigationController>();
        //   joy.enabled = false;
        initialGravity = characterMotor.movement.gravity;
        initialMaxFallSpeed = characterMotor.movement.maxFallSpeed;
        initialMaxGroundAcceleration = characterMotor.movement.maxGroundAcceleration;
        intialMaxAirAcceleration = characterMotor.movement.maxAirAcceleration;
    }

    void Update()
    {
        if (vm.Arm.IsConnected)
        {
            if (vm.IsButtonDown() || VRTools.GetKeyDown(KeyCode.G))
            {
                attachedPosition = vm.Virtuose.Pose.position;
                referenceArticulars = vm.Virtuose.Articulars;

                characterMotor.movement.gravity = initialGravity * MovementMultiplier;
                characterMotor.movement.maxFallSpeed = initialGravity * MovementMultiplier;
                characterMotor.movement.maxGroundAcceleration = initialGravity * MovementMultiplier;
                characterMotor.movement.maxAirAcceleration = initialGravity * MovementMultiplier;
                characterMotor.grounded = false;
            }
            if (vm.IsButtonUp() || VRTools.GetKeyUp(KeyCode.G))
            {
                characterMotor.movement.gravity = initialGravity;
                characterMotor.movement.maxFallSpeed = initialMaxFallSpeed;
                characterMotor.movement.maxGroundAcceleration = initialMaxGroundAcceleration;
                characterMotor.movement.maxAirAcceleration = intialMaxAirAcceleration;
                characterMotor.grounded = true;
            }

            Vector3 difference = vm.Virtuose.Pose.position - attachedPosition;

            if (vm.IsButtonPressed())         
            {
                Vector3 translation = difference * Mathf.Exp(difference.magnitude) * TranslationMultiplier  * VRTools.GetDeltaTime();

                nav.Translation = objectDirectionToFollow.transform.rotation * translation;
                float rotationYaxis = vm.Virtuose.Joystick(referenceArticulars).x;
                nav.RotationYAxis =  (Mathf.Abs(rotationYaxis) > RotationTreshold) ?  RotationMultiplier * rotationYaxis * VRTools.GetDeltaTime() : 0;
                
                float[] speed = vm.Virtuose.Speed;
                for (int s = 0; s < speed.Length; s++)
                {
                    float speedMultiplier = 1 - Mathf.Log(difference.magnitude + 1);
                    speedMultiplier = Mathf.Clamp01(speedMultiplier);
                    speed[s] *= speedMultiplier;
                }
                vm.Virtuose.Speed = speed;
                vm.Virtuose.AddForce = new float[] { 0 , 0, -5 , 0, 0, 0};
            }
            else
            {
                vm.Virtuose.Speed = vm.Virtuose.Speed;
                vm.Virtuose.Pose = vm.Virtuose.Pose;
                nav.Translation = Vector3.zero;
                nav.RotationYAxis = 0;
            }
        }
    }
}
