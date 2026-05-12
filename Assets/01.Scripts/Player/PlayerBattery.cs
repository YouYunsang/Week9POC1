using System;
using UnityEngine;

public sealed class PlayerBattery : MonoBehaviour
{
    [SerializeField] private PlayerBatteryData _batteryData;

    private float _currentBattery;

    public event Action<float, float> BatteryChanged;

    public float CurrentBattery => _currentBattery;
    public float MaxBattery => _batteryData != null ? _batteryData.MaxBattery : 0.0f;
    public float BatteryRatio => MaxBattery <= 0.0f ? 0.0f : Mathf.Clamp01(_currentBattery / MaxBattery);
    public bool HasBattery => _currentBattery > 0.0f;

    private void Awake()
    {
        if (_batteryData == null)
        {
            Debug.LogError($"{nameof(PlayerBattery)}: PlayerBatteryData가 연결되지 않았습니다.");
            return;
        }

        // 시작 배터리 수치를 초기화한다.
        _currentBattery = _batteryData.InitialBattery;
    }

    private void Start()
    {
        // UI가 구독을 완료한 뒤 초기 배터리 상태를 발행한다.
        RaiseBatteryChanged();
    }

    public bool CanConsume(float amount)
    {
        return amount <= 0.0f || _currentBattery > 0.0f;
    }

    public bool TryConsume(float amount)
    {
        if (amount <= 0.0f)
        {
            return true;
        }

        if (_currentBattery <= 0.0f)
        {
            return false;
        }

        // 배터리를 소모하고 0 아래로 내려가지 않게 제한한다.
        _currentBattery = Mathf.Max(0.0f, _currentBattery - amount);

        RaiseBatteryChanged();

        return true;
    }

    public void Recharge(float amount)
    {
        if (amount <= 0.0f || _batteryData == null)
        {
            return;
        }

        // 배터리를 회복하고 최대값을 넘지 않게 제한한다.
        _currentBattery = Mathf.Min(_batteryData.MaxBattery, _currentBattery + amount);

        RaiseBatteryChanged();
    }

    public float GetFlashlightDrainAmount(float deltaTime)
    {
        if (_batteryData == null)
        {
            return 0.0f;
        }

        // 프레임 시간에 따른 손전등 소모량을 계산한다.
        return _batteryData.FlashlightDrainPerSecond * deltaTime;
    }

    private void RaiseBatteryChanged()
    {
        BatteryChanged?.Invoke(_currentBattery, MaxBattery);
    }
}