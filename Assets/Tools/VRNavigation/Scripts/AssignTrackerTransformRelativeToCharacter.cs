using UnityEngine;
using System.Collections;


/// <summary>
/// Recover tracker position from local user view.
/// </summary>
public class AssignTrackerTransformRelativeToCharacter : MonoBehaviour 
{
    public GameObject tracked;
    public GameObject referential;

    void Start()
    {
        UpdateTrasnform();
    }

    void Update()
    {
        UpdateTrasnform();
    }

	void UpdateTrasnform()
	{
        Vector3 position = referential.transform.InverseTransformPoint(tracked.transform.position); 
        position.y = tracked.transform.position.y; //here localPosition == position
        transform.localPosition = position;
        transform.localRotation = tracked.transform.rotation;
	}
}
