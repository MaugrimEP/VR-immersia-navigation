using UnityEngine;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

using System;
using System.Collections;
using MiddleVR_Unity3D;

public class HumanController : MonoBehaviour
{
    [Header("OBJECTS")]
    public Transform cameraViewPoint;
    public CharacterController characterController;

    //Input
    public VirtuoseElasticNavigation InputController;
    private Vector2 input;

    [Header("PHYSICS")]
    public float speed = 5f;
    public float acceleration = 11f;
    public float turnSpeed = 5f;
    public float turnSpeedMin = 7f;
    public float turnSpeedMax = 20f;
    public float jumpVelocity = 10f;
    public float gravity = 10f;
    private Vector3 velocityXZ;
    private Vector3 velocity;
    private bool grounded = false;

    //Computed or so values
    [HideInInspector]
    public Vector3 MovingDirection;
    private Vector3 previousPosition;

    [Header("Falling")]
    public float airVelocity;
    public float TimeToFall;
    public bool Falling;
    private float timeBeforeFall;

    private void Start()
    {
        grounded = false;
        previousPosition = characterController.transform.position;
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            timeBeforeFall = TimeToFall;
            Falling = false;
        }
        else
        {
            Falling = timeBeforeFall <= 0;
            if (!Falling) timeBeforeFall -= VRTools.GetDeltaTime();
        }

        if (!Falling)
        {
            DoInput();
            CalculateGround();
            DoMove();
            DoGravity();
            DoJump();
            CalculateComputedValues();

            float YRotation = InputController.OrientedRotation().y;
            YRotation = YRotation * turnSpeed * VRTools.GetDeltaTime();
            if (InputController.vm.IsButtonPressed())
            {
                velocity.x = 0;
                velocity.z = 0;
                YRotation = 0f;
            }
            characterController.Move(velocity * VRTools.GetDeltaTime());
            characterController.transform.Rotate(Vector3.up * YRotation, Space.Self);
        }
        else
        {
            FallingControle();
        }
    }

    private void FallingControle()
    {
        if (InputController.vm.IsButtonPressed()) return;
        Vector3 Translation = InputController.GetTranslation();
        input.y = Translation.z;
        characterController.Move(Translation * airVelocity * VRTools.GetDeltaTime());

        float YRotation = InputController.OrientedRotation().y;
        YRotation = YRotation * turnSpeed * VRTools.GetDeltaTime();
        if (InputController.vm.IsButtonPressed())
        {
            velocity.x = 0;
            velocity.z = 0;
            YRotation = 0f;
        }
        characterController.transform.Rotate(Vector3.up * YRotation, Space.Self);
    }

    private void DoInput()
    {
        if (InputController.vm.IsButtonPressed()) input = Vector2.zero;
        else (input.x, input.y) = (0f, InputController.GetTranslation().z);

        input = Vector2.ClampMagnitude(input, 1);

        if (InputController.vm.IsButtonPressed()) input = Vector2.zero;
    }

    private void CalculateGround()
    {
        grounded = characterController.isGrounded;
    }

    private void DoMove()
    {
        float tS = velocity.magnitude / speed;
        turnSpeed = Mathf.Lerp(turnSpeedMax, turnSpeedMin, tS);

        velocityXZ = velocity;
        velocityXZ.y = 0f;

        velocityXZ = Vector3.Lerp(velocityXZ, characterController.transform.forward * input.y.Signe() * input.magnitude * speed, acceleration * VRTools.GetDeltaTime());
        velocity = new Vector3(velocityXZ.x, velocity.y, velocityXZ.z);
    }

    private void DoGravity()
    {
        if (grounded)
            velocity.y = -0.5f;
        else
            velocity.y -= gravity * VRTools.GetDeltaTime();
        velocity.y = Mathf.Clamp(velocity.y, -10f, 10f);
    }

    private void DoJump()
    {
        if (grounded && InputController.Jump() && !InputController.vm.IsButtonPressed())
            velocity.y = jumpVelocity;
    }

    private void CalculateComputedValues()
    {
        MovingDirection = previousPosition - characterController.transform.position;
        previousPosition = characterController.transform.position;
    }

    public float GetSpeed()
    {
        return input.y;
    }
}


