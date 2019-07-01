using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCollision : MonoBehaviour {

    new Rigidbody rigidbody;

    public bool IsCollided;

    void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    //private void FixedUpdate()
    //{
    //    VRTools.Log("V " + rigidbody.velocity.ToString("F3"));

    //}

    void OnCollisionEnter(Collision collision)
    {
       // VRTools.Log("Col enter" + collision.impulse.ToString("F3"));
        IsCollided = true;
    }

    void OnCollisionExit(Collision collision)
    {
       // VRTools.Log("Col exit" + collision.impulse.ToString("F3"));
        IsCollided = false;
    }

    void OnCollisionStay(Collision collision)
    {
        //VRTools.Log("Col Stay" + collision.impulse.ToString("F3"));
        IsCollided = true;
    }
}
