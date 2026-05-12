using UnityEngine;

public sealed class PlayerCondition : MonoBehaviour
{
    private bool _isInputBlocked;
    private bool _isMovementBlocked;
    private bool _isInteractionBlocked;

    public bool CanReceiveInput => !_isInputBlocked;
    public bool CanMove => CanReceiveInput && !_isMovementBlocked;
    public bool CanInteract => CanReceiveInput && !_isInteractionBlocked;

    public void SetInputBlocked(bool isBlocked)
    {
        // 외부 시스템이 플레이어 전체 입력을 막을 수 있게 한다.
        _isInputBlocked = isBlocked;
    }

    public void SetMovementBlocked(bool isBlocked)
    {
        // 패닉, 연출, 이벤트 중 이동만 막을 수 있게 한다.
        _isMovementBlocked = isBlocked;
    }

    public void SetInteractionBlocked(bool isBlocked)
    {
        // 연출이나 조작 불능 상태에서 상호작용만 막을 수 있게 한다.
        _isInteractionBlocked = isBlocked;
    }

    public void ResetCondition()
    {
        // 상태를 기본 조작 가능 상태로 되돌린다.
        _isInputBlocked = false;
        _isMovementBlocked = false;
        _isInteractionBlocked = false;
    }
}