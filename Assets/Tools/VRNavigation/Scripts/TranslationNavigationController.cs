using UnityEngine;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

using System;
using System.Collections;

public class TranslationNavigationController : MonoBehaviour
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

    /// <summary>
    /// Constraint movement to a fix Y plane. (Jittering with Character Controller without).
    /// </summary>
    public bool fixedHeight = true;

    /// <summary>
    /// Move only when input value are above the threshold. [0..1].
    /// </summary>

    public CharacterController character;
    public NavMeshAgent navMeshAgent;

    public GameObject objectToMove;

    public GameObject pivotPoint;

    public GameObject movingPlateform;

    public Vector3 Translation
    {
        get;
        set;
    }

    public float RotationYAxis
    {
        get;
        set;
    }

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

        if (rotateMode == RotateMode.Around)
            objectToMove.transform.RotateAround(pivotPoint.transform.position, Vector3.up, RotationYAxis);

        else if (rotateMode == RotateMode.Direct)
            objectToMove.transform.Rotate(Vector3.up, RotationYAxis);

        Vector3 translation = Translation;

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
    }
}

