using UnityEngine;

[CreateAssetMenu(
    fileName = "PanicData",
    menuName = "ScriptableObjects/Player/Panic Data")]
public sealed class PanicData : ScriptableObject
{
    [Header("Panic")]
    [SerializeField] private float _maxPanic = 100.0f;
    [SerializeField] private float _initialPanic = 0.0f;

    [Header("Light")]
    [SerializeField] private float _panicIncreasePerSecondInDark = 6.0f;
    [SerializeField] private float _panicDecreasePerSecondInLight = 3.0f;

    [Header("Mutant")]
    [SerializeField] private float _mutantJumpScarePanicAmount = 25.0f;
    [SerializeField] private float _mutantThreatPanicPerSecond = 10.0f;

    [Header("Control Loss")]
    [SerializeField] private float _controlLossDuration = 1.6f;
    [SerializeField] private float _controlLossMoveSpeed = 5.0f;
    [SerializeField] private float _panicAfterControlLoss = 50.0f;

    public float MaxPanic => _maxPanic;
    public float InitialPanic => _initialPanic;
    public float PanicIncreasePerSecondInDark => _panicIncreasePerSecondInDark;
    public float PanicDecreasePerSecondInLight => _panicDecreasePerSecondInLight;
    public float MutantJumpScarePanicAmount => _mutantJumpScarePanicAmount;
    public float MutantThreatPanicPerSecond => _mutantThreatPanicPerSecond;
    public float ControlLossDuration => _controlLossDuration;
    public float ControlLossMoveSpeed => _controlLossMoveSpeed;
    public float PanicAfterControlLoss => _panicAfterControlLoss;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 패닉 최대값과 초기값을 안전한 범위로 제한한다.
        _maxPanic = Mathf.Max(1.0f, _maxPanic);
        _initialPanic = Mathf.Clamp(_initialPanic, 0.0f, _maxPanic);

        // 증가/감소 수치는 음수가 되지 않게 제한한다.
        _panicIncreasePerSecondInDark = Mathf.Max(0.0f, _panicIncreasePerSecondInDark);
        _panicDecreasePerSecondInLight = Mathf.Max(0.0f, _panicDecreasePerSecondInLight);

        // 변이체 관련 패닉 수치를 안전한 범위로 제한한다.
        _mutantJumpScarePanicAmount = Mathf.Max(0.0f, _mutantJumpScarePanicAmount);
        _mutantThreatPanicPerSecond = Mathf.Max(0.0f, _mutantThreatPanicPerSecond);

        // 통제 상실 수치를 안전한 범위로 제한한다.
        _controlLossDuration = Mathf.Max(0.1f, _controlLossDuration);
        _controlLossMoveSpeed = Mathf.Max(0.0f, _controlLossMoveSpeed);
        _panicAfterControlLoss = Mathf.Clamp(_panicAfterControlLoss, 0.0f, _maxPanic);
    }
#endif
}