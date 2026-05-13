using UnityEngine;

public readonly struct RescueSignalRecord
{
    public RescueSignalRecord(string signalId, RescueSignalType signalType, Vector2Int cellPosition)
    {
        SignalId = signalId;
        SignalType = signalType;
        CellPosition = cellPosition;
    }

    public string SignalId { get; }
    public RescueSignalType SignalType { get; }
    public Vector2Int CellPosition { get; }
}