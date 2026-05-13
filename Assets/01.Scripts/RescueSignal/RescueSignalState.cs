using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class RescueSignalState : MonoBehaviour
{
    private readonly Dictionary<string, RescueSignalRecord> _activeSignals = new Dictionary<string, RescueSignalRecord>();

    public event Action SignalChanged;

    public IReadOnlyCollection<RescueSignalRecord> ActiveSignals => _activeSignals.Values;

    public void RegisterSignal(string signalId, RescueSignalType signalType, Vector2Int cellPosition)
    {
        if (string.IsNullOrWhiteSpace(signalId))
        {
            Debug.LogWarning($"{nameof(RescueSignalState)}: SignalId가 비어 있습니다.");
            return;
        }

        if (_activeSignals.ContainsKey(signalId))
        {
            return;
        }

        // 맵 UI에 표시할 구조 신호를 등록한다.
        _activeSignals.Add(signalId, new RescueSignalRecord(signalId, signalType, cellPosition));

        SignalChanged?.Invoke();

        Debug.Log($"구조 신호 감지: {signalId}, Type: {signalType}, Cell: {cellPosition}");
    }

    public void ResolveSignal(string signalId)
    {
        if (string.IsNullOrWhiteSpace(signalId))
        {
            return;
        }

        if (!_activeSignals.Remove(signalId))
        {
            return;
        }

        // 구조 신호가 해결되었으므로 맵 UI에서 제거한다.
        SignalChanged?.Invoke();

        Debug.Log($"구조 신호 비활성화: {signalId}");
    }

    public bool HasSignal(string signalId)
    {
        return !string.IsNullOrWhiteSpace(signalId) && _activeSignals.ContainsKey(signalId);
    }

    public void ClearAll()
    {
        _activeSignals.Clear();
        SignalChanged?.Invoke();
    }
}