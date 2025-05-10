using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class TouchInputMini3 : MonoBehaviour
{
    [HideInInspector]
    public bool tappedOnScreen;


    [SerializeField] private UIMini3 mini3UIManager;

    // private Finger curFinger;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        SubscribeToMini3Events();
    }

    private void OnDisable()
    {
        UnsubscribeFromMini3Events();
        EnhancedTouchSupport.Disable();
    }

    private void SubscribeToMini3Events()
    {
        ETouch.Touch.onFingerDown += HandleMini3FingerDown;
        ETouch.Touch.onFingerUp += HandleMini3FingerUp;
        ETouch.Touch.onFingerMove += HandleMini3FingerMove;
    }

    private void UnsubscribeFromMini3Events()
    {
        ETouch.Touch.onFingerDown -= HandleMini3FingerDown;
        ETouch.Touch.onFingerUp -= HandleMini3FingerUp;
        ETouch.Touch.onFingerMove -= HandleMini3FingerMove;
    }


    private void HandleMini3FingerDown(Finger finger)
    {
        tappedOnScreen = true;
    }

    private void HandleMini3FingerUp(Finger finger)
    {
        tappedOnScreen = false;
    }

    private void HandleMini3FingerMove(Finger finger)
    {
        // tappedOnScreen = false;
    }

    public void TriggerTapOnScreen()
    {
        tappedOnScreen = !tappedOnScreen;
    }

//     private void ResetTouchState()
//     {
//         curFinger = null;
//     }
}
