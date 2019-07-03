using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public HumanController humanController;

    private vrCommand VRSetAnimationSpeed;
    private static int id;

    private void Start()
    {
        id++;
        VRSetAnimationSpeed = new vrCommand($"AnimationController_{name}_{id}", SetAnimatorSpeed);
    }

    private void Update()
    {
        VRSetAnimationSpeed.Do(humanController.GetSpeed());
    }

    [VRCommand]
    private vrValue SetAnimatorSpeed(vrValue VRspeed)
    {
        float realSpeed = VRspeed.GetFloat();
        float speed = Mathf.Abs(realSpeed);

        animator.SetFloat("Speed", speed);
        animator.SetFloat("RealSpeed", realSpeed);
        return null;
    }
}
