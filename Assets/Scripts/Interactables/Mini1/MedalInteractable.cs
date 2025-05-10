using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalInteractable : MonoBehaviour
{
    private Rigidbody rb;
    private void OnEnable()
    {
        TouchObjectDetectorMini1.OnObjectTouched += HandleMedalTouched;
    }
    private void Start()
    {
        tag = "Mini1MedalInteractable";
        rb = GetComponent<Rigidbody>();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("MedalInteractable: Object " + transform.gameObject.name + " collided with Player!");
    }

    private void OnDisable()
    {
        TouchObjectDetectorMini1.OnObjectTouched -= HandleMedalTouched;
    }

    private void HandleMedalTouched(GameObject gameObject)
    {
        // Possible stack over another
        // rb.isKinematic = false;
        if(gameObject != this.gameObject)
        {
            // this.gameObject.SetActive(false);
        }
        
        else
        // if(gameObject == this.gameObject)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.excludeLayers = 0;
        }

        // Same height
        // rb.excludeLayers = 0;
    }
}
