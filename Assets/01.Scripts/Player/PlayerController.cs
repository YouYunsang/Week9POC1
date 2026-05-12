using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCondition))]
public sealed class PlayerController : MonoBehaviour
{
    private PlayerInputReader _inputReader;
    private PlayerMovement _movement;
    private PlayerCondition _condition;

    private Vector2 _moveInput;
    private bool _isSprinting;

    public bool IsSprinting => _isSprinting;
    public bool CanMove => _condition != null && _condition.CanMove;

    private void Awake()
    {
        // 같은 GameObject 내부 컴포넌트를 Awake에서 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _movement = GetComponent<PlayerMovement>();
        _condition = GetComponent<PlayerCondition>();
    }

    private void OnEnable()
    {
        // 이동 관련 입력 이벤트를 구독한다.
        _inputReader.MoveInputChanged += HandleMoveInputChanged;
        _inputReader.SprintInputChanged += HandleSprintInputChanged;
    }

    private void OnDisable()
    {
        // 비활성화 시 이벤트 구독을 해제한다.
        _inputReader.MoveInputChanged -= HandleMoveInputChanged;
        _inputReader.SprintInputChanged -= HandleSprintInputChanged;
    }

    private void Update()
    {
        Vector2 finalMoveInput = _condition.CanMove ? _moveInput : Vector2.zero;
        bool finalSprintState = _condition.CanMove && _isSprinting;

        // 이동 물리는 PlayerMovement가 처리하고, Controller는 입력만 전달한다.
        _movement.SetMoveInput(finalMoveInput);
        _movement.SetSprintState(finalSprintState);
    }

    public void SetMovementEnabled(bool isEnabled)
    {
        // 외부 시스템이 플레이어 이동 가능 여부를 제어할 수 있게 한다.
        _condition.SetMovementBlocked(!isEnabled);

        if (isEnabled)
        {
            return;
        }

        ClearMovementInput();
    }

    private void HandleMoveInputChanged(Vector2 moveInput)
    {
        if (!_condition.CanReceiveInput)
        {
            _moveInput = Vector2.zero;
            return;
        }

        // 이동 입력을 저장하고 실제 전달은 Update에서 처리한다.
        _moveInput = moveInput;
    }

    private void HandleSprintInputChanged(bool isSprinting)
    {
        if (!_condition.CanReceiveInput)
        {
            _isSprinting = false;
            return;
        }

        // Sprint는 유지하되 이번 단계에서는 배터리/패닉과 연결하지 않는다.
        _isSprinting = isSprinting;
    }

    private void ClearMovementInput()
    {
        // 이동 입력과 Sprint 상태를 초기화한다.
        _moveInput = Vector2.zero;
        _isSprinting = false;

        _movement.SetMoveInput(Vector2.zero);
        _movement.SetSprintState(false);
    }
}