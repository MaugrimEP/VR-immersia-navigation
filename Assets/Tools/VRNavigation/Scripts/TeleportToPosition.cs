using UnityEngine;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

using System.Collections;
using System.Collections.Generic;

public class TeleportToPosition : MonoBehaviour
{
    public Transform ReferenceObjectToTeleport;
    public Transform ReferenceNode;
    public Transform Character;

    public List<Transform> Positions = new List<Transform>();

    int currentPosition = 0;

    public KeyCode NextPositionKey = KeyCode.N;
    public KeyCode PreviousPositionKey = KeyCode.P;

    public uint NextPositionButton = 1;
    public uint PreviousPositionButton = 2;

    public bool UsePositionsRotation = true;

    /// <summary>
    /// Disable nav mesh agent vhen teleporting.
    /// </summary>
    NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
    }

    void Update()
    {
        if (VRTools.GetKeyDown(NextPositionKey) || VRTools.IsButtonToggled(NextPositionButton, true))
        {
            changeTransform();

            currentPosition++;
            if (currentPosition >= Positions.Count)
                currentPosition = 0;
        }
        if (VRTools.GetKeyDown(PreviousPositionKey) || VRTools.IsButtonToggled(PreviousPositionButton, true))
        {
            changeTransform();

            currentPosition--;
            if (currentPosition < 0)
                currentPosition = Positions.Count - 1;
        }
    }

    void changeTransform()
    {
        bool reactivateNavmeshAgent = false;
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.enabled = false;
            reactivateNavmeshAgent = true;
        }

        //Need rotation first to use it in position calculation
        if (UsePositionsRotation)
            SetNewRotation(Positions[currentPosition].localRotation);
        SetNewPosition(Positions[currentPosition].localPosition);


        if (reactivateNavmeshAgent)
            navMeshAgent.enabled = true;
    }

    public void SetNewPosition(Vector3 position)
    {
        Vector3 referencePosition = ReferenceObjectToTeleport.transform.localRotation * ReferenceNode.transform.localPosition;
        referencePosition.y = 0;
        ReferenceObjectToTeleport.transform.localPosition = position - referencePosition;

        Character.transform.localPosition = ReferenceObjectToTeleport.transform.localPosition;
    }

    public void SetNewRotation(Quaternion rotation)
    {
        ReferenceObjectToTeleport.transform.localRotation = rotation;
        Character.transform.localRotation = rotation;
    }
}
