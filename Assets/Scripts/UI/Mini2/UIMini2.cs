using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SwappingImages
{
    public string imageName;
    public Sprite image;
}

public class UIMini2 : MonoBehaviour
{
    
    [Tooltip("Mini2 UI")]
    [SerializeField] private GameObject instructionUI;
    [SerializeField] private GameObject gamePlayUI;

    [Header("Instruction UI")]
    [SerializeField] private GameObject leftInstruction;
    [SerializeField] private GameObject rightInstruction;
    [SerializeField] private GameObject instructionContinueHint;
    [SerializeField] private GameObject goalInstruction;
    [SerializeField] private GameObject countdownText;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Map")]
    [SerializeField] private Image mapImage;
    [SerializeField] private SwappingImages[] mapImages;

    [Header("Ending")]
    [SerializeField] private GameObject endingUI;
    [SerializeField] private Button backToWorldButton;
    [SerializeField] private GameObject endingBg;
    [SerializeField] private SwappingImages[] resultBg;

    [SerializeField] private TextMeshProUGUI[] individualScore;
    [SerializeField] private TextMeshProUGUI totalScore;

    [Header("UI Sound")]
    [SerializeField] private Sound countdownSFX;

    private Mini2Manager mini2Manager;
    private Mini2Manager.Mini2State currentState;

    private float score;

    private void Awake()
    {
        InitializeMini2UI();
        mini2Manager = FindObjectOfType<Mini2Manager>();
    }

    private void Start()
    {
        mini2Manager.onMini2StateChanged.AddListener(HandleMini2StateChanged);
        score = 0;
    }

    private void HandleMini2StateChanged(Mini2Manager.Mini2State state)
    {
        currentState = state;

        switch(currentState)
        {
            case Mini2Manager.Mini2State.Intro:
                HideInstructionUI();
                HideGamePlayUI();
                break;
            
            case Mini2Manager.Mini2State.Instruction:
                // HideGamePlayUI();
                ShowInstructionUI();
                break;

            case Mini2Manager.Mini2State.GamePlay:
                HideInstructionUI();
                ShowGamePlayUI();
                break;

            case Mini2Manager.Mini2State.GameOver:
                // totalScore.text =  "Score: " + mini2Manager.currentScore.ToString();
                endingBg.GetComponent<Image>().sprite = SetResultBackgroundImage(score);
                HideInstructionUI();
                HideGamePlayUI();
                UpdateEndingResultValues();
                endingUI.SetActive(true);
                SoundManager.Instance.PlayMusic("Mini2_Ending", true);
                break;
        }

    }
    

    private void Update()
    {
        if(currentState == Mini2Manager.Mini2State.GamePlay)
        {
            int minutes = Mathf.FloorToInt(mini2Manager.timeRemaining / 60);
            int seconds = Mathf.FloorToInt(mini2Manager.timeRemaining % 60);
            timerText.text = string.Format("{0:D2} : {1:D2}", minutes, seconds);

            score = mini2Manager.currentScore;
            // scoreFill.fillAmount = Mathf.Min(score/scoreBound, 1f);

            scoreText.text = mini2Manager.currentScore.ToString();
        }
        
    }

    private void InitializeMini2UI()
    {
        backToWorldButton.onClick.AddListener(HandleBackToWorldClicked);
    }

    private void ShowInstructionUI()
    {
        instructionUI.SetActive(true);
        StartCoroutine(DisplayInstructions());
    }

    private void HideInstructionUI()
    {
        instructionUI.SetActive(false);
    }
    private void ShowGamePlayUI()
    {
        if(mini2Manager.instructionPlayed)
        {
            gamePlayUI.SetActive(true);
        }
    }

    private void HideGamePlayUI()
    {
        gamePlayUI.SetActive(false);
    }

    private void UpdateEndingResultValues()
    {
        int i = 0;
        foreach (KeyValuePair<string, float> bacteriaKilled in mini2Manager.bacteriaKilledDictionary)
        {
            individualScore[i].text = bacteriaKilled.Value.ToString();
            i++;
        }

        totalScore.text = scoreText.text;
    }

    private void HandleBackToWorldClicked()
    {
        if(GameManager.Instance.finishedMiniCount < 2)
        {
            GameManager.Instance.AddCompleteMiniCount();
        }
        GameManager.Instance.StartGame();
        SoundManager.Instance.StopMusic();
        backToWorldButton.interactable = false;
        // SoundManager.Instance.PlayMusic("InWorld_BGM", true);
    }

    private Sprite SetResultBackgroundImage(float scoreValue)
    {
        SwappingImages resultBackgroundImage = Array.Find(resultBg, bg => bg.imageName == "Bronze");
        
        if(scoreValue >= 20f)
        {
            resultBackgroundImage = Array.Find(resultBg, bg => bg.imageName == "Gold");
        }
        else if(scoreValue >= 10f)
        {
            resultBackgroundImage = Array.Find(resultBg, bg => bg.imageName == "Silver");
        }
        
        return resultBackgroundImage.image;
    }

    public void ChangeMapToImage(string imageName)
    {
        SwappingImages currentAreaImage = Array.Find(mapImages, image => image.imageName == imageName);
        
        if(currentAreaImage == null)
        {
            Debug.LogError("Image not Found");
        }
        else
        {
            mapImage.sprite = currentAreaImage.image;
        }
        
    }

    private IEnumerator DisplayInstructions()
    {
        while(!mini2Manager.touchedProceedInstruction)
        {
            leftInstruction.SetActive(true);
            rightInstruction.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            leftInstruction.SetActive(false);
            rightInstruction.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            if(mini2Manager.canTouch)
            {
                instructionContinueHint.SetActive(true);
            }
        }

        yield return StartCoroutine(DisplayGoal());

        yield return StartCoroutine(CountdownToResume());
        // instruction closed
        mini2Manager.instructionTrigger = false;
    }

    private IEnumerator DisplayGoal()
    {
        leftInstruction.SetActive(false);
        rightInstruction.SetActive(false);
        instructionContinueHint.SetActive(false);
        goalInstruction.SetActive(true);
        yield return new WaitForSeconds(5f);
        goalInstruction.SetActive(false);
        SoundManager.StartFade(0.5f, 0f);
    }

    private IEnumerator CountdownToResume()
    {
        SoundManager.Instance.StopMusic();
        countdownText.SetActive(true);
        TextMeshProUGUI counterText = countdownText.GetComponent<TextMeshProUGUI>();
        float countdownTimer = 3f;

        while(countdownTimer > 0f)
        {
            counterText.text = countdownTimer.ToString();
            SoundManager.Instance.PlaySFX(countdownSFX, mini2Manager.transform, 1f, false);
            yield return new WaitForSeconds(1f);
            countdownTimer -= 1f;
        }

        countdownText.SetActive(false);
        SoundManager.Instance.PlayMusic("Mini2_BGM", false);
    }
}
