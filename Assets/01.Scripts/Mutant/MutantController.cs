using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class MutantController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MutantData _mutantData;
    [SerializeField] private MutantRoomSensor _roomSensor;
    [SerializeField] private RescueSignalSource _rescueSignalSource;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _dormantColor = Color.white;
    [SerializeField] private Color _revealedMutantColor = new Color(80.0f / 255.0f, 14.0f / 255.0f, 14.0f / 255.0f, 1.0f);

    private Rigidbody2D _rigidbody;
    private Transform _playerTransform;
    private Vector3 _originalScale;
    private Tween _scaleTween;

    private MutantState _state = MutantState.Dormant;
    private bool _hasTriggered;
    private bool _isPlayerInRoom;
    private float _approachTimer;

    public MutantState State => _state;
    public bool HasTriggered => _hasTriggered;

    private void Awake()
    {
        // 이동 처리를 위해 Rigidbody2D를 캐싱한다.
        _rigidbody = GetComponent<Rigidbody2D>();

        if (_spriteRenderer == null)
        {
            // 같은 오브젝트의 SpriteRenderer를 캐싱한다.
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // DOTween Scale 연출 복구를 위해 원래 크기를 저장한다.
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (_roomSensor == null)
        {
            return;
        }

        // 방 감지 이벤트를 구독한다.
        _roomSensor.PlayerEnteredRoom += HandlePlayerEnteredRoom;
        _roomSensor.PlayerExitedRoom += HandlePlayerExitedRoom;
    }

    private void OnDisable()
    {
        if (_roomSensor != null)
        {
            // 비활성화 시 이벤트 구독을 해제한다.
            _roomSensor.PlayerEnteredRoom -= HandlePlayerEnteredRoom;
            _roomSensor.PlayerExitedRoom -= HandlePlayerExitedRoom;
        }

        KillScaleTween();
    }

    private void Start()
    {
        EnterDormant();
    }

    private void Update()
    {
        if (_state != MutantState.Dormant)
        {
            return;
        }

        if (_hasTriggered || !_isPlayerInRoom || _playerTransform == null)
        {
            return;
        }

        TryStartJumpScareApproach();
    }

    private void FixedUpdate()
    {
        if (_state != MutantState.Approaching)
        {
            return;
        }

        TickApproach(Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            return;
        }

        // 이번 단계에서는 접촉 처리만 확인한다.
        Debug.Log("변이체가 플레이어와 접촉했습니다.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // 변이체 Collider를 Trigger로 사용할 경우를 위한 접촉 로그.
        Debug.Log("변이체가 플레이어와 접촉했습니다.");
    }

    private void SetSpriteColor(Color color)
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        // 변이체의 현재 시각 색상을 변경한다.
        _spriteRenderer.color = color;
    }

    private void HandlePlayerEnteredRoom(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        _isPlayerInRoom = true;
    }

    private void HandlePlayerExitedRoom(Transform playerTransform)
    {
        if (_playerTransform != playerTransform)
        {
            return;
        }

        _isPlayerInRoom = false;

        if (_state == MutantState.Dormant)
        {
            _playerTransform = null;
        }
    }

    private void TryStartJumpScareApproach()
    {
        if (_mutantData == null)
        {
            Debug.LogWarning($"{nameof(MutantController)}: MutantData가 연결되지 않았습니다.");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer > _mutantData.DetectionDistance)
        {
            return;
        }

        StartJumpScareApproach();
    }

    private void StartJumpScareApproach()
    {
        _hasTriggered = true;
        _state = MutantState.Approaching;
        _approachTimer = 0.0f;

        // 돌진 시작 순간 시체처럼 보이던 색을 변이체 색으로 바꾼다.
        SetSpriteColor(_revealedMutantColor);

        // 변이체 접근 시작 이벤트를 발행해 플레이어 패닉을 증가시킨다.
        GameEventBus.RaiseMutantJumpScareStarted(transform.position);

        // 변이체가 구조 신호를 발생시키고 있었다면 접근 시작 순간 신호를 제거한다.
        if (_rescueSignalSource != null)
        {
            _rescueSignalSource.ForceResolve();
        }

        // 잠복 펄스를 끊고 점프스케어 Scale 연출을 재생한다.
        PlayJumpScarePulse();

        Debug.Log("변이체 점프스케어 접근 시작");
    }

    private void TickApproach(float deltaTime)
    {
        if (_mutantData == null || _playerTransform == null)
        {
            StopAndEnterThreatening();
            return;
        }

        _approachTimer += deltaTime;

        Vector2 currentPosition = _rigidbody.position;
        Vector2 playerPosition = _playerTransform.position;
        Vector2 toPlayer = playerPosition - currentPosition;

        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer <= _mutantData.StoppingDistanceFromPlayer ||
            _approachTimer >= _mutantData.MaxApproachDuration)
        {
            StopAndEnterThreatening();
            return;
        }

        Vector2 moveDirection = toPlayer.normalized;

        // Rigidbody2D.linearVelocity로 빠르게 접근한다.
        _rigidbody.linearVelocity = moveDirection * _mutantData.ApproachSpeed;
    }

    private void StopAndEnterThreatening()
    {
        // 접근을 멈추고 플레이어 앞에서 압박 상태로 전환한다.
        _rigidbody.linearVelocity = Vector2.zero;
        _state = MutantState.Threatening;

        PlayThreateningPulse();

        Debug.Log("변이체 접근 종료: Threatening 상태 진입");
    }

    private void EnterDormant()
    {
        _state = MutantState.Dormant;

        // 변이체는 기본 상태에서 약하게 꿈틀거리지만 이동하지 않는다.
        _rigidbody.linearVelocity = Vector2.zero;

        // 잠복 상태에서는 시체처럼 보이도록 흰색을 유지한다.
        SetSpriteColor(_dormantColor);

        PlayDormantPulse();
    }

    private void PlayDormantPulse()
    {
        if (_mutantData == null)
        {
            return;
        }

        KillScaleTween();

        // 숨어 있는 동안 약하게 꿈틀거리는 Scale 펄스.
        _scaleTween = transform
            .DOScale(_originalScale * _mutantData.DormantPulseScale, _mutantData.DormantPulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void PlayJumpScarePulse()
    {
        if (_mutantData == null)
        {
            return;
        }

        KillScaleTween();

        Sequence sequence = DOTween.Sequence();

        // 접근 시작 순간 몸이 확 커졌다가 돌아오는 점프스케어 느낌.
        sequence.Append(transform.DOScale(
            _originalScale * _mutantData.JumpScareScale,
            _mutantData.JumpScareScaleDuration));

        sequence.Append(transform.DOScale(
            _originalScale,
            _mutantData.JumpScareScaleDuration));

        _scaleTween = sequence;
    }

    private void PlayThreateningPulse()
    {
        if (_mutantData == null)
        {
            return;
        }

        KillScaleTween();

        // 플레이어 앞에서 멈춘 뒤 압박감을 주는 반복 펄스.
        _scaleTween = transform
            .DOScale(_originalScale * _mutantData.ThreateningPulseScale, _mutantData.ThreateningPulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void KillScaleTween()
    {
        if (_scaleTween == null)
        {
            return;
        }

        _scaleTween.Kill();
        _scaleTween = null;

        // Tween 중단 후 원래 크기로 복구한다.
        transform.localScale = _originalScale;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_mutantData == null)
        {
            return;
        }

        // 변이체 거리 감지 범위를 표시한다.
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.45f);
        Gizmos.DrawWireSphere(transform.position, _mutantData.DetectionDistance);

        // 플레이어 앞에서 멈추는 거리 기준을 표시한다.
        Gizmos.color = new Color(1.0f, 0.75f, 0.0f, 0.45f);
        Gizmos.DrawWireSphere(transform.position, _mutantData.StoppingDistanceFromPlayer);
    }
#endif
}