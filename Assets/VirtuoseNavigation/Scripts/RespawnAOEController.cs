using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAOEController : MonoBehaviour
{
    public Transform RespawnPoint;
    public GameObject Player;

    private void OnTriggerExit(Collider _)
    {
        Player.transform.position = RespawnPoint.position;
    }
}
