using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtuoseElasticNavigation : MonoBehaviour
{
    public VirtuoseManager vm;

    private bool attached = false;
    private Vector3 attachedPosition;
    private Quaternion attachedRotation;

    public float TranslationTreshold = 0.01f;
    public float RotationTreshold = 0.05f;

    [Header("SphereParameters")]
    public float SphereWalkRadius;
    public float SphereRunAddRadius;
    public float ForceFactor;

    private void Start()
    {
        attached = false;
    }

    void Update()
    {
        if (!vm.Arm.IsConnected) return;

        if (!attached)
        {
            (attachedPosition, attachedRotation) = vm.Virtuose.Pose;
            attached = true;
        }

        Vector3 differencePos = vm.Virtuose.Pose.position - attachedPosition;

        Vector3 forces = -differencePos * ForceFactor;
        vm.Virtuose.virtAddForce = (forces, torques: Vector3.zero);

        if (differencePos.magnitude > SphereWalkRadius + SphereRunAddRadius)
        {
            Vector3 PositionOnSphere = differencePos.GetPointInSphere(SphereWalkRadius + SphereRunAddRadius);
            vm.Virtuose.Pose = (attachedPosition + PositionOnSphere, vm.Virtuose.Pose.rotation);
        }
        else
        {
            vm.Virtuose.SetPoseIdentity();
        }
    }

    /// <summary>
    /// Get input on 3 axis that goes in [-1;1]
    /// </summary>
    /// <returns></returns>
    public Vector3 GetTranslation()
    {
        if (!vm.Arm.IsConnected || !vm.IsButtonPressed()) return Vector3.zero;

        Vector3 differencePos = vm.Virtuose.Pose.position - attachedPosition;
        Vector3 translation = (differencePos / (SphereWalkRadius + SphereRunAddRadius)).Clamp(-1f, 1f);
        translation = translation.Treshold(TranslationTreshold, 0f);
        return translation;
    }

    public Quaternion GetRotation()
    {
        if (!vm.Arm.IsConnected || !vm.IsButtonPressed()) return Quaternion.identity;

        Quaternion differenceRot = vm.Virtuose.Pose.rotation * Quaternion.Inverse(attachedRotation);
        differenceRot = differenceRot.Treshold(RotationTreshold, Quaternion.identity);
        return differenceRot;
    }
}
