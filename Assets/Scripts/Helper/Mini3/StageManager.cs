// StageManager.cs
using UnityEngine;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    public enum StageType
    {
        Tutorial,
        Road,
        Offroad,
        Reward
    }
    
    [System.Serializable]
    public class Stage
    {
        public StageType stageType;
        public string stageName;
        public int prefabGroupIndex;     // Index of prefab group to use
        public UnityEvent onStageStart;  // Events to trigger when stage starts
    }
    
    // Fixed sequence: Tutorial -> Road -> Reward -> Road -> Offroad -> Reward -> Offroad
    private static readonly StageType[] STAGE_SEQUENCE = new StageType[]
    {
        StageType.Tutorial,
        StageType.Road,
        // StageType.Reward,
        StageType.Road,
        StageType.Offroad,
        // StageType.Reward,
        StageType.Offroad
    };
    
    [Header("Stage Prefabs and Settings")]
    public Stage tutorialStage;
    public Stage roadStage;
    public Stage offroadStage;
    public Stage rewardStage;
    
    [Header("References")]
    public BlockManager blockManager;

    private Transform playerTransform;
    private int currentSequenceIndex = -1;
    private const float STAGE_LENGTH = 1000f;  // Length of each stage
    
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (blockManager == null)
        {
            blockManager = FindObjectOfType<BlockManager>();
        }
    }
    
    private void Update()
    {
        CheckStageProgression();
    }
    
    private void CheckStageProgression()
    {
        // Calculate current stage based on player position
        int newSequenceIndex = Mathf.FloorToInt(playerTransform.position.z / STAGE_LENGTH);
        
        // Don't loop the sequence - stay on last stage
        newSequenceIndex = Mathf.Min(newSequenceIndex, STAGE_SEQUENCE.Length - 1);
        
        if (newSequenceIndex != currentSequenceIndex)
        {
            ChangeStage(newSequenceIndex);
        }
    }
    
    private Stage GetStageForType(StageType type)
    {
        switch (type)
        {
            case StageType.Tutorial:
                return tutorialStage;
            case StageType.Road:
                return roadStage;
            case StageType.Offroad:
                return offroadStage;
            case StageType.Reward:
                return rewardStage;
            default:
                Debug.LogError($"Unknown stage type: {type}");
                return null;
        }
    }
    
    private void ChangeStage(int newSequenceIndex)
    {
        if (newSequenceIndex < 0 || newSequenceIndex >= STAGE_SEQUENCE.Length)
            return;
            
        currentSequenceIndex = newSequenceIndex;
        StageType currentStageType = STAGE_SEQUENCE[currentSequenceIndex];
        Stage newStage = GetStageForType(currentStageType);
        
        if (newStage == null)
            return;
        
        // Update block manager
        blockManager.SetActiveGroup(newStage.prefabGroupIndex);
           
        // Trigger stage events
        newStage.onStageStart.Invoke();
        
        //Debug.Log($"Entering {newStage.stageName} (Stage {currentSequenceIndex + 1}/{STAGE_SEQUENCE.Length})");
    }

    public float GetCurrentStageProgress()
    {
        return blockManager.GetStageProgress();
    }
    
    public int GetCurrentStageNumber()
    {
        return currentSequenceIndex + 1;
    }
    
    public int GetTotalStages()
    {
        return STAGE_SEQUENCE.Length;
    }
    
    // public StageType GetCurrentStageType()
    // {
    //     if (currentSequenceIndex >= 0 && currentSequenceIndex < STAGE_SEQUENCE.Length)
    //     {
    //         return STAGE_SEQUENCE[currentSequenceIndex];
    //     }
    //     return StageType.Tutorial; // Default to Tutorial if something goes wrong
    // }
    
    public StageType GetCurrentStageType()
    {
        if(playerTransform.position.z > 200f)
        {
            //Debug.Log(STAGE_SEQUENCE[Mathf.Abs(Mathf.FloorToInt((playerTransform.position.z - 200f) % 1000))]);
            return STAGE_SEQUENCE[Mathf.Abs(Mathf.FloorToInt((playerTransform.position.z - 200f) % 1000))];
        }
        return StageType.Tutorial;
    }
}