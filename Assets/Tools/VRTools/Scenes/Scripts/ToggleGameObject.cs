using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleGameObject : MonoBehaviour
{
    public GameObject gameObjectToToggle;

    public VRInput input;

    private void Update()
    {
        if (input.IsToggled()) Toggle();
    }

    public void Toggle()
    {
        gameObjectToToggle.SetActive(!gameObjectToToggle.activeSelf);
    }
}
