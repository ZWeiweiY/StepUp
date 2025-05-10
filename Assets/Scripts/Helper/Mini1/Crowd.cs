using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    [Range(0, 1)] public float defaultSpeed;
    [Range(1, 5)] public float cheeringSpeed;
    [Range(0, 1)] public float randomnessFactor;

    public float maximumHeight;

    [HideInInspector] public float currentSpeedFactor;

    private void Awake()
    {
        currentSpeedFactor = defaultSpeed;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            UpdateState("Cheer");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdateState("Idle");
        }
    }
    private void UpdateState(string state)
    {
        switch (state)
        {
            case "Idle":
                currentSpeedFactor = defaultSpeed;

                break;
            case "Cheer":
                currentSpeedFactor = cheeringSpeed;
                break;

        }
    }
}