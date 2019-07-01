using UnityEngine;

#if UNITY_5_6_OR_NEWER
using UnityEngine.AI;
#endif

using System.Collections;

public enum NavigationMode 
{
    CharacterController,
    NavMesh,
    Fly,
    None
}

/// <summary>
/// Change navigation mode on runtime.
/// </summary>
public class NavigationManager : MonoBehaviour 
{

    public NavigationMode[] availableNavigationMode = { NavigationMode.CharacterController, NavigationMode.NavMesh, NavigationMode.Fly, NavigationMode.None };
    int currentNavigationMode;

    public KeyCode changeNavigationModeKey = KeyCode.N;
    public KeyCode modifierChangeNavigationModeKey = KeyCode.None;

    public JoystickNavigationController joystickNavigationController;
    public CollisionOffsetFromController collisionOffset;

    public CharacterController characterController;
    public CharacterMotor characterMotor;
    public NavMeshAgent navMeshAgent;

    public GameObject camera;
    public MouseLook mouseLook;

    float sensitivity;

    void Reset()
    {
        characterController = GetComponentInChildren<CharacterController>();
        characterMotor = GetComponentInChildren<CharacterMotor>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        joystickNavigationController = GetComponent<JoystickNavigationController>();
        camera = GetComponentInChildren<Camera>().gameObject;
        mouseLook = camera.GetComponent<MouseLook>();
    }

    void OnEnable()
    {
        sensitivity = mouseLook.sensitivityX;
        currentNavigationMode = 0;
        ChangeMode((availableNavigationMode.Length > 0) ? availableNavigationMode[0] : NavigationMode.None);
    }

    void Update()
    {
        if(VRTools.GetKeyDown(changeNavigationModeKey) && (modifierChangeNavigationModeKey == KeyCode.None || VRTools.GetKeyPressed(modifierChangeNavigationModeKey)))
        {
            currentNavigationMode = (currentNavigationMode + 1) % availableNavigationMode.Length;
            ChangeMode((availableNavigationMode.Length > currentNavigationMode) ? availableNavigationMode[currentNavigationMode] : NavigationMode.None);
        }

    }

    public void ChangeMode(NavigationMode mode)
    {
        VRTools.Log("[VRNavigation] Change navigation mode to " + mode.ToString());

        switch(mode)
        {
            case NavigationMode.CharacterController :
                characterController.enabled = true;
                characterMotor.enabled = true;
                joystickNavigationController.enabled = true;
                collisionOffset.collisionMode = CollisionOffsetFromController.CollisionMode.CharacterMove;
                joystickNavigationController.translateMode = JoystickNavigationController.TranslateMode.CharacterMove;
                joystickNavigationController.fixedHeight = true;
                navMeshAgent.enabled = false;
                mouseLook.sensitivityX = sensitivity;
                mouseLook.sensitivityY = sensitivity;
                break;

            case NavigationMode.Fly :
                characterController.enabled = false;
                characterMotor.enabled = false;
                joystickNavigationController.enabled = true;
                collisionOffset.collisionMode = CollisionOffsetFromController.CollisionMode.None;
                joystickNavigationController.translateMode = JoystickNavigationController.TranslateMode.Direct;
                joystickNavigationController.fixedHeight = false;
                navMeshAgent.enabled = false;
                mouseLook.sensitivityX = sensitivity;
                mouseLook.sensitivityY = sensitivity;
                break;

            case NavigationMode.NavMesh:
                characterController.enabled = false;
                characterMotor.enabled = true;
                joystickNavigationController.enabled = true;
                collisionOffset.collisionMode = CollisionOffsetFromController.CollisionMode.NavMeshMove;
                joystickNavigationController.translateMode = JoystickNavigationController.TranslateMode.NavMeshMove;
                joystickNavigationController.fixedHeight = true;
                NavMeshHit navMeshHit;
                NavMesh.SamplePosition(characterController.transform.position, out navMeshHit, 2000, NavMesh.AllAreas);
                //Add same offset to the camera to stay sync.
                //camera.transform.position += navMeshHit.position - characterController.transform.position; FIXME
                characterController.transform.position = navMeshHit.position;
                mouseLook.sensitivityX = sensitivity;
                mouseLook.sensitivityY = sensitivity;
                navMeshAgent.enabled = true;
                break;

            case NavigationMode.None:
                characterController.enabled = false;
                characterMotor.enabled = false;
                collisionOffset.collisionMode = CollisionOffsetFromController.CollisionMode.None;
                joystickNavigationController.translateMode = JoystickNavigationController.TranslateMode.Direct;
                joystickNavigationController.fixedHeight = false;
                joystickNavigationController.enabled = false;
                characterController.enabled = false;
                navMeshAgent.enabled = false;
                mouseLook.sensitivityX = 0;
                mouseLook.sensitivityY = 0;
                break;
        }
    }
}
