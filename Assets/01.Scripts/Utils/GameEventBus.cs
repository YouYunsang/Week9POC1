using System;
using UnityEngine;

public static class GameEventBus
{
    public static event Action ReturnToBaseRequested;
    public static event Action PlayerDied;
    public static event Action<GameResultType> RunEnded;
    public static event Action<NoiseEventData> NoiseEmitted;
    public static event Action<Vector2> MutantJumpScareStarted;

    public static void RaiseReturnToBaseRequested()
    {
        ReturnToBaseRequested?.Invoke();
    }

    public static void RaisePlayerDied()
    {
        PlayerDied?.Invoke();
    }

    public static void RaiseRunEnded(GameResultType resultType)
    {
        RunEnded?.Invoke(resultType);
    }

    public static void RaiseNoiseEmitted(NoiseEventData noiseEventData)
    {
        NoiseEmitted?.Invoke(noiseEventData);
    }

    public static void RaiseMutantJumpScareStarted(Vector2 position)
    {
        // 변이체가 점프스케어 접근을 시작했음을 알린다.
        MutantJumpScareStarted?.Invoke(position);
    }
}