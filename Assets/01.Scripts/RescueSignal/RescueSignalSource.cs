using UnityEngine;

public sealed class RescueSignalSource : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RescueSignalState _signalState;
    [SerializeField] private Transform _playerTransform;

    [Header("Signal")]
    [SerializeField] private string _signalId = "signal_001";
    [SerializeField] private RescueSignalType _signalType = RescueSignalType.Corpse;
    [SerializeField] private Vector2Int _cellPosition;
    [SerializeField] private bool _emitsSignal = true;

    [Header("Distance")]
    [SerializeField] private float _detectionRadius = 8.0f;
    [SerializeField] private float _resolveRadius = 1.2f;

    private bool _isDetected;
    private bool _isResolved;

    private void Update()
    {
        if (_isResolved)
        {
            return;
        }

        if (!_emitsSignal)
        {
            return;
        }

        if (_signalState == null || _playerTransform == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, _playerTransform.position);

        if (!_isDetected && distance <= _detectionRadius)
        {
            DetectSignal();
        }

        if (_isDetected && distance <= _resolveRadius)
        {
            ResolveSignal();
        }
    }

    public void ForceResolve()
    {
        if (_isResolved)
        {
            return;
        }

        ResolveSignal();
    }

    private void DetectSignal()
    {
        _isDetected = true;

        // 플레이어가 감지 반경에 들어오면 맵 UI에 구조 신호를 표시한다.
        _signalState.RegisterSignal(_signalId, _signalType, _cellPosition);
    }

    private void ResolveSignal()
    {
        _isResolved = true;

        // 플레이어가 충분히 가까워지면 맵 UI에서 구조 신호를 제거한다.
        _signalState.ResolveSignal(_signalId);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _detectionRadius = Mathf.Max(0.1f, _detectionRadius);
        _resolveRadius = Mathf.Max(0.1f, _resolveRadius);

        if (_resolveRadius > _detectionRadius)
        {
            _resolveRadius = _detectionRadius;
        }

        if (_signalType == RescueSignalType.MutantWithoutSignal)
        {
            _emitsSignal = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 구조 신호 감지 반경.
        Gizmos.color = new Color(0.2f, 0.9f, 1.0f, 0.45f);
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        // 구조 신호 비활성화 반경.
        Gizmos.color = new Color(1.0f, 0.3f, 0.2f, 0.65f);
        Gizmos.DrawWireSphere(transform.position, _resolveRadius);
    }
#endif
}