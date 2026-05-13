using UnityEngine;

public readonly struct MapStampRecord
{
    public MapStampRecord(MapStampType stampType, Vector2 mapPosition)
    {
        StampType = stampType;
        MapPosition = mapPosition;
    }

    public MapStampType StampType { get; }
    public Vector2 MapPosition { get; }
}