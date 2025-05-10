using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [HideInInspector]
    public float elapsedTime = 0f;
    public bool isRunning = false;

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public string GetFormattedTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
        return string.Format("{0:D2}:{1:D2}", 
            timeSpan.Minutes, 
            timeSpan.Seconds);
    }
}
