using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public HumanController humanController;
    public float SpeedMultiplier;

    private vrCommand VRSetAnimationParams;
    private static int id;

    private void Start()
    {
        id++;
        VRSetAnimationParams = new vrCommand($"AnimationController_{name}_{id}", SetAnimatorParams);
    }

    private void Update()
    {
        VRSetAnimationParams.Do();
    }

    [VRCommand]
    private vrValue SetAnimatorParams(vrValue _)
    {
        float realSpeed = humanController.GetSpeed();
        float speed = Mathf.Abs(realSpeed);
        animator.SetFloat("Speed", speed * SpeedMultiplier);
        animator.SetFloat("RealSpeed", realSpeed * SpeedMultiplier);

        animator.SetBool("Flying", humanController.state==HumanController.State.Flying);
        animator.SetBool("Walking", humanController.state==HumanController.State.Walking);

        return null;
    }
}
