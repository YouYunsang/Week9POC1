using UnityEngine;

public sealed class MapPanelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader _inputReader;
    [SerializeField] private PlayerCondition _playerCondition;
    [SerializeField] private CanvasGroup _miniMapCanvasGroup;
    [SerializeField] private CanvasGroup _expandedMapCanvasGroup;

    private bool _isExpandedMapOpen;

    private void Start()
    {
        // 시작 시 미니맵은 표시하고, 확대맵은 숨긴다.
        SetMiniMapVisible(true);
        SetExpandedMapVisible(false);
    }

    private void OnEnable()
    {
        if (_inputReader == null)
        {
            return;
        }

        // Tab 맵 입력 이벤트를 구독한다.
        _inputReader.MapToggleInputStarted += HandleMapToggleInputStarted;
    }

    private void OnDisable()
    {
        if (_inputReader == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _inputReader.MapToggleInputStarted -= HandleMapToggleInputStarted;
    }

    private void HandleMapToggleInputStarted()
    {
        if (_isExpandedMapOpen)
        {
            CloseExpandedMap();
            return;
        }

        OpenExpandedMap();
    }

    private void OpenExpandedMap()
    {
        _isExpandedMapOpen = true;

        SetExpandedMapVisible(true);

        if (_playerCondition != null)
        {
            // 확대맵 사용 중에는 이동만 막고, 부력은 유지한다.
            _playerCondition.SetMovementBlocked(true);
        }

        // 확대맵 조작 준비를 위해 커서를 표시한다.
        HideCursor.ShowCursorForUI();
    }

    private void CloseExpandedMap()
    {
        _isExpandedMapOpen = false;

        SetExpandedMapVisible(false);

        if (_playerCondition != null)
        {
            // 확대맵을 닫으면 이동을 다시 허용한다.
            _playerCondition.SetMovementBlocked(false);
        }

        // 게임 플레이 상태로 돌아가므로 커서를 숨긴다.
        HideCursor.HideCursorForGameplay();
    }

    private void SetMiniMapVisible(bool isVisible)
    {
        SetCanvasGroupVisible(_miniMapCanvasGroup, isVisible, false);
    }

    private void SetExpandedMapVisible(bool isVisible)
    {
        SetCanvasGroupVisible(_expandedMapCanvasGroup, isVisible, isVisible);
    }

    private void SetCanvasGroupVisible(CanvasGroup canvasGroup, bool isVisible, bool canBlockRaycast)
    {
        if (canvasGroup == null)
        {
            return;
        }

        // CanvasGroup으로 패널 전체 표시 상태를 제어한다.
        canvasGroup.alpha = isVisible ? 1.0f : 0.0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = canBlockRaycast;
    }
}