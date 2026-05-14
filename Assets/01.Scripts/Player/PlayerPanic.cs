using System;
using UnityEngine;

[RequireComponent(typeof(PlayerFlashlight))]
[RequireComponent(typeof(PlayerCondition))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerPanic : MonoBehaviour
{
    [SerializeField] private PanicData _panicData;

    private PlayerFlashlight _flashlight;
    private PlayerCondition _condition;
    private PlayerMovement _movement;
    private PlayerController _controller;
    private Rigidbody2D _rigidbody;

    private float _currentPanic;
    private bool _isControlLost;
    private float _controlLossTimer;
    private Vector2 _controlLossDirection;

    public event Action<float, float> PanicChanged;
    public event Action ControlLossStarted;
    public event Action ControlLossEnded;

    public float CurrentPanic => _currentPanic;
    public float MaxPanic => _panicData != null ? _panicData.MaxPanic : 0.0f;
    public float PanicRatio => MaxPanic <= 0.0f ? 0.0f : Mathf.Clamp01(_currentPanic / MaxPanic);
    public bool IsControlLost => _isControlLost;

    private void Awake()
    {
        // 같은 GameObject의 컴포넌트를 캐싱한다.
        _flashlight = GetComponent<PlayerFlashlight>();
        _condition = GetComponent<PlayerCondition>();
        _movement = GetComponent<PlayerMovement>();
        _controller = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody2D>();

        if (_panicData == null)
        {
            Debug.LogError($"{nameof(PlayerPanic)}: PanicData가 연결되지 않았습니다.");
            return;
        }

        // 시작 패닉 수치를 초기화한다.
        _currentPanic = _panicData.InitialPanic;
    }

    private void OnEnable()
    {
        // 변이체 점프스케어 이벤트를 구독한다.
        GameEventBus.MutantJumpScareStarted += HandleMutantJumpScareStarted;
    }

    private void OnDisable()
    {
        // 비활성화 시 이벤트 구독을 해제한다.
        GameEventBus.MutantJumpScareStarted -= HandleMutantJumpScareStarted;
    }

    private void Start()
    {
        RaisePanicChanged();
    }

    private void Update()
    {
        if (_panicData == null)
        {
            return;
        }

        if (_isControlLost)
        {
            TickControlLoss(Time.deltaTime);
            return;
        }

        TickPanicByLight(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_isControlLost)
        {
            return;
        }

        // 통제 상실 중에는 플레이어를 랜덤 방향으로 강제 이동시킨다.
        _rigidbody.linearVelocity = _controlLossDirection * _panicData.ControlLossMoveSpeed;
    }

    public void AddPanic(float amount)
    {
        if (_panicData == null || amount <= 0.0f || _isControlLost)
        {
            return;
        }

        SetPanic(_currentPanic + amount);
    }

    public void AddMutantThreatPanic(float deltaTime)
    {
        if (_panicData == null)
        {
            return;
        }

        // 변이체 Threatening 상태 근처에 있을 때 패닉을 지속 증가시킨다.
        AddPanic(_panicData.MutantThreatPanicPerSecond * deltaTime);
    }

    public void ReducePanic(float amount)
    {
        if (_panicData == null || amount <= 0.0f || _isControlLost)
        {
            return;
        }

        SetPanic(_currentPanic - amount);
    }

    private void TickPanicByLight(float deltaTime)
    {
        if (_flashlight != null && _flashlight.IsOn)
        {
            // 손전등이 켜져 있으면 패닉이 천천히 감소한다.
            ReducePanic(_panicData.PanicDecreasePerSecondInLight * deltaTime);
            return;
        }

        // 손전등이 꺼져 있으면 어둠 속에 있다고 보고 패닉이 증가한다.
        AddPanic(_panicData.PanicIncreasePerSecondInDark * deltaTime);
    }

    private void SetPanic(float value)
    {
        _currentPanic = Mathf.Clamp(value, 0.0f, _panicData.MaxPanic);
        RaisePanicChanged();

        if (_currentPanic >= _panicData.MaxPanic)
        {
            StartControlLoss();
        }
    }

    private void StartControlLoss()
    {
        if (_isControlLost)
        {
            return;
        }

        _isControlLost = true;
        _controlLossTimer = 0.0f;
        _controlLossDirection = GetRandomControlLossDirection();

        if (_controller != null)
        {
            // 통제 상실 중 PlayerController 입력 전달을 막는다.
            _controller.enabled = false;
        }

        if (_condition != null)
        {
            // 통제 상실 중 상호작용은 막는다.
            _condition.SetInteractionBlocked(true);
        }

        if (_movement != null)
        {
            // 기존 입력 이동을 제거한다.
            _movement.SetMoveInput(Vector2.zero);
            _movement.SetSprintState(false);
        }

        ControlLossStarted?.Invoke();

        Debug.Log("패닉 100: 통제 상실 시작");
    }

    private void TickControlLoss(float deltaTime)
    {
        _controlLossTimer += deltaTime;

        if (_controlLossTimer < _panicData.ControlLossDuration)
        {
            return;
        }

        EndControlLoss();
    }

    private void EndControlLoss()
    {
        _isControlLost = false;

        if (_rigidbody != null)
        {
            // 강제 이동을 멈춘다.
            _rigidbody.linearVelocity = Vector2.zero;
        }

        if (_movement != null)
        {
            // 이동 입력 상태를 초기화한다.
            _movement.SetMoveInput(Vector2.zero);
            _movement.SetSprintState(false);
        }

        if (_condition != null)
        {
            // 상호작용 가능 상태로 복구한다.
            _condition.SetInteractionBlocked(false);
        }

        if (_controller != null)
        {
            // PlayerController를 다시 활성화한다.
            _controller.enabled = true;
        }

        _currentPanic = Mathf.Clamp(_panicData.PanicAfterControlLoss, 0.0f, _panicData.MaxPanic);
        RaisePanicChanged();

        ControlLossEnded?.Invoke();

        Debug.Log("통제 상실 종료");
    }

    private Vector2 GetRandomControlLossDirection()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

        if (randomDirection.sqrMagnitude <= 0.001f)
        {
            return Vector2.right;
        }

        return randomDirection;
    }

    private void HandleMutantJumpScareStarted(Vector2 position)
    {
        if (_panicData == null)
        {
            return;
        }

        // 변이체가 접근을 시작하면 패닉을 즉시 증가시킨다.
        AddPanic(_panicData.MutantJumpScarePanicAmount);
    }

    private void RaisePanicChanged()
    {
        PanicChanged?.Invoke(_currentPanic, MaxPanic);
    }
}