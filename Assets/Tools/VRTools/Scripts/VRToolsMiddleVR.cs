using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

#if MIDDLEVR
using MiddleVR_Unity3D;
#endif

#if MIDDLEVR

/// <summary>
/// MVR tools. Provides abstraction to interface with MiddleVR creating an
/// abstraction layer between MiddleVR API and the Unity app.
/// 
/// It provides: 
/// 
/// Methods to acces input data from MVR devices (buttons, axis, keyboard).
/// Methods to cluster methods like isMaster() isClient()
/// Methods to create sync objects (buttons, axis)
/// 
/// </summary>
public class VRToolsMiddleVR : UnitySingleton<VRToolsMiddleVR>, IVRTools
{
    private static vrKeyboard _kb = null;
    private static List<vrButtons> _buttons = new List<vrButtons>();
    private static List<vrAxis> _axis = new List<vrAxis>();
    private static List<vrJoystick> _joysticks = new List<vrJoystick>();
    private static List<vrWand> _wands = new List<vrWand>();

    private static UnityTcpClient controllerDaemonClient;
    public int controllerDaemonPort = 9512;
    public string controllerDaemonHostName = "natpoint";

    private static bool hasOpenVRDriver = false;

    public bool DebugMode = false;

    private static Transform centerNode = null;

    public static Transform CenterNode
    {
        get
        {
            if (centerNode == null)
            {
                centerNode = GameObject.FindObjectOfType<VRManagerScript>().VRSystemCenterNode.transform;
                if (centerNode == null)
                    centerNode = GameObject.Find("VRSystemCenterNode").transform;
            }
            return centerNode;
        }
    }

    public static bool HasOpenVRDriver
    {
        get
        {
            return hasOpenVRDriver;
        }
    }

    private static vrDriver openVRDriver = null;

    public static vrDriver OpenVRDriver
    {
        get
        {
            return openVRDriver;
        }
    }

    /// <summary>
    /// Controls whether the singleton has been initialized or not
    /// </summary>
    private static bool _init = false;


