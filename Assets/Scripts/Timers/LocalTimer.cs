using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalTimer
{
    private bool paused = true;
    private bool stopped = false;

    public float timeToEnd { get; private set; }
    public Queue<Action> actions = new();

    public LocalTimer(float totalTime)
    {
        timeToEnd = totalTime;
    }

    public LocalTimer Start()
    {
        paused = false;
        return this;
    }

    public LocalTimer Pause()
    {
        paused = true;
        return this;
    }

    public LocalTimer Stop()
    {
        stopped = true;
        timeToEnd = 0;
        return this;
    }

    public void UpdateTime()
    {
        if (paused || GameTimer.TimeMultiplier == GameTimer.STOPPED)
            return;

        timeToEnd -= Time.deltaTime * GameTimer.TimeMultiplier;
        if (timeToEnd <= 0f && !stopped)
        {
            OnEnd();
        }
    }

    private void OnEnd()
    {
        for (int i = 0; i < actions.Count; ++i)
        {
            Action a = actions.Dequeue();
            a.Invoke();
        }
    }

    public LocalTimer DoAfter(Action action)
    {
        actions.Enqueue(action);
        return this;
    }
}
