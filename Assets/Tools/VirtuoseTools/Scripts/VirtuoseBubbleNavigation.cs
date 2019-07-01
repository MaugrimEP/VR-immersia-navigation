using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirtuoseBubbleNavigation : MonoBehaviour
{
    public VirtuoseManager vm;

    [Range(0, 2)]
    public float DistanceMultiplier = 1;

    public float SpeedMultiplier = 1;
    public float BubbleCenter = 0.6f;
    public float ThresholdDistanceInf = 0.15f;
    public float ThresholdDistanceSup = 0.18f;
    public bool NeedButton = false;

    Vector2 wantedPosition;
    (Vector3, Quaternion) currentPose;
    bool isMoving = false;

    void Start()
    {
        if(!vm)
            vm = GetComponent<VirtuoseManager>();
        vm.LogErrorIfNull();
    }

    void Update()
    {
        if (vm.Arm.IsConnected)
        {
            Vector2 carrierPosition = vm.Virtuose.Scale1CarrierPosition;

            if (!NeedButton || vm.IsButtonPressed())
            {
                Vector3 bubble = vm.Virtuose.BubblePosition(BubbleCenter);
                Vector3 physical = vm.Virtuose.PhysicalPose.position;
                Vector3 diff = physical - bubble;
                diff.y = 0;

                if (diff.magnitude > ThresholdDistanceSup)
                {
                    wantedPosition = carrierPosition + new Vector2(diff.x, diff.z) * DistanceMultiplier;
                    vm.Virtuose.Scale1CarrierPosition = wantedPosition;
                    currentPose = vm.Virtuose.Pose;
                    isMoving = true;
                }
                else if (diff.magnitude > ThresholdDistanceInf) { }
                else
                {
                    // vm.Virtuose.Pose = vm.Virtuose.Pose;
                    vm.Virtuose.Scale1CarrierPosition = carrierPosition;
                    currentPose = (Vector3.zero, Quaternion.identity);
                    isMoving = false;
                }
            }
            else
                vm.Virtuose.Scale1CarrierPosition = carrierPosition;

            float[] articularsSpeed = vm.Virtuose.ArticularsSpeed;
            for (int a = 0; a < articularsSpeed.Length; a++)
                articularsSpeed[a] *= SpeedMultiplier;
             vm.Virtuose.ArticularsSpeed = articularsSpeed;
        }
    } 
}
