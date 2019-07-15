using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParcourtManager : MonoBehaviour
{
    [HideInInspector]
    public List<CollectableController> collectables;

    public GameObject resetAOE;

    public ObjectifController objectifController;

    private void Start()
    {
        collectables = new List<CollectableController>();
        foreach (Transform child in transform)
        {
            CollectableController cc = child.GetComponent<CollectableController>();
            collectables.Add(cc);
            cc.OnCollected += UpdateTarget;
        }
        resetAOE.GetComponent<ResetAOEController>().OnReset += UpdateTarget;
        UpdateTarget();
    }

    private Transform GetCurrentTarget()
    {
        foreach (CollectableController cc in collectables)
        {
            if (!cc.isSucced)
                return cc.ObjectifPosition;
        }
        return null;
    }

    public void UpdateTarget()
    {
        objectifController.Target = GetCurrentTarget();
    }
}
