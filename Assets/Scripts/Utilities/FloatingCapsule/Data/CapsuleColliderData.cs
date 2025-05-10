using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleColliderData
{
    public CapsuleCollider capsuleCollider {get; private set;}
    public Vector3 colliderCenterInLocalSpace {get; private set;}

    public void Initialize(GameObject gameObject)
    {
        if(capsuleCollider != null)
        {
            return;
        }

        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        UpdateColliderData();
    }

    public void UpdateColliderData()
    {
        colliderCenterInLocalSpace = capsuleCollider.center;
    }
}
