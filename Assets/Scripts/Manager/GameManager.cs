using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager>
{

    // public string playerId;

    public int finishedMiniCount {get; private set;}
    public bool endingActivated {get; private set;}
    // public Vector3 spawnPosition {get; private set;}

    // private Vector3[] spawnPositionsAfterGame;
    [SerializeField] private GameObject endingScene;
    [SerializeField] private GameObject creditsScene;
    [SerializeField] private Button backToMainMenuButton;
    public enum GameState
    {
        MainMenu,
        InWorld,
        MiniGame1,
        MiniGame2,
        MiniGame3,
        Ending
    }

    public class GameStateChangeEvent : UnityEvent<GameState> { }

    private GameState _currentState;

    public GameStateChangeEvent onGameStateChanged;

    public GameState CurrentState
    {
        get { return _currentState; }
        private set
        {
            _currentState = value;
            HandleGameStateChange(_currentState);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (onGameStateChanged == null)
            onGameStateChanged = new GameStateChangeEvent();
    }

    private void HandleGameStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                //Debug.Log("GameManager: MainMenu");
                //LevelManager.Instance.LoadScene(LevelManager.Scenes.MainMenuScene.ToString());
                break;
            case GameState.InWorld:
                //Debug.Log("GameManager:InWorld");
                //LevelManager.Instance.LoadScene(LevelManager.Scenes.WorldScene.ToString());
                break;
            case GameState.MiniGame1:
                //Debug.Log("GameManager:MiniGame1");
                //LevelManager.Instance.LoadScene(LevelManager.Scenes.Mini1Scene.ToString());
                break;
            case GameState.MiniGame2:
                //Debug.Log("GameManager:MiniGame2");
                //LevelManager.Instance.LoadScene(LevelManager.Scenes.Mini2Scene.ToString());
                break;
            case GameState.MiniGame3:
                //Debug.Log("GameManager:MiniGame3");
                //LevelManager.Instance.LoadScene(LevelManager.Scenes.Mini3Scene.ToString());
                break;
            case GameState.Ending:
                //Debug.Log("GameManager:Ending");
                //LevelManager.Instance.LoadScene(LevelManager.Scenes.Mini3Scene.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        onGameStateChanged.Invoke(newState);
    }

    public void StartGame()
    {
        LevelManager.Instance.LoadScene(LevelManager.Scenes.WorldScene);
        CurrentState = GameState.InWorld;
        SoundManager.Instance.PlayMusic("InWorld_BGM", true);
        // Debug.Log("StartGame");

    }

    public void ReturnToMainMenu()
    {
        CurrentState = GameState.MainMenu;
        LevelManager.Instance.LoadScene(LevelManager.Scenes.MainMenuScene);
        // Debug.Log("ReturnToMainMenu");

    }

    public void StartMiniGame1()
    {
        CurrentState = GameState.MiniGame1;
        LevelManager.Instance.LoadScene(LevelManager.Scenes.Mini1Scene);

    }

    public void StartMiniGame2()
    {
        CurrentState = GameState.MiniGame2;
        LevelManager.Instance.LoadScene(LevelManager.Scenes.Mini2Scene);
        // Debug.Log("StartMiniGame2");

    }

    public void StartMiniGame3()
    {
        CurrentState = GameState.MiniGame3;
        LevelManager.Instance.LoadScene(LevelManager.Scenes.Mini3Scene);
        // Debug.Log("StartMiniGame3");

    }

    public void AddCompleteMiniCount()
    {
        finishedMiniCount++;
        // spawnPosition = spawnPositionsAfterGame[finishedMiniCount];

        if(finishedMiniCount == 3 && !endingActivated)
        {
            TriggerEndingActivation();
        }
        else
        {
            endingScene.SetActive(false);
        }
    }

    public void TriggerEndingActivation()
    {
        endingActivated = true;
        StartCoroutine(PlayEnding());
        //Debug.Log("Ending activation is now " + endingActivated);
    }

    private void Start()
    {
        // playerId = Guid.NewGuid().ToString();
        // Debug.Log("GameManager: Player ID is: " + playerId);
        finishedMiniCount = 0;
        endingActivated = false;

        // InitializeSpawnPositions();

        endingScene.SetActive(false);
        backToMainMenuButton.onClick.AddListener(HandleMainMenuClicked);
    }

    // private void InitializeSpawnPositions()
    // {
    //     spawnPositionsAfterGame[0] = new Vector3(-11.872f, 4.12f, -26.741f);
    //     spawnPositionsAfterGame[1] = new Vector3(-3.15f, 2.02f, -8.38f);
    //     spawnPositionsAfterGame[2] = new Vector3(-6.9f, -1.09f, 6.03f);
    //     spawnPositionsAfterGame[3] = new Vector3(-11.872f, 4.12f, -26.741f);
    // }

    private void Update()
    {
    
    }

    private void HandleMainMenuClicked()
    {
        endingScene.SetActive(false);
        ReturnToMainMenu();
    }

    private IEnumerator PlayEnding()
    {
        yield return StartCoroutine(PlayEndingAnimation());
        //Debug.Log("GAMEMANAGER: Ending Video Done Playing");
        VideoManager.Instance.CloseVideoScreen();
        SoundManager.Instance.PlayMusic("InWorld_BGM", true);
        StartCoroutine(ShowCreditsPage());
        
    }

    private IEnumerator PlayEndingAnimation()
    {
        yield return new WaitForSeconds(5f);
        VideoManager.Instance.SetVideoPlaybackSpeed(1f);
        CurrentState = GameState.Ending;
        SoundManager.Instance.StopMusic();
        VideoManager.Instance.PlayVideo();
        yield return new WaitForSeconds(1f);

        while (VideoManager.Instance.IsVideoPlaying())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        //Debug.Log("Video Done Playing");
    }

    private IEnumerator ShowCreditsPage()
    {
        endingScene.SetActive(true);
        creditsScene.SetActive(true);
        yield return new WaitForSeconds(8f);
        creditsScene.SetActive(false);
    }

}
