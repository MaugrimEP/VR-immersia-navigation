using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAnimReinit : MonoBehaviour
{

    public enum AnimationState { Idle, Walk, Run };
    public AnimationState currentState;
    public float WalkRunTreshold;
    public HumanController humanController;

    public float idleRotation;
    public float walkRotation;
    public float runRotation;

    void Update()
    {
        SetState(humanController.GetAbsSpeed());
        float rotationY = GetCurrentRotation();
        transform.localRotation = Quaternion.Euler(Vector3.up* rotationY);
    }

    private float GetCurrentRotation()
    {
        switch (currentState)
        {
            case AnimationState.Idle:
                return idleRotation;
            case AnimationState.Walk:
                return walkRotation;
            case AnimationState.Run:
                return runRotation;
            default:
                return 0f;
        }
    }

    public void SetState(float speed)
    {
        if (speed == 0) currentState = AnimationState.Idle;
        if (0 < speed && speed < WalkRunTreshold) currentState = AnimationState.Walk;
        if (WalkRunTreshold <= speed) currentState = AnimationState.Run;
    }
}
