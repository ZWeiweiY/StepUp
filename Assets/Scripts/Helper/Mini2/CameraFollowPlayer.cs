using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Range(.1f, 1f)]
    [SerializeField] private float followDamping;
    [SerializeField] private Transform playerTransform;
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, playerTransform.position, 1/followDamping * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, playerTransform.rotation, 1/followDamping * Time.fixedDeltaTime);
    }
}
