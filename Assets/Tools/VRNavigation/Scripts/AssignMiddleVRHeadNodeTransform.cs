using UnityEngine;
using System.Collections;

/// <summary>
/// Force MiddleVR node position.
/// Should be call directly after VRManagerScript
/// </summary>
public class AssignMiddleVRHeadNodeTransform : MonoBehaviour
{
    public string MiddleVRHeadNodeName = "HeadNode";
    GameObject middleVRHeadNode;

    public string MiddleVRRootNode = "CenterNode";
    GameObject middleVRRootNode;

    public CharacterController characterController;
    public GameObject HeadNode;

    void Reset()
    {
        MiddleVRHeadNodeName = "HeadNode";
        MiddleVRRootNode = "CenterNode";
        characterController = FindObjectOfType<CharacterController>();
        HeadNode = gameObject;
    }

	void OnEnable()
    {
        middleVRHeadNode = GameObject.Find(MiddleVRHeadNodeName);
        middleVRRootNode = GameObject.Find(MiddleVRRootNode);

        if (middleVRRootNode == null)
            middleVRRootNode = GameObject.Find("VRSystemCenterNode");

        if (middleVRRootNode == null)
            middleVRRootNode = GameObject.Find("RootNode");

        if (middleVRRootNode == null)
            Debug.LogError("Cannot find RootNode");
        }

    void Update()
    {
        middleVRRootNode.transform.rotation = characterController.transform.rotation;
        middleVRHeadNode.transform.position = HeadNode.transform.position;
    }

}
