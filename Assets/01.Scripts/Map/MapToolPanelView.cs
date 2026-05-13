using UnityEngine;
using UnityEngine.UI;

public sealed class MapToolPanelView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapToolState _mapToolState;

    [Header("Buttons")]
    [SerializeField] private Button _noneButton;
    [SerializeField] private Button _wallButton;
    [SerializeField] private Button _mutantStampButton;
    [SerializeField] private Button _keyCardDoorStampButton;
    [SerializeField] private Button _batteryChargerStampButton;

    private void Awake()
    {
        if (_noneButton != null)
        {
            // 도구 해제 버튼을 연결한다.
            _noneButton.onClick.AddListener(HandleNoneButtonClicked);
        }

        if (_wallButton != null)
        {
            // 벽 도구 버튼을 연결한다.
            _wallButton.onClick.AddListener(HandleWallButtonClicked);
        }

        if (_mutantStampButton != null)
        {
            // 변이체 스탬프 버튼을 연결한다.
            _mutantStampButton.onClick.AddListener(HandleMutantStampButtonClicked);
        }

        if (_keyCardDoorStampButton != null)
        {
            // ID 카드 문 스탬프 버튼을 연결한다.
            _keyCardDoorStampButton.onClick.AddListener(HandleKeyCardDoorStampButtonClicked);
        }

        if (_batteryChargerStampButton != null)
        {
            // 배터리 충전기 스탬프 버튼을 연결한다.
            _batteryChargerStampButton.onClick.AddListener(HandleBatteryChargerStampButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (_noneButton != null)
        {
            _noneButton.onClick.RemoveListener(HandleNoneButtonClicked);
        }

        if (_wallButton != null)
        {
            _wallButton.onClick.RemoveListener(HandleWallButtonClicked);
        }

        if (_mutantStampButton != null)
        {
            _mutantStampButton.onClick.RemoveListener(HandleMutantStampButtonClicked);
        }

        if (_keyCardDoorStampButton != null)
        {
            _keyCardDoorStampButton.onClick.RemoveListener(HandleKeyCardDoorStampButtonClicked);
        }

        if (_batteryChargerStampButton != null)
        {
            _batteryChargerStampButton.onClick.RemoveListener(HandleBatteryChargerStampButtonClicked);
        }
    }

    private void HandleNoneButtonClicked()
    {
        if (_mapToolState == null)
        {
            return;
        }

        _mapToolState.SelectNone();
    }

    private void HandleWallButtonClicked()
    {
        if (_mapToolState == null)
        {
            return;
        }

        _mapToolState.SelectWallTool();
    }

    private void HandleMutantStampButtonClicked()
    {
        if (_mapToolState == null)
        {
            return;
        }

        _mapToolState.SelectMutantStamp();
    }

    private void HandleKeyCardDoorStampButtonClicked()
    {
        if (_mapToolState == null)
        {
            return;
        }

        _mapToolState.SelectKeyCardDoorStamp();
    }

    private void HandleBatteryChargerStampButtonClicked()
    {
        if (_mapToolState == null)
        {
            return;
        }

        _mapToolState.SelectBatteryChargerStamp();
    }
}