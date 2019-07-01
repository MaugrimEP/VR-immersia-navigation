using UnityEngine;
using System.Collections;

/// <summary>
/// Assign tracker position and rotation to a game object.
/// 
/// Script execution order:
/// - After scripts which update tracker transform (VRManagerScript, ViconSDKManager)
/// - Before scripts using GameObject positions.
/// 
/// </summary>
public class AssignTransformFromTracker : MonoBehaviour 
{
    /// <summary>
    /// All possible names for the tracker.
    /// <example>Immersia : HeadNode, Immermove : HEAD, Immersia : HandNode, Immermove : Vicon001_AP</example>
    /// </summary>
    public string[] trackerNames;

    /// <summary>
    /// Current valid tracker name.
    /// </summary>
    string trackerName = "";

    /// <summary>
    /// Relative offset.
    /// </summary>
    Vector3 offset;

    void Start()
    {
        SearchTracker();
        offset = transform.localPosition;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        AssignTransform();
    }

    void Update()
    {
        AssignTransform();
    }

	void AssignTransform ()
    {
        if (trackerName != "")
        {
            transform.position = VRTools.GetTrackerPosition(trackerName) + offset;
            transform.rotation = VRTools.GetTrackerRotation(trackerName);
        }
        else
            SearchTracker();
    }

    void SearchTracker()
    {
        foreach (string name in trackerNames)
            if(name.Contains(";"))
            {
                string tName = name.Split(';')[0];
                string sName = name.Split(';')[1];

                if (VRTools.GetTrackerPosition(tName) != Vector3.zero)
                {
                    trackerName = tName;
                }
            }
            else if (VRTools.GetTrackerPosition(name) != Vector3.zero)
            {
                trackerName = name;
            }
    }
    
    [ContextMenu("SetHead")]
    void SetHead()
    {
        trackerNames = new string[] { "HeadNode", "HEAD" };
    }

    [ContextMenu("SetHand")]
    void SetHand()
    {
        trackerNames = new string[] { "Hand_HT27", "Hand_D", "RHAND" };
    }

    [ContextMenu("SetWand")]
    void SetWand()
    {
        trackerNames = new string[] { "HandNode", "ViconAP_001;Root" };
    }
}
