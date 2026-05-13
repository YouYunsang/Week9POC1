using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MapGridData",
    menuName = "ScriptableObjects/Map/Map Grid Data")]
public sealed class MapGridData : ScriptableObject
{
    [Header("Grid")]
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private float _cellSize = 2.0f;

    [Header("World")]
    [SerializeField] private Vector2 _worldOrigin;

    [Header("Valid Cells")]
    [SerializeField] private List<Vector2Int> _validCells = new List<Vector2Int>();

#if UNITY_EDITOR
    [Header("Editor Add Rectangle")]
    [SerializeField] private Vector2Int _editorRectangleStart = Vector2Int.zero;
    [SerializeField] private Vector2Int _editorRectangleSize = new Vector2Int(3, 3);

    [Header("Editor Add Row Pattern")]
    [SerializeField] private Vector2Int _editorRowPatternStart = Vector2Int.zero;
    [SerializeField] private List<int> _editorRowWidths = new List<int> { 4, 3, 3 };
#endif

    public int Width => _width;
    public int Height => _height;
    public float CellSize => _cellSize;
    public Vector2 WorldOrigin => _worldOrigin;
    public IReadOnlyList<Vector2Int> ValidCells => _validCells;

    public Vector2Int MinCell => CalculateMinCell();
    public Vector2Int MaxCell => CalculateMaxCell();
    public int BoundsWidth => MaxCell.x - MinCell.x + 1;
    public int BoundsHeight => MaxCell.y - MinCell.y + 1;

    public bool IsValidCell(Vector2Int cellPosition)
    {
        // 음수 좌표도 Valid Cells에 있으면 유효한 맵 Cell이다.
        return _validCells.Contains(cellPosition);
    }

    public Vector2Int ConvertWorldToCell(Vector2 worldPosition)
    {
        Vector2 localPosition = worldPosition - _worldOrigin;

        int x = Mathf.FloorToInt(localPosition.x / _cellSize);
        int y = Mathf.FloorToInt(localPosition.y / _cellSize);

        // 월드 좌표를 맵 Cell 좌표로 변환한다.
        return new Vector2Int(x, y);
    }

    private Vector2Int CalculateMinCell()
    {
        if (_validCells.Count <= 0)
        {
            return Vector2Int.zero;
        }

        int minX = _validCells[0].x;
        int minY = _validCells[0].y;

        for (int i = 1; i < _validCells.Count; i++)
        {
            Vector2Int cell = _validCells[i];

            minX = Mathf.Min(minX, cell.x);
            minY = Mathf.Min(minY, cell.y);
        }

        return new Vector2Int(minX, minY);
    }

    private Vector2Int CalculateMaxCell()
    {
        if (_validCells.Count <= 0)
        {
            return Vector2Int.zero;
        }

        int maxX = _validCells[0].x;
        int maxY = _validCells[0].y;

        for (int i = 1; i < _validCells.Count; i++)
        {
            Vector2Int cell = _validCells[i];

            maxX = Mathf.Max(maxX, cell.x);
            maxY = Mathf.Max(maxY, cell.y);
        }

        return new Vector2Int(maxX, maxY);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // CellSize는 월드 좌표 변환에 사용되므로 0 이하가 되면 안 된다.
        _cellSize = Mathf.Max(0.1f, _cellSize);

        _width = Mathf.Max(1, _width);
        _height = Mathf.Max(1, _height);

        _editorRectangleSize.x = Mathf.Max(1, _editorRectangleSize.x);
        _editorRectangleSize.y = Mathf.Max(1, _editorRectangleSize.y);

        for (int i = 0; i < _editorRowWidths.Count; i++)
        {
            _editorRowWidths[i] = Mathf.Max(0, _editorRowWidths[i]);
        }
    }

    [ContextMenu("Add Rectangle Cells")]
    private void AddRectangleCells()
    {
        int addedCount = 0;

        for (int y = 0; y < _editorRectangleSize.y; y++)
        {
            for (int x = 0; x < _editorRectangleSize.x; x++)
            {
                Vector2Int cellPosition = new Vector2Int(
                    _editorRectangleStart.x + x,
                    _editorRectangleStart.y + y);

                if (TryAddValidCell(cellPosition))
                {
                    addedCount++;
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"{nameof(MapGridData)}: 사각형 영역 Cell {addedCount}개 추가 완료");
    }

    [ContextMenu("Add Row Pattern Cells")]
    private void AddRowPatternCells()
    {
        int addedCount = 0;

        for (int y = 0; y < _editorRowWidths.Count; y++)
        {
            int rowWidth = _editorRowWidths[y];

            for (int x = 0; x < rowWidth; x++)
            {
                Vector2Int cellPosition = new Vector2Int(
                    _editorRowPatternStart.x + x,
                    _editorRowPatternStart.y + y);

                if (TryAddValidCell(cellPosition))
                {
                    addedCount++;
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"{nameof(MapGridData)}: 줄 패턴 Cell {addedCount}개 추가 완료");
    }

    [ContextMenu("Clean Valid Cells")]
    private void CleanValidCells()
    {
        HashSet<Vector2Int> uniqueCells = new HashSet<Vector2Int>();

        for (int i = _validCells.Count - 1; i >= 0; i--)
        {
            Vector2Int cell = _validCells[i];

            if (uniqueCells.Contains(cell))
            {
                _validCells.RemoveAt(i);
                continue;
            }

            uniqueCells.Add(cell);
        }

        SortValidCells();

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"{nameof(MapGridData)}: Valid Cells 정리 완료");
    }

    [ContextMenu("Sort Valid Cells")]
    private void SortValidCells()
    {
        _validCells.Sort(CompareCellPosition);

        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Clear Valid Cells")]
    private void ClearValidCells()
    {
        _validCells.Clear();

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"{nameof(MapGridData)}: Valid Cells 전체 삭제 완료");
    }

    private bool TryAddValidCell(Vector2Int cellPosition)
    {
        if (_validCells.Contains(cellPosition))
        {
            return false;
        }

        // 음수 좌표도 유효한 우주선 내부 칸으로 추가할 수 있다.
        _validCells.Add(cellPosition);
        return true;
    }

    private int CompareCellPosition(Vector2Int left, Vector2Int right)
    {
        if (left.y != right.y)
        {
            return left.y.CompareTo(right.y);
        }

        return left.x.CompareTo(right.x);
    }
#endif
}