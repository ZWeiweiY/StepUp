using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class Mini1Manager : MonoBehaviour
{

    public int totalRounds {get; private set;}
    [SerializeField] float roundAssestSize = 35.0f;
    public int _currentRound {get; private set;}

    [HideInInspector]
    public bool endingCameraMovement = false;

    [SerializeField] private Jumper jumper;
    [SerializeField] private Transform jumperVisual;
    [SerializeField] private Mini1Camera mini1Camera;

    [SerializeField] private Sound bounceSound;
    [SerializeField] private Sound successSound;
    [SerializeField] private Sound crowdSound;

    private float jumperHeight = 1f;
    private float firstJumpHeight = 5.0f;
    private float firstJumpDuration = 1.0f;

    [SerializeField] private Transform[] boardTransforms;

    [SerializeField] private MedalParent[] medalParents;

    private float boardToNextDistance = 15.0f;

    // [SerializeField] private Mini1Animation mini1Animation;
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private GameObject environment;
    [SerializeField] private GameObject crowdPrefab;
    [SerializeField] private GameObject bookParent;
    [SerializeField] private Animator bookAnimator;
    [SerializeField] private string bookAnimatorStateName = "bookAnimation";
    [SerializeField] private float totalBookAnimationFrames = 1100f;
    [SerializeField] private float pauseAtFrame = 600f;
    [SerializeField] private float bookAnimationSpeed = 5f;

    private float lerpTime= 1f;

    public enum Mini1State
    {
        Intro,
        GamePlayInteract,
        GamePlayJump1,
        GamePlayJump2,
        Complete
    }

    public class Mini1StateChangeEvent : UnityEvent<Mini1State> { }

    private Mini1State _currentMini1State;

    public Mini1StateChangeEvent onMini1StateChanged;

    public Mini1State CurrentMini1State
    {
        get{ return _currentMini1State; }
        private set
        {
            _currentMini1State = value;
            HandleMini1StateChange(_currentMini1State);
        }
    }

    private void Awake()
    {
        if(onMini1StateChanged == null){
            onMini1StateChanged = new Mini1StateChangeEvent();
        }
    }

    private void HandleMini1StateChange(Mini1State newState)
    {
        switch(newState)
        {
            case Mini1State.Intro:
                //Debug.Log("Mini1Manager: [State] Intro!");
                break;
            case Mini1State.GamePlayInteract:
                //Debug.Log("Mini1Manager: [State] Play Interact!");
                // UnsubscribeIntroTouch();
                break;
            case Mini1State.GamePlayJump1:
                //Debug.Log("Mini1Manager: [State] Play Jump1!");
                break;
            case Mini1State.GamePlayJump2:
                //Debug.Log("Mini1Manager: [State] Play Jump2!");
                break;
            case Mini1State.Complete:
                //Debug.Log("Mini1Manager: [State] Complete!");
                UnsubscribeGamePlayTouch();
                break;
        }

        onMini1StateChanged.Invoke(newState);
    }

    private UIMini1 uiManager;
    // Start is called before the first frame update
    private void Start()
    {
        uiManager = FindObjectOfType<UIMini1>();
        // Debug.Log("Mini1Manager: I was Created!");
        CurrentMini1State = Mini1State.Intro;
        totalRounds = 5;
        _currentRound = 1;

        // Tutorial Coroutine
        // mini1IntroVideoPlayer.playOnAwake = false;
        // introVideoFrame.SetActive(false);
        // SubscribeIntroTouch();
        
        StartMiniGame1();

    }

    // Update is called once per frame
    private void Update()
    {
        // Debug.Log(mini1IntroVideoPlayer.frame);
        // Debug.Log(mini1IntroVideoPlayer.isPlaying);
    }

    private void OnDestroy(){
        // Debug.Log("Mini1Manager: I am destroyed");
    }

    private void StartMiniGame1()
    {
        SoundManager.Instance.PlayMusic("Mini1_BGM", true);
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        yield return StartCoroutine(BookOpen());
        yield return StartCoroutine(PlayIntroVideo());
        VideoManager.Instance.CloseVideoScreen();
        yield return StartCoroutine(BookClose());
        // SubscribeIntroTouch();
        StartCoroutine(FirstCameraMove());

    }

    private IEnumerator BookOpen()
    {
        bookAnimator.Play(bookAnimatorStateName, 0, 0f);
        bookAnimator.speed = bookAnimationSpeed;
        while (bookAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < (pauseAtFrame / totalBookAnimationFrames))
        {
            yield return null;
        }

        // Pause the animation
        bookAnimator.speed = 0;
        yield return new WaitForSeconds(1f);
        
        //Zoom in Camera
        mini1Camera.ChangeToIntroBookZoomInCamera();

        yield return new WaitForSeconds(mini1Camera.cameraSwitchTime + 1f);
        environment.SetActive(false);
        //Debug.Log("Book Opened!");
    }

    private IEnumerator PlayIntroVideo()
    {
        // yield return new WaitForSeconds(3f);
        VideoManager.Instance.SetVideoPlaybackSpeed(1.5f);
        VideoManager.Instance.PlayVideo();
        mini1Camera.ChangeToIntroBookZoomOutCamera();

        yield return new WaitForSeconds(1f);

        while(VideoManager.Instance.IsVideoPlaying())
        {
            yield return null;
        }
        //Debug.Log("Video Done Playing");

    }
    private IEnumerator BookClose()
    {
        environment.SetActive(true);
        Instantiate(crowdPrefab, environment.transform);
        VideoManager.Instance.CloseVideoScreen();
        yield return new WaitForSeconds(3f);
        VideoManager.Instance.CloseVideoScreen();
        mini1Camera.ChangeToIntroCamera();
        yield return new WaitForSeconds(mini1Camera.cameraSwitchTime + 1f);
        bookAnimator.speed = bookAnimationSpeed;
        // Wait for the animation to complete
        while (bookAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        //Debug.Log("Book Closed!");

        // Reset to avoid potential bugs
        // bookAnimator.speed = 1;
        // book.SetActive(false);
        bookAnimator.gameObject.SetActive(false);
    }

    // private void SubscribeIntroTouch()
    // {
    //     // Testing
    //     TouchInputMini1.OnInputDetected += HandleIntroToGame;
    // }

    // private void UnsubscribeIntroTouch()
    // {
    //     // Testing
    //     TouchInputMini1.OnInputDetected -= HandleIntroToGame;
    // }


    private void SubscribeGamePlayTouch()
    {
        TouchObjectDetectorMini1.OnObjectTouched += HandleMedalTouched;
    }

    private void UnsubscribeGamePlayTouch()
    {
        TouchObjectDetectorMini1.OnObjectTouched -= HandleMedalTouched;
    }


    private void EnterGamePlay()
    {
        if(_currentRound > totalRounds){
            // mini1Animation.SetGameOverCelebration(true);

            playerAnimator.SetBool("gameOver", true);
            //Debug.Log("Mini1Manager: No more rounds! Exit Game!");
            CurrentMini1State = Mini1State.Complete;
            mini1Camera.ChangeToEndingCamera();
            StartCoroutine(mini1Camera.TriggerEndingCameraMovement());
            SoundManager.Instance.PlaySFX(successSound, jumper.transform, 1f, false);
            SoundManager.Instance.PlaySFX(crowdSound, jumper.transform, 1f, false);
        }
        else
        {
            CurrentMini1State = Mini1State.GamePlayInteract;
            // StartCoroutine(SetupMedalsRandomPosition());
            mini1Camera.ChangeToInteractCamera();
            StartCoroutine(EnableTouchAfterCameraSet());
        }
    }

    private IEnumerator FirstCameraMove()
    {
        yield return StartCoroutine(PostIntroCamerMovement());
        // yield return new WaitForSeconds(mini1Camera.cameraSwitchTime);
        
        yield return StartCoroutine(uiManager.DisplayInstructions());
        
        EnterGamePlay();
    }


    private IEnumerator PostIntroCamerMovement()
    {
        mini1Camera.ChangeToResultCamera();
        yield return new WaitForSeconds(mini1Camera.cameraSwitchTime);
        Destroy(bookParent);
        yield return null;
    }
    private IEnumerator EnableTouchAfterCameraSet()
    {
        // yield return new WaitForSeconds(mini1Camera.cameraSwitchTime);
        yield return StartCoroutine(medalParents[_currentRound-1].TurnAndScatter());
        // yield return new WaitForSeconds(mini1Camera.cameraSwitchTime);
        SubscribeGamePlayTouch();
    }

    private void HandleMedalTouched(GameObject gameObject)
    {
        // Debug.Log("Object: " + gameObject.name[gameObject.name.Length - 1] +" is at " + GetObjectCenterPositionOnXZ(gameObject));
        UnsubscribeGamePlayTouch();
        StartCoroutine(HandleResult(gameObject));
    }

    // private void HandleIntroToGame(UnityEngine.Vector2 touchPosition)
    // {
    //     StartCoroutine(FirstCameraMove());
    //     // UnsubscribeIntroTouch();
    // }

    private IEnumerator HandleResult(GameObject gameObject)
    {   
        // Show outline here
        // gameObject.showOutline(); 
        
        int selectedIndex = int.Parse(gameObject.name);

        // Sink other medals
        yield return StartCoroutine(SinkNonPickedMedals(gameObject));

        

        // Debug.Log("Second jump compare: " + (selectedIndex == _currentRound));
        if(selectedIndex == _currentRound)
        {
            // mini1Animation.SetSuccessJump(true);
            playerAnimator.SetBool("success", true);
            // playerAnimator.SetBool("successJump", true);
            float successMultiplier = gameObject.transform.position.z < medalParents[_currentRound-1].transform.position.z ? 0.7f : 0.55f;
            // animator 
            playerAnimator.SetFloat("successMultiplier", successMultiplier);
        }
        else
        {
            // mini1Animation.SetSuccessJump(false);
            playerAnimator.SetBool("success", false);
            // playerAnimator.SetBool("successJump", false);
            float failMultiplier = gameObject.transform.position.z < medalParents[_currentRound-1].transform.position.z + 0.87f ? 1f : 1.5f;
            // animator 
            playerAnimator.SetFloat("failMultiplier", failMultiplier);
        }
        yield return StartCoroutine(FirstJump(gameObject));
        // Debug.Log("Mini1Manager: Start Jump 2!");
        SoundManager.Instance.PlaySFX(bounceSound, gameObject.transform, 1f, false);
        yield return StartCoroutine(SecondJump(gameObject));

        // Debug.Log("ResetState");
        EnterGamePlay();

    }

    private IEnumerator SinkNonPickedMedals(GameObject gameObject)
    {   
        float timeElapsed = 0;
        Vector3 targetPosition = gameObject.transform.localPosition;
        targetPosition.y -= 5f;

        while (timeElapsed < lerpTime)
        {   
            float t = timeElapsed / lerpTime;
            t = t * t * (3f - 2f * t);
            foreach (GameObject piece in medalParents[_currentRound-1].children)
            {
                if(piece != gameObject)
                {
                    piece.transform.localPosition = Vector3.Lerp(piece.transform.localPosition, targetPosition, t/50f);
                }
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FirstJump(GameObject gameObject)
    {
        CurrentMini1State = Mini1State.GamePlayJump1;
        jumper.SetJumpParameters(GetObjectCenterPositionOnXZ(gameObject), firstJumpHeight, firstJumpDuration);
        mini1Camera.ChangeToResultCamera();
        
        yield return new WaitForSeconds(mini1Camera.cameraSwitchTime);

        

        // Prepare Jump Animation 
        // yield return StartCoroutine(mini1Animation.PlayPrepareAnimations("medalSelected", "Prepare_swinghands", "Prepare_swinghands"));
        // yield return new WaitForSeconds(mini1Animation.animationDurations["Prepare_streching"]/2 + mini1Animation.animationDurations["Prepare_swinghands"]/2 );

        playerAnimator.SetTrigger("medalSelected");

        jumper.Jump();
        
        yield return new WaitForSeconds(firstJumpDuration);
        yield return null;
    }

    private IEnumerator SecondJump(GameObject gameObject)
    {        
        CurrentMini1State = Mini1State.GamePlayJump2;
        float nextRoundZPosition = roundAssestSize * _currentRound;
        Transform currentBoard = boardTransforms[_currentRound-1];
        UnityEngine.Vector3 nextRoundStart = new UnityEngine.Vector3(0f, 1.05f, nextRoundZPosition);

        int selectedIndex = int.Parse(gameObject.name);
        // Debug.Log("Second jump compare: " + (selectedIndex == _currentRound));
        if(selectedIndex == _currentRound)
        {
            //Debug.Log("Correct Jump!");
            yield return StartCoroutine(CorrectJump(CalculateSecondJumpParameters(nextRoundZPosition, currentBoard, nextRoundStart)));
        }
        else
        {
            //Debug.Log("Wrong Jump!");
            yield return StartCoroutine(WrongJump(CalculateSecondJumpParameters(nextRoundZPosition, currentBoard, nextRoundStart)));
        }

        yield return null;
    }

    private IEnumerator CorrectJump((UnityEngine.Vector3, List<float>) jumpParameters)
    {   
        jumper.SetJumpParameters(jumpParameters.Item1, jumpParameters.Item2[0] + jumperHeight, jumpParameters.Item2[1]);
        // Debug.Log("Jumping to " + jumpParameters.Item1);
        jumper.Jump();
        if(_currentRound > 1)
        {
            StartCoroutine(PerformJumpTricks(jumpParameters.Item2[1]));
        }
        
        yield return new WaitForSeconds(jumpParameters.Item2[1]);
        yield return new WaitForSeconds(mini1Camera.cameraSwitchTime);
        _currentRound += 1;
        // Debug.Log("Mini1Manager: Nice Jump! Begin Round " + _currentRound);
        yield return null;
    }

    private IEnumerator WrongJump((UnityEngine.Vector3, List<float>) jumpParameters)
    {
        float hitHeight = Mathf.Min(jumpParameters.Item2[0] - jumperHeight - UnityEngine.Random.Range(3f, 4f), boardTransforms[_currentRound - 1].localScale.y/2 +  boardTransforms[_currentRound - 1].position.y);
        jumper.SetJumpParameters(jumpParameters.Item1, hitHeight, jumpParameters.Item2[1]);
        // Debug.Log("Jumping to " + jumpParameters.Item1 + " will fail");
        jumper.Jump();
        yield return new WaitForSeconds(jumpParameters.Item2[1]);
        UnityEngine.Vector3 restartPosition = jumpParameters.Item1;
        restartPosition.z -= roundAssestSize;
        float recoverJumpDuration = 1f;
        jumper.SetJumpParameters(restartPosition, UnityEngine.Random.Range(3f, 4f), recoverJumpDuration);
        // mini1Animation.TriggerFailRecovery();
        playerAnimator.SetTrigger("failRecovery");

        jumper.Jump();
        float recoveryBuffer = 0.5f;    // time for recover animation
        yield return new WaitForSeconds(recoverJumpDuration + recoveryBuffer);  //
        yield return new WaitForSeconds(mini1Camera.cameraSwitchTime);
        // Debug.Log("Mini1Manager: Try Again! Begin Round " + _currentRound);
        yield return null;
    }

    private IEnumerator PerformJumpTricks(float duration)
    {

        float elapsedTime = 0f;
        int totalRotation = Random.Range(0, 2) == 0 ? 360 : -360; ; // Total degrees to rotate
        int randomAxisSelection = Random.Range(0, 2);
        while (elapsedTime < duration)
        {
            // Calculate how much to rotate this frame
            float rotationThisFrame = (totalRotation * Time.deltaTime) / duration;

            // Rotate the object around the Y-axis
            jumperVisual.Rotate(randomAxisSelection * rotationThisFrame, (randomAxisSelection ^ 1) * rotationThisFrame, 0, Space.World);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the object ends exactly at 360 degrees rotation (if necessary)
        jumperVisual.Rotate(0, totalRotation - (elapsedTime * (totalRotation / duration)), 0, Space.World);

    }

    private (UnityEngine.Vector3, List<float>) CalculateSecondJumpParameters(float nextRoundZPosition, Transform currentBoard, UnityEngine.Vector3 nextRoundStart)
    {
        List<float> secondJumpParameters;
        secondJumpParameters = new List<float>();

        //1
        float currentBoardDistance = nextRoundZPosition - boardToNextDistance - jumper.transform.position.z - (currentBoard.localScale.z / 2);
        // Debug.Log("Board z pos = " +  (nextRoundZPosition - boardToNextDistance) + " Thickness " + currentBoard.localScale.z);
        //2
        float currentBoardHeight = currentBoard.position.y + currentBoard.localScale.y / 2 - jumper.transform.position.y;
        // Debug.Log(currentBoard.localScale.y + " cbh " +  currentBoardHeight);
        //3
        float midPoint = (nextRoundStart - jumper.transform.position).z / 2;

        float resultHeight =  currentBoardHeight * midPoint / currentBoardDistance;

        float secondJumpDuration = UnityEngine.Random.Range(2f, 3f);

        secondJumpParameters.Add(resultHeight);
        secondJumpParameters.Add(secondJumpDuration);

        return (nextRoundStart, secondJumpParameters);
    }
    private UnityEngine.Vector3 GetObjectCenterPositionOnXZ(GameObject gameObject)
    {
        UnityEngine.Vector3 centerPosOnXZ;
        Renderer gameObjectRenderer = gameObject.GetComponent<Renderer>();

        centerPosOnXZ = gameObjectRenderer.bounds.center;
        // centerPosOnXZ.y = 0;
        centerPosOnXZ.y = gameObject.transform.position.y + jumperHeight;
        // Debug.Log("Jumping to" + centerPosOnXZ);
        return centerPosOnXZ;
    }
}

