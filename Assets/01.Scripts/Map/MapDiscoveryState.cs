using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MapDiscoveryState : MonoBehaviour
{
    [SerializeField] private MapGridData _mapGridData;

    private readonly HashSet<Vector2Int> _revealedCells = new HashSet<Vector2Int>();

    private Vector2Int _currentPlayerCell;
    private bool _hasCurrentPlayerCell;

    public event Action<Vector2Int> CellRevealed;
    public event Action DiscoveryChanged;
    public event Action<Vector2Int> PlayerCellChanged;

    public MapGridData MapGridData => _mapGridData;
    public Vector2Int CurrentPlayerCell => _currentPlayerCell;
    public bool HasCurrentPlayerCell => _hasCurrentPlayerCell;

    public bool IsRevealed(Vector2Int cellPosition)
    {
        return _revealedCells.Contains(cellPosition);
    }

    public void EnterCell(Vector2Int cellPosition)
    {
        if (_mapGridData == null)
        {
            Debug.LogWarning($"{nameof(MapDiscoveryState)}: MapGridData가 연결되지 않았습니다.");
            return;
        }

        if (!_mapGridData.IsValidCell(cellPosition))
        {
            Debug.LogWarning($"{nameof(MapDiscoveryState)}: Valid Cell이 아닙니다. Cell: {cellPosition}");
            return;
        }

        // 플레이어의 현재 맵 Cell을 Trigger 기준 좌표로 갱신한다.
        SetCurrentPlayerCell(cellPosition);

        // 플레이어가 들어온 Cell을 밝힌다.
        RevealCell(cellPosition);
    }

    public bool RevealCell(Vector2Int cellPosition)
    {
        if (_mapGridData == null)
        {
            Debug.LogWarning($"{nameof(MapDiscoveryState)}: MapGridData가 연결되지 않았습니다.");
            return false;
        }

        if (!_mapGridData.IsValidCell(cellPosition))
        {
            return false;
        }

        if (!_revealedCells.Add(cellPosition))
        {
            return false;
        }

        // 새 Cell이 밝혀졌음을 알린다.
        CellRevealed?.Invoke(cellPosition);
        DiscoveryChanged?.Invoke();

        Debug.Log($"Map Cell Revealed: {cellPosition}");

        return true;
    }

    public IReadOnlyCollection<Vector2Int> GetRevealedCells()
    {
        return _revealedCells;
    }

    public void ClearAll()
    {
        // 디버그나 재시작 시 맵 발견 상태를 초기화한다.
        _revealedCells.Clear();
        _hasCurrentPlayerCell = false;

        DiscoveryChanged?.Invoke();
    }

    private void SetCurrentPlayerCell(Vector2Int cellPosition)
    {
        if (_hasCurrentPlayerCell && _currentPlayerCell == cellPosition)
        {
            return;
        }

        _currentPlayerCell = cellPosition;
        _hasCurrentPlayerCell = true;

        // 플레이어 Cell 변경을 UI에 알린다.
        PlayerCellChanged?.Invoke(_currentPlayerCell);

        Debug.Log($"Player Map Cell Changed: {_currentPlayerCell}");
    }
}