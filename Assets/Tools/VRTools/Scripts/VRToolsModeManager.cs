using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Change VRTools mode in runtime. Disable unused dependant scripts (MiddleVR | Unity).
/// All dependant scripts must be activated in order to be disable and reactivated.
/// Inactive scripts are discarded. Reactive scripts are recorded during the desactivation.
/// 
/// Mode priority : 
/// - Executable Argument (UNITY | MIDDLEVR)
/// - Start with MiddleVR (--config)
/// - Select mode on Editor
/// 
///  Script execution order (at least for initialization)
///  - Before VRManagerScript and every script which use MiddleVR
/// </summary>
public class VRToolsModeManager : MonoBehaviour
{
    public VRToolsMode vrToolsMode;

    List<MonoBehaviour> middleVRComponents = new List<MonoBehaviour>();
    List<MonoBehaviour> unityComponents = new List<MonoBehaviour>();

    bool middleVRComponentsState = true;
    bool unityComponentsState = true;

    GameObject middleVRManagerGameObject;
    GameObject middleVRSystemCenterGameObject;
    GameObject middleVRWandGameObject;
    Camera mainCamera;

    const string UNITY = "UNITY";
    const string MIDDLEVR = "MIDDLEVR";

    [ContextMenu("Force Update Mode")]
    void OnValidate()
    {
        //Only on runtime.
        if (Application.isPlaying)
        {
            DisableUnusedEnableUsedScripts();
            VRTools.Mode = vrToolsMode;
        }
    }

    void Awake()
    {
        mainCamera = Camera.main;

        if (SetModeFromExecutableArgument()) { }

#if MIDDLEVR
        else if (WasStartedWithMiddleVR())
        {
            Debug.Log("[VRTools] Application started with MiddelVR. Change VRTools mode to MiddleVR.");
            vrToolsMode = VRToolsMode.MIDDLEVR;
            VRTools.Mode = vrToolsMode;
        }
#endif
        DisableUnusedEnableUsedScripts();
        VRTools.Mode = vrToolsMode;
    }

    bool SetModeFromExecutableArgument()
    {
        //Ignore case for argument
        HashSet<string> arguments = new HashSet<string>(System.Environment.GetCommandLineArgs(), System.StringComparer.OrdinalIgnoreCase);

        if (arguments.Contains(UNITY))
        {
            ChangeMode(VRToolsMode.UNITY);
            return true;
        }
#if MIDDLEVR
        else if (arguments.Contains(MIDDLEVR))
        {
            ChangeMode(VRToolsMode.MIDDLEVR);
            return true;
        }
#endif      
        return false;
    }

    void ChangeMode(VRToolsMode vrToolsMode)
    {
        this.vrToolsMode = vrToolsMode;
        VRTools.Mode = vrToolsMode;
        DisableUnusedEnableUsedScripts();
    }

    void DisableUnusedEnableUsedScripts()
    {
#if MIDDLEVR
        FindMiddleVRGameObject();
        if (vrToolsMode == VRToolsMode.MIDDLEVR)
            EnableMiddleVRScripts();
        else
            DisableMiddleVRScripts();
#endif

        if (vrToolsMode == VRToolsMode.UNITY)
            EnableUnityScripts();
        else
            DisableUnityScripts();
    }


#if MIDDLEVR
    /// <summary>
    /// Search if the application was started with MiddleVR.
    /// </summary>
    /// <returns>True if started with MiddleVR</returns>
    bool WasStartedWithMiddleVR()
    {
        HashSet<string> parameters = new HashSet<string>(System.Environment.GetCommandLineArgs());
        return parameters.Contains("--config");
    }

    void FindMiddleVRGameObject()
    {
        VRManagerScript vrManager = FindObjectOfType<VRManagerScript>();
        if (vrManager != null)
        {
            middleVRManagerGameObject = vrManager.gameObject;
            middleVRSystemCenterGameObject = (vrManager.VRSystemCenterNode != null) ?
                vrManager.VRSystemCenterNode :
                GameObject.Find("VRSystemCenterNode");
     
            VRWand vrWand = FindObjectOfType<VRWand>();
            if (vrWand != null)
                middleVRWandGameObject = vrWand.gameObject;
        }
    }

