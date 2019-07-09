using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtuoseElasticNavigation : MonoBehaviour
{
    public VirtuoseManager vm;

    private bool attached = false;
    private Vector3 attachedPosition;
    private Quaternion attachedRotation;

    public float TranslationTreshold = 0.05f;
    public Vector3 RotationTreshold;
    public float JumpValue;
    public float MaximalRotation = 45f;

    [Header("SphereParameters")]
    public float SphereRadius;
    public float ForceFactor;
    public float TorqueFactor;

    private void Start()
    {
        attached = false;
    }

    void Update()
    {
        if (!vm.Arm.IsConnected) return;

        if (!attached)//|| vm.IsButtonToggled())
        {
            (attachedPosition, attachedRotation) = vm.Virtuose.Pose;
            attached = true;
        }

        Vector3 differencePos = vm.Virtuose.Pose.position - attachedPosition;
        Vector3 forces = -differencePos * ForceFactor;
        Vector3 torques = (OrientedRotation()/180f) * TorqueFactor;

        vm.Virtuose.virtAddForce = (forces, torques);
        vm.Virtuose.SetSpeedIdentity();

        if (differencePos.magnitude > SphereRadius)
        {
            Vector3 PositionOnSphere = differencePos.GetPointInSphere(SphereRadius);
            vm.Virtuose.Pose = (
                attachedPosition + new Vector3(PositionOnSphere.x, 0f, PositionOnSphere.z),
                vm.Virtuose.Pose.rotation);
        }
        else
        {
            (Vector3 virtPos, Quaternion virtRot) = vm.Virtuose.Pose;
            vm.Virtuose.Pose = (
                new Vector3(virtPos.x, attachedPosition.y, virtPos.z),
                virtRot
                );
        }
    }

    /// <summary>
    /// Get input on 3 axis that goes in [-1;1]
    /// </summary>
    /// <returns></returns>
    public Vector3 GetTranslation()
    {
        if (!vm.Arm.IsConnected) return Vector3.zero;

        Vector3 differencePos = vm.Virtuose.Pose.position - attachedPosition;
        Vector3 translation = (differencePos / (SphereRadius)).Clamp(-1f, 1f);
        translation = translation.Treshold(TranslationTreshold, 0f);
        return translation;
    }

    /// <summary>
    /// Rotation without treshold
    /// </summary>
    /// <returns></returns>
    public Quaternion GetRotation()
    {
        if (!vm.Arm.IsConnected) return Quaternion.identity;

        Quaternion differenceRot = vm.Virtuose.Pose.rotation * Quaternion.Inverse(attachedRotation);
        return differenceRot;
    }

    public bool Jump()
    {
        return GetTranslation().y> JumpValue;
    }

    private float ToMouseInput(float angle)
    {
        float normalizedRotation = Mathf.Clamp(angle.OrientedAngle() / MaximalRotation, -1, 1);
        return normalizedRotation;
    }

    public float GetMouseX()
    {
        float rotation = GetRotation().eulerAngles.y.TresholdLower(RotationTreshold.y, 0f);

        return ToMouseInput(rotation);
    }

    public float GetMouseY()
    {
        float rotation = GetRotation().eulerAngles.x.TresholdLower(RotationTreshold.x, 0f);
        return ToMouseInput(rotation);
    }

    /// <summary>
    /// return the angular deviation in [-180;180], with a treshold
    /// </summary>
    /// <returns></returns>
    public Vector3 OrientedRotation()
    {
        Vector3 rotation = GetRotation().eulerAngles;

        rotation.x = rotation.x.OrientedAngle().TresholdLower(RotationTreshold.x, 0f); ;
        rotation.y = rotation.y.OrientedAngle().TresholdLower(RotationTreshold.y, 0f); ;
        rotation.z = rotation.z.OrientedAngle().TresholdLower(RotationTreshold.z, 0f); ;

        return rotation;
    }

    public bool IsResetingAttached()
    {
        return false;
    }
}
