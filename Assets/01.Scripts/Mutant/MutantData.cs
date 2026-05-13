using UnityEngine;

[CreateAssetMenu(
    fileName = "MutantData",
    menuName = "ScriptableObjects/Mutant/Mutant Data")]
public sealed class MutantData : ScriptableObject
{
    [Header("Detection")]
    [SerializeField] private float _detectionDistance = 3.0f;

    [Header("Approach")]
    [SerializeField] private float _approachSpeed = 8.0f;
    [SerializeField] private float _stoppingDistanceFromPlayer = 1.2f;
    [SerializeField] private float _maxApproachDuration = 1.0f;

    [Header("Dormant Pulse")]
    [SerializeField] private float _dormantPulseScale = 1.04f;
    [SerializeField] private float _dormantPulseDuration = 1.2f;

    [Header("Jump Scare Pulse")]
    [SerializeField] private float _jumpScareScale = 1.25f;
    [SerializeField] private float _jumpScareScaleDuration = 0.12f;

    [Header("Threatening Pulse")]
    [SerializeField] private float _threateningPulseScale = 1.08f;
    [SerializeField] private float _threateningPulseDuration = 0.55f;

    public float DetectionDistance => _detectionDistance;
    public float ApproachSpeed => _approachSpeed;
    public float StoppingDistanceFromPlayer => _stoppingDistanceFromPlayer;
    public float MaxApproachDuration => _maxApproachDuration;
    public float DormantPulseScale => _dormantPulseScale;
    public float DormantPulseDuration => _dormantPulseDuration;
    public float JumpScareScale => _jumpScareScale;
    public float JumpScareScaleDuration => _jumpScareScaleDuration;
    public float ThreateningPulseScale => _threateningPulseScale;
    public float ThreateningPulseDuration => _threateningPulseDuration;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 감지 거리와 이동 수치가 음수가 되지 않게 제한한다.
        _detectionDistance = Mathf.Max(0.1f, _detectionDistance);
        _approachSpeed = Mathf.Max(0.1f, _approachSpeed);
        _stoppingDistanceFromPlayer = Mathf.Max(0.1f, _stoppingDistanceFromPlayer);
        _maxApproachDuration = Mathf.Max(0.1f, _maxApproachDuration);

        // DOTween Scale 연출 수치를 안전한 범위로 제한한다.
        _dormantPulseScale = Mathf.Max(1.0f, _dormantPulseScale);
        _dormantPulseDuration = Mathf.Max(0.05f, _dormantPulseDuration);

        _jumpScareScale = Mathf.Max(1.0f, _jumpScareScale);
        _jumpScareScaleDuration = Mathf.Max(0.01f, _jumpScareScaleDuration);

        _threateningPulseScale = Mathf.Max(1.0f, _threateningPulseScale);
        _threateningPulseDuration = Mathf.Max(0.05f, _threateningPulseDuration);
    }
#endif
}