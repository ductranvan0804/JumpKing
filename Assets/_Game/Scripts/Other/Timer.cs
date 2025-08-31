using UnityEngine;
using UnityEngine.UI;
using System;

public class GameTimer : Singleton<GameTimer>
{
    private float elapsedTime = 0f;
    private bool isPaused = false;

    void Update()
    {
        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
        }
    }


    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SetElapsedTime(float time)
    {
        elapsedTime = time;
    }

    public void AddTime(float seconds)
    {
        elapsedTime += seconds;
    }
}
