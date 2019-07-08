using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;
using UnityEngine;

public class NodeHierarchyControllerTPS : MonoBehaviour
{
    private Transform RootNode;
    public Transform CharacterTransform;
    public VirtuoseElasticNavigation InputController;

    [Header("CAMERA PARAMETERS")]
    public float heading = 0;
    public float headingSpeed = 15;
    public float tilt = 15;
    public float tiltSpeed = 15;
    public float cameraDistance = 10;
    public float playerHeight = 1;

    private Quaternion previousRotation;

    private vrCommand VRMyUpdate;
    private static int id;

    void Start()
    {
        id++;
        VRMyUpdate = new vrCommand($"AnimationController_{name}_{id}", MyUpdate);

        RootNode = GameObject.Find("RootNode").transform;
        previousRotation = RootNode.transform.rotation;
    }

    [VRCommand]
    private vrValue MyUpdate(vrValue _)
    {
        if (InputController.IsButtonPressed())
            FreeCam();
        else
            BehindCam();
        return null;
    }

    void LateUpdate()
    {
        VRMyUpdate.Do();
    }

    private void BehindCam()
    {
        RootNode.transform.rotation = previousRotation;
        RootNode.transform.RotateAround(
            CharacterTransform.position,
            Vector3.up,
            (CharacterTransform.rotation * Quaternion.Inverse(RootNode.transform.rotation)).eulerAngles.y);

        Vector3 CalculedOffset = -Vector3.forward * cameraDistance + Vector3.up * playerHeight;

        RootNode.transform.position = CharacterTransform.position + CharacterTransform.localRotation * CalculedOffset;

        heading = RootNode.transform.rotation.eulerAngles.y;
        tilt = RootNode.transform.rotation.eulerAngles.x;

        previousRotation = RootNode.transform.rotation;
    }

    private void FreeCam()
    {
        heading += (InputController.OrientedRotation().y * headingSpeed * VRTools.GetDeltaTime()).OrientedAngle();
        tilt += InputController.OrientedRotation().x * tiltSpeed * VRTools.GetDeltaTime();
        tilt = Mathf.Clamp(tilt, 0, 80);

        RootNode.transform.rotation = Quaternion.Euler(tilt, heading, 0f);
        RootNode.transform.position = CharacterTransform.position - RootNode.transform.forward * cameraDistance;
    }
}
