using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CapsuleColliderUtility
{
    public CapsuleColliderData capsuleColliderData {get; private set;}
    [field: SerializeField] public DefaultColliderData defaultColliderData {get; private set;}
    [field: SerializeField] public SlopeData slopeData {get; private set;}

    public void Initialize(GameObject gameObject)
    {
        if(capsuleColliderData != null)
        {
            return;
        }
        capsuleColliderData = new CapsuleColliderData();

        capsuleColliderData.Initialize(gameObject);
    }
    public void CalculateCapsuleColliderDimensions()
    {
        SetCapsuleColliderRadius(defaultColliderData.Radius);
        SetCapsuleColliderHeight(defaultColliderData.Height * (1f - slopeData.StepHeightPercentage));
        RecalculateCapsuleColliderCenter();

        float halfColliderHeight = capsuleColliderData.capsuleCollider.height / 2f;
        if (halfColliderHeight < capsuleColliderData.capsuleCollider.radius)
        {
            SetCapsuleColliderRadius(halfColliderHeight);
        }

        capsuleColliderData.UpdateColliderData();
    }

    public void SetCapsuleColliderRadius(float radius)
    {
        capsuleColliderData.capsuleCollider.radius = radius;
    }

    public void SetCapsuleColliderHeight(float height)
    {
        capsuleColliderData.capsuleCollider.height = height;
    }

    public void RecalculateCapsuleColliderCenter()
    {
        float colliderHeightDifference = defaultColliderData.Height - capsuleColliderData.capsuleCollider.height;
        
        Vector3 newColliderCenter = new Vector3(0f, defaultColliderData.CenterY + (colliderHeightDifference / 2f), 0f);

        capsuleColliderData.capsuleCollider.center = newColliderCenter;
    }
}
