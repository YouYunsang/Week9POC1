using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerBatteryData",
    menuName = "ScriptableObjects/Player/Player Battery Data")]
public sealed class PlayerBatteryData : ScriptableObject
{
    [Header("Battery")]
    [SerializeField] private float _maxBattery = 100.0f;
    [SerializeField] private float _initialBattery = 100.0f;

    [Header("Flashlight")]
    [SerializeField] private float _flashlightDrainPerSecond = 2.0f;

    public float MaxBattery => _maxBattery;
    public float InitialBattery => _initialBattery;
    public float FlashlightDrainPerSecond => _flashlightDrainPerSecond;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 최대 배터리는 0 이하가 되지 않게 제한한다.
        _maxBattery = Mathf.Max(1.0f, _maxBattery);

        // 시작 배터리는 0~최대 배터리 사이로 제한한다.
        _initialBattery = Mathf.Clamp(_initialBattery, 0.0f, _maxBattery);

        // 손전등 소모 속도는 음수가 되지 않게 제한한다.
        _flashlightDrainPerSecond = Mathf.Max(0.0f, _flashlightDrainPerSecond);
    }
#endif
}