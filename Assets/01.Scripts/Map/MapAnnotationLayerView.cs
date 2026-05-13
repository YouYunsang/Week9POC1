using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class MapAnnotationLayerView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapAnnotationState _annotationState;
    [SerializeField] private MapGridView _mapGridView;
    [SerializeField] private RectTransform _lineLayer;
    [SerializeField] private RectTransform _stampLayer;

    [Header("Line")]
    [SerializeField] private Image _lineImagePrefab;
    [SerializeField] private float _lineThickness = 4.0f;
    [SerializeField] private Color _wallColor = Color.white;

    [Header("Stamp")]
    [SerializeField] private Image _stampImagePrefab;
    [SerializeField] private float _stampSize = 24.0f;
    [SerializeField] private Color _mutantColor = Color.red;
    [SerializeField] private Color _keyCardDoorColor = Color.yellow;
    [SerializeField] private Color _batteryChargerColor = Color.green;

    private readonly List<GameObject> _spawnedObjects = new List<GameObject>();

    private void OnEnable()
    {
        if (_annotationState == null)
        {
            return;
        }

        // 수동 기록 변경 이벤트를 구독한다.
        _annotationState.AnnotationChanged += Refresh;
    }

    private void OnDisable()
    {
        if (_annotationState == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _annotationState.AnnotationChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        ClearSpawnedObjects();

        if (_annotationState == null || _mapGridView == null)
        {
            return;
        }

        EnsureAnnotationLayersAreOnTop();

        DrawWalls();
        DrawStamps();
    }

    private void EnsureAnnotationLayersAreOnTop()
    {
        ConfigureAnnotationLayer(_lineLayer);
        ConfigureAnnotationLayer(_stampLayer);

        if (_lineLayer != null)
        {
            _lineLayer.SetAsLastSibling();
        }

        if (_stampLayer != null)
        {
            _stampLayer.SetAsLastSibling();
        }
    }

    private void ConfigureAnnotationLayer(RectTransform layer)
    {
        if (layer == null)
        {
            return;
        }

        layer.anchorMin = new Vector2(0.0f, 1.0f);
        layer.anchorMax = new Vector2(0.0f, 1.0f);
        layer.pivot = new Vector2(0.0f, 1.0f);
        layer.anchoredPosition = Vector2.zero;
    }

    private void DrawWalls()
    {
        foreach (MapEdgeKey edgeKey in _annotationState.WallEdges)
        {
            DrawWall(edgeKey);
        }
    }

    private void DrawWall(MapEdgeKey edgeKey)
    {
        if (_lineImagePrefab == null || _lineLayer == null)
        {
            return;
        }

        Image lineImage = Instantiate(_lineImagePrefab, _lineLayer);
        RectTransform lineRectTransform = lineImage.GetComponent<RectTransform>();

        //ConfigureAnnotationItemRect(lineRectTransform);

        lineImage.color = _wallColor;

        Vector2 cellCenter = _mapGridView.ConvertCellCenterToAnchoredPosition(edgeKey.CellPosition);
        float cellSize = _mapGridView.CellPixelSize;

        if (edgeKey.Direction == MapEdgeDirection.Up)
        {
            // Cell 위쪽 경계에 가로 벽 선을 그린다.
            lineRectTransform.anchoredPosition = cellCenter + new Vector2(0.0f, cellSize * 0.5f);
            lineRectTransform.sizeDelta = new Vector2(cellSize, _lineThickness);
        }
        else if (edgeKey.Direction == MapEdgeDirection.Right)
        {
            // Cell 오른쪽 경계에 세로 벽 선을 그린다.
            lineRectTransform.anchoredPosition = cellCenter + new Vector2(cellSize * 0.5f, 0.0f);
            lineRectTransform.sizeDelta = new Vector2(_lineThickness, cellSize);
        }

        _spawnedObjects.Add(lineImage.gameObject);
    }

    private void DrawStamps()
    {
        IReadOnlyList<MapStampRecord> stamps = _annotationState.Stamps;

        for (int i = 0; i < stamps.Count; i++)
        {
            DrawStamp(stamps[i]);
        }
    }

    private void DrawStamp(MapStampRecord stamp)
    {
        if (_stampImagePrefab == null || _stampLayer == null)
        {
            return;
        }

        Image stampImage = Instantiate(_stampImagePrefab, _stampLayer);
        RectTransform stampRectTransform = stampImage.GetComponent<RectTransform>();
        //ConfigureAnnotationItemRect(stampRectTransform);

        // 스탬프 타입에 맞는 색상을 적용한다.
        stampImage.color = GetStampColor(stamp.StampType);

        stampRectTransform.anchoredPosition = _mapGridView.ConvertMapPositionToAnchoredPosition(stamp.MapPosition);
        stampRectTransform.sizeDelta = new Vector2(_stampSize, _stampSize);

        _spawnedObjects.Add(stampImage.gameObject);
    }

    private Color GetStampColor(MapStampType stampType)
    {
        switch (stampType)
        {
            case MapStampType.Mutant:
                return _mutantColor;

            case MapStampType.KeyCardDoor:
                return _keyCardDoorColor;

            case MapStampType.BatteryCharger:
                return _batteryChargerColor;

            default:
                return Color.white;
        }
    }

    private void ClearSpawnedObjects()
    {
        for (int i = _spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (_spawnedObjects[i] != null)
            {
                Destroy(_spawnedObjects[i]);
            }
        }

        _spawnedObjects.Clear();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 선 두께와 스탬프 크기는 0 이하가 되지 않게 제한한다.
        _lineThickness = Mathf.Max(1.0f, _lineThickness);
        _stampSize = Mathf.Max(1.0f, _stampSize);
    }
#endif
}
