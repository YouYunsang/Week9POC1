using System;
using UnityEngine;

public sealed class MapPlayerTracker : MonoBehaviour
{
    [SerializeField] private MapGridData _mapGridData;
    [SerializeField] private Transform _playerTransform;

    private Vector2Int _currentPlayerCell;
    private bool _hasCurrentCell;

    public event Action<Vector2Int> PlayerCellChanged;

    public Vector2Int CurrentPlayerCell => _currentPlayerCell;
    public bool HasCurrentCell => _hasCurrentCell;

    private void Update()
    {
        if (_mapGridData == null || _playerTransform == null)
        {
            return;
        }

        Vector2Int nextCell = _mapGridData.ConvertWorldToCell(_playerTransform.position);

        if (_hasCurrentCell && nextCell == _currentPlayerCell)
        {
            return;
        }

        // 플레이어가 위치한 맵 Cell이 바뀌었음을 기록한다.
        _currentPlayerCell = nextCell;
        _hasCurrentCell = true;

        PlayerCellChanged?.Invoke(_currentPlayerCell);
    }
}