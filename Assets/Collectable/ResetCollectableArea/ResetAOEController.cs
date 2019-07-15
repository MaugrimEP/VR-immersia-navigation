using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAOEController : MonoBehaviour
{
    public GameObject CheckPointContainer;
    [HideInInspector]
    public List<CollectableController> CollectableControllers;

    private void Start()
    {
        CollectableControllers = new List<CollectableController>();
        foreach (Transform child in CheckPointContainer.transform)
            CollectableControllers.Add(child.GetComponent<CollectableController>());
    }

    private void OnTriggerEnter(Collider _)
    {
        foreach (CollectableController cc in CollectableControllers)
            cc.ResetColor();
    }

    private void OnTriggerStay(Collider _)
    {
        foreach (CollectableController cc in CollectableControllers)
            cc.ResetColor();
    }
}
