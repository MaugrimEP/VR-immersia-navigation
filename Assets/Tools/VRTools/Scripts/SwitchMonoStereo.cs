using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allow to switch between mono and stereo view with stereo cameras (MiddleVR).
/// Need to be added to activated gameObject.
/// </summary>
public class SwitchMonoStereo : MonoBehaviour 
{
    public KeyCode switchKey = KeyCode.M;
    public KeyCode modifierSwitchKey = KeyCode.LeftShift;

    public bool StereoOn {get; set;}

    float InitialInterEyeDistance;

#if MIDDLEVR

    void Start()
    {
        StereoOn = true;
        InitialInterEyeDistance = GetCameraIntereyeDistance();
    }

    void Update () 
    {
        if (VRTools.GetKeyDown(switchKey) && (modifierSwitchKey == KeyCode.None || VRTools.GetKeyPressed(modifierSwitchKey)))
        {
            if (StereoOn)
                SetMonoView();
            
            else
                SetStereoView();
            
            StereoOn = !StereoOn;
        }
    }

    void SetMonoView()
    {
        SetAllCamerasIntereyeDistance(0);
    }

    void SetStereoView()
    {
        SetAllCamerasIntereyeDistance(InitialInterEyeDistance);
    }

    float GetCameraIntereyeDistance()
    {
        vrDisplayManager displayMgr = MiddleVR.VRDisplayMgr;

        // For each vrCameraStereo, invert inter eye distance.
        for (uint i = 0, iEnd = displayMgr.GetCamerasNb(); i < iEnd; ++i)
        {
            vrCamera cam = displayMgr.GetCameraByIndex(i);
            if (cam.IsA("CameraStereo"))
            {
                vrCameraStereo stereoCam = displayMgr.GetCameraStereoById(cam.GetId());
                return stereoCam.GetInterEyeDistance();
            }
        }
        return 0;
    }

    void SetAllCamerasIntereyeDistance(float distance)
    {
        vrDisplayManager displayMgr = MiddleVR.VRDisplayMgr;

        for (uint i = 0, iEnd = displayMgr.GetCamerasNb(); i < iEnd; ++i)
        {
            vrCamera cam = displayMgr.GetCameraByIndex(i);
            if (cam.IsA("CameraStereo"))
            {
                vrCameraStereo stereoCam = displayMgr.GetCameraStereoById(cam.GetId());
                stereoCam.SetInterEyeDistance(distance);
            }
        }
    }
#endif
}
