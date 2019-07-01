using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Interface for VRTools features.
/// <see cref="VRTools">
/// </summary>
public interface IVRTools
{
    /// <see cref="VRTools.IsButtonToggled(uint, int)">
    bool IsButtonToggled(uint button, int wand = 0);

    /// <see cref="VRTools.IsButtonToggled(uint, bool, int)">
    bool IsButtonToggled(uint button, bool pressed, int wand = 0);

    /// <see cref="VRTools.IsButtonPressed(uint, int)">
    bool IsButtonPressed(uint button, int wand = 0);

    /// <see cref="VRTools.IsButtonPressed(string, uint)">
    bool IsButtonPressed(string device, uint button);

    /// <see cref="VRTools.IsButtonPressed(string)">
    List<uint> IsButtonPressed(string device);

    /// <see cref="VRTools.GetWandAxisValue(uint, int)">
    float GetWandAxisValue(uint axis, int wand = 0);

    /// <see cref="VRTools.GetWandHorizontalValue(int)">
    float GetWandHorizontalValue(int wand = 0);

    /// <see cref="VRTools.GetWandVerticalValue(int)">
    float GetWandVerticalValue(int wand = 0);

    /// <see cref="VRTools.GetKeyDown(UnityEngine.KeyCode)">
    bool GetKeyDown(KeyCode key);

    /// <see cref="VRTools.GetKeyUp(UnityEngine.KeyCode)">
    bool GetKeyUp(KeyCode key);

    /// <see cref="VRTools.GetKeyPressed(UnityEngine.KeyCode)">
    bool GetKeyPressed(KeyCode key);

    /// <summary>
    /// Handle the effective vibration
    /// </summary>
    /// <param name="controllerID"></param>
    /// <param name="duration"></param>
    /// <param name="intensity"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    IEnumerator Vibrate(int controllerID, float duration, float intensity, uint axis);

    /// <see cref="VRTools.GetInstance(System.Action<IVRTools>)">
    void GetInstance(System.Action<IVRTools> callback);

    /// <summary>
    /// Call callback only when fully initialized.
    /// </summary>
    /// <param name="resultCallback"></param>
    /// <returns></returns>
    IEnumerator InitRoutine(Action<IVRTools> resultCallback);

    /// <see cref="VRTools.Ready">
    bool Ready { get; }

    /// <see cref="VRTools.IsClient">
    bool IsClient();

    /// <see cref="VRTools.IsMaster">
    bool IsMaster();

    /// <see cref="VRTools.IsCluster">
    bool IsCluster();

    /// <see cref="VRTools.GetDeltaTime">
    float GetDeltaTime();

    /// <see cref="VRTools.GetTime">
    float GetTime();

    /// <see cref="VRTools.GetFrameCount">
    uint GetFrameCount();

    /// <see cref="VRTools.Log(string)">
    void Log(string textToLog);

    /// <see cref="VRTools.LogWarning(string)">
    void LogWarning(string textToLog);

    /// <see cref="VRTools.LogError(string)">
    void LogError(string textToLog);

    /// <see cref="VRTools.GetTrackerPosition(string)">
    Vector3 GetTrackerPosition(string trackerName);

    /// <see cref="VRTools.GetTrackerRotation(string)">
    Quaternion GetTrackerRotation(string trackerName);
}
