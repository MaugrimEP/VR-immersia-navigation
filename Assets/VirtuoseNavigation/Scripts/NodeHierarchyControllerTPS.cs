using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHierarchyControllerTPS : MonoBehaviour
{
    private Transform RootNode;
    public Transform CharacterTransform;
    public VirtuoseElasticNavigation InputController;

    [Header("CAMERA PARAMETERS")]
    public float heading = 0;
    public float tilt = 15;
    public float cameraDistance = 10;
    public float playerHeight = 1;

    private Vector3 cameraOffset;

    void Start()
    {
        RootNode = GameObject.Find("RootNode").transform;
    }

    void LateUpdate()
    {
        if (InputController.IsButtonPressed())
            FreeCam();
        else
            BehindCam();
    }

    private void BehindCam()
    {
        RootNode.transform.RotateAround(
            CharacterTransform.position,
            Vector3.up,
            (CharacterTransform.rotation * Quaternion.Inverse(RootNode.transform.rotation)).eulerAngles.y);

        Vector3 CalculedOffset = -Vector3.forward * cameraDistance + Vector3.up * playerHeight;

        RootNode.transform.position = CharacterTransform.position + CharacterTransform.localRotation * CalculedOffset;
    }

    private void FreeCam()
    {
        heading += (InputController.GetMouseX() * VRTools.GetDeltaTime() * 180f);
        heading %= 360f;
        tilt += InputController.GetMouseY() * VRTools.GetDeltaTime() * 180f;
        tilt = Mathf.Clamp(tilt, 0, 80);

        RootNode.rotation = CharacterTransform.rotation * Quaternion.Euler(tilt, heading, 0f);
        RootNode.transform.position = CharacterTransform.position - CharacterTransform.forward * cameraDistance + Vector3.up * playerHeight;

        cameraOffset = RootNode.transform.position;
    }
}
