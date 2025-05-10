using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini3PlayerContact : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField] private float constantHealthDecreaseDuration = 1f;
    [SerializeField] private int constantHealthDecreaseAmount = 5;

    [Header("Reference")]
    [SerializeField] private Mini3Manager mini3Manager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private UIMini3 uiMini3;
    [SerializeField] private Sound screamSound;
    [SerializeField] private Sound failSound;
    [SerializeField] private Sound successSound;
    [SerializeField] private InstructionTrigger tapToJumpInstruction;
    [SerializeField] private InstructionTrigger constantHealthDecreaseInstruction;
    [SerializeField] private float hitObstacleInstructionTriggerDuration = 2f;
    public GameTimer gameTimer;

    private int maxHealth = 100;
    private int currentHealth;

    private float elapsedTime = 0f;

    private Mini3PlayerController playerController;
    private Mini3PlayerCollect playerCollect;
    private Vector3 worldOffset = Vector3.zero;

    private bool rewarding = false;

    public bool triggeredConstantHealthInstruction{get; private set;} = false;
    private bool shoeOffInstructionDisplayed = false;
    private bool shoeOnInstructionDisplayed = false;

    public bool gameEnded{get; private set;} = false;    
    private void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<Mini3PlayerController>();
        playerCollect = GetComponent<Mini3PlayerCollect>();
        uiMini3.UpdateHealthUI(currentHealth, maxHealth);
    }

    private void Update()
    {
        if((gameTimer.isRunning && mini3Manager.CurrentMini3State == Mini3Manager.Mini3State.GamePlay) || triggeredConstantHealthInstruction)
        {
            elapsedTime += Time.deltaTime;
            
            if(elapsedTime >= constantHealthDecreaseDuration 
            && !rewarding
            // && stageManager.GetCurrentStageType() != StageManager.StageType.Reward
            )
            {
                elapsedTime = 0f;
                TakeDamage(constantHealthDecreaseAmount);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Mini3Contactables contactable = other.GetComponent<Mini3Contactables>();
        if(contactable != null && !gameEnded)
        {
            contactable.ApplyEffect(this);
            return;
        }
    }

    public void TakeDamage(int amount)
    {
        if(playerCollect != null && !playerCollect.isPowerUp)
        {
            currentHealth -= amount;
            // animator.SetTrigger("tripped");
            if(amount > constantHealthDecreaseAmount)
            {
                SoundManager.Instance.PlaySFX(screamSound, gameObject.transform, 0.2f, false);
                if(!shoeOffInstructionDisplayed)
                {
                    // Pause and Show UI if first hit
                    gameTimer.StopTimer();
                    shoeOffInstructionDisplayed = true;
                    uiMini3.TriggerShoeOffHitInstruction(hitObstacleInstructionTriggerDuration);
                    playerController.TriggerPause(hitObstacleInstructionTriggerDuration);
                    
                }
            }
            currentHealth = Mathf.Max(currentHealth, 0);
            uiMini3.UpdateHealthUI(currentHealth, maxHealth);

            if(currentHealth <= 0)
            {
                FailedGame();
            }
        }

        else if(playerCollect.isPowerUp && !shoeOnInstructionDisplayed && amount > constantHealthDecreaseAmount)
        {
            // Pause and Show UI if first hit
            gameTimer.StopTimer();
            shoeOnInstructionDisplayed = true;
            uiMini3.TriggerShoeOnHitInstruction(hitObstacleInstructionTriggerDuration);
            playerController.TriggerPause(hitObstacleInstructionTriggerDuration);
        }

    }

    public void TriggerTrippedAnimation()
    {
        if(playerCollect != null && !playerCollect.isPowerUp)
        {
            playerCollect.animator.SetTrigger("tripped");
        }
    }

    public void StartGame()
    {
        gameTimer.elapsedTime = 0f;
        gameTimer.StartTimer();
        mini3Manager.StartGamePlay();
        uiMini3.ShowGamePlayUI();
    }
    
    public void FailedGame()
    {
        gameTimer.StopTimer();
        triggeredConstantHealthInstruction = false;
        // playerController.tapToJumpInstructionTriggered = false;
        gameEnded = true;
        StartCoroutine(HandleGameFailed());
    }

    public void FinishedGame()
    {
        gameTimer.StopTimer();
        triggeredConstantHealthInstruction = false;
        // playerController.tapToJumpInstructionTriggered = false;
        gameEnded = true;
        StartCoroutine(HandleGameFinished());
    }

    public void HandlePowerUpActivated(int amount)
    {
        // Add code here to respond to power-up activation
        // currentHealth += amount;
        // currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentHealth = maxHealth;
        uiMini3.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void ResetPositionTrigger()
    {
        // Instead of moving the player, offset all obstacles/collectibles
        worldOffset.z += transform.position.z; // Store how far we've gone
        
        // Find all game objects that need to be repositioned
        // You might want to create a parent object containing all your game objects
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Mini3Blocks"); // Add this tag to your obstacles/collectibles
        
        foreach (GameObject obj in gameObjects)
        {
            Vector3 newPos = obj.transform.position;
            newPos.z = worldOffset.z;
            // obj.transform.position = newPos;
            Instantiate(obj, newPos, Quaternion.identity);
        }
        
        // Reset world offset
        worldOffset = Vector3.zero;
    }

    public void RewardTrigger()
    {
        rewarding = !rewarding;
    }

    public void InstructionTrigger(string text)
    {
        uiMini3.SetInstructionText(text);
        
        if(text == tapToJumpInstruction.instructionText)
        {
            // playerController.tapToJumpInstructionTriggered = true;
            uiMini3.ShowTapInstruction();
        }

        if(text == constantHealthDecreaseInstruction.instructionText)
        {
            triggeredConstantHealthInstruction = true;
            uiMini3.HideTapInstruction();
        }

    }

    private IEnumerator HandleGameFailed()
    {
        SoundManager.Instance.SetMusicPitch(1f);
        // player slow down
        yield return StartCoroutine(playerController.SlowDownPlayer(durationToStop: 3f, finished: false));
        SoundManager.Instance.PlaySFX(failSound, gameObject.transform, 1f, false);
        yield return new WaitForSeconds(2f);
        // show ui screen
        mini3Manager.TriggerGameOver(finished: false);
        yield return null;
    }

    private IEnumerator HandleGameFinished()
    {
        uiMini3.SetFinishGameTime();
        SoundManager.Instance.SetMusicPitch(1f);
        // player slow down
        yield return StartCoroutine(playerController.SlowDownPlayer(durationToStop: 5f, finished: true));
        SoundManager.Instance.PlaySFX(successSound, gameObject.transform, 1f, false);
        yield return new WaitForSeconds(5f);
        // show ui screen
        mini3Manager.TriggerGameOver(finished: true);
        yield return null;
    }
}
