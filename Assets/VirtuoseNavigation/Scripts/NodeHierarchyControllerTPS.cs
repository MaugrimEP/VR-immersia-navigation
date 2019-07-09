using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;
using UnityEngine;

public class NodeHierarchyControllerTPS : MonoBehaviour
{
    private Transform RootNode;
    public Transform CharacterTransform;
    public VirtuoseElasticNavigation InputController;

    public float TimeBeforeInactif = 10f;
    private bool Actif = true;
    private float InactifCounter = 0f;

    [Header("CAMERA PARAMETERS")]
    public float heading = 0;
    public float headingSpeed = 15;
    public float tilt = 0f;
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
        Actif = true;
        InactifCounter = TimeBeforeInactif;
    }

    [VRCommand]
    private vrValue MyUpdate(vrValue _)
    {
        UpdateActif();

        if (Actif)
            BehindCam();
        else
            FreeCam();
        return null;
    }

    private void UpdateActif()
    {
        if (InputController.GetTranslation().magnitude == 0f && InputController.OrientedRotation().magnitude == 0f)
        {
            if (InactifCounter >= 0f) InactifCounter -= VRTools.GetDeltaTime();
        }
        else
        {
            InactifCounter = TimeBeforeInactif;
        }

        Actif = InactifCounter >= 0f;
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
        previousRotation = RootNode.transform.rotation;
    }

    private void FreeCam()
    {
        heading += (headingSpeed * VRTools.GetDeltaTime()).OrientedAngle();

        RootNode.transform.rotation = Quaternion.Euler(tilt, heading, 0);
        RootNode.transform.position = CharacterTransform.position - RootNode.transform.forward * cameraDistance;
    }
}
