//using UnityEngine;
//using System.Collections;

//public class AudioListenerManager : MonoBehaviour
//{
//    public Camera Camera;
//    public CharacterController CharacterController;

//    void Reset()
//    {
//        Camera = Camera.main;
//        CharacterController = FindObjectOfType<CharacterController>();
//    }
	
//    void Update () 
//    {
//        if (VRTools.GetMode() == VRTools.UNITY)
//            UnityMode();
//        else if (VRTools.GetMode() == VRTools.MIDDLEVR)
//            MiddleVRMode();
//    }

//    /// <summary>
//    /// Camera position and rotation
//    /// </summary>
//    void UnityMode()
//    {
//        transform.position = Camera.transform.position;
//        transform.rotation = Camera.transform.rotation;
//    }

//    /// <summary>
//    /// On character position and rotation.
//    /// </summary>
//    void MiddleVRMode()
//    {
//        transform.position = CharacterController.transform.position;
//        transform.rotation = CharacterController.transform.rotation;
//    }

//    /// <summary>
//    /// On Character position and camera rotation
//    /// </summary>
//    void ViconSDKMode()
//    {
//        transform.position = CharacterController.transform.position;
//        transform.rotation = Camera.transform.rotation;
//    }
//}
