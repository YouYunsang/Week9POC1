using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInputReader _playerInputReader;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();

        if (_playerInputReader == null)
        {
            return;
        }

        // 이동 입력을 PlayerInputReader로 전달한다.
        _playerInputReader.SetMoveInput(moveInput);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        bool isSprinting = context.ReadValueAsButton();

        if (_playerInputReader == null)
        {
            return;
        }

        // Sprint 입력 상태를 PlayerInputReader로 전달한다.
        _playerInputReader.SetSprintInput(isSprinting);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (_playerInputReader == null)
        {
            return;
        }

        if (context.performed)
        {
            // 상호작용 시작 입력을 알린다.
            _playerInputReader.NotifyInteractInputStarted();
            return;
        }

        if (context.canceled)
        {
            // 상호작용 취소 입력을 알린다.
            _playerInputReader.NotifyInteractInputCanceled();
        }
    }

    public void OnFlashlight(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (_playerInputReader == null)
        {
            return;
        }

        // 손전등 토글 입력을 알린다.
        _playerInputReader.NotifyFlashlightToggleInputStarted();
    }

    private void OnDisable()
    {
        if (_playerInputReader == null)
        {
            return;
        }

        // InputManager가 꺼질 때 입력 상태를 초기화한다.
        _playerInputReader.ResetInput();
    }
}