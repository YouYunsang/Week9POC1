using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MapAnnotationState : MonoBehaviour
{
    [Header("Stamp")]
    [SerializeField] private float _stampToggleDistance = 0.35f;

    private readonly HashSet<MapEdgeKey> _wallEdges = new HashSet<MapEdgeKey>();
    private readonly List<MapStampRecord> _stamps = new List<MapStampRecord>();

    public event Action AnnotationChanged;

    public IReadOnlyCollection<MapEdgeKey> WallEdges => _wallEdges;
    public IReadOnlyList<MapStampRecord> Stamps => _stamps;

    public void ToggleWall(MapEdgeKey edgeKey)
    {
        if (_wallEdges.Contains(edgeKey))
        {
            // 같은 위치에 벽이 있으면 삭제한다.
            _wallEdges.Remove(edgeKey);
        }
        else
        {
            // 같은 위치에 벽이 없으면 추가한다.
            _wallEdges.Add(edgeKey);
        }

        AnnotationChanged?.Invoke();
    }

    public void ToggleStamp(MapStampType stampType, Vector2 mapPosition)
    {
        int existingIndex = FindNearbyStampIndex(stampType, mapPosition);

        if (existingIndex >= 0)
        {
            // 같은 타입의 가까운 스탬프가 있으면 삭제한다.
            _stamps.RemoveAt(existingIndex);
        }
        else
        {
            // 가까운 스탬프가 없으면 새로 추가한다.
            _stamps.Add(new MapStampRecord(stampType, mapPosition));
        }

        AnnotationChanged?.Invoke();
    }

    public void ClearAll()
    {
        // 모든 수동 기록을 초기화한다.
        _wallEdges.Clear();
        _stamps.Clear();
        AnnotationChanged?.Invoke();
    }

    private int FindNearbyStampIndex(MapStampType stampType, Vector2 mapPosition)
    {
        float closestDistanceSqr = _stampToggleDistance * _stampToggleDistance;
        int closestIndex = -1;

        for (int i = 0; i < _stamps.Count; i++)
        {
            MapStampRecord stamp = _stamps[i];

            if (stamp.StampType != stampType)
            {
                continue;
            }

            float distanceSqr = (stamp.MapPosition - mapPosition).sqrMagnitude;

            if (distanceSqr > closestDistanceSqr)
            {
                continue;
            }

            closestDistanceSqr = distanceSqr;
            closestIndex = i;
        }

        return closestIndex;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 스탬프 삭제 판정 거리는 음수가 되지 않게 제한한다.
        _stampToggleDistance = Mathf.Max(0.01f, _stampToggleDistance);
    }
#endif
}