using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCondition))]
[RequireComponent(typeof(PlayerInteractionDetector))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerFlashlight))]
public sealed class PlayerController : MonoBehaviour
{
    private PlayerInputReader _inputReader;
    private PlayerMovement _movement;
    private PlayerCondition _condition;
    private PlayerInteractionDetector _interactionDetector;
    private PlayerFlashlight _flashlight;

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
        _interactionDetector = GetComponent<PlayerInteractionDetector>();
        _flashlight = GetComponent<PlayerFlashlight>();
    }

    private void OnEnable()
    {
        // 입력 이벤트를 구독한다.
        _inputReader.MoveInputChanged += HandleMoveInputChanged;
        _inputReader.SprintInputChanged += HandleSprintInputChanged;
        _inputReader.InteractInputStarted += HandleInteractInputStarted;
        _inputReader.FlashlightToggleInputStarted += HandleFlashlightToggleInputStarted;
    }

    private void OnDisable()
    {
        // 비활성화 시 이벤트 구독을 해제한다.
        _inputReader.MoveInputChanged -= HandleMoveInputChanged;
        _inputReader.SprintInputChanged -= HandleSprintInputChanged;
        _inputReader.InteractInputStarted -= HandleInteractInputStarted;
        _inputReader.FlashlightToggleInputStarted -= HandleFlashlightToggleInputStarted;
    }

    private void Update()
    {
        // 플레이어 상태에 따라 실제 이동 입력을 결정한다.
        Vector2 finalMoveInput = _condition.CanMove ? _moveInput : Vector2.zero;
        bool finalSprintState = _condition.CanMove && _isSprinting;

        // 이동 전용 컴포넌트에 최종 입력을 전달한다.
        _movement.SetMoveInput(finalMoveInput);
        _movement.SetSprintState(finalSprintState);
    }

    public void SetMovementEnabled(bool isEnabled)
    {
        // 패닉, 이벤트 연출 등에서 플레이어 이동을 제어할 수 있게 한다.
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

        // 입력값은 Controller가 보관하고, 이동은 Update에서 처리한다.
        _moveInput = moveInput;
    }

    private void HandleSprintInputChanged(bool isSprinting)
    {
        if (!_condition.CanReceiveInput)
        {
            _isSprinting = false;
            return;
        }

        // Sprint는 유지하되, 이번 단계에서는 배터리/패닉과 연결하지 않는다.
        _isSprinting = isSprinting;
    }

    private void HandleInteractInputStarted()
    {
        if (!_condition.CanReceiveInput || !_condition.CanInteract)
        {
            return;
        }

        // 현재 감지된 상호작용 대상을 실행한다.
        _interactionDetector.TryInteract();
    }

    private void HandleFlashlightToggleInputStarted()
    {
        if (!_condition.CanReceiveInput)
        {
            return;
        }

        // F 입력으로 손전등을 켜거나 끈다.
        _flashlight.Toggle();
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