using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAOEController : MonoBehaviour
{
    public GameObject CheckPointContainer;

    public event System.Action OnReset;

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
        Reset();
    }

    private void Reset()
    {
        foreach (CollectableController cc in CollectableControllers)
            cc.ResetColor();
        OnReset();
    }
}
