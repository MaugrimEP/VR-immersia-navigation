using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;



public class VirtuoseManager : MonoBehaviour
{
    public enum VirtuoseIPHelper
    {
        Simulator,
        Scale1_6001,
        Scale1_6002,
        Scale1_6003,
        Scale1_6004,
        Desktop_5125,
        Desktop_5126
    }

    public VirtuoseArm Arm;
    public VirtuoseAPIHelper Virtuose;

    public Vector3 BaseFramePosition;

    public VirtuoseIPHelper VirtuoseIP;

    public VirtuoseAPI.VirtCommandType CommandType = VirtuoseAPI.VirtCommandType.COMMAND_TYPE_VIRTMECH;

    [Range(0, 3)]
    public float ForceFactor = 1;

    [Range(0.001f, 0.1f)]
    public float Timestep = 0.01f;

    [Range(VirtuoseAPIHelper.MIN_MASS, VirtuoseAPIHelper.MAX_MASS)]
    public float mass = 0.2f;
    [Range(VirtuoseAPIHelper.MIN_INERTIE, VirtuoseAPIHelper.MAX_INERTIE)]
    public float inertie = 0.1f;

    public KeyCode powerOnKey = KeyCode.P;

    static bool virtuoseInitInThisFrame = false;

    bool[] buttonsPressed = new bool[4];
    bool[] buttonsToggled = new bool[4];

    bool isMaster;

    public bool Initialized
    {
        get; private set;
    }

    void OnEnable()
    { 
        if (VRTools.IsMaster())
            StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        //Wait one frame to avoid virtuose timeout, due to first frame lag in MiddleVR.
        yield return null;

        if (virtuoseInitInThisFrame)
        {//Wait one frame if another virtuose already have been init in the same frame.
         //It seems to be unstable if they are initilized in the same frame.
            yield return null;
            virtuoseInitInThisFrame = false;
        }
        else
            virtuoseInitInThisFrame = true;

        OpenArm();
        if (Arm.IsConnected)
        {
            InitArm();

            //Disable watchdog to allow to pause editor without having timeout error.
            if (Application.isEditor)
                Virtuose.ControlConnexion = false;

            Initialized = true;
        }
    }
    
    public IEnumerator WaitVirtuoseConnexion(Action action)
    {
        while (!Arm.IsConnected)
            yield return null;

        action();
    }

    void Start()
    {
        isMaster = VRTools.IsMaster(); //need to cache the value for OnDisable when MiddleVR is already
    }

    void OnDisable()
    {
        if (Initialized)
        {
            if (Application.isEditor)
                Virtuose.ControlConnexion = true;

            if (isMaster)
                DisconnectArm();

            Initialized = false;
        }
    }

    void OnValidate()
    {
        if (Arm.IsConnected)
        {
            Virtuose.ForceFactor = ForceFactor;
            Virtuose.Timestep = Timestep;
        }
        Arm.Ip = IpFromVirtuose(VirtuoseIP);      
    }

    string IpFromVirtuose(VirtuoseIPHelper virtuoseIP)
    {
        switch(virtuoseIP)
        {
            case VirtuoseIPHelper.Scale1_6001:
                return "131.254.154.172#6001"; //neflier
            case VirtuoseIPHelper.Scale1_6002:
                return "131.254.154.172#6002";
            case VirtuoseIPHelper.Scale1_6003:
                return "131.254.154.172#6003";
            case VirtuoseIPHelper.Scale1_6004:
                return "131.254.154.172#6004";
            case VirtuoseIPHelper.Desktop_5125:
                return "131.254.154.16#5125"; //immersion8
            case VirtuoseIPHelper.Desktop_5126:
                return "131.254.18.52#5126";  //immersion10
            case VirtuoseIPHelper.Simulator:
            default:
                return "127.0.0.1";
        }
    }

    void OpenArm()
    {
        Virtuose = new VirtuoseAPIHelper(Arm);
        (int majorVersion, int minorVersion) = Virtuose.APIVersion;
        VRTools.Log("[VirtuoseManager] Virtuose API version : " + majorVersion + "." + minorVersion);

        Virtuose.Open(Arm.Ip);

        if (Arm.IsConnected)
        {
            (majorVersion, minorVersion) = Virtuose.ControllerVersion;
            VRTools.Log("[VirtuoseManager] Virtuose controller version : " + majorVersion + "." + minorVersion);
        }
    }

    void InitArm()
    {
        Virtuose.InitDefault();
        if(ForceFactor != 1)
            Virtuose.ForceFactor = ForceFactor;

        if (Timestep != 0.01f)
            Virtuose.Timestep = Timestep;

        Virtuose.BaseFrame = (BaseFramePosition, Quaternion.identity);
        Virtuose.CommandType = CommandType;
        Virtuose.Power = true;

        if (CommandType == VirtuoseAPI.VirtCommandType.COMMAND_TYPE_VIRTMECH)
        {
            Virtuose.AttachVO(mass, inertie);
        }
    }
    
    void Update()
    {
        if (VRTools.IsMaster() && Initialized)
        {
            Virtuose.UpdateArm();
     
            if (Arm.IsConnected)
                UpdateArm();
            else if (!Arm.IsConnected)
                VRTools.LogError("[Error][VirtuoseManager] Arm not connected. Cannot execute virtuose command.");

            //Press both button to power again.
            if (Arm.IsConnected && !Virtuose.Power &&
                ((Virtuose.IsButtonPressed() && !Virtuose.DeadMan) ||
                VRTools.GetKeyDown(powerOnKey)))
            {
                VRTools.Log("[Info][VirtuoseManager] Force power on.");
                Virtuose.Power = true;
            }
        }
    }
    
    void UpdateArm()
    {       
        for(int b = 0; b < 3; b++)
        {
            bool buttonState = Virtuose.Button(b);   
            buttonsToggled[b] = buttonsPressed[b] != buttonState;
            buttonsPressed[b] = buttonState;
        }    
    }

    public bool IsButtonPressed(int button = 2)
    {
        return buttonsPressed[button];
    }

    public bool IsButtonToggled(int button = 2)
    {
        return buttonsToggled[button];
    }

    public bool IsButtonDown(int button = 2)
    {
        return buttonsToggled[button] && buttonsPressed[button];
    }

    public bool IsButtonUp(int button = 2)
    {
        return buttonsToggled[button] && !buttonsPressed[button];
    }

    /// <summary>
    /// Transform int button state into boolean button state.
    /// </summary>
    /// <param name="state">0: button released, 1: button pushed</param>
    /// <returns>True if pushed, False if released</returns>
    bool GetButtonState(int state)
    {
        return state == 1;
    }

    void DisconnectArm()
    {
        Virtuose.StopLoop();
        Virtuose.Power = false;
        Virtuose.DetachVO();
        Virtuose.Close();
    }
}