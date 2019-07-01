using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Feed virtuose joystick data to the navigation controller.
/// Thus need to be call before the navigation controller.
/// </summary>
public class VirtuoseNavigationController : MonoBehaviour
{
    //public JoystickNavigationController joystickNavigationController;
    public VirtuoseManager virtuoseManager;

    float[] referenceArticulars;

    [Range(0, 1)]
    public float Threshold = 0.2f;

    void Reset()
    {
        //joystickNavigationController = FindObjectOfType<JoystickNavigationController>();
        virtuoseManager = FindObjectOfType<VirtuoseManager>();
    }

    void Start()
    {
        StartCoroutine(WaitConnexion());
    }

    IEnumerator WaitConnexion()
    {
        bool init = false;
        while (!init)
        {
            yield return null;
            if (virtuoseManager.Arm.IsConnected)
            {
                referenceArticulars = virtuoseManager.Virtuose.Articulars;
                init = true;
            }
        }
    }

    void Update()
    {
        if (virtuoseManager.Arm.IsConnected && referenceArticulars != null)
        {
            if (virtuoseManager.Virtuose.IsButtonToggled())
                referenceArticulars = virtuoseManager.Virtuose.Articulars;
            
            Vector2 axes = virtuoseManager.Virtuose.Joystick(referenceArticulars);
            if (Mathf.Abs(axes.x) < Threshold)
                axes.x = 0;

            if (Mathf.Abs(axes.y) < Threshold)
                axes.y = 0;

            //joystickNavigationController.SetAxes(axes);
        }
    }
}
