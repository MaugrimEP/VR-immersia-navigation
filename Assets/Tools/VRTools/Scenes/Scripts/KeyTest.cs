using UnityEngine;
using System.Collections;
using System;

public class KeyTest : MonoBehaviour {

    // Use this for initialization
    void Awake()
    {
        if (VRTools.GetKeyPressed(KeyCode.A))
        { }
    }

    // Use this for initialization
    void Start () {
        if (VRTools.GetKeyPressed(KeyCode.A))
        { }
    }
	
	// Update is called once per frame
	void Update () {
        foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
        {
            if (VRTools.GetKeyPressed(keycode))
                Debug.Log(Enum.GetName(typeof(KeyCode), keycode)+ " pressed");
        }
	}
}
