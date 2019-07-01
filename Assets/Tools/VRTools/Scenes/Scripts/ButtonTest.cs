using UnityEngine;
using System.Collections;

public class ButtonTest : MonoBehaviour {

    // Use this for initialization
    void Awake()
    {
        if (VRTools.IsButtonPressed(0))
        { }
        else
            Debug.Log("button 0 not pressed");
    }

    // Use this for initialization
    void Start()
    {
        if (VRTools.IsButtonPressed(0))
        { }
        else
            Debug.Log("button 0 not pressed");
    }


    // Update is called once per frame
    void Update () {
        uint buttonNumber = 3;
#if MIDDLEVR
        buttonNumber = MiddleVR.VRDeviceMgr.GetWandByIndex(0).GetButtonsNb();
#endif
        for (uint button = 0; button < buttonNumber; button++)

        {
            //Debug.Log("checking button " + button);
            if (VRTools.IsButtonPressed(button))
                Debug.Log("Button " + button + " pressed");
        }
    }
}
