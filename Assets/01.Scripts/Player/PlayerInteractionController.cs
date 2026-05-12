using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerCondition))]
[RequireComponent(typeof(PlayerInteractionDetector))]
public sealed class PlayerInteractionController : MonoBehaviour
{
    private PlayerInputReader _inputReader;
    private PlayerCondition _condition;
    private PlayerInteractionDetector _interactionDetector;

    private void Awake()
    {
        // 같은 GameObject 내부 컴포넌트를 Awake에서 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _condition = GetComponent<PlayerCondition>();
        _interactionDetector = GetComponent<PlayerInteractionDetector>();
    }

    private void Reset()
    {
        // 같은 GameObject 내부 컴포넌트를 Reset에서 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _condition = GetComponent<PlayerCondition>();
        _interactionDetector = GetComponent<PlayerInteractionDetector>();
    }

    private void OnEnable()
    {
        // 상호작용 입력 이벤트를 구독한다.
        _inputReader.InteractInputStarted += HandleInteractInputStarted;
    }

    private void OnDisable()
    {
        // 비활성화 시 이벤트 구독을 해제한다.
        _inputReader.InteractInputStarted -= HandleInteractInputStarted;
    }

    private void HandleInteractInputStarted()
    {
        if (!_condition.CanReceiveInput || !_condition.CanInteract)
        {
            return;
        }

        // 현재 감지 중인 상호작용 대상을 실행한다.
        _interactionDetector.TryInteract();
    }
}