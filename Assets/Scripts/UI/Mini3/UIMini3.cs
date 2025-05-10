using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIMini3 : MonoBehaviour
{
    [Header("Tutorial UI")]
    [SerializeField] private GameObject instructionTextParent;
    [SerializeField] private GameObject instructionBackground;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private GameObject[] tapInstructions;
    [SerializeField] private GameObject hitObstacleInstructionParent;
    [SerializeField] private Image hitObstacleInstructionImage;
    [SerializeField] private TextMeshProUGUI hitObstacleInstructionText;
    [SerializeField] private SwappingImages[] hitObstacleInstructionElements;

    [Header("Game Play UI")]
    [SerializeField] private GameObject gameplayUIParent;
    [SerializeField] private GameObject timerParent;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI coinCountText;
    [SerializeField] private TextMeshProUGUI[] powerUpCountTexts;
    [SerializeField] private Sprite[] powerUpCountSprites;
    [SerializeField] private Image[] powerUpImages;
    [SerializeField] private Image[] powerUpCountdownFillImages;
    [SerializeField] private Image healthBarImage;
    // [SerializeField] private TextMeshProUGUI healthText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject endingParent;
    [SerializeField] private GameObject finishGameEnding;
    [SerializeField] private Sprite[] finishEndingSprites;
    [SerializeField] private Image finishEndingImage;
    [SerializeField] private GameObject failedGameEnding;
    [SerializeField] private TextMeshProUGUI[] finishCoinCount;

    [SerializeField] private Button backToWorldButton;
    [SerializeField] private Button retryButton;

    [SerializeField] private TextMeshProUGUI finishTimeText;

    private Mini3Manager mini3Manager;
    private GameTimer gameTimer;
    private Mini3PlayerContact mini3PlayerContact;

    private int currentReferenceShoeGroupIndex;
    private int currentFillImageIndex;

    private void Start()
    {
        mini3Manager = FindObjectOfType<Mini3Manager>();
        mini3Manager.onMini3StateChanged.AddListener(HandleMini3StateChanged);
        
        backToWorldButton.onClick.AddListener(HandleBackToWorldClicked);
        retryButton.onClick.AddListener(HandleRetryClicked);
 
        
        gameTimer = FindObjectOfType<GameTimer>();
        mini3PlayerContact = FindObjectOfType<Mini3PlayerContact>();
        if(gameTimer == null)
        {
            Debug.LogError("GameTimer Not Found in Scene!");
        }
        if (mini3PlayerContact == null)
        {
            Debug.LogError("PlayerContact Not Found in Scene!");
        }
        foreach (Image powerUpCountdownFillImage in powerUpCountdownFillImages)
        {
            powerUpCountdownFillImage.gameObject.SetActive(false);
        }

        currentReferenceShoeGroupIndex = 0;
        currentFillImageIndex = 0;
    }

    private void HandleMini3StateChanged(Mini3Manager.Mini3State _currentState)
    {
        if(_currentState == Mini3Manager.Mini3State.GamePlay)
        {
            timerParent.SetActive(true);
        }

        if(_currentState == Mini3Manager.Mini3State.GameOver)
        {
            foreach (TextMeshProUGUI textMeshProUGUI in finishCoinCount)
            {
                textMeshProUGUI.text = coinCountText.text;
            }
            ShowEndingUI();
            // Debug.Log(mini3Manager.passedGame);
            GameObject showingPage = mini3Manager.passedGame ? finishGameEnding : failedGameEnding;
            Sprite resultBackgroundSprite = int.Parse(coinCountText.text) > 250 ? finishEndingSprites[0] : finishEndingSprites[1];
            finishEndingImage.sprite = resultBackgroundSprite;
            showingPage.SetActive(true);
        }
    }

    private void Update()
    {
        if (gameTimer != null && gameTimer.isRunning)
        {
            UpdateGameTimer();
        }
    }

    public void UpdateCoinCount(int count)
    {
        coinCountText.text = count.ToString();
    }

    public void UpdatePowerUpCount(int count)
    {
        powerUpCountTexts[currentReferenceShoeGroupIndex].text = count.ToString();
        powerUpImages[currentReferenceShoeGroupIndex].sprite = powerUpCountSprites[count];
    }

    public void UpdatePowerUpCountdown(float normalizedRemainingTime)
    {
        powerUpCountdownFillImages[currentFillImageIndex].fillAmount = normalizedRemainingTime;
        if(normalizedRemainingTime < 0.25f)
        {
            powerUpCountdownFillImages[currentFillImageIndex].color = Color.red;
        }
        else
        {
            powerUpCountdownFillImages[currentFillImageIndex].color = Color.white;
        }
    }

    public void ShowPowerUpCountdown(bool show)
    {
        powerUpCountdownFillImages[currentFillImageIndex].gameObject.SetActive(show);
        powerUpImages[currentFillImageIndex].sprite = powerUpCountSprites[show ? powerUpCountSprites.Length-1 : 0];
    }

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void SetInstructionText(string text)
    {
        instructionBackground.SetActive(true);
        instructionTextParent.SetActive(true);
        instructionText.text = text;
    }

    public void ShowTapInstruction()
    {
        StartCoroutine(SwitchTapImages());
    }
    public void HideTapInstruction()
    {   
        foreach (GameObject obj in tapInstructions)
        {
            obj.SetActive(false);
        }
    }

    private IEnumerator SwitchTapImages()
    {
        int idx = 0;

        while(!mini3PlayerContact.triggeredConstantHealthInstruction)
        {
            tapInstructions[idx].SetActive(true);
            tapInstructions[tapInstructions.Length - 1 - idx].SetActive(false);
            yield return new WaitForSeconds(0.5f);
            idx = tapInstructions.Length - 1 - idx;
        }
        
    }

    private void UpdateGameTimer()
    {
        timerText.text = gameTimer.GetFormattedTime();
    }

    public void ShowGamePlayUI()
    {
        instructionTextParent.SetActive(false);
        gameplayUIParent.SetActive(true);
        endingParent.SetActive(false);
    }

    private void ShowEndingUI()
    {
        gameplayUIParent.SetActive(false);
        endingParent.SetActive(true);
    }

    private void HandleBackToWorldClicked()
    {   
        SoundManager.Instance.StopMusic();
        backToWorldButton.interactable = false;
        if(GameManager.Instance.finishedMiniCount < 3 && !GameManager.Instance.endingActivated)
        {
            GameManager.Instance.AddCompleteMiniCount();
            //GameManager.Instance.TriggerEndingActivation();
        }
        GameManager.Instance.StartGame();
        
        // SoundManager.Instance.PlayMusic("InWorld_BGM", true);
    }

    private void HandleRetryClicked()
    {
        GameManager.Instance.StartMiniGame3();
    }

    public void SetFinishGameTime()
    {
        finishTimeText.text = timerText.text;
    }

    public void SwitchPowerUpCountGroup()
    {
        currentReferenceShoeGroupIndex = (currentReferenceShoeGroupIndex + 1) % powerUpCountdownFillImages.Length;
    }

    public void SwitchPowerUpCountdownFillImage()
    {
        currentFillImageIndex = (currentFillImageIndex + 1) % powerUpCountdownFillImages.Length;
    }

    public void TriggerShoeOffHitInstruction(float duration)
    {
        //Debug.Log("UI Trigger: Hit without shoes!");
        StartCoroutine(DisplayHitObstacleInstruction(false, duration));
    }
    public void TriggerShoeOnHitInstruction(float duration)
    {
        //Debug.Log("UI Trigger: Hit with shoes!");
        StartCoroutine(DisplayHitObstacleInstruction(true, duration));
    }

    private IEnumerator DisplayHitObstacleInstruction(bool shoesOn, float duration)
    {
        hitObstacleInstructionParent.SetActive(true);
        // Set elements 
        if(shoesOn)
        {
            hitObstacleInstructionText.text = hitObstacleInstructionElements[1].imageName;
            hitObstacleInstructionImage.sprite = hitObstacleInstructionElements[1].image;
        }
        else
        {
            hitObstacleInstructionText.text = hitObstacleInstructionElements[0].imageName;
            hitObstacleInstructionImage.sprite = hitObstacleInstructionElements[0].image;
        }
        yield return new WaitForSeconds(duration);
        hitObstacleInstructionParent.SetActive(false);
    }

}
