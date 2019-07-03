using UnityEngine;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

using System;
using System.Collections;
using MiddleVR_Unity3D;

public class HumanController : MonoBehaviour
{
    public CharacterController character;
    public VirtuoseElasticNavigation InputController;

    public float TranslationSpeed;
    public float RotationSpeed;

    private float speed;
    private float absSpeed;

    void Update()
    {

        float YRotation = InputController.GetRotation().eulerAngles.y;
        YRotation = YRotation < 180 ? YRotation : YRotation - 360;
        YRotation = YRotation * RotationSpeed * VRTools.GetDeltaTime();
        character.transform.Rotate(Vector3.up * YRotation, Space.Self);

        Vector3 translation = InputController.GetTranslation() * TranslationSpeed * VRTools.GetDeltaTime();
        translation = character.transform.localRotation * Vector3.forward * translation.z;

        speed = InputController.GetTranslation().z ;
        absSpeed = Mathf.Abs(speed);

        character.Move(translation);
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetAbsSpeed()
    {
        return absSpeed;
    }
}


