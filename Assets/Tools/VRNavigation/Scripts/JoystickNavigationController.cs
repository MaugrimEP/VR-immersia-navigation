using UnityEngine;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

using System;
using System.Collections;

/**
 * Generic joystick character controller using VRTools.
 * Navigation direction should follow the joystick tracker or the head tracker forward direction
 * depending in how is configured.
 */
public class JoystickNavigationController : MonoBehaviour
{
    public enum RotateMode
    {
        Direct,
        Around
    }

    public enum TranslateMode
    {
        Direct,
        CharacterMove,
        NavMeshMove
    }

    public RotateMode rotateMode;
    public TranslateMode translateMode;

    public double rotateSpeed = 50;
    public double translateSpeed = 1;

    /// <summary>
    /// Constraint movement to a fix Y plane. (Jittering with Character Controller without).
    /// </summary>
    public bool fixedHeight = true;

    /// <summary>
    /// Move only when input value are above the threshold. [0..1].
    /// </summary>
    public double inputThreshold = 0.2;

    public CharacterController character;
    public NavMeshAgent navMeshAgent;

    public GameObject objectToMove;

    public GameObject pivotPoint;

    public GameObject objectDirectionToFollow;

    public GameObject movingPlateform;

    public uint changeObjectToFollowIndex = 2;

    double x;
    double y;

    void Start()
    {
        if (character == null)
            character = GetComponentInChildren<CharacterController>();

        if (navMeshAgent == null)
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        character.transform.localPosition = objectToMove.transform.localPosition;
    }

    void Update()
    {
        character.transform.localRotation = objectToMove.transform.localRotation;

        float wandx = VRTools.GetWandHorizontalValue();
        float wandy = VRTools.GetWandVerticalValue();

        if (Math.Abs(wandx) < inputThreshold)
            wandx = 0;

        if (Math.Abs(wandy) < inputThreshold)
            wandy = 0;

        x += wandx;
        y += wandy;

        if (VRTools.GetKeyPressed(KeyCode.LeftArrow))
            x -= 1;
        if (VRTools.GetKeyPressed(KeyCode.RightArrow))
            x += 1;

        if (VRTools.GetKeyPressed(KeyCode.UpArrow))
            y += 1;
        if (VRTools.GetKeyPressed(KeyCode.DownArrow))
            y -= 1;

        if (rotateMode == RotateMode.Around)
            objectToMove.transform.RotateAround(pivotPoint.transform.position, Vector3.up, (float)(x * rotateSpeed * VRTools.GetDeltaTime()));

        else if(rotateMode == RotateMode.Direct)
            objectToMove.transform.Rotate(Vector3.up, (float)(x * rotateSpeed * VRTools.GetDeltaTime()));

        Vector3 translation = objectDirectionToFollow.transform.forward * VRTools.GetDeltaTime() * (float)(y * translateSpeed);

        if (translation.magnitude > 3)
            translation.Normalize();

        if (fixedHeight)
            translation.y = 0;

        Vector3 currentCharacterPosition = character.transform.position;
        if (translateMode == TranslateMode.Direct)
            character.transform.position += translation;

        else if (translateMode == TranslateMode.CharacterMove)
            character.Move(translation);

        else if (translateMode == TranslateMode.NavMeshMove)
            navMeshAgent.Move(translation);


        Vector3 realTranslation = character.transform.position - currentCharacterPosition;
        objectToMove.transform.localPosition += realTranslation;

        //Force correct position of the character to avoid drift
        if (movingPlateform == null)
        {
            character.transform.position = new Vector3(pivotPoint.transform.position.x, character.transform.position.y, pivotPoint.transform.position.z);
            objectToMove.transform.position = new Vector3(objectToMove.transform.position.x, character.transform.position.y, objectToMove.transform.position.z);
        }
        else
        {
            objectToMove.transform.localEulerAngles = new Vector3(0, objectToMove.transform.localEulerAngles.y, 0);
            Vector3 localHeadPosition = movingPlateform.transform.InverseTransformPoint(pivotPoint.transform.position);
            character.transform.localPosition = new Vector3(localHeadPosition.x, objectToMove.transform.localPosition.y, localHeadPosition.z);
        }

        x = 0;
        y = 0;
    }

    public void SetAxes(Vector2 axes)
    {
        x = axes.x;
        y = axes.y;
    }
    public void SetAxes(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
