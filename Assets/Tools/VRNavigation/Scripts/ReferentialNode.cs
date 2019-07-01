using UnityEngine;
using System.Collections;

/// <summary>
/// Referential node is used to get user position for collision.
/// </summary>
public class ReferentialNode : MonoBehaviour 
{
    public GameObject[] referencialOrders;
    public GameObject referentialFromRootNode;
    public int lostFrameBeforeSwap = 10;

    int[] frameSinceLastUpdate;
    Vector3[] previousPosition;
    Quaternion[] previousRotation;

    void Start()
    {
        frameSinceLastUpdate = new int[referencialOrders.Length];
        previousPosition = new Vector3[referencialOrders.Length];
        previousRotation = new Quaternion[referencialOrders.Length];
    }

	void Update () 
    {
        transform.position = Vector3.zero;

        for (int r = 0; r < referencialOrders.Length; r++)
        {
            //Use approximate equality due to lack of consistancy from input tracker value. MiddleVR have some epsilon for disabled tracker.
            if (previousPosition[r] == referencialOrders[r].transform.position &&
                previousRotation[r] == referencialOrders[r].transform.rotation)
            {
                frameSinceLastUpdate[r]++;
            }
            else
            {

                frameSinceLastUpdate[r] = 0;
            }
            previousPosition[r] = referencialOrders[r].transform.position;
            previousRotation[r] = referencialOrders[r].transform.rotation;
        }
        
        for (int r = 0; r < referencialOrders.Length; r++)
        {
            if (referencialOrders[r].transform.position != Vector3.zero &&
                frameSinceLastUpdate[r] < lostFrameBeforeSwap)
            {
                transform.position = referencialOrders[r].transform.position;
                break;
            }
        }

        referentialFromRootNode.transform.localPosition = transform.localPosition;
        referentialFromRootNode.transform.localRotation = transform.localRotation;
	}
}
