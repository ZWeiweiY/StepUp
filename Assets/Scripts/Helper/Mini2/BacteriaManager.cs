using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaManager : MonoBehaviour
{
    [SerializeField] private Transform foot;
    public BacteriaDataSO[] bacteriaSOs;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnCooldown = 1f;

    // [SerializeField] private float spawnHeight = 1f;
    [SerializeField] private int maxSpawnedBacterias = 5;
    [SerializeField] private GameObject spawnVisualEffects;
    [SerializeField] private GameObject killedVisualEffects;
    [SerializeField] private Sound bacteriaSpawnSound;
    [SerializeField] private Sound bacteriaKilledSound;

    private List<GameObject> spawnedBacterias = new List<GameObject>();

    private MeshCollider footMeshCollider;
    private Mesh footMesh;
    private int[] footMeshTriangles;
    private Vector3[] footMeshVertices;
    private float timeSinceLastSpawn = 0f;

    private Mini2Manager mini2Manager;
    private Mini2Manager.Mini2State currentState;

    [Header("Debug Visualization")]
    [SerializeField] private bool showNormals = true;
    [SerializeField] private float normalLength = 1f; // Made this longer by default
    [SerializeField] private Color normalColor = Color.blue;
    [SerializeField] private bool invertNormals = false; // Add option to flip normals
    [SerializeField] private bool showOnlyOneNormal = false; // Option to show just one normal for clarity


    private void Awake()
    {
        InitializeMeshData();
    }

    private void InitializeMeshData()
    {
        if (foot == null)
        {
            Debug.LogError("Foot transform not assigned!");
            return;
        }

        footMeshCollider = foot.GetComponent<MeshCollider>();
        if (footMeshCollider == null)
        {
            Debug.LogError("No MeshCollider found on foot object!");
            return;
        }

        footMesh = footMeshCollider.sharedMesh;
        if (footMesh == null)
        {
            Debug.LogError("No mesh found in MeshCollider!");
            return;
        }

        footMeshTriangles = footMesh.triangles;
        footMeshVertices = footMesh.vertices;

        //Debug.Log($"Mesh initialized with {footMeshVertices.Length} vertices and {footMeshTriangles.Length/3} triangles");
    }

    private void Start()
    {
        mini2Manager = FindObjectOfType<Mini2Manager>();
        mini2Manager.onMini2StateChanged.AddListener(HandleMini2StateChanged);
    }

    // private void OnValidate()
    // {
    //     // Re-initialize mesh data when values are changed in the inspector
    //     InitializeMeshData();
    // }
    
    private void HandleMini2StateChanged(Mini2Manager.Mini2State state)
    {
        currentState = state;

        if(currentState == Mini2Manager.Mini2State.GameOver)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(currentState == Mini2Manager.Mini2State.GamePlay)
        {
             timeSinceLastSpawn += Time.deltaTime;

            if(timeSinceLastSpawn >= spawnInterval && spawnedBacterias.Count < maxSpawnedBacterias)
            {
                SpawnBacteria();
                timeSinceLastSpawn = 0;
            }
        }
       
    }
    private void SpawnBacteria()
    {
        if(footMeshCollider == null)
        {
            Debug.LogError("No foot or No MeshCollider for foot found!");
            return;
        }
        if(bacteriaSOs.Length == 0) return;
        int randomIndex = Random.Range(0, bacteriaSOs.Length);
        var(randomPoint, pointNormal) = GetOnMeshInstantiateValues();
        // Debug.Log(pointNormal);
        GameObject spawnedBacteria = Instantiate(bacteriaSOs[randomIndex].prefab, randomPoint, Quaternion.identity, transform);
        GameObject spawnedVFX = Instantiate(spawnVisualEffects, randomPoint, Quaternion.identity, spawnedBacteria.transform);
        SoundManager.Instance.PlaySFX(bacteriaSpawnSound, spawnedBacteria.transform, 1f, true);


        BacteriaCollectable bacteriaCollectable = spawnedBacteria.GetComponent<BacteriaCollectable>();
        if(bacteriaCollectable != null)
        {
            bacteriaCollectable.Initialize(bacteriaSOs[randomIndex], foot, this);
            bacteriaCollectable.SetSurfaceNormal(pointNormal);
            StartCoroutine(EnableCollectionAfterSpawn(bacteriaCollectable, spawnedVFX));
        }
        else
        {
            Debug.LogError("Bacteria component not found on the instantiated prefab!");
        }
    
        spawnedBacterias.Add(spawnedBacteria);
        // Debug.Log("Spawned!");
    }

    public void DespawnObject(GameObject obj)
    {
        if (spawnedBacterias.Contains(obj))
        {
            StartCoroutine(DestroyObjectAfterKilled(obj));
        }
    }

    private (Vector3, Vector3) GetOnMeshInstantiateValues()
    {
        Vector3 spawnPoint = Vector3.zero;
        Vector3 normal = Vector3.zero;

        if(footMesh != null)
        {
            int randomTriangleIndex = Random.Range(0, footMeshTriangles.Length / 3) * 3;

            Vector3 v1 = footMeshVertices[footMeshTriangles[randomTriangleIndex]];
            Vector3 v2 = footMeshVertices[footMeshTriangles[randomTriangleIndex + 1]];
            Vector3 v3 = footMeshVertices[footMeshTriangles[randomTriangleIndex + 2]];
            
            // Debug.Log(v1 + " and " + v2 + " and " + v3);

            Vector3 randomPoint = (v1 + v2 + v3) / 3f; // Center

            v1 = foot.TransformPoint(v1);
            v2 = foot.TransformPoint(v2);
            v3 = foot.TransformPoint(v3);

            Vector3 edge1 = v3 - v1;
            Vector3 edge2 = v2 - v1;
            
            normal = Vector3.Cross(edge1, edge2).normalized;
            // Debug.Log(normal);
            normal = -normal;
            
            spawnPoint = foot.transform.TransformPoint(randomPoint);
            normal = foot.transform.TransformDirection(normal);
        }

        return (spawnPoint, normal);
    }

    private void OnDrawGizmos()
    {
        if (!showNormals) return;

        // Initialize mesh data if needed
        if (footMeshCollider == null)
            footMeshCollider = foot?.GetComponent<MeshCollider>();
        if (footMesh == null && footMeshCollider != null)
            footMesh = footMeshCollider.sharedMesh;
        if (footMeshTriangles == null && footMesh != null)
            footMeshTriangles = footMesh.triangles;
        if (footMeshVertices == null && footMesh != null)
            footMeshVertices = footMesh.vertices;

        if (footMesh == null || footMeshVertices == null || footMeshTriangles == null || foot == null)
        {
            Debug.LogWarning("Missing required components for normal visualization");
            return;
        }

        Gizmos.color = normalColor;

        // If we only want to show one normal for debugging
        if (showOnlyOneNormal)
        {
            DrawNormalForTriangle(0);
            return;
        }

        // Draw all normals
        for (int i = 0; i < footMeshTriangles.Length; i += 3)
        {
            DrawNormalForTriangle(i);
        }
    }

    private void DrawNormalForTriangle(int triangleStartIndex)
    {
        Vector3 v1 = footMeshVertices[footMeshTriangles[triangleStartIndex]];
        Vector3 v2 = footMeshVertices[footMeshTriangles[triangleStartIndex + 1]];
        Vector3 v3 = footMeshVertices[footMeshTriangles[triangleStartIndex + 2]];

        // Calculate triangle center point
        Vector3 centerPoint = (v1 + v2 + v3) / 3f;

        v1 = foot.TransformPoint(v1);
        v2 = foot.TransformPoint(v2);
        v3 = foot.TransformPoint(v3);
        // Calculate normal
        Vector3 edge1 = v3 - v1;
        Vector3 edge2 = v2 - v1;
        Vector3 normal = Vector3.Cross(edge1, edge2).normalized;
        
        // Invert normal if needed
        if (invertNormals)
        {
            normal = -normal;
        }

        // Transform to world space
        Vector3 worldCenterPoint = foot.TransformPoint(centerPoint);
        Vector3 worldNormal = foot.TransformDirection(normal);
        worldNormal = normal;

        // Draw normal line
        Gizmos.color = normalColor;
        Gizmos.DrawLine(worldCenterPoint, worldCenterPoint + worldNormal * normalLength);
        
        // Draw sphere at base
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(worldCenterPoint, normalLength * 0.05f);
        
        // Draw sphere at tip
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(worldCenterPoint + worldNormal * normalLength, normalLength * 0.05f);

        Gizmos.color = normalColor;
        Gizmos.DrawLine(worldCenterPoint, worldCenterPoint + worldNormal * normalLength);

        // Draw the triangle edges in a different color for reference
        Gizmos.color = Color.yellow;
        Vector3 worldV1 = foot.TransformPoint(v1);
        Vector3 worldV2 = foot.TransformPoint(v2);
        Vector3 worldV3 = foot.TransformPoint(v3);
        
        Gizmos.DrawLine(worldV1, worldV2);
        Gizmos.DrawLine(worldV2, worldV3);
        Gizmos.DrawLine(worldV3, worldV1);
    }
    
    private IEnumerator TriggerBacteriaEffects(GameObject bacteriaGameObject, GameObject vfxParent)
    {
        foreach (Transform childfx in vfxParent.transform)
        {
            childfx.GetComponent<ParticleSystem>().Play();
        }
        Destroy(vfxParent, spawnCooldown);
        yield return new WaitForSeconds(spawnCooldown);
    }

    private IEnumerator EnableCollectionAfterSpawn(BacteriaCollectable bacteria, GameObject spawnVFX)
    {
        bacteria.SetCollectable(false);
        yield return StartCoroutine(TriggerBacteriaEffects(bacteria.gameObject, spawnVFX));
        bacteria.SetCollectable(true);
    }

    private IEnumerator DestroyObjectAfterKilled(GameObject killedBacteria)
    {
        //Fly away

        //Play Sound
        SoundManager.Instance.PlaySFX(bacteriaKilledSound, killedBacteria.transform, 1f, true);
        GameObject killedVFX = Instantiate(killedVisualEffects, killedBacteria.transform.position, Quaternion.identity, killedBacteria.transform);
        yield return StartCoroutine(TriggerBacteriaEffects(killedBacteria, killedVFX));
        spawnedBacterias.Remove(killedBacteria);
        Destroy(killedBacteria);
    }
}
