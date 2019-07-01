using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (VRTools.IsButtonToggled(0, 0))
            VRTools.TriggerVibration(0, 1000);
        if (VRTools.IsButtonToggled(1, 0))
            VRTools.TriggerVibration(0);
        if (VRTools.IsButtonToggled(2, 0))
            VRTools.TriggerVibration(0,500);
        if (VRTools.IsButtonToggled(0, 1))
            VRTools.TriggerVibration(1,5000);
        if (VRTools.IsButtonToggled(4, 0))
            VRTools.TriggerVibration(1);
    }
}
