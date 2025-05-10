using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaCollectable : MonoBehaviour
{
    // Start is called before the first frame update

    public BacteriaDataSO bacteriaData {get; private set;}
    private Transform foot;
    private BacteriaManager bacteriaManager;
    private Rigidbody rb;
    // private Collider collider;
    private Vector3 surfaceNormal;
    private bool isCollectable = true;

    public void Initialize(BacteriaDataSO data, Transform footTransform, BacteriaManager spawner)
    {
        bacteriaData = data;
        foot = footTransform;
        bacteriaManager = spawner;

        // collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
    }

    // Add this new method to set the surface normal and align the object
    public void SetSurfaceNormal(Vector3 normal)
    {
        surfaceNormal = normal;
        AlignWithSurface();
    }

    private void AlignWithSurface()
    {
        if (surfaceNormal == Vector3.zero) return;
        
        // Calculate rotation from up vector to surface normal
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.down, surfaceNormal);
        
        // Apply the rotation to the bacteria
        transform.rotation = targetRotation;
        
        // Add a random rotation around the normal axis to add variety
        // transform.Rotate(surfaceNormal, Random.Range(0f, 360f), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isCollectable)
        {
            // collider.isTrigger = false;
            // rb.AddExplosionForce(1000f, transform.position, 10f); // Fixed the AddExplosionForce method call
            //Debug.Log("Killed " + bacteriaData.bacteriaName + ". Add points: " + bacteriaData.value);
            isCollectable = false;
            
            //score system
            Mini2Manager mini2Manager = FindObjectOfType<Mini2Manager>();
            if(mini2Manager != null)
            {
                mini2Manager.AddPoints(bacteriaData);
            }
            bacteriaManager.DespawnObject(gameObject);   
        }
    }

    public void SetCollectable(bool collectable)
    {
        isCollectable = collectable;
    }
}
