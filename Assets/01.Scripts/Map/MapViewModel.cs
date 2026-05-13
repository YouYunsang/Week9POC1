using System;
using UnityEngine;

public sealed class MapViewModel : MonoBehaviour
{
    [SerializeField] private MapDiscoveryState _mapDiscoveryState;

    public event Action MapChanged;
    public event Action<Vector2Int> PlayerCellChanged;

    public MapGridData MapGridData => _mapDiscoveryState != null
        ? _mapDiscoveryState.MapGridData
        : null;

    public Vector2Int CurrentPlayerCell => _mapDiscoveryState != null && _mapDiscoveryState.HasCurrentPlayerCell
        ? _mapDiscoveryState.CurrentPlayerCell
        : Vector2Int.zero;

    public bool HasCurrentPlayerCell => _mapDiscoveryState != null && _mapDiscoveryState.HasCurrentPlayerCell;

    private void OnEnable()
    {
        if (_mapDiscoveryState == null)
        {
            return;
        }

        // 맵 발견 상태 변경 이벤트를 구독한다.
        _mapDiscoveryState.DiscoveryChanged += HandleDiscoveryChanged;

        // 플레이어 Cell 변경 이벤트를 구독한다.
        _mapDiscoveryState.PlayerCellChanged += HandlePlayerCellChanged;
    }

    private void OnDisable()
    {
        if (_mapDiscoveryState == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _mapDiscoveryState.DiscoveryChanged -= HandleDiscoveryChanged;
        _mapDiscoveryState.PlayerCellChanged -= HandlePlayerCellChanged;
    }

    public bool IsCellRevealed(Vector2Int cellPosition)
    {
        if (_mapDiscoveryState == null)
        {
            return false;
        }

        return _mapDiscoveryState.IsRevealed(cellPosition);
    }

    private void HandleDiscoveryChanged()
    {
        // View에 맵 표시 갱신을 요청한다.
        MapChanged?.Invoke();
    }

    private void HandlePlayerCellChanged(Vector2Int playerCell)
    {
        // View에 플레이어 중심 위치 갱신을 요청한다.
        PlayerCellChanged?.Invoke(playerCell);
    }
}