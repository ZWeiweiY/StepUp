using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputMini1 : MonoBehaviour
{
    public static event Action<Vector2> OnInputDetected;

    public Touch touch;

    [SerializeField] private Mini1Manager mini1Manager;

    private void Start()
    {
        if(mini1Manager == null)
        {
            mini1Manager = new Mini1Manager();
        }
        
    }
    // Update is called once per frame
    private void Update()
    {
        if(mini1Manager.CurrentMini1State == Mini1Manager.Mini1State.GamePlayInteract || mini1Manager.CurrentMini1State == Mini1Manager.Mini1State.Intro)
        {
            if(Input.touchCount > 0){
                touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                    OnInputDetected?.Invoke(touch.position);
                }
            }
            // Editor Test
            else if(Input.GetMouseButton(0)) 
            {
                OnInputDetected.Invoke(Input.mousePosition);
            }
        }
       
    }
}
