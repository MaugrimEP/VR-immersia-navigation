using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAnimReinit : MonoBehaviour
{

    public enum AnimationState { Idle, Walk, Run , Fly};
    public AnimationState currentState;
    public float WalkRunTreshold;
    public HumanController humanController;

    public Vector3 idleRotation;
    public Vector3 walkRotation;
    public Vector3 runRotation;
    public Vector3 flyRotation;

    void Update()
    {
        SetState(humanController.GetSpeed());
        Vector3 rotation = GetCurrentRotation();
        transform.localRotation = Quaternion.Euler(rotation);
    }

    private Vector3 GetCurrentRotation()
    {
        switch (currentState)
        {
            case AnimationState.Idle:
                return idleRotation;
            case AnimationState.Walk:
                return walkRotation;
            case AnimationState.Run:
                return runRotation;
            case AnimationState.Fly:
                return flyRotation;
            default:
                return Vector3.zero;
        }
    }

    public void SetState(float speed)
    {
        if (speed == 0) currentState = AnimationState.Idle;
        if (0 < speed && speed < WalkRunTreshold) currentState = AnimationState.Walk;
        if (WalkRunTreshold <= speed) currentState = AnimationState.Run;
        if (humanController.state == HumanController.State.Flying) currentState = AnimationState.Fly;
    }
}
