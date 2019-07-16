using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour
{
    public ToggleGameObject toggler;

    void Update()
    {
        if (VRTools.IsButtonToggled(1)) toggler.Toggle();
    }
}
