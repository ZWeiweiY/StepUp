using UnityEngine;
using UnityEngine.Events;

public class Mini3Manager : MonoBehaviour
{
    public enum Mini3State
    {
        Intro,
        GamePlay,
        GameOver
    }

    public class Mini3StateChangeEvent : UnityEvent<Mini3State> { }

    private Mini3State _currentMini3State;

    public Mini3StateChangeEvent onMini3StateChanged;

    public bool passedGame {get; private set;}

    public Mini3State CurrentMini3State
    {
        get{ return _currentMini3State; }
        private set
        {
            _currentMini3State = value;
            HandleMini3StateChange(_currentMini3State);
        }
    }

    private void HandleMini3StateChange(Mini3State newState)
    {
        switch(newState)
        {
            case Mini3State.Intro:
                //Debug.Log("Mini3Manager: [State] Intro!");
                SoundManager.Instance.PlayMusic("Mini3_BGM", true);
                break;
            case Mini3State.GamePlay:
                //Debug.Log("Mini3Manager: [State] Play!");
                break;
            case Mini3State.GameOver:
                //Debug.Log("Mini3Manager: [State] Over!");
                SoundManager.Instance.StopMusic();
                // Add ending bgm
                break;
            
        }
        
        onMini3StateChanged.Invoke(newState);
    }
    private void Awake()
    {
        if(onMini3StateChanged == null)
        {
            onMini3StateChanged = new Mini3StateChangeEvent();
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        CurrentMini3State = Mini3State.Intro;
    }

    public void StartGamePlay()
    {
        CurrentMini3State = Mini3State.GamePlay;
        passedGame = false;
    }

    public void TriggerGameOver(bool finished)
    {
        passedGame = finished;
        CurrentMini3State = Mini3State.GameOver;
        
    }

}
