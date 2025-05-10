using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnSoapController : MonoBehaviour
{
    [SerializeField] private TouchInputMini2 touchInputManager;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float surfaceDetectionRadius = 0.5f;
    [SerializeField] private LayerMask footLayerMask;
    [SerializeField] private Transform cameraTransform;
    // [SerializeField] private MyJoystick myJoystick;
    [SerializeField] private int raycastCount = 8;
    [SerializeField] private int samplesPerRaycast = 5;
    
    private Rigidbody rb;
    private Vector3 currentNormal;
    private Vector3 targetNormal;
    private List<Vector3> nearbyNormals = new List<Vector3>();

    private Vector2 movementInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        DetectSurface();
        // UpdateMovement();
        // UpdatePlayerOrientation();
    }

    private void DetectSurface()
    {
        nearbyNormals.Clear();
        // Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, surfaceDetectionRadius, footLayerMask);

        // foreach (Collider collider in nearbyColliders)
        // {
        //     Vector3 closestPoint = collider.ClosestPoint(transform.position);
        //     Vector3 normal = (transform.position - closestPoint).normalized;
        //     nearbyNormals.Add(normal);
        // }

        for (int i = 0; i < raycastCount; i++)
        {
             // Calculate angle in the range -90 to 90 degrees
            float angle = Mathf.Lerp(-90f, 90f, (float)i / (raycastCount - 1));
            Vector3 direction = Quaternion.Euler(0, angle, 0) * -transform.up;

            Debug.DrawRay(transform.position, direction * 5f, Color.red);
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, surfaceDetectionRadius, footLayerMask))
            {
                
                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider != null && !meshCollider.convex)
                {
                    SampleMeshNormals(meshCollider, hit.point);
                }
                else
                {
                    nearbyNormals.Add(hit.normal);
                }
            }
        }

        if (nearbyNormals.Count > 0)
        {
            targetNormal = Vector3.zero;
            foreach (Vector3 normal in nearbyNormals)
            {
                targetNormal += normal;
            }
            targetNormal.Normalize();
            // Smooth out the target normal
            targetNormal = Vector3.Slerp(currentNormal, targetNormal, Time.deltaTime * 10f);
        }
        else
        {
            targetNormal = Vector3.up;
        }

        currentNormal = targetNormal;
    }

    private void UpdateMovement()
    {
        Vector3 movement = Vector3.zero;
        movementInput = touchInputManager.movementAmount;
        if (movementInput != Vector2.zero)
        {
            movement = CalculateDirection();
            movement = Vector3.ProjectOnPlane(movement, currentNormal).normalized;

            MovePlayer(movement);
        }
        rb.AddForce(-currentNormal * gravity, ForceMode.Acceleration);
    }

    private void MovePlayer(Vector3 movement)
    {
        if (movement.magnitude == 0)
        {
            return;
        }

        if(movement != Vector3.zero)
        {
            // Move the player
            rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);

            // Rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movement, currentNormal);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
    }

    private void UpdatePlayerOrientation()
    {
        currentNormal = Vector3.Slerp(currentNormal, targetNormal, Time.deltaTime * 5f);
        transform.up = currentNormal;
    }

    private void SampleMeshNormals(MeshCollider meshCollider, Vector3 hitPoint)
    {
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < samplesPerRaycast; i++)
        {
            Vector3 randomPoint = hitPoint + UnityEngine.Random.insideUnitSphere * (surfaceDetectionRadius * 0.1f);
            Vector3 localPoint = meshCollider.transform.InverseTransformPoint(randomPoint);

            for (int j = 0; j < triangles.Length; j += 3)
            {
                Vector3 v1 = vertices[triangles[j]];
                Vector3 v2 = vertices[triangles[j + 1]];
                Vector3 v3 = vertices[triangles[j + 2]];

                if (PointInTriangle(localPoint, v1, v2, v3))
                {
                    Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
                    nearbyNormals.Add(meshCollider.transform.TransformDirection(normal));
                    break;
                }
            }
        }
    }

    bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v < 1);
    }
    

    private Vector3 CalculateDirection()
    {
        Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y);
        return Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, surfaceDetectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, currentNormal);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetNormal);
    }


}
