using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;


/// <inheritdoc/>
public class VRToolsUnity : UnitySingleton<VRToolsUnity>, IVRTools
{

    /// <inheritdoc/>
    public void GetInstance(System.Action<IVRTools> callback)
    {
        Instance.StartCoroutine(Instance.InitRoutine(callback));
    }

    /// <inheritdoc/>
    public IEnumerator InitRoutine(Action<IVRTools> resultCallback)
    {
        resultCallback(this);
        yield return null;
    }

    /// <summary>
    /// Returns true, always
    /// </summary>
    public bool Ready 
    { 
        get 
        {
            return true;
        } 
    }

    #region Button
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Button Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, int wand = 0)
    {
        return IsButtonToggled(button, false, wand);
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, bool pressed, int wand = 0)
    {
        if (pressed)
            return Input.GetMouseButtonDown((int)button);
        else
            return Input.GetMouseButtonUp((int)button);
    }

    /// <inheritdoc/>
    public bool IsButtonPressed(uint button, int wand = 0)
    {
        return Input.GetMouseButton((int)button);
    }

    /// <inheritdoc/>
    public bool IsButtonPressed(string device, uint button)
    {
        if (device == "mouse" || device == "MOUSE")
        {
            return IsButtonPressed(button);
        }
        else
        {
            Debug.LogError("[VRTools] No device in VRTools Unity mode.");
            return false;
        }
    }

    /// <inheritdoc/>
    public List<uint> IsButtonPressed(string device)
    {
        Debug.LogError("[VRTools] No device in VRTools Unity mode.");
        List<uint> pressedButtons = new List<uint>();
        return pressedButtons;
    }
    #endregion

    #region Axis
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Axis Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public float GetWandAxisValue(uint axis, int wand = 0)
    {
        if (axis == 1)
            return Input.GetAxis("Vertical");
        else if (axis == 0)
            return Input.GetAxis("Horizontal");
        else if (axis == 2)
            return Input.GetAxis("Gear");
        else
            return 0;
    }

    public float GetWandHorizontalValue(int wand = 0)
    {
        return Input.GetAxis("Horizontal");
    }

    public float GetWandVerticalValue(int wand = 0)
    {
        return Input.GetAxis("Vertical");

    }
    #endregion

    #region Keyboard
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Keyboard Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    /// <inheritdoc/>
    public bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    /// <inheritdoc/>
    public bool GetKeyPressed(KeyCode key)
    {
        return Input.GetKey(key);
    }
    #endregion

    #region Vibration
    /// <summary>
    /// Handle the effective vibration
    /// </summary>
    /// <param name="controllerID"></param>
    /// <param name="duration"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    public IEnumerator Vibrate(int controllerID, float duration, float intensity, uint axis)
    {
        yield return null;
    }
    #endregion

    #region Cluster
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Cluster Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool IsClient()
    {
        return false;
    }

    /// <inheritdoc/>
    public bool IsMaster()
    {
        return true;
    }

    /// <inheritdoc/>
    public bool IsCluster()
    {
        return false;
    }
    #endregion

    #region TimeAndFrame
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Time/Frame Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    /// <inheritdoc/>
    public float GetTime()
    {
        return Time.time;
    }

    /// <inheritdoc/>
    public uint GetFrameCount()
    {
        return (uint)Time.frameCount;
    }
    #endregion

    #region Tracker
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Tracker Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public Vector3 GetTrackerPosition(string trackerName)
    {
        return Vector3.zero;
    }

    /// <inheritdoc/>
    public Vector3 GetTrackerPosition(string trackerName, string segmentName)
    {
        return Vector3.zero;
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName)
    {
        return Quaternion.identity;
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName, string segmentName)
    {
        return Quaternion.identity;
    }
    #endregion

    #region Log
    /// <inheritdoc/>
    public void Log(string textToLog)
    {
        Debug.Log(textToLog);
    }

    /// <inheritdoc/>
    public void LogWarning(string textToLog)
    {
        Debug.LogWarning(textToLog);
    }

    /// <inheritdoc/>
    public void LogError(string textToLog)
    {
        Debug.LogError(textToLog);
    }
    #endregion
}

