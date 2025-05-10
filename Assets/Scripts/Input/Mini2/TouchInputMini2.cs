using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class TouchInputMini2 : MonoBehaviour
{
    public Vector2 movementAmount { get; private set; }

    [SerializeField] private UIMini2 mini2UIManager;
   
    private Finger curFinger;
    private Vector2 curFingerPosition;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        SubscribeToMini2Events();
    }

    private void OnDisable()
    {
        UnsubscribeFromMini2Events();
        EnhancedTouchSupport.Disable();
    }

    private void SubscribeToMini2Events()
    {
        ETouch.Touch.onFingerDown += HandleMini2FingerDown;
        ETouch.Touch.onFingerUp += HandleMini2FingerUp;
        ETouch.Touch.onFingerMove += HandleMini2FingerMove;
    }

    private void UnsubscribeFromMini2Events()
    {
        ETouch.Touch.onFingerDown -= HandleMini2FingerDown;
        ETouch.Touch.onFingerUp -= HandleMini2FingerUp;
        ETouch.Touch.onFingerMove -= HandleMini2FingerMove;
    }

    private void Update()
    {
        if (curFinger != null && !curFinger.isActive)
        {
            UpdateToLatestActiveFinger();
        }

        if (curFinger != null && curFinger.isActive)
        {
            UpdateFingerPosition(curFinger.currentTouch);
            MovementMapping(curFingerPosition);
        }
    }

    private void UpdateToLatestActiveFinger()
    {
        var activeTouches = ETouch.Touch.activeTouches;
        
        if (activeTouches.Count > 0)
        {
            curFinger = activeTouches[activeTouches.Count - 1].finger;
        }
        else
        {
            ResetTouchState();
        }
    }

    private void HandleMini2FingerDown(Finger finger)
    {
        curFinger = finger;
    }

    private void HandleMini2FingerUp(Finger finger)
    {
        if (curFinger == finger)
        {
            UpdateToLatestActiveFinger();
        }
    }

    private void HandleMini2FingerMove(Finger finger)
    {
        if (finger.isActive)
        {
            curFinger = finger;
        }
    }

    private void UpdateFingerPosition(ETouch.Touch currentTouch)
    {
        curFingerPosition = currentTouch.screenPosition;
    }

    private void ResetTouchState()
    {
        curFinger = null;
        movementAmount = Vector2.zero;
        curFingerPosition = Vector2.zero;
    }

    private void MovementMapping(Vector2 fingerPosition)
    {
        if (fingerPosition.x < Screen.width / 2f)
        {
            movementAmount = new Vector2(-1f, 0f);
        }
        else if (fingerPosition.x > Screen.width / 2f)
        {
            movementAmount = new Vector2(1f, 0f);
        }
        else
        {
            movementAmount = Vector2.zero;
        }
    }
}