using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleGameObject : MonoBehaviour
{
    public GameObject gameObjectToToggle;

    public VRInput input;

    void Update()
    {
        if (input.IsToggled())
            gameObjectToToggle.SetActive(!gameObjectToToggle.activeSelf);
    }
}
