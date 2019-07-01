using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtuoseMoveCarrier : MonoBehaviour
{
    public VirtuoseManager vm;
    public Vector2 Offset;
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
            //float[] articularsSpeed = vm.Virtuose.ArticularsSpeed;
            if (vm.IsButtonPressed())
            {
                //Vector2 offset = new Vector2( VRTools.GetDeltaTime(), VRTools.GetDeltaTime());
                vm.Virtuose.Scale1CarrierPosition = Offset + Position;
                // articularsSpeed[0] = 0.1f;
                // articularsSpeed[1] = 0.1f;
            }
            else
                vm.Virtuose.Scale1CarrierPosition = vm.Virtuose.Scale1CarrierPosition;
            
            //vm.Virtuose.ArticularsSpeed = articularsSpeed;
        }
    } 
}
