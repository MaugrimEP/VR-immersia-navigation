using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHierarchyControllerTPS : MonoBehaviour
{
    private Transform RootNode;
    public HumanController Character;
    public Vector3 OffsetFromCharacter;

    void Start()
    {
        RootNode = GameObject.Find("RootNode").transform;
    }

    void Update()
    {
        RootNode.transform.RotateAround(Character.transform.position, Vector3.up, (Character.transform.rotation * Quaternion.Inverse(RootNode.transform.rotation)).eulerAngles.y);
        RootNode.transform.position = Character.transform.position +  Character.transform.localRotation * OffsetFromCharacter;
    }
}
