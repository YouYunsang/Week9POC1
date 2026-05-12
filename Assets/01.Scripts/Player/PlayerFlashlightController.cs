using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerCondition))]
[RequireComponent(typeof(PlayerFlashlight))]
public sealed class PlayerFlashlightController : MonoBehaviour
{
    private PlayerInputReader _inputReader;
    private PlayerCondition _condition;
    private PlayerFlashlight _flashlight;

    private void Awake()
    {
        // 같은 GameObject 내부 컴포넌트를 Awake에서 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _condition = GetComponent<PlayerCondition>();
        _flashlight = GetComponent<PlayerFlashlight>();
    }

    private void Reset()
    {
        // 같은 GameObject 내부 컴포넌트를 Reset에서 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _condition = GetComponent<PlayerCondition>();
        _flashlight = GetComponent<PlayerFlashlight>();
    }

    private void OnEnable()
    {
        // 손전등 토글 입력 이벤트를 구독한다.
        _inputReader.FlashlightToggleInputStarted += HandleFlashlightToggleInputStarted;
    }

    private void OnDisable()
    {
        // 비활성화 시 이벤트 구독을 해제한다.
        _inputReader.FlashlightToggleInputStarted -= HandleFlashlightToggleInputStarted;
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
}