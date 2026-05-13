using UnityEngine;
using UnityEngine.EventSystems;

public sealed class MapAnnotationInputController : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private MapGridView _mapGridView;
    [SerializeField] private MapToolState _mapToolState;
    [SerializeField] private MapAnnotationState _annotationState;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_mapGridView == null || _mapToolState == null || _annotationState == null)
        {
            return;
        }

        switch (_mapToolState.CurrentToolMode)
        {
            case MapToolMode.Wall:
                HandleWallClick(eventData);
                break;

            case MapToolMode.Stamp:
                HandleStampClick(eventData);
                break;
        }
    }

    private void HandleWallClick(PointerEventData eventData)
    {
        if (!_mapGridView.TryGetEdgeKeyFromScreenPoint(
                eventData.position,
                eventData.pressEventCamera,
                out MapEdgeKey edgeKey))
        {
            return;
        }

        // 클릭한 그리드 경계에 벽을 토글한다.
        _annotationState.ToggleWall(edgeKey);
    }

    private void HandleStampClick(PointerEventData eventData)
    {
        if (!_mapGridView.TryGetMapPositionFromScreenPoint(
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 mapPosition))
        {
            return;
        }

        // 클릭한 맵 위치에 현재 선택된 타입의 스탬프를 토글한다.
        _annotationState.ToggleStamp(_mapToolState.CurrentStampType, mapPosition);
    }
}