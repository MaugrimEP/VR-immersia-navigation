using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public HumanController humanController;

    private void Start()
    {
        MVRTools.RegisterCommands(this);
    }

    private void Update()
    {
        vrVec2 param = new vrVec2(humanController.GetSpeed(), humanController.state == HumanController.State.Walking ? 1 : 2);
        MVRTools.GetCommand("SetAnimatorParams").Do(param);
    }

    [VRCommand]
    private vrValue SetAnimatorParams(vrVec2 realSpeed_mode)
    {
        float realSpeed = realSpeed_mode.x();
        int mode = (int)realSpeed_mode.y();

        float speed = Mathf.Abs(realSpeed);
        animator.SetFloat("Speed", speed);
        animator.SetFloat("RealSpeed", realSpeed);


        animator.SetBool("Walking", mode == 1);
        animator.SetBool("Flying", mode == 2);

        return null;
    }
}