    /// <summary>
    /// Call Init() unity middlevr is fully configured. Wait one frame between each call.
    /// </summary>
    void Awake()
    {
        Instance.StartCoroutine(Instance.InitRoutine());
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    public static VRToolsMiddleVR GetInstance()
    {
        return Instance;
    }

    /// <summary>
    /// Calls the given callback with the instance when it's initialized.
    /// </summary>
    public void GetInstance(System.Action<IVRTools> callback)
    {
        GetInstance().StartCoroutine(GetInstance().InitRoutine(callback));
    }

    /// <see cref="Instance"/>
    public IEnumerator InitRoutine(Action<IVRTools> resultCallback)
    {
        while (!_init)
        {
            yield return null;
            Init();
        }
        resultCallback(this);
    }

    /// <see cref="Instance"/>
    IEnumerator InitRoutine()
    {
        while (!_init)
        {
            yield return null;
            Init();
        }
    }

    /// <summary>
    /// Returns true when the initialization is done
    /// </summary>
    public bool Ready 
    { 
        get 
        {
            return _init;
        } 
    }


    private static void CheckIfUsingOpenVR()
    {
        for (int i = 0; i < MiddleVR.VRDeviceMgr.GetDriversNb(); i++)
        {
            Debug.Log(MiddleVR.VRDeviceMgr.GetDriver((uint)i).GetType().GetName());
            if (MiddleVR.VRDeviceMgr.GetDriver((uint)i).GetName() == "OpenVR Driver")
            {

                Debug.Log("Using OpenVR Driver");
                openVRDriver = MiddleVR.VRDeviceMgr.GetDriver((uint)i);
                hasOpenVRDriver = true;
                break;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Non-Static
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Intializes all the devices defined in the MiddleVR configuration
    /// </summary>
    private void Init()
    {
        bool initialisationIncomplete = false;

        if (MiddleVR.VRDeviceMgr != null)
        {
            for (uint w = 0; w < MiddleVR.VRDeviceMgr.GetWandsNb(); w++)
            {
                vrWand wand = MiddleVR.VRDeviceMgr.GetWandByIndex(w);
                _wands.Add(wand);
                vrButtons b = wand.GetButtons();

                if (b != null)
                {
                    _buttons.Add(b);

                    Debug.Log("[MVRTools] Add button device: " + b.GetName() + " with " + b.GetButtonsNb() + " buttons");
                }
                else
                {
                    Debug.LogWarningFormat("Could not find wand {0} buttons", wand.GetName());
                    initialisationIncomplete = true;
                }

                vrAxis axis = wand.GetAxis();

                if (axis != null)
                {
                    _axis.Add(axis);

                    Debug.Log("[MVRTools] Add axis device: " + axis.GetName() + " with " + axis.GetAxisNb() + " axes");
                }
                else
                {
                    Debug.LogWarningFormat("Could not find wand {0} axis", wand.GetName());
                    initialisationIncomplete = true;
                }
            }

            _kb = MiddleVR.VRDeviceMgr.GetKeyboard();
            if (_kb == null)
            {
                Debug.LogWarning("Could not find keyboard");
                initialisationIncomplete = true;
            }

            CheckIfUsingOpenVR();

            // If we can't find an AudioListenerHandler, add one
            if (FindObjectOfType<AudioListenerHandler>() == null)
            {
                gameObject.AddComponent<AudioListenerHandler>();
            }

            if (FindObjectOfType<SwitchMonoStereo>() == null)
                gameObject.AddComponent<SwitchMonoStereo>();

            if (FindObjectOfType<HideMiddleVRText>() == null)
                gameObject.AddComponent<HideMiddleVRText>();

            if (FindObjectOfType<VRShortcutReload>() == null)
                gameObject.AddComponent<VRShortcutReload>();

            if (FindObjectOfType<VRShortcutInvertEyes>() == null)
                gameObject.AddComponent<VRShortcutInvertEyes>();

            if (IsCluster() && IsMaster() && controllerDaemonClient == null)
                controllerDaemonClient = UnityTcpClientManager.Instance.CreateClient(controllerDaemonHostName, controllerDaemonPort, handleIncomingData);

            if (initialisationIncomplete)
                Debug.LogWarning("VRTools initialization incomplete. Maybe you tried to call a method too soon. Change script execution order or use GetInstance(Action callback) or use the Ready property before calling VRTools");
            else
                _init = true;
        }
    }

    private void handleIncomingData(string obj)
    {
        foreach (string data in obj.Split('\n'))
        {
            if (String.Compare("Ping", data) == 0)
            {
                controllerDaemonClient.SendMessage("Pong");
            }
            else if (String.Compare("Refused", data) == 0)
            {
                Debug.LogFormat("Connection refused by daemon");
            }
        }
    }



#region Button
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Button Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool IsButtonPressed(uint button, int wand = 0)
    {
        if (wand < _wands.Count)
            return _wands[wand].IsButtonPressed(button);
        else
            return false;
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, int wand = 0)
    {
        return IsButtonToggled(button, false, wand);
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, bool pressed, int wand = 0)
    {
        if (wand < _wands.Count)
            return _wands[wand].IsButtonToggled(button,pressed);
        else
            return false;
    }

    /// <inheritdoc/>
    public bool IsButtonPressed(string device, uint button)
    {
        vrButtons vrb = MiddleVR.VRDeviceMgr.GetButtons(device);
        if (vrb != null)
            return vrb.IsPressed(button);

        return false;
    }

    /// <inheritdoc/>
    public List<uint> IsButtonPressed(string device)
    {
        List<uint> pressedButtons = new List<uint>();

        vrButtons vrb = MiddleVR.VRDeviceMgr.GetButtons(device);
        if (vrb != null)
        {
            for (uint i = 0; i < vrb.GetButtonsNb(); i++)
            {
                if (vrb.IsPressed(i)) pressedButtons.Add(i);
            }
        }
        return pressedButtons;
    }
#endregion

    /// <summary>
    /// Creates a new button device for synchorizing values among clients </summary>
    /// <returns>The pointer to the device</returns>
    /// <param name="name">Name of the device.</param>
    /// <param name="numButtons">Number of buttons of the device.</param>
    public vrButtons CreateSyncButtonDevice(string name, uint numButtons)
    {
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
    public void SetButtonState(string device, uint button, bool state)
    {
        vrButtons vrb = MiddleVR.VRDeviceMgr.GetButtons(device);
        if (vrb != null)
            vrb.SetPressedState(button, state);
    }

#region Axis
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Axis Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public float GetWandAxisValue(uint axis, int wand = 0)
    {
        if (wand < _wands.Count)
        {
            return _wands[wand].GetAxis().GetValue(axis);
        }
        else if (_joysticks.Count != 0)
        {
            return _joysticks[wand].GetAxisValue(axis);
        }
        return 0;
    }

    /// <inheritdoc/>
    public float GetWandHorizontalValue(int wand = 0)
    {
        return _wands[wand].GetHorizontalAxisValue();
    }

    /// <inheritdoc/>
    public float GetWandVerticalValue(int wand = 0)
    {
        return _wands[wand].GetVerticalAxisValue();
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
        if (_kb == null)
            return MiddleVR.VRDeviceMgr.IsKeyPressed(translateKeyCode(key)) &&
                   MiddleVR.VRDeviceMgr.IsKeyToggled(translateKeyCode(key));
        return _kb.IsKeyToggled(translateKeyCode(key));
    }

    /// <inheritdoc/>
    public bool GetKeyUp(KeyCode key)
    {
        if (_kb == null)
            return !MiddleVR.VRDeviceMgr.IsKeyPressed(translateKeyCode(key)) &&
                   MiddleVR.VRDeviceMgr.IsKeyToggled(translateKeyCode(key));
        return _kb.IsKeyToggled(translateKeyCode(key), false);
    }

    /// <inheritdoc/>
    public bool GetKeyPressed(KeyCode key)
    {
        if (_kb == null)
            return MiddleVR.VRDeviceMgr.IsKeyPressed(translateKeyCode(key));
        return _kb.IsKeyPressed(translateKeyCode(key));
    }
#endregion

#region Vibration
    /// <summary>
    /// Handle the effective vibration
    /// </summary>
    /// <param name="controllerID"></param>
    /// <param name="duration"> duration in millisecond</param>
    /// <param name="axis"></param>
    /// <returns></returns>
    public IEnumerator Vibrate(int controllerID, float duration, float intensity, uint axis)
    {
        if (DebugMode)
            Debug.Log("should vibrate");

        if (IsMaster())
        {
            if (controllerDaemonClient != null)
            {
                if (DebugMode)
                    Debug.Log("connected " + controllerID);
                StringBuilder stringbuild = new StringBuilder();

                if (controllerID == 0)
                {
                    stringbuild.Append("Senso ");
                    SendVibrationMessage(stringbuild, duration, intensity);
                }
                if (controllerID == 1)
                {
                    stringbuild.Append("PS ");
                    SendVibrationMessage(stringbuild, duration, intensity);
                }
            }
            else
            {
                float time = .0f;
                while (time < duration)
                {
                    time += VRTools.GetDeltaTime() * 1000;

                    if (hasOpenVRDriver)
                    {

                        // The parameters for the "vrDriverOpenVRSDK.TriggerHapticPulse" vrCommand are:
                        // - ControllerId: int
                        //   It is the controller we want to make vibrate. The first controller is
                        //   the controller 0. If ControllerId is -1 then all the
                        //   connected controllers will receive the haptic pulse.
                        // - Axis: uint
                        //   It is the axis we want to make vibrate on the controller. Controllers
                        //   usually have only one axis but they can have more. The first
                        //   axis is the axis 0.
                        // - VibrationTime: uint
                        //   It is the time in microseconds the pulse will last. It can last
                        //   up to 3 milliseconds.

                        // Note that after this call the application may not trigger another haptic
                        // pulse on this controller and axis combination for 5 ms.
                        var value = vrValue.CreateList();

                        value.AddListItem(new vrValue(controllerID));
                        value.AddListItem(new vrValue(axis));
                        value.AddListItem(new vrValue(3000));

                        MiddleVR.VRKernel.ExecuteCommand("vrDriverOpenVRSDK.TriggerHapticPulse", value);
                    }

                    yield return new WaitForSeconds(.002f);
                }


            }
        }
    }

    private void SendVibrationMessage(StringBuilder stringbuild, float duration, float intensity)
    {
        stringbuild.Append("vibration ");
        stringbuild.Append(intensity);
        stringbuild.Append(" ");
        stringbuild.Append(duration);
        stringbuild.Append(" \n");
        controllerDaemonClient.SendMessage(stringbuild.ToString());
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
        return MiddleVR.VRClusterMgr.IsCluster() && MiddleVR.VRClusterMgr.IsClient();
    }

    /// <inheritdoc/>
    public bool IsMaster()
    {
        return !MiddleVR.VRClusterMgr.IsCluster() || MiddleVR.VRClusterMgr.IsServer();
    }

    /// <inheritdoc/>
    public bool IsCluster()
    {
        return MiddleVR.VRClusterMgr.IsCluster();
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
        return (float)(MiddleVR.VRKernel.GetDeltaTime());
    }

    /// <inheritdoc/>
    public float GetTime()
    {
        return (float)(MiddleVR.VRKernel.GetTime() / 1000);
    }

    /// <inheritdoc/>
    public uint GetFrameCount()
    {
        return MiddleVR.VRKernel.GetFrame();
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
        vrNode3D node = MiddleVR.VRDisplayMgr.GetNode(trackerName);
        if (node == null)
            return Vector3.zero;
        return MVRTools.ToUnity(node.GetPositionVRSystemWorld());
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName)
    {
        vrNode3D node = MiddleVR.VRDisplayMgr.GetNode(trackerName);
        if (node == null)
            return Quaternion.identity;
        return MVRTools.ToUnity(node.GetOrientationVRSystemWorld());
    }
#endregion

#region Log
    /// <inheritdoc/>
    public void Log(string textToLog)
    {
        MiddleVR_Unity3D.MVRTools.Log(2, textToLog);
    }

    /// <inheritdoc/>
    public void LogWarning(string textToLog)
    {
        MiddleVR_Unity3D.MVRTools.Log(1, textToLog);
    }

    /// <inheritdoc/>
    public void LogError(string textToLog)
    {
        MiddleVR_Unity3D.MVRTools.Log(0, textToLog);
    }
#endregion

    /// <summary>
    /// Translates Unity key code to the MiddleVR KeyCode with respect to azerty keyboard
    /// </summary>
    /// <returns>The MiddleVR key code.</returns>
    /// <param name="k">Unity Keycode to translate</param>
    private static uint translateKeyCode(KeyCode k)
    {
        switch (k)
        {
            case KeyCode.A: return MiddleVR.VRK_Q;
            case KeyCode.B: return MiddleVR.VRK_B;
            case KeyCode.C: return MiddleVR.VRK_C;
            case KeyCode.D: return MiddleVR.VRK_D;
            case KeyCode.E: return MiddleVR.VRK_E;
            case KeyCode.F: return MiddleVR.VRK_F;
            case KeyCode.G: return MiddleVR.VRK_G;
            case KeyCode.H: return MiddleVR.VRK_H;
            case KeyCode.I: return MiddleVR.VRK_I;
            case KeyCode.J: return MiddleVR.VRK_J;
            case KeyCode.K: return MiddleVR.VRK_K;
            case KeyCode.L: return MiddleVR.VRK_L;
            case KeyCode.M: return MiddleVR.VRK_SEMICOLON;
            case KeyCode.N: return MiddleVR.VRK_N;
            case KeyCode.O: return MiddleVR.VRK_O;
            case KeyCode.P: return MiddleVR.VRK_P;
            case KeyCode.Q: return MiddleVR.VRK_A;
            case KeyCode.R: return MiddleVR.VRK_R;
            case KeyCode.S: return MiddleVR.VRK_S;
            case KeyCode.T: return MiddleVR.VRK_T;
            case KeyCode.U: return MiddleVR.VRK_U;
            case KeyCode.V: return MiddleVR.VRK_V;
            case KeyCode.W: return MiddleVR.VRK_Z;
            case KeyCode.X: return MiddleVR.VRK_X;
            case KeyCode.Y: return MiddleVR.VRK_Y;
            case KeyCode.Z: return MiddleVR.VRK_W;
            case KeyCode.Alpha1: return MiddleVR.VRK_1;
            case KeyCode.Alpha2: return MiddleVR.VRK_2;
            case KeyCode.Alpha3: return MiddleVR.VRK_3;
            case KeyCode.Alpha4: return MiddleVR.VRK_4;
            case KeyCode.Alpha5: return MiddleVR.VRK_5;
            case KeyCode.Alpha6: return MiddleVR.VRK_6;
            case KeyCode.Alpha7: return MiddleVR.VRK_7;
            case KeyCode.Alpha8: return MiddleVR.VRK_8;
            case KeyCode.Alpha9: return MiddleVR.VRK_9;
            case KeyCode.Alpha0: return MiddleVR.VRK_0;
            case KeyCode.Space: return MiddleVR.VRK_SPACE;
            case KeyCode.UpArrow: return MiddleVR.VRK_UP;
            case KeyCode.DownArrow: return MiddleVR.VRK_DOWN;
            case KeyCode.LeftArrow: return MiddleVR.VRK_LEFT;
            case KeyCode.RightArrow: return MiddleVR.VRK_RIGHT;
            case KeyCode.Keypad0: return MiddleVR.VRK_NUMPAD0;
            case KeyCode.Keypad1: return MiddleVR.VRK_NUMPAD1;
            case KeyCode.Keypad2: return MiddleVR.VRK_NUMPAD2;
            case KeyCode.Keypad3: return MiddleVR.VRK_NUMPAD3;
            case KeyCode.Keypad4: return MiddleVR.VRK_NUMPAD4;
            case KeyCode.Keypad5: return MiddleVR.VRK_NUMPAD5;
            case KeyCode.Keypad6: return MiddleVR.VRK_NUMPAD6;
            case KeyCode.Keypad7: return MiddleVR.VRK_NUMPAD7;
            case KeyCode.Keypad8: return MiddleVR.VRK_NUMPAD8;
            case KeyCode.Keypad9: return MiddleVR.VRK_NUMPAD9;
            case KeyCode.KeypadDivide: return MiddleVR.VRK_DIVIDE;
            case KeyCode.KeypadMultiply: return MiddleVR.VRK_MULTIPLY;
            case KeyCode.KeypadMinus: return MiddleVR.VRK_SUBTRACT;
            case KeyCode.KeypadPlus: return MiddleVR.VRK_ADD;
            case KeyCode.KeypadEnter: return MiddleVR.VRK_NUMPADENTER;
            case KeyCode.KeypadPeriod: return MiddleVR.VRK_DECIMAL;
            case KeyCode.Insert: return MiddleVR.VRK_INSERT;
            case KeyCode.Delete: return MiddleVR.VRK_DELETE;
            case KeyCode.Home: return MiddleVR.VRK_HOME;
            case KeyCode.End: return MiddleVR.VRK_END;
            case KeyCode.PageUp: return MiddleVR.VRK_PRIOR;
            case KeyCode.PageDown: return MiddleVR.VRK_NEXT;
            case KeyCode.Escape: return MiddleVR.VRK_ESCAPE;
            case KeyCode.LeftControl: return MiddleVR.VRK_LCONTROL;
            case KeyCode.RightControl: return MiddleVR.VRK_RCONTROL;
            case KeyCode.LeftAlt: return MiddleVR.VRK_ALTLEFT;
            case KeyCode.RightAlt: return MiddleVR.VRK_ALTRIGHT;
            case KeyCode.LeftShift: return MiddleVR.VRK_LSHIFT;
            case KeyCode.RightShift: return MiddleVR.VRK_RSHIFT;
            case KeyCode.Less: return MiddleVR.VRK_OEM_102;
            case KeyCode.Comma: return MiddleVR.VRK_M;
            case KeyCode.Semicolon: return MiddleVR.VRK_COMMA;
            case KeyCode.Colon: return MiddleVR.VRK_PERIOD;
            case KeyCode.Exclaim: return MiddleVR.VRK_SLASH;
            case KeyCode.Return: return MiddleVR.VRK_RETURN;
            case KeyCode.Equals: return MiddleVR.VRK_EQUALS;
            case KeyCode.Backspace: return MiddleVR.VRK_BACK;
            case KeyCode.RightBracket: return MiddleVR.VRK_MINUS;
            case KeyCode.Tab: return MiddleVR.VRK_TAB;
            case KeyCode.F1: return MiddleVR.VRK_F1;
            case KeyCode.F2: return MiddleVR.VRK_F2;
            case KeyCode.F3: return MiddleVR.VRK_F3;
            case KeyCode.F4: return MiddleVR.VRK_F4;
            case KeyCode.F5: return MiddleVR.VRK_F5;
            case KeyCode.F6: return MiddleVR.VRK_F6;
            case KeyCode.F7: return MiddleVR.VRK_F7;
            case KeyCode.F8: return MiddleVR.VRK_F8;
            case KeyCode.F9: return MiddleVR.VRK_F9;
            case KeyCode.F10: return MiddleVR.VRK_F10;
            case KeyCode.F11: return MiddleVR.VRK_F11;
            case KeyCode.F12: return MiddleVR.VRK_F12;
            case KeyCode.ScrollLock: return MiddleVR.VRK_SCROLL;
            case KeyCode.Pause: return MiddleVR.VRK_PAUSE;
            default:
                //Debug.Log("Unknown key asked : " + k);
                return MiddleVR.VRK_ESCAPE;
        }
    }
}

#endif

