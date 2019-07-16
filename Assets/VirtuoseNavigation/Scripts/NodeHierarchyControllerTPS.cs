using MiddleVR_Unity3D;
using UnityEngine;

public class NodeHierarchyControllerTPS : MonoBehaviour
{
    private Transform RootNode;
    public Transform CharacterTransform;
    public VirtuoseElasticNavigation InputController;

    [Header("CAMERA PARAMETERS")]
    public float cameraDistance = 10;

    private vrCommand VRMyUpdate;
    private static int id;

    void Start()
    {
        id++;
        VRMyUpdate = new vrCommand($"AnimationController_{name}_{id}", MyUpdate);

        RootNode = GameObject.Find("RootNode").transform;
    }

    [VRCommand]
    private vrValue MyUpdate(vrValue _)
    {
        BehindCam();
        return null;
    }

    void LateUpdate()
    {
        VRMyUpdate.Do();
    }

    private void BehindCam()
    {
        RootNode.transform.position = CharacterTransform.position - CharacterTransform.forward * cameraDistance;
        RootNode.transform.LookAt(CharacterTransform.position + CharacterTransform.forward * 30f);
    }
}
