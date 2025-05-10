using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class Mini2Manager : MonoBehaviour
{
    public enum Mini2State
    {
        Intro,
        Instruction,
        GamePlay,
        GameOver
    }

    public class Mini2StateChangeEvent : UnityEvent<Mini2State> { }

    private Mini2State _currentMini2State;

    public Mini2StateChangeEvent onMini2StateChanged;

    public Mini2State CurrentMini2State
    {
        get{ return _currentMini2State; }
        private set
        {
            _currentMini2State = value;
            HandleMini2StateChange(_currentMini2State);
        }
    }
    private void HandleMini2StateChange(Mini2State newState)
    {
        switch(newState)
        {
            case Mini2State.Intro:
                //Debug.Log("Mini2Manager: [State] Intro!");
                break;
            case Mini2State.Instruction:
                //Debug.Log("Mini2Manager: [State] Instruction!");
                StartInstructionFreezeCounter();
                break;
            case Mini2State.GamePlay:
                //Debug.Log("Mini2Manager: [State] Play!");
                break;
            case Mini2State.GameOver:
                //Debug.Log("Mini2Manager: [State] Over!");
                SoundManager.Instance.StopMusic();
                break;
        }

        onMini2StateChanged.Invoke(newState);
    }


    public float timeRemaining {get; private set;}
    public float currentScore {get; private set;}
    public Dictionary<string, float> bacteriaKilledDictionary {get; private set;}
    
    public bool instructionPlayed {get; private set;}
    public bool touchedProceedInstruction {get; private set;}
    public bool canTouch {get; private set;}
    [SerializeField] private float gameDuration = 120f;
    // [SerializeField] private VideoPlayer mini2IntroVideoPlayer;
    [SerializeField] private TouchInputMini2 touchInputManager;
    [SerializeField] private BacteriaManager bacteriaManager;
    [SerializeField] private GameObject environment;



    [HideInInspector]
    public bool instructionTrigger;

    private Mini2Camera mini2Camera;

    public void AddPoints(BacteriaDataSO bacteriaData)
    {
        currentScore += bacteriaData.value;
        bacteriaKilledDictionary[bacteriaData.bacteriaName]++;
    }


    private void Awake()
    {
        if(onMini2StateChanged == null){
            onMini2StateChanged = new Mini2StateChangeEvent();
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        mini2Camera = GetComponent<Mini2Camera>();
        CurrentMini2State = Mini2State.Intro;
        timeRemaining = gameDuration;
        currentScore = 0f;

        instructionTrigger = false;
        instructionPlayed = false;
        touchedProceedInstruction = false;

        InitializeKillDictionary();
        EnterGame();
    }

    // Update is called once per frame
    private void Update()
    {
        if(_currentMini2State == Mini2State.GamePlay)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                CurrentMini2State = Mini2State.GameOver;
            }
        }
        if(instructionTrigger && !instructionPlayed && _currentMini2State != Mini2State.Instruction)
        {
            CurrentMini2State = Mini2State.Instruction;
            instructionPlayed = true;
        }

        if(touchInputManager.movementAmount != Vector2.zero && canTouch)
        {
            touchedProceedInstruction = true;
        }

        if(_currentMini2State == Mini2State.Instruction && !instructionTrigger)
        {
            CurrentMini2State = Mini2State.GamePlay;
        }
        // Debug.Log(touchedProceedInstruction);
    }

    private void InitializeKillDictionary()
    {
        bacteriaKilledDictionary = new Dictionary<string, float>();
        if(bacteriaManager != null && bacteriaManager.bacteriaSOs != null)
        {
            BacteriaDataSO[] bacteriasList = bacteriaManager.bacteriaSOs;

            // Debug.Log(bacteriasList.Length);
            
            foreach (BacteriaDataSO bacteriaSO in bacteriasList)
            {
                bacteriaKilledDictionary[bacteriaSO.bacteriaName] = 0f;
            }

            // Debug.Log(bacteriaKilledDictionary.Count);
        }
        
    }
    private void EnterGame()
    {
        SoundManager.Instance.PlayMusic("Mini2_Intro", true); 
        StartCoroutine(StartIntro());
    }

    private IEnumerator StartIntro()
    {
        // Debug.Log("Start Game in 5 seconds");
        environment.SetActive(false);
        yield return StartCoroutine(PlayPortalAnimation());
        environment.SetActive(true);
        //Debug.Log("Mini2Manager: IntroVideoFinished");
        // mini2Camera.ChangeToGamePlayCamera();
        // yield return new WaitForSeconds(mini2Camera.cameraSwitchTime);
        
        
        // yield return new WaitForSeconds(5f);
        // Debug.Log("Waiting done");
        VideoManager.Instance.CloseVideoScreen();

        yield return new WaitForSeconds(1f);
        VideoManager.Instance.CloseVideoScreen();

        mini2Camera.ChangeToFallingCamera();
        // yield return new WaitForSeconds(mini2Camera.cameraSwitchTime);

        
        CurrentMini2State = Mini2State.GamePlay;
        
        yield return new WaitForSeconds(4f);
        mini2Camera.ChangeToGamePlayCamera();
        mini2Camera.DestroyIntroCameras();
    }

    private IEnumerator PlayPortalAnimation()
    {
        VideoManager.Instance.SetVideoPlaybackSpeed(1.5f);
        VideoManager.Instance.PlayVideo();

        yield return new WaitForSeconds(1f);

        while (VideoManager.Instance.IsVideoPlaying())
        {
            yield return null;
        }
        //Debug.Log("Video Done Playing");
    }

    private void StartInstructionFreezeCounter()
    {
        StartCoroutine(FreezeInstruction());
    }

    private IEnumerator FreezeInstruction()
    {
        yield return new WaitForSeconds(5f);
        canTouch = true;
    }
}
