using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class TouchInputInWorld : MonoBehaviour
{
    public Vector2 movementAmount { get; private set; }

    [SerializeField] private UIInWorld inWorldUIManager;
    [SerializeField] private Vector2 joyStickSize = new Vector2(200, 200);
    private Finger moveFinger;
    private const float JoystickDragThreshold = 0.1f;

    private void OnEnable()
    {   
        EnhancedTouchSupport.Enable();
        SubscribeToInWorldEvents();

    }

    private void OnDisable()
    {
        UnsubscribeFromInWorldEvents();
        EnhancedTouchSupport.Disable();
    }


    private void SubscribeToInWorldEvents()
    {
        ETouch.Touch.onFingerDown += HandleInWorldFingerDown;
        ETouch.Touch.onFingerUp += HandleInWorldFingerUp;
        ETouch.Touch.onFingerMove += HandleInWorldFingerMove;
    }

    private void UnsubscribeFromInWorldEvents()
    {
        ETouch.Touch.onFingerDown -= HandleInWorldFingerDown;
        ETouch.Touch.onFingerUp -= HandleInWorldFingerUp;
        ETouch.Touch.onFingerMove -= HandleInWorldFingerMove;
    }

    private void HandleInWorldFingerDown(Finger finger)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Ending)
        {
            if (moveFinger == null)
            {
                moveFinger = finger;
                movementAmount = Vector2.zero;
                inWorldUIManager.joystick.gameObject.SetActive(true);
                inWorldUIManager.joystick.RectTransform.position = ClampStartPosition(finger.screenPosition);
            }
        }
    }

    private void HandleInWorldFingerUp(Finger finger)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Ending)
        {
            if (moveFinger == finger)
            {
                ResetJoystick();
            }
        }
    }

    private void HandleInWorldFingerMove(Finger finger)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Ending)
        {
            if (moveFinger == finger)
            {
                UpdateJoystickPosition(finger.currentTouch);
            }
        }
    }

    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if(startPosition.x < inWorldUIManager.joystick.RectTransform.rect.width / 2)
        {
            startPosition.x = inWorldUIManager.joystick.RectTransform.rect.width / 2;
        }
        else if(startPosition.x > Screen.width - inWorldUIManager.joystick.RectTransform.rect.width / 2)
        {
            startPosition.x = Screen.width - inWorldUIManager.joystick.RectTransform.rect.width / 2;
        }
        
        if(startPosition.y < inWorldUIManager.joystick.RectTransform.rect.height / 2)
        {
            startPosition.y = inWorldUIManager.joystick.RectTransform.rect.height / 2;
        }
        else if(startPosition.y > Screen.height - inWorldUIManager.joystick.RectTransform.rect.height / 2)
        {
            startPosition.y = Screen.height - inWorldUIManager.joystick.RectTransform.rect.height / 2;
        }
        return startPosition;
    }

    private void ResetJoystick()
    {
        moveFinger = null;
        movementAmount = Vector2.zero;
        inWorldUIManager.joystick.Knob.anchoredPosition = Vector2.zero;
        inWorldUIManager.joystick.gameObject.SetActive(false);
    }

    private void UpdateJoystickPosition(ETouch.Touch currentTouch)
    {
        if (inWorldUIManager.joystick.RectTransform == null)
        {
            Debug.LogError("Joystick or its RectTransform is null!");
            return;
        }
        Vector2 joystickCenter = inWorldUIManager.joystick.RectTransform.position;
        float maxMovementDistance = joyStickSize.x / 2;
        Vector2 touchDelta = currentTouch.screenPosition - joystickCenter;
        
        Vector2 knobPosition = Vector2.ClampMagnitude(touchDelta, maxMovementDistance);
        
        inWorldUIManager.joystick.Knob.anchoredPosition = knobPosition;
        movementAmount = knobPosition / maxMovementDistance;

        if (movementAmount.magnitude < JoystickDragThreshold)
        {
            movementAmount = Vector2.zero;
        }
    }
}
