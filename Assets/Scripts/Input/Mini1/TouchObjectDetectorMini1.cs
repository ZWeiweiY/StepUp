using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchObjectDetectorMini1 : MonoBehaviour
{
    public static event Action<GameObject> OnObjectTouched;

    public GameObject touchedObject;

    [SerializeField] private string medalTag = "Mini1MedalInteractable";

    private void OnEnable()
    {
        TouchInputMini1.OnInputDetected += DetectTouchObject;
    }

    private void OnDisable()
    {
        TouchInputMini1.OnInputDetected -= DetectTouchObject;
    }
    private void DetectTouchObject(Vector2 inputPosition)
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            touchedObject = hit.collider.gameObject;

            if(touchedObject.CompareTag(medalTag))
            {
                OnObjectTouched?.Invoke(touchedObject);
            }
        }

    }
}
