using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisableOnClients : MonoBehaviour {

    public List<GameObject> GameObjectsToDisable = new List<GameObject>();

    public List<Behaviour> BehavioursToDisable = new List<Behaviour>();

    
    // Use this for initialization
    void Start()
    {
        if (VRTools.IsClient())
        {
            foreach (Behaviour mb in BehavioursToDisable)
            {
                mb.enabled = false;
            }
            foreach (GameObject go in GameObjectsToDisable)
            {
                go.SetActive(false);
            }
        }
    }
}
