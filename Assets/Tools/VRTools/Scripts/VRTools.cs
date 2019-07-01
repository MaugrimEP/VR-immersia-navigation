using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

#if MIDDLEVR
using MiddleVR_Unity3D;
#endif

public enum VRToolsMode
{
    UNITY,
#if MIDDLEVR
    MIDDLEVR,
#endif
}

/// <summary>
/// MVR tools. Provides abstraction to interface with MiddleVR creating an
/// abstraction layer between MiddleVR API and the Unity app.
/// 
/// It provides static methods: 
/// 
/// to change the underlying manager SetMode()
/// to acces input data from devices (buttons, axis, keyboard).
/// to cluster information like isMaster() isClient()
/// to create sync objects (buttons, axis)
/// 
/// </summary>
public class VRTools : UnitySingleton<VRTools>
{

    private static VRToolsMode mode;
    static IVRTools modeInstance;

    /// <summary>
    /// Access to the VRTools mode.
    /// </summary>
    public static VRToolsMode Mode
    {
        get
        {
            return mode;
        }
        set
        {
            mode = value;
            if (mode == VRToolsMode.UNITY)
                modeInstance = VRToolsUnity.Instance;
#if MIDDLEVR
            if (mode == VRToolsMode.MIDDLEVR)
                modeInstance = VRToolsMiddleVR.Instance;
#endif
        }
    }

    /// <summary>
    /// Get active mode.
    /// </summary>
    /// <returns>UNITY/MIDDLEVR</returns>
    public static string GetMode()
    {
        return mode.ToString();
    }