    void DisableMiddleVRScripts()
    {
        if (middleVRComponentsState)
        {
            //ChangeGameObjectState(middleVRSystemCenterGameObject, false); //May be used in none MiddleVR mode too.
            ChangeGameObjectState(middleVRManagerGameObject, false); 
            ChangeGameObjectState(middleVRWandGameObject, false);

            string[] types =
            { 
                "VRClusterObject",  
                "VRApplySharedTransform",
                "VRShareTransform",
                "VRWebView",
                "VRActor",
                "VRAttachToNode",
                "VRPhysicsBody",
                "VRPhysicsBodyManipulatorIPSI",
                "VRPhysicsConstraintBallSocket",
                "VRPhysicsConstraintCylindrical",
                "VRPhysicsConstraintFixed",
                "VRPhysicsConstraintHelical",
                "VRPhysicsConstraintHinge",
                "VRPhysicsConstraintPlanar",
                "VRPhysicsConstraintPrismatic",
                "VRPhysicsConstraintUJoint",
                "VRPhysicsDeactivateAllContacts",
                "VRPhysicsDisableAllCollisions",
                "VRPhysicsDisableCollisions",
                "VRPhysicsEnableCollisions",
                "VRPhysicsShowContacts",
                "VRFPSInputController",
                "AssignMiddleVRHeadNodeTransform",
                "AddHeadNodeAudioListener"
            };

            middleVRComponents = GetAllActivesMBFromTypes(types);

            foreach (MonoBehaviour mb in middleVRComponents)
                mb.enabled = false;

            middleVRComponentsState = false;
            Debug.Log("[VRTools] Disable " + middleVRComponents.Count + " MiddleVR scripts.");
        }
    }

    void EnableMiddleVRScripts()
    {
        if (!middleVRComponentsState)
        {
           // ChangeGameObjectState(middleVRSystemCenterGameObject, true); May be used in none MiddleVR mode too.
            ChangeGameObjectState(middleVRManagerGameObject, true);
            ChangeGameObjectState(middleVRWandGameObject, true);

            mainCamera.enabled = false;
            
            foreach (MonoBehaviour mb in middleVRComponents)
                mb.enabled = true;

            middleVRComponentsState = true;
            Debug.Log("[VRTools] Enable " + middleVRComponents.Count + " MiddleVR scripts.");
        }
    }
#endif

    void DisableUnityScripts()
    {
        if (unityComponentsState)
        {
            string[] types =
            { 
                "UnityStandardAssets.Characters.FirstPerson.FirstPersonController",
                "FPSInputController",
                "MouseLook",
                "SetUpUnityCamera"
            };

            unityComponents = GetAllActivesMBFromTypes(types);

            foreach (MonoBehaviour mb in unityComponents)
                mb.enabled = false;

            unityComponentsState = false;
            Debug.Log("[VRTools] Disable " + unityComponents.Count + " Unity scripts.");
        }
    }

    void EnableUnityScripts()
    {
        if (!unityComponentsState)
        {
            mainCamera.enabled = true;

            foreach (MonoBehaviour mb in unityComponents)
                mb.enabled = true;

            unityComponentsState = true;
            Debug.Log("[VRTools] Enable " + unityComponents.Count + " Unity scripts.");
        }
    }

    public static void ChangeGameObjectState(GameObject go, bool state)
    {
        if (go != null)
            go.SetActive(state);
    }

    public static List<MonoBehaviour> GetAllActivesMBFromTypes(string[] types)
    {
        List<MonoBehaviour> activesMBs = new List<MonoBehaviour>();
        foreach (string strType in types)
        {
            System.Type type = TypeFromString(strType);
            if (type != null)
            {
                MonoBehaviour[] mbs = FindObjectsOfType(type) as MonoBehaviour[];
                foreach (MonoBehaviour mb in mbs)
                    if (mb.enabled)
                        activesMBs.Add(mb);
            }
        }
        return activesMBs;
    }

    public static System.Type TypeFromString(string strType)
    {
        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly asm in assemblies)
        {
            System.Type type = asm.GetType(strType);
            if (type != null)
                return type;
        }
        return null;
    }
}
