using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectToFollowManager : MonoBehaviour
{
    public GameObject[] objectsToFollow;

    public VRInput nextObject;

    int currentIndex = 0;

    void Update()
    {
        if (nextObject.IsToggled())
        {
            currentIndex = objectsToFollow.Next(currentIndex);
            VRTools.Log("[ObjectToFollowManager] Change object to follow to " + objectsToFollow[currentIndex].name + " (" + currentIndex + ")");
        }

        transform.position = objectsToFollow[currentIndex].transform.position;
        transform.rotation = objectsToFollow[currentIndex].transform.rotation;
    }


}
