using UnityEngine;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

using System.Collections;


/// <summary>
/// Apply the difference between the wanted translation and the real translation 
/// of the controller on object when controller is colliding.
/// It doesn't solve height problem where user go through roof.
/// Either head go through vertical object or feet lose ground. First one is prefered here.
/// Usefull to simulate collision in a none constraint environnement (eg. use in a CAVE).
/// </summary>
public class CollisionOffsetFromController : MonoBehaviour
{
    public CharacterController character;
    public NavMeshAgent navMeshAgent;

    public GameObject reference;
    public GameObject objectToMove;

    public GameObject movingPlatform;

    Vector3 previousReferenceLocalPosition;
    Vector3 previousReferencePosition;
    Vector3 previousControllerPosition;

    public CollisionMode collisionMode;

    public enum CollisionMode
    {
        None,
        CharacterMove,
        NavMeshMove
    }


    void Start()
    {
        Vector3 referenceRelative = movingPlatform == null ? reference.transform.position : movingPlatform.transform.InverseTransformPoint(reference.transform.position);
        character.transform.localPosition = new Vector3(referenceRelative.x, objectToMove.transform.localPosition.y, referenceRelative.z);// -Vector3.Scale(head.transform.localPosition, Vector3.zero);

        previousReferenceLocalPosition = reference.transform.localPosition;
        previousReferencePosition = reference.transform.position;
        previousControllerPosition = character.transform.position;
    }

    void Update()
    {
        if (collisionMode != CollisionMode.None)
        {
            Vector3 currentCharacterPosition = character.transform.position;

            Vector3 offsetReference = reference.transform.localPosition - previousReferenceLocalPosition;
            offsetReference.y = 0; //Ignore height because it mess up with character position on ground.

            if (collisionMode == CollisionMode.CharacterMove)
                character.Move(offsetReference);

            else if (collisionMode == CollisionMode.NavMeshMove)
                navMeshAgent.Move(offsetReference);

            //Apply difference from reference node to rootnode due to collision.
            Vector3 realOffsetTranslation = character.transform.position - currentCharacterPosition;
            objectToMove.transform.localPosition -= offsetReference - realOffsetTranslation;
/*
            Debug.Log("Real: " + realOffsetTranslation.ToString("F3") + " offref: " + offsetReference.ToString("F3") +
                " expected:" + expectedOffsetTranslation.ToString("F3"));*/

            //Vector3 offsetController = character.transform.position - previousControllerPosition;
            //Vector3 differenceHeadController = offsetReference - offsetController;
            //objectToMove.transform.localPosition -= differenceHeadController;
        }

        previousReferenceLocalPosition = reference.transform.localPosition;
        previousReferencePosition = reference.transform.position;
        previousControllerPosition = character.transform.position;
    }

}
