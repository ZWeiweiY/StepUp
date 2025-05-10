using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIMini1 : MonoBehaviour
{
    [Tooltip("Mini1 UI")]
    [SerializeField] private GameObject instructionUI;
    [SerializeField] private GameObject gamePlayUI;

    [Header("Step")]
    [SerializeField] private TextMeshProUGUI stepText;

    [Header("Ending")]
    [SerializeField] private GameObject endingBg;
    [SerializeField] private Button backToWorldButton;
    // [Header("UI Sound")]
    // [SerializeField] private Sound clickOnMedalSFX;

    private Mini1Manager mini1Manager;

    // Start is called before the first frame update
    private void Start()
    {
        mini1Manager = FindObjectOfType<Mini1Manager>();
        mini1Manager.onMini1StateChanged.AddListener(HandleMini1StateChanged);
        backToWorldButton.onClick.AddListener(HandleBackToWorldClicked);
    }

    private void HandleMini1StateChanged(Mini1Manager.Mini1State newState)
    {
        if(newState == Mini1Manager.Mini1State.Intro)
        {
            HideGameplayUI();
        }

        if(newState == Mini1Manager.Mini1State.GamePlayInteract)
        {
            ShowGamePlayUI();
            UpdateStepCount();
        }

        if(newState == Mini1Manager.Mini1State.Complete)
        {
            StartCoroutine(ShowEndingPage());
        }
    }

    private void HandleBackToWorldClicked()
    {
        if(GameManager.Instance.finishedMiniCount < 1)
        {
            GameManager.Instance.AddCompleteMiniCount();
        }
        GameManager.Instance.StartGame();
        SoundManager.Instance.StopMusic();
        backToWorldButton.interactable = false;
        // SoundManager.Instance.PlayMusic("InWorld_BGM", true);
    }

    public IEnumerator DisplayInstructions()
    {
        instructionUI.SetActive(true);
        yield return new WaitForSeconds(5f);
        instructionUI.SetActive(false);
    }

    private void ShowGamePlayUI()
    {
        gamePlayUI.SetActive(true);
    }

    private void HideGameplayUI()
    {
        gamePlayUI.SetActive(false);
    }

    private void UpdateStepCount()
    {
        stepText.text = Mathf.Min(mini1Manager._currentRound, mini1Manager.totalRounds).ToString();
    }

    private IEnumerator ShowEndingPage()
    {
        yield return new WaitForSeconds(5f);
        while(!mini1Manager.endingCameraMovement)
        {
            yield return null;
        }
        HideGameplayUI();
        endingBg.SetActive(true);
    }
}
