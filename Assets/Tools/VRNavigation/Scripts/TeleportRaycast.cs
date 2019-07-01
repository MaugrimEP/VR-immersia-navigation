using UnityEngine;
using System.Collections;

public class TeleportRaycast : MonoBehaviour 
{
    public GameObject handToRayCast;

    public uint raycastButtonIndex;
    public uint validateTeleportIndex;
    public float raycastMaxDistance = 15;

    public Color incorrectRaycastColor = Color.red;
    public Color correctRaycastColor = Color.green;

    public MeshRenderer markerRenderer;

    public TeleportToPosition teleportToPosition;

    enum TeleportState
    {
        DISABLE,
        ACTIVE_NO_RAYCAST,
        RAYCAST
    }

    TeleportState teleportState = TeleportState.DISABLE;

    LineRenderer lineRenderer;
    Material material;
    Vector3 lastRaycastPosition;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        markerRenderer.enabled = false;

        material = lineRenderer.material;

        ChangeRayColor(incorrectRaycastColor);
    }

	void Update () 
    {
        if (VRTools.IsButtonToggled(validateTeleportIndex, false) && teleportState == TeleportState.RAYCAST)
            Teleport();

        if (VRTools.IsButtonToggled(raycastButtonIndex, true))
            ActivateRay();

        if (VRTools.IsButtonToggled(raycastButtonIndex, false))
            DesactivateRay();

        if (teleportState != TeleportState.DISABLE)
            UpdateRay();
	}

    void Teleport()
    {
        VRTools.Log("Teleport to new position: " + lastRaycastPosition);
        teleportToPosition.SetNewPosition(lastRaycastPosition);
    }

    void ActivateRay()
    {
        teleportState = TeleportState.ACTIVE_NO_RAYCAST;
        lineRenderer.enabled = true;
    }

    void DesactivateRay()
    {
        teleportState = TeleportState.DISABLE;

        lineRenderer.enabled = false;
        markerRenderer.enabled = false;
    }

    void UpdateRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(handToRayCast.transform.position, handToRayCast.transform.forward, out hit, raycastMaxDistance))
        {
            lastRaycastPosition = hit.point;
            if(teleportState == TeleportState.ACTIVE_NO_RAYCAST)
            {
                ChangeRayColor(correctRaycastColor);
                markerRenderer.enabled = true;
            }

            teleportState = TeleportState.RAYCAST;
            markerRenderer.transform.position = hit.point;
            markerRenderer.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
        }

        else
        {
            if (teleportState == TeleportState.RAYCAST)
            {
                ChangeRayColor(incorrectRaycastColor);
                markerRenderer.enabled = false;
            }
            teleportState = TeleportState.ACTIVE_NO_RAYCAST;
        }

        lineRenderer.SetPosition(0, handToRayCast.transform.position);
        if (teleportState == TeleportState.RAYCAST)
            lineRenderer.SetPosition(1, hit.point);
        else
            lineRenderer.SetPosition(1, handToRayCast.transform.position + handToRayCast.transform.forward);
    }

    void ChangeRayColor(Color color)
    {
        material.SetColor("_Color", color);
    }
}
