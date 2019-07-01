using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Hide MiddleVR GUI watermark in standalone application.
/// </summary>
public class HideMiddleVRText : MonoBehaviour
{
    List<GUIText> middleVRGUITexts;

	void Start () 
    {
        middleVRGUITexts = new List<GUIText>();
		//Search all gameobject with "__" name
        var guiTextGameObjects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "__");

        foreach (GameObject guiTextGameObject in guiTextGameObjects)
            middleVRGUITexts.AddRange(guiTextGameObject.GetComponents<GUIText>());

        if (middleVRGUITexts != null)
            foreach (GUIText middleVRGUIText in middleVRGUITexts)        
                middleVRGUIText.enabled = false;        
	}

    public void Update()
    {
        if (VRTools.GetKeyDown(KeyCode.P))
            ToggleGUITexts();
    }

    void ToggleGUITexts()
    {
        if (middleVRGUITexts != null)
            foreach (GUIText middleVRGUIText in middleVRGUITexts)
                middleVRGUIText.enabled = !middleVRGUIText.enabled;
    }

}
