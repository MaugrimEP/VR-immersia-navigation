using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAOEController : MonoBehaviour
{
    public List<CollectableController> CollectableControllers;

    private void OnTriggerEnter(Collider other)
    {
        foreach (CollectableController cc in CollectableControllers)
            cc.ResetColor();
    }

}
