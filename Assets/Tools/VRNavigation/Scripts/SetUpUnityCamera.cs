using UnityEngine;
using System.Collections;

public class SetUpUnityCamera : MonoBehaviour 
{
    public float height = 1.7f;
    public bool isCrouch = false;

    public KeyCode crouchKey = KeyCode.C;

    void Update()
    {
        if (VRTools.GetKeyDown(crouchKey))
        {
            isCrouch = !isCrouch;
        }
        changeHeight();
    }

    void changeHeight()
    {
        transform.localPosition = new Vector3(0, isCrouch ? height / 2 : height, 0);
    }
}
