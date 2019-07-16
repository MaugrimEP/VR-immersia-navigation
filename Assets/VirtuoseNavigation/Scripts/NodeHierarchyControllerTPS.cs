using MiddleVR_Unity3D;
using UnityEngine;

public class NodeHierarchyControllerTPS : MonoBehaviour
{
    private Transform RootNode;
    public Transform CharacterTransform;
    public VirtuoseElasticNavigation InputController;

    [Header("CAMERA PARAMETERS")]
    public float cameraDistance = 10;
    public float cameraOffsetY = 0f;

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

        if (InputController.vm.IsButtonPressed())
        {
            RootNode.transform.position = CharacterTransform.position - CharacterTransform.forward * cameraDistance + Vector3.up * cameraOffsetY;
        }
        else
        {
            RootNode.transform.position = CharacterTransform.position - CharacterTransform.forward * cameraDistance;
        }
        RootNode.transform.LookAt(CharacterTransform.position + CharacterTransform.forward * 30f);
    }
}
