using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BlockManager : MonoBehaviour
{
    [System.Serializable]
    public class PrefabGroup
    {
        public string groupName;
        public GameObject[] prefabs;
    }

    public PrefabGroup[] prefabGroups;   // Different groups of prefabs for different stages
    public int activeGroupIndex = 0;     // Current active group index
    public int blocksAhead = 3;          // Number of blocks to maintain ahead of player
    public int blocksBehind = 2;         // Number of blocks to keep behind player
    public float destroyDistance = 400f;  // Distance behind player to destroy blocks
    
    [SerializeField] private bool isRewardStageActive;
    [SerializeField] private Transform endingBlockParent;
    private Transform playerTransform;
    public List<GameObject> activeBlocks = new List<GameObject>();
    private const float BLOCK_LENGTH = 200f;     // Z scale of each block
    private const float STAGE_LENGTH = 1000f;    // Total length of a stage (5 blocks)
    private const int BLOCKS_PER_STAGE = 5;      // Number of blocks per stage
    private float GAME_LENGTH;
    private float lastSpawnZ = 0f;
    private int blocksSpawnedInCurrentStage = 0;
    
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeBlocks();
        GAME_LENGTH = isRewardStageActive? 7200f : 5200f;
        //Debug.Log("Game length is " + GAME_LENGTH);
    }
    
    private void Update()
    {
        ManageBlocks();
    }
    
    private void InitializeBlocks()
    {
        // Spawn initial blocks
        float currentZ = 0f;
        for (int i = 0; i < blocksAhead; i++)
        {
            SpawnBlock(currentZ);
            currentZ += BLOCK_LENGTH;
        }
        lastSpawnZ = currentZ;

        // Spawn last block
        GameObject[] tutorialPrefabs = prefabGroups[0].prefabs;
        // Instantiate(tutorialPrefabs[Random.Range(0, tutorialPrefabs.Length)], endingBlockParent.position, Quaternion.identity, endingBlockParent);
        Instantiate(tutorialPrefabs[0], endingBlockParent.position, Quaternion.identity, endingBlockParent);
    }
    
    private void ManageBlocks()
    {
        // Check if we need to spawn more blocks ahead
        float playerZ = playerTransform.position.z;
        while (lastSpawnZ - playerZ < (BLOCK_LENGTH * blocksAhead) && lastSpawnZ < GAME_LENGTH)
        {
            SpawnBlock(lastSpawnZ);
            lastSpawnZ += BLOCK_LENGTH;
        }
        
        
        // Remove blocks that are too far behind
        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            if (activeBlocks[i] != null)
            {
                float blockZ = activeBlocks[i].transform.position.z;
                if (blockZ < playerZ - destroyDistance)
                {
                    Destroy(activeBlocks[i]);
                    activeBlocks.RemoveAt(i);
                }
            }
        }
    }
    
    private void SpawnBlock(float zPosition)
    {
        if (prefabGroups == null || prefabGroups.Length == 0 || 
            prefabGroups[activeGroupIndex].prefabs.Length == 0)
            return;
            
        // Get random prefab from current group
        GameObject[] currentPrefabs = prefabGroups[activeGroupIndex].prefabs;
        GameObject prefab = null;

        if(activeGroupIndex != 0)
        {
            prefab = currentPrefabs[Random.Range(0, currentPrefabs.Length)];
        }
        else
        {
            prefab = currentPrefabs[blocksSpawnedInCurrentStage];
        }

        
        // Spawn block at position relative to player's x and y
        Vector3 spawnPosition = new Vector3(
            playerTransform.position.x,
            0f,
            zPosition
        );
        
        GameObject block = Instantiate(prefab, spawnPosition, Quaternion.identity);
        activeBlocks.Add(block);
        
        // Track blocks in current stage
        blocksSpawnedInCurrentStage = (int)((zPosition % STAGE_LENGTH) / BLOCK_LENGTH);
        //Debug.Log("Current Active Group: " + prefabGroups[activeGroupIndex].groupName + " spawn block: " + blocksSpawnedInCurrentStage);
    }
    
    public void SetActiveGroup(int groupIndex)
    {
        if (groupIndex >= 0 && groupIndex < prefabGroups.Length)
        {
            activeGroupIndex = groupIndex;
            blocksSpawnedInCurrentStage = 0; // Reset block counter for new stage
        }
    }

    public float GetStageProgress()
    {
        // Returns progress through current stage (0 to 1)
        float localZ = playerTransform.position.z % STAGE_LENGTH;
        return localZ / STAGE_LENGTH;
    }
}