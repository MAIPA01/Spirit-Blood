using UnityEngine;
using NaughtyAttributes;

public delegate void GameTimeChanged();

public class GameTimer : MonoBehaviour
{
    public static readonly float STOPPED = 0f;
    public static readonly float PLAYING = 1f;

    public static float TimeMultiplier { get; private set; } = PLAYING;

    public static event GameTimeChanged OnStopped = null;
    public static event GameTimeChanged OnStart = null;

    [Button]
    public static void StopTime()
    {
        TimeMultiplier = STOPPED;
        OnStopped?.Invoke();
    }

    [Button]
    public static void StartTime()
    {
        TimeMultiplier = PLAYING;
        OnStart?.Invoke();
    }
}
