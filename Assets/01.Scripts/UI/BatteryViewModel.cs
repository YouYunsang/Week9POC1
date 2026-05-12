using System;
using UnityEngine;

public sealed class BatteryViewModel : MonoBehaviour
{
    private const int BATTERY_CELL_COUNT = 5;

    [SerializeField] private PlayerBattery _playerBattery;

    public event Action<float[]> CellFillAmountsChanged;

    private readonly float[] _cellFillAmounts = new float[BATTERY_CELL_COUNT];

    private void OnEnable()
    {
        if (_playerBattery == null)
        {
            return;
        }

        // 배터리 변경 이벤트를 구독한다.
        _playerBattery.BatteryChanged += HandleBatteryChanged;

        // 활성화 시 현재 상태를 즉시 반영한다.
        HandleBatteryChanged(_playerBattery.CurrentBattery, _playerBattery.MaxBattery);
    }

    private void OnDisable()
    {
        if (_playerBattery == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _playerBattery.BatteryChanged -= HandleBatteryChanged;
    }

    private void HandleBatteryChanged(float currentBattery, float maxBattery)
    {
        float batteryRatio = maxBattery <= 0.0f
            ? 0.0f
            : Mathf.Clamp01(currentBattery / maxBattery);

        float scaledCells = batteryRatio * BATTERY_CELL_COUNT;

        for (int i = 0; i < BATTERY_CELL_COUNT; i++)
        {
            // 각 칸의 fillAmount를 0~1로 계산한다.
            _cellFillAmounts[i] = Mathf.Clamp01(scaledCells - i);
        }

        CellFillAmountsChanged?.Invoke(_cellFillAmounts);
    }
}