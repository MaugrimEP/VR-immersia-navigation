using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKiller : MonoBehaviour
{
    private GameObject Player;
    private Vector3 RespawnPoint;

    private void Start()
    {
        Player = GameObject.Find("Character");
        RespawnPoint = GameObject.Find("respawnPoint").transform.position;
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider _)
    {
        Player.transform.position = RespawnPoint;
    }
}
