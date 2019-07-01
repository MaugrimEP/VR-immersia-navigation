using UnityEngine;
using System.Collections;

public class DebugTime : MonoBehaviour {
#if MIDDLEVR
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Time : " + Time.time + " - " + MiddleVR.VRKernel.GetTime() + " - " + VRTools.GetTime() + " - " + Time.fixedTime + " - " + Time.unscaledTime);
        Debug.Log("DeltaTime : " + Time.deltaTime + " - " + MiddleVR.VRKernel.GetDeltaTime() + " - " + VRTools.GetDeltaTime() + " - " + Time.fixedDeltaTime + " - " + Time.unscaledDeltaTime);
	}
#endif
}
