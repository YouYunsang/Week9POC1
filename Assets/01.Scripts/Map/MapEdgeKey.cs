using System;
using UnityEngine;

public readonly struct MapEdgeKey : IEquatable<MapEdgeKey>
{
    public MapEdgeKey(Vector2Int cellPosition, MapEdgeDirection direction)
    {
        CellPosition = cellPosition;
        Direction = direction;
    }

    public Vector2Int CellPosition { get; }
    public MapEdgeDirection Direction { get; }

    public static MapEdgeKey CreateNormalized(Vector2Int cellPosition, MapEdgeDirection direction)
    {
        switch (direction)
        {
            case MapEdgeDirection.Left:
                // 왼쪽 변은 왼쪽 칸의 오른쪽 변으로 통일한다.
                return new MapEdgeKey(cellPosition + Vector2Int.left, MapEdgeDirection.Right);

            case MapEdgeDirection.Down:
                // 아래쪽 변은 아래 칸의 위쪽 변으로 통일한다.
                return new MapEdgeKey(cellPosition + Vector2Int.down, MapEdgeDirection.Up);

            default:
                // 위쪽/오른쪽 변은 그대로 저장한다.
                return new MapEdgeKey(cellPosition, direction);
        }
    }

    public bool Equals(MapEdgeKey other)
    {
        return CellPosition.Equals(other.CellPosition) &&
               Direction == other.Direction;
    }

    public override bool Equals(object obj)
    {
        return obj is MapEdgeKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CellPosition, Direction);
    }

    public static bool operator ==(MapEdgeKey left, MapEdgeKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MapEdgeKey left, MapEdgeKey right)
    {
        return !left.Equals(right);
    }
}