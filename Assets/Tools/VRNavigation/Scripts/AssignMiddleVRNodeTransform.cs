using UnityEngine;
using System.Collections;

/// <summary>
/// Force MiddleVR node position.
/// Should be call directly after VRManagerScript
/// </summary>
public class AssignMiddleVRNodeTransform : MonoBehaviour
{
    public string MiddleVRNodeName = "HandNode";
    GameObject middleVRNode;

    void Reset()
    {
        MiddleVRNodeName = "HandNode";
    }

	void OnEnable()
    {
        middleVRNode = GameObject.Find(MiddleVRNodeName);
	}

    void Update()
    {
        middleVRNode.transform.position = transform.position;
        middleVRNode.transform.rotation = transform.rotation;
    }

}
