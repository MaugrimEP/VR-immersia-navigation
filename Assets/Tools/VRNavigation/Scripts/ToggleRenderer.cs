using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleRenderer : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.H;
    public KeyCode modifierToggleKey = KeyCode.LeftShift;

    public bool initialState = true;

    public Renderer[] renderers;

    void Reset()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Start()
    {
        changeRendererState(initialState);
    }

    void Update()
    {
        if (VRTools.GetKeyDown(toggleKey) && (modifierToggleKey == KeyCode.None || VRTools.GetKeyPressed(modifierToggleKey)))
        {
            toggleRenderer();
        }
    }

    void toggleRenderer()
    {
        for (int r = 0; r < renderers.Length; r++)
            renderers[r].enabled = !renderers[r].enabled;
    }

    void changeRendererState(bool state)
    {
        for (int r = 0; r < renderers.Length; r++)
            renderers[r].enabled = state;
    }
}
