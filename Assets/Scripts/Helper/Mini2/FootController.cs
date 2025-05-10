using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Mini2PlayerMovement mini2PlayerMovement = other.GetComponent<Mini2PlayerMovement>();
        if(mini2PlayerMovement != null)
        {
            if(mini2PlayerMovement.foot == transform) return;
            //Debug.Log("FOOTCONTROLLER: I AM CALLED");
            mini2PlayerMovement.foot = transform;
        }
    }
    
}
