using System;
using UnityEngine;

public sealed class PlayerInputReader : MonoBehaviour
{
    public event Action<Vector2> MoveInputChanged;
    public event Action<bool> SprintInputChanged;
    public event Action InteractInputStarted;
    public event Action InteractInputCanceled;
    public event Action FlashlightToggleInputStarted;

    public Vector2 MoveInput { get; private set; }
    public bool IsSprinting { get; private set; }

    public void SetMoveInput(Vector2 moveInput)
    {
        // InputManager에서 받은 이동 입력을 저장한다.
        MoveInput = moveInput;

        // 이동 입력 변경을 구독자에게 전달한다.
        MoveInputChanged?.Invoke(MoveInput);
    }

    public void SetSprintInput(bool isSprinting)
    {
        // Sprint 입력 상태를 저장한다.
        IsSprinting = isSprinting;

        // Sprint 상태 변경을 구독자에게 전달한다.
        SprintInputChanged?.Invoke(IsSprinting);
    }

    public void NotifyInteractInputStarted()
    {
        // 상호작용 입력 시작을 알린다.
        InteractInputStarted?.Invoke();
    }

    public void NotifyInteractInputCanceled()
    {
        // Hold 상호작용 확장 가능성을 위해 취소 이벤트는 남긴다.
        InteractInputCanceled?.Invoke();
    }

    public void NotifyFlashlightToggleInputStarted()
    {
        // 손전등 토글 입력을 알린다.
        FlashlightToggleInputStarted?.Invoke();
    }

    public void ResetInput()
    {
        // 입력 상태가 남아 플레이어가 계속 움직이는 문제를 막는다.
        MoveInput = Vector2.zero;
        IsSprinting = false;

        MoveInputChanged?.Invoke(MoveInput);
        SprintInputChanged?.Invoke(IsSprinting);
        InteractInputCanceled?.Invoke();
    }

    private void OnDisable()
    {
        // 비활성화 시 입력 상태를 초기화한다.
        ResetInput();
    }
}