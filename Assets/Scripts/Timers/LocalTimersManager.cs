using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalTimerContainer
{
    private LocalTimer timer = null;

    public LocalTimerContainer(ref LocalTimer timer)
    {
        this.timer = timer;
    }

    public LocalTimerContainer Start()
    {
        timer.Start();
        return this;
    }

    public LocalTimerContainer Pause()
    {
        timer.Pause();
        return this;
    }

    public LocalTimerContainer Stop()
    {
        timer.Stop();
        return this;
    }

    public LocalTimerContainer DoAfter(Action action)
    {
        timer.DoAfter(action);
        return this;
    }
}

public class LocalTimersManager : MonoBehaviour
{
    public static LocalTimersManager instance = null;
    private List<LocalTimer> timers = new();
    private List<LocalTimer> toAddTimers = new();

    private static LocalTimersManager Init()
    {
        if (instance != null)
        {
            return instance;
        }

        var obj = new GameObject("Local Timers Handler");
        instance = obj.AddComponent<LocalTimersManager>();
        return instance;
    }

    // Update is called once per frame
    void Update()
    {
        timers.AddRange(toAddTimers);
        toAddTimers.Clear();

        if (timers.Count == 0)
        {
            return;
        }

        List<LocalTimer> newTimers = new();
        newTimers.AddRange(timers);
        foreach (var timer in timers)
        {
            timer.UpdateTime();
            if (timer.timeToEnd <= 0f)
            {
                newTimers.Remove(timer);
            }
        }
        timers = newTimers;
    }

    public static LocalTimerContainer CreateNewTimer(float totalTime)
    {
        LocalTimersManager timersHandler = LocalTimersManager.Init();
        LocalTimer newTimer = new(totalTime);
        timersHandler.toAddTimers.Add(newTimer);
        return new LocalTimerContainer(ref newTimer);
    }
}
