using System.Collections.Generic;
using UnityEngine;

public sealed class MapGridView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapViewModel _viewModel;
    [SerializeField] private RectTransform _viewportRectTransform;
    [SerializeField] private RectTransform _cellContainer;
    [SerializeField] private MapCellView _cellViewPrefab;

    [Header("Cell UI")]
    [SerializeField] private float _cellPixelSize = 18.0f;
    [SerializeField] private float _cellSpacing = 2.0f;

    [Header("Follow")]
    [SerializeField] private bool _followPlayerCell = true;

    public float CellPixelSize => _cellPixelSize;
    public float CellSpacing => _cellSpacing;
    public float Step => _cellPixelSize + _cellSpacing;

    private readonly Dictionary<Vector2Int, MapCellView> _cellViews = new Dictionary<Vector2Int, MapCellView>();

    private void Start()
    {
        ConfigureRectTransforms();
        BuildGrid();
        Refresh();
        CenterOnPlayerCell();
    }

    private void OnEnable()
    {
        if (_viewModel == null)
        {
            return;
        }

        // 맵 발견 상태 변경 이벤트를 구독한다.
        _viewModel.MapChanged += Refresh;

        // 플레이어 Cell 변경 이벤트를 구독한다.
        _viewModel.PlayerCellChanged += HandlePlayerCellChanged;
    }

    private void OnDisable()
    {
        if (_viewModel == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _viewModel.MapChanged -= Refresh;
        _viewModel.PlayerCellChanged -= HandlePlayerCellChanged;
    }

    public void BuildGrid()
    {
        if (_viewModel == null || _viewModel.MapGridData == null)
        {
            Debug.LogWarning($"{nameof(MapGridView)}: ViewModel 또는 MapGridData가 없습니다.");
            return;
        }

        if (_viewportRectTransform == null || _cellContainer == null || _cellViewPrefab == null)
        {
            Debug.LogWarning($"{nameof(MapGridView)}: Viewport, CellContainer, CellViewPrefab 연결을 확인하세요.");
            return;
        }

        ClearExistingCells();

        MapGridData gridData = _viewModel.MapGridData;

        for (int i = 0; i < gridData.ValidCells.Count; i++)
        {
            Vector2Int cellPosition = gridData.ValidCells[i];

            MapCellView cellView = Instantiate(_cellViewPrefab, _cellContainer);
            RectTransform cellRectTransform = cellView.GetComponent<RectTransform>();

            // Cell UI의 기준을 중앙으로 맞춘다.
            cellRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            cellRectTransform.anchorMax = new Vector2(0.0f, 1.0f);
            cellRectTransform.pivot = new Vector2(0.5f, 0.5f);

            // 음수 좌표를 포함한 실제 Cell 좌표를 UI 좌표로 변환한다.
            cellRectTransform.anchoredPosition = ConvertCellToAnchoredPosition(cellPosition, gridData);
            cellRectTransform.sizeDelta = new Vector2(_cellPixelSize, _cellPixelSize);

            cellView.Initialize(cellPosition);
            _cellViews.Add(cellPosition, cellView);
        }

        ResizeContainer(gridData);
    }

    public void Refresh()
    {
        if (_viewModel == null)
        {
            return;
        }

        foreach (KeyValuePair<Vector2Int, MapCellView> pair in _cellViews)
        {
            bool isRevealed = _viewModel.IsCellRevealed(pair.Key);

            // 각 Cell의 발견 상태를 UI에 반영한다.
            pair.Value.SetRevealed(isRevealed);
        }
    }

    public void CenterOnPlayerCell()
    {
        if (!_followPlayerCell || _viewModel == null || !_viewModel.HasCurrentPlayerCell)
        {
            return;
        }

        if (_cellContainer == null || _viewportRectTransform == null)
        {
            return;
        }

        MapGridData gridData = _viewModel.MapGridData;

        if (gridData == null)
        {
            return;
        }

        Vector2 playerCellUiPosition = ConvertCellToAnchoredPosition(
            _viewModel.CurrentPlayerCell,
            gridData);

        Vector2 viewportCenter = GetViewportCenter();

        // 플레이어 Cell이 Viewport 중앙에 오도록 CellContainer를 반대로 이동시킨다.
        _cellContainer.anchoredPosition = viewportCenter - playerCellUiPosition;
    }

    private void HandlePlayerCellChanged(Vector2Int playerCell)
    {
        CenterOnPlayerCell();
    }

    private Vector2 ConvertCellToAnchoredPosition(Vector2Int cellPosition, MapGridData gridData)
    {
        float step = _cellPixelSize + _cellSpacing;

        Vector2Int minCell = gridData.MinCell;
        Vector2Int maxCell = gridData.MaxCell;

        int localX = cellPosition.x - minCell.x;
        int localY = maxCell.y - cellPosition.y;

        float x = localX * step + _cellPixelSize * 0.5f;
        float y = -localY * step - _cellPixelSize * 0.5f;

        return new Vector2(x, y);
    }

    private Vector2 GetViewportCenter()
    {
        Rect rect = _viewportRectTransform.rect;

        // Viewport의 로컬 좌표 기준 중앙점이다.
        return rect.center;
    }

    private void ResizeContainer(MapGridData gridData)
    {
        float step = _cellPixelSize + _cellSpacing;

        float width = gridData.BoundsWidth * step;
        float height = gridData.BoundsHeight * step;

        // Valid Cells의 실제 Bounds 기준으로 컨테이너 크기를 조정한다.
        _cellContainer.sizeDelta = new Vector2(width, height);
    }

    private void ConfigureRectTransforms()
    {
        if (_cellContainer == null)
        {
            return;
        }

        // CellContainer는 Viewport 중앙을 기준으로 이동하는 Content 역할을 한다.
        _cellContainer.anchorMin = new Vector2(0.5f, 0.5f);
        _cellContainer.anchorMax = new Vector2(0.5f, 0.5f);
        _cellContainer.pivot = new Vector2(0.0f, 1.0f);
    }

    private void ClearExistingCells()
    {
        _cellViews.Clear();

        if (_cellContainer == null)
        {
            return;
        }

        for (int i = _cellContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = _cellContainer.GetChild(i);

            // 런타임에서 기존 셀 UI를 정리한다.
            Destroy(child.gameObject);
        }
    }

    public bool TryGetEdgeKeyFromScreenPoint(
    Vector2 screenPosition,
    Camera eventCamera,
    out MapEdgeKey edgeKey)
    {
        edgeKey = default;

        if (!TryGetCellLocalPoint(screenPosition, eventCamera, out Vector2 localPoint))
        {
            return false;
        }

        if (!TryGetCellAndDirection(localPoint, out Vector2Int cellPosition, out MapEdgeDirection direction))
        {
            return false;
        }

        // 같은 경계가 항상 같은 데이터로 저장되도록 정규화한다.
        edgeKey = MapEdgeKey.CreateNormalized(cellPosition, direction);

        return true;
    }

    public bool TryGetMapPositionFromScreenPoint(
        Vector2 screenPosition,
        Camera eventCamera,
        out Vector2 mapPosition)
    {
        mapPosition = Vector2.zero;

        if (!TryGetCellLocalPoint(screenPosition, eventCamera, out Vector2 localPoint))
        {
            return false;
        }

        MapGridData gridData = _viewModel.MapGridData;

        if (gridData == null)
        {
            return false;
        }

        float step = Step;
        Vector2Int minCell = gridData.MinCell;
        Vector2Int maxCell = gridData.MaxCell;

        // UI 로컬 좌표를 맵 연속 좌표로 변환한다.
        float mapX = minCell.x + (localPoint.x / step);
        float mapY = maxCell.y + (localPoint.y / step);

        mapPosition = new Vector2(mapX, mapY);

        return true;
    }

    public Vector2 ConvertCellCenterToAnchoredPosition(Vector2Int cellPosition)
    {
        if (_viewModel == null || _viewModel.MapGridData == null)
        {
            return Vector2.zero;
        }

        // 특정 Cell의 중앙 UI 좌표를 반환한다.
        return ConvertCellToAnchoredPosition(cellPosition, _viewModel.MapGridData);
    }

    public Vector2 ConvertMapPositionToAnchoredPosition(Vector2 mapPosition)
    {
        if (_viewModel == null || _viewModel.MapGridData == null)
        {
            return Vector2.zero;
        }

        MapGridData gridData = _viewModel.MapGridData;
        float step = Step;

        Vector2Int minCell = gridData.MinCell;
        Vector2Int maxCell = gridData.MaxCell;

        float x = (mapPosition.x - minCell.x) * step;
        float y = (mapPosition.y - maxCell.y) * step;

        return new Vector2(x, y);
    }

    private bool TryGetCellLocalPoint(
        Vector2 screenPosition,
        Camera eventCamera,
        out Vector2 localPoint)
    {
        localPoint = Vector2.zero;

        if (_cellContainer == null)
        {
            return false;
        }

        // 화면 좌표를 CellContainer 기준 로컬 좌표로 변환한다.
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _cellContainer,
            screenPosition,
            eventCamera,
            out localPoint);
    }

    private bool TryGetCellAndDirection(
        Vector2 localPoint,
        out Vector2Int cellPosition,
        out MapEdgeDirection direction)
    {
        cellPosition = Vector2Int.zero;
        direction = MapEdgeDirection.Up;

        if (_viewModel == null || _viewModel.MapGridData == null)
        {
            return false;
        }

        MapGridData gridData = _viewModel.MapGridData;
        float step = Step;

        Vector2Int minCell = gridData.MinCell;
        Vector2Int maxCell = gridData.MaxCell;

        float localGridX = localPoint.x / step;
        float localGridY = -localPoint.y / step;

        int localCellX = Mathf.FloorToInt(localGridX);
        int localCellY = Mathf.FloorToInt(localGridY);

        int cellX = minCell.x + localCellX;
        int cellY = maxCell.y - localCellY;

        cellPosition = new Vector2Int(cellX, cellY);

        if (!gridData.IsValidCell(cellPosition))
        {
            return false;
        }

        float xInStep = localPoint.x - localCellX * step;
        float yInStep = -localPoint.y - localCellY * step;

        if (xInStep < 0.0f || xInStep > _cellPixelSize ||
            yInStep < 0.0f || yInStep > _cellPixelSize)
        {
            return false;
        }

        float distanceToLeft = xInStep;
        float distanceToRight = _cellPixelSize - xInStep;
        float distanceToUp = yInStep;
        float distanceToDown = _cellPixelSize - yInStep;

        float minDistance = distanceToUp;
        direction = MapEdgeDirection.Up;

        if (distanceToRight < minDistance)
        {
            minDistance = distanceToRight;
            direction = MapEdgeDirection.Right;
        }

        if (distanceToDown < minDistance)
        {
            minDistance = distanceToDown;
            direction = MapEdgeDirection.Down;
        }

        if (distanceToLeft < minDistance)
        {
            direction = MapEdgeDirection.Left;
        }

        return true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // UI Cell 크기와 간격이 음수가 되지 않도록 제한한다.
        _cellPixelSize = Mathf.Max(1.0f, _cellPixelSize);
        _cellSpacing = Mathf.Max(0.0f, _cellSpacing);
    }
#endif
}