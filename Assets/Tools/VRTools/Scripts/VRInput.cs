using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Regroup key and button input.
/// </summary>
public class VRInput : MonoBehaviour
{
    public KeyCode Key;
    public KeyCode[] KeysModifier;

    /// <summary>
    /// < 0 value mean that button is disabled.
    /// </summary>
    public int button = -1;

    /// <summary>
    /// Is input toggled.
    /// </summary>
    /// <param name="toggledDown">Tru for toggled down input, false for toggled up.</param>
    /// <returns>true if toggled, false else.</returns>
    public bool IsToggled(bool toggledDown = true)
    {
        return IsKeyToggled(toggledDown) || IsButtonToggled(toggledDown); 
    }

    bool IsKeyToggled(bool toggledDown)
    {
        return (toggledDown ? VRTools.GetKeyDown(Key) : VRTools.GetKeyUp(Key))  && 
            KeysModifier.All(key => VRTools.GetKeyPressed(key));
    }

    bool IsButtonToggled(bool toggledDown)
    {
        return button >= 0 && VRTools.IsButtonToggled((uint)button, toggledDown);
    }

    /// <summary>
    /// Is input are pressed.
    /// </summary>
    /// <returns>true if pressed, false else.</returns>
    public bool IsPressed()
    {
        return IsKeyPressed() || IsButtonPressed();
    }

    bool IsKeyPressed()
    {
        return VRTools.GetKeyPressed(Key) &&
            KeysModifier.All(key => VRTools.GetKeyPressed(key));
    }

    bool IsButtonPressed()
    {
        return button >= 0 && VRTools.IsButtonPressed((uint)button);
    }
}
