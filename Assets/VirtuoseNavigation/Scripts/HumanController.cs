using UnityEngine;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

using System;
using System.Collections;
using MiddleVR_Unity3D;

public class HumanController : MonoBehaviour
{
    public Transform cameraTransform;


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

    //Computed or so values
    [HideInInspector]
    public Vector3 MovingDirection;
    private Vector3 previousPosition;

    [Header("Flying")]
    public float airVelocity;
    public Vector3 flyingRotationSpeed;

    public enum State { Default, Walking, Flying }
    [HideInInspector]
    public State state;

    private void Awake()
    {
        previousPosition = characterController.transform.position;
        state = State.Walking;
    }

    void Update()
    {
        if (InputController.vm.IsButtonPressed())
        {
            AudioManager.instance.PlayFadeNoReset("Wind1", 5f);

            FallingControle();
            state = State.Flying;
        }
        else
        {
            AudioManager.instance.StopFade("Wind1", 5f);

            state = State.Walking;
            ResetCharacterRotation();
            DoInput();
            DoMove();
            DoGravity();
            DoJump();
            CalculateComputedValues();

            float YRotation = InputController.OrientedRotation().y;
            YRotation = YRotation * turnSpeed * VRTools.GetDeltaTime();
            if (InputController.IsResetingAttached())
            {
                velocity.x = 0;
                velocity.z = 0;
                YRotation = 0f;
            }
            characterController.Move(velocity * VRTools.GetDeltaTime());
            characterController.transform.Rotate(Vector3.up * YRotation, Space.Self);
        }
    }

    private void ResetCharacterRotation()
    {
        characterController.transform.rotation = Quaternion.Euler(Vector3.up * characterController.transform.rotation.eulerAngles.y);
    }

    private void FallingControle()
    {
        if (InputController.IsResetingAttached()) return;
        Vector3 Translation = InputController.GetTranslation();
        Vector3 Rotation = InputController.OrientedRotation();
        input.y = Translation.z;

        characterController.Move(characterController.transform.TransformDirection(Translation) * airVelocity * VRTools.GetDeltaTime());

        characterController.transform.Rotate(cameraTransform.right, Rotation.x * flyingRotationSpeed.x * VRTools.GetDeltaTime(), Space.World);
        characterController.transform.Rotate(cameraTransform.up, Rotation.y * flyingRotationSpeed.y * VRTools.GetDeltaTime(), Space.World);
        characterController.transform.Rotate(Vector3.forward, Rotation.z * flyingRotationSpeed.z * VRTools.GetDeltaTime());

        Vector3 rotationCharacter = characterController.transform.localRotation.eulerAngles;
        Vector3 rotationCharacterClamped = new Vector3(
            Mathf.Clamp(rotationCharacter.x.OrientedAngle(), -45, +45),
            rotationCharacter.y,
            Mathf.Clamp(rotationCharacter.z.OrientedAngle(), -50, +50)
            );

        characterController.transform.localRotation = Quaternion.Euler(rotationCharacterClamped);
    }

    private void DoInput()
    {
        if (InputController.IsResetingAttached()) input = Vector2.zero;
        else (input.x, input.y) = (0f, InputController.GetTranslation().z);

        input = Vector2.ClampMagnitude(input, 1);

        if (InputController.IsResetingAttached()) input = Vector2.zero;
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
        if (characterController.isGrounded)
            velocity.y = -0.5f;
        else
            velocity.y -= gravity * VRTools.GetDeltaTime();
        velocity.y = Mathf.Clamp(velocity.y, -10f, 10f);
    }

    private void DoJump()
    {
        if (characterController.isGrounded && InputController.Jump() && !InputController.IsResetingAttached())
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