    /// <summary>
    /// Default mode is MiddleVR if present.
    /// </summary>
    static VRTools()
    {
        modeInstance = VRToolsUnity.Instance;
#if MIDDLEVR
        modeInstance = VRToolsMiddleVR.Instance;
#endif    
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    public static VRTools GetInstance()
    {
        return Instance;
    }

    /// <summary>
    /// Calls the given callback with the instance when it's initialized.
    /// </summary>
    public static void GetInstance(System.Action<IVRTools> callback)
    {
        GetInstance().StartCoroutine(modeInstance.InitRoutine(callback));
    }

    /// <summary>
    /// Returns true when VRTools is ready to use
    /// </summary>
    bool Ready 
    { 
        get 
        {
            return modeInstance != null && modeInstance.Ready;
        } 
    }

    #region Button
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Button Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns whether the toggle status of the button has changed or not
    /// </summary>
    /// <returns><c>true</c>, if button was toggled, <c>false</c> otherwise.</returns>
    /// <param name="button">Button Id.</param>
    /// <param name="wand">The interaction wand, 0 by defalt.</param>
    public static bool IsButtonToggled(uint button, int wand = 0)
    {
        return IsButtonToggled(button, false, wand);
    }

    /// <summary>
    /// Returns changes in the toogle state of the given button
    /// </summary>
    /// <returns><c>True</c>, if the toggled state has changed, <c>false</c> otherwise.</returns>
    /// <param name="button">Id button</param>
    /// <param name="pressed">Allows to detect when the button has been pressed (true) or released (false)</param>
    /// <param name="wand">The interaction wand, 0 by defalt.</param>
    public static bool IsButtonToggled(uint button, bool pressed, int wand = 0)
    {
        return modeInstance.IsButtonToggled(button, pressed, wand);
    }

    /// <summary>
    /// Returns whether a button is pressed or not
    /// </summary>
    /// <returns><c>true</c>, the button was pressed, <c>false</c> otherwise.</returns>
    /// <param name="button">Button Id</param>
    public static bool IsButtonPressed(uint button, int wand = 0)
    {
        return modeInstance.IsButtonPressed(button, wand);
    }

    /// <summary>
    /// Returns whether the i-th button of a specific device is pressed.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool IsButtonPressed(string device, uint button)
    {
        return modeInstance.IsButtonPressed(device, button);
    }

    /// <summary>
    /// Returns the list of all the buttons pressed in the button device
    /// </summary>
    /// <param name="device"></param>
    public static List<uint> IsButtonPressed(string device)
    {
        return modeInstance.IsButtonPressed(device);
    }
#endregion

#if MIDDLEVR
    /// <summary>
    /// Creates a new button device for synchorizing values among clients </summary>
    /// <returns>The pointer to the device</returns>
    /// <param name="name">Name of the device.</param>
    /// <param name="numButtons">Number of buttons of the device.</param>
    public static vrButtons CreateSyncButtonDevice(string name, uint numButtons)
    {
        if (MiddleVR.VRDeviceMgr == null) return null;

        vrButtons vrb = MiddleVR.VRDeviceMgr.CreateButtons(name);

        if (vrb != null)
        {
            vrb.SetButtonsNb(numButtons);

            MiddleVRTools.Log("[+] Created shared event button " + name);

            MiddleVR.VRClusterMgr.AddSynchronizedObject(vrb, 0);
        }
        else
        {
            MiddleVRTools.Log("[!] Error creating a shared event button " + name);
        }

        return vrb;
    }

    /// <summary>
    /// Enables the change of the button state of a vrButton. It should be used
    /// only for synch purposes. If the device does not exist it will create a
    /// new shared button device
    /// </summary>
    /// <param name="device">The string representing the device.</param>
    /// <param name="button">The button id which has to be updated.</param>
    /// <param name="state">The new state of the button</param>
    public static void SetButtonState(string device, uint button, bool state)
    {
        if (MiddleVR.VRDeviceMgr == null) return;

        vrButtons vrb = MiddleVR.VRDeviceMgr.GetButtons(device);

        if (vrb != null) vrb.SetPressedState(button, state);
    }

#endif

    #region Axis
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Axis Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    public static float GetWandAxisValue(uint axis, int wand = 0)
    {
        return modeInstance.GetWandAxisValue(axis, wand);
    }

    /// <summary>
    /// Return horizontal value of specified wand.
    /// </summary>
    /// <param name="wand">Index of the wand.</param>
    /// <returns>Horizontal value.</returns>
    public static float GetWandHorizontalValue(int wand = 0)
    {
        return modeInstance.GetWandHorizontalValue(wand);
    }

    /// <summary>
    /// Return vertical value of specified wand.
    /// </summary>
    /// <param name="wand">Index of the wand.</param>
    /// <returns>Vertical value.</returns>
    public static float GetWandVerticalValue(int wand = 0)
    {
        return modeInstance.GetWandVerticalValue(wand);
    }
    #endregion

    #region Keyboard
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Keyboard Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns whether the key has been pressed
    /// </summary>
    /// <returns><c>true</c>, when the key has been pressed, <c>false</c> otherwise.</returns>
    /// <param name="key">Unity keycode</param>
    public static bool GetKeyDown(KeyCode key)
    {
        return modeInstance.GetKeyDown(key);
    }

    /// <summary>
    /// Returns whether the key has been released
    /// </summary>
    /// <returns><c>true</c>, the key has been released, <c>false</c> otherwise.</returns>
    /// <param name="key">Unity keycode</param>
    public static bool GetKeyUp(KeyCode key)
    {
        return modeInstance.GetKeyUp(key);
    }

    /// <summary>
    /// Returns whether the key is currently pressed
    /// </summary>
    /// <returns><c>true</c>, if the key is down, <c>false</c> otherwise.</returns>
    /// <param name="key">Unity keycode</param>
    public static bool GetKeyPressed(KeyCode key)
    {
        return modeInstance.GetKeyPressed(key);
    }
    #endregion

    #region Vibration
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Vibration Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Trigger the vibration of a controller
    /// </summary>
    /// <param name="controllerID">The ID of the controller that will vibrate. All controllers will vibrate if -1</param>
    /// <param name="duration">Time in seconds the vibration will last.(Minimum vibration duration is 3ms)</param>
    /// <param name="axis">The axis to vibrate. There is usually only one</param>
    public static void TriggerVibration(int controllerID = 0, float duration = 100f, float intensity = 0.75f, uint axis = 0)
    {
        GetInstance().StartCoroutine(modeInstance.Vibrate(controllerID, duration, intensity, axis));
    }

    #endregion

    #region Cluster
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Cluster Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns true if the unity client is a cluster client.
    /// </summary>
    /// <returns><c>true</c>, if client, <c>false</c> otherwise.</returns>
    public static bool IsClient()
    {
        return modeInstance.IsClient();
    }

    /// <summary>
    /// Returns true if the unity client is the master server or there is no cluster.
    /// </summary>
    /// <returns><c>true</c>, if client, <c>false</c> otherwise.</returns>
    public static bool IsMaster()
    {
        return modeInstance.IsMaster();
    }


    /// <summary>
    /// Returns true if the unity client is in cluster mode.
    /// </summary>
    /// <returns><c>true</c>, if cluster, <c>false</c> otherwise.</returns>
    public static bool IsCluster()
    {
        return modeInstance.IsCluster();
    }
    #endregion

    #region TimeAndFrame
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Time/Frame Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the delta time in seconds. Must be used instead of Time.delaTime!
    /// </summary>
    /// <returns>The delta time.</returns>
    public static float GetDeltaTime()
    {
        return modeInstance.GetDeltaTime();
    }

    /// <summary>
    /// Gets the time in second, since the application started, at the beginning of the last frame. Must be used instead of Time.time!
    /// </summary>
    /// <returns>The time.</returns>
    public static float GetTime()
    {
        return modeInstance.GetTime();
    }

    /// <summary>
    /// The total number of frames that have passed since the beginning of the application.
    /// </summary>
    /// <returns>Frame number.</returns>
    public static uint GetFrameCount()
    {
        return modeInstance.GetFrameCount();
    }

    /// <summary>
    /// Use VRTools.WaitForSeconds instead of regular WaitForSeconds.
    /// 
    /// Usage:
    /// yield return StartCoroutine(VRTools.WaitForSeconds(5))
    /// Instead of:
    /// yield return new WaitForSeconds(5);
    /// </summary>
    /// <param name="timeToWait">Time to wait, in second.</param>
    /// <returns></returns>
    public static IEnumerator WaitForSeconds(float timeToWait)
    {
        float t = 0;
        while (t < timeToWait)
        {
            t += VRTools.GetDeltaTime();
            yield return null;
        }
    }
    #endregion

    #region Tracker
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Tracker Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get tracker absolute positions in meter if available.
    /// </summary>
    /// <param name="trackerName">Tracker name.</param>
    /// <returns>Tracker positions in meter or Vector3.zero if not available.</returns>
    public static Vector3 GetTrackerPosition(string trackerName)
    {
        return modeInstance.GetTrackerPosition(trackerName);
    }

    /// <summary>
    /// Get tracker absolute orientation if available.
    /// </summary>
    /// <param name="trackerName">Tracker name.</param>
    /// <returns>Tracker orientation or Quaternion.identity if not available.</returns>
    public static Quaternion GetTrackerRotation(string trackerName)
    {
        return modeInstance.GetTrackerRotation(trackerName);
    }
    #endregion

    #region Log
    /// <summary>
    /// Log the given text.
    /// </summary>
    /// <param name="text">Text to add to the log.</param>
    public static void Log(string textToLog)
    {
        modeInstance.Log(textToLog);
    }

    /// <summary>
    /// Log warning the given text.
    /// </summary>
    /// <param name="text">Text to add to the log.</param>
    public static void LogWarning(string textToLog)
    {
        modeInstance.LogWarning(textToLog);
    }

    /// <summary>
    /// Log error the given text.
    /// </summary>
    /// <param name="text">Text to add to the log.</param>
    public static void LogError(string textToLog)
    {
        modeInstance.LogError(textToLog);
    }
    #endregion

}

