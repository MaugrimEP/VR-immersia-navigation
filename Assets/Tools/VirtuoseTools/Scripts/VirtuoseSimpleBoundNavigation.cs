using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtuoseSimpleBoundNavigation : MonoBehaviour
{
    public VirtuoseManager vm;

    [Range(1, 3)]
    public float speed = 3;
    public Vector2 Position;

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
            if (vm.IsButtonPressed())
            {
                Position = vm.Virtuose.Scale1CarrierPosition;

                if (vm.Virtuose.IsAxeInBound((int)VirtuoseAPIHelper.InBoundScale1Axe.LEFT_AXE_1))               
                    Position[0] += VRTools.GetDeltaTime() * speed;
                
                if (vm.Virtuose.IsAxeInBound((int)VirtuoseAPIHelper.InBoundScale1Axe.RIGHT_AXE_1))
                    Position[0] -= VRTools.GetDeltaTime() * speed;
           
                if (vm.Virtuose.IsAxeInBound((int)VirtuoseAPIHelper.InBoundScale1Axe.INF_AXE_2))
                    Position[1] -= VRTools.GetDeltaTime() * speed;

                if (vm.Virtuose.IsAxeInBound((int)VirtuoseAPIHelper.InBoundScale1Axe.SUP_AXE_2))
                    Position[1] += VRTools.GetDeltaTime() * speed;

                vm.Virtuose.Scale1CarrierPosition = Position;
            }
            else
                vm.Virtuose.Scale1CarrierPosition = vm.Virtuose.Scale1CarrierPosition;            
        }
    } 
}
