using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class DoorController : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private int _interactionPriority = 30;

    [Header("Access")]
    [SerializeField] private DoorAccessType _accessType = DoorAccessType.AlwaysOpen;
    [SerializeField] private int _requiredKeyCardId = 1;

    [Header("Door Visual")]
    [SerializeField] private Transform _doorVisualTransform;
    [SerializeField] private SpriteRenderer _doorSpriteRenderer;
    [SerializeField] private Collider2D _doorCollider;
    [SerializeField] private bool _useManualDoorHeight;
    [SerializeField] private float _manualDoorHeight = 2.0f;

    [Header("Tween")]
    [SerializeField] private float _openDuration = 0.8f;
    [SerializeField] private float _stayOpenDuration = 2.0f;
    [SerializeField] private float _closeDuration = 0.6f;
    [SerializeField] private Ease _openEase = Ease.OutCubic;
    [SerializeField] private Ease _closeEase = Ease.InCubic;

    private Vector3 _closedPosition;
    private Vector3 _closedScale;
    private Sequence _doorSequence;

    private bool _isUnlocked;
    private bool _isOpen;
    private bool _isAnimating;

    private string _message;
    private float _messageEndTime;

    public int InteractionPriority => _interactionPriority;

    private void Awake()
    {
        if (_doorCollider == null)
        {
            // 문 Collider를 캐싱한다.
            _doorCollider = GetComponent<Collider2D>();
        }

        if (_doorVisualTransform == null)
        {
            // 별도 Visual Transform이 없으면 자기 자신을 Visual로 사용한다.
            _doorVisualTransform = transform;
        }

        if (_doorSpriteRenderer == null)
        {
            // 같은 오브젝트 또는 자식에서 SpriteRenderer를 찾는다.
            _doorSpriteRenderer = _doorVisualTransform.GetComponent<SpriteRenderer>();
        }

        // 닫힌 상태의 위치와 스케일을 저장한다.
        _closedPosition = _doorVisualTransform.position;
        _closedScale = _doorVisualTransform.localScale;
    }

    private void OnDisable()
    {
        KillDoorSequence();
    }

    public bool CanInteract(InteractionContext context)
    {
        if (_isAnimating || _isOpen)
        {
            return false;
        }

        if (context.PlayerCondition == null || !context.PlayerCondition.CanInteract)
        {
            return false;
        }

        if (_accessType == DoorAccessType.KeyCard)
        {
            return context.PlayerInventory != null;
        }

        return true;
    }

    public void Interact(InteractionContext context)
    {
        if (!CanInteract(context))
        {
            return;
        }

        if (!CanOpen(context))
        {
            ShowMessage($"ID 카드 {_requiredKeyCardId}가 필요하다.");
            return;
        }

        OpenDoor();
    }

    public string GetInteractionPrompt()
    {
        if (_accessType == DoorAccessType.KeyCard && !_isUnlocked)
        {
            return $"E: ID 카드 {_requiredKeyCardId} 문";
        }

        return "E: 문 열기";
    }

    private bool CanOpen(InteractionContext context)
    {
        if (_accessType == DoorAccessType.AlwaysOpen)
        {
            return true;
        }

        if (_accessType == DoorAccessType.KeyCard)
        {
            if (_isUnlocked)
            {
                // 한 번 인증된 ID 카드 문은 이후 계속 열 수 있다.
                return true;
            }

            if (context.PlayerInventory == null)
            {
                return false;
            }

            if (!context.PlayerInventory.HasKeyCard(_requiredKeyCardId))
            {
                return false;
            }

            // ID 카드를 한 번 사용하면 해당 문은 영구 Unlock된다.
            _isUnlocked = true;
            Debug.Log($"ID 카드 {_requiredKeyCardId} 인증 완료. 문 잠금 해제.");
            return true;
        }

        return false;
    }

    private void OpenDoor()
    {
        if (_doorVisualTransform == null)
        {
            Debug.LogWarning($"{nameof(DoorController)}: Door Visual Transform이 연결되지 않았습니다.");
            return;
        }

        KillDoorSequence();

        _isAnimating = true;
        _isOpen = true;

        if (_doorCollider != null)
        {
            // 문이 열리는 동안 통과 가능하도록 Collider를 끈다.
            _doorCollider.enabled = false;
        }

        float doorHeight = GetDoorHeight();
        Vector3 openPosition = _closedPosition + Vector3.up * (doorHeight * 0.5f);
        Vector3 openScale = new Vector3(_closedScale.x, 0.0f, _closedScale.z);

        _doorSequence = DOTween.Sequence();

        // 문의 아랫부분이 위로 말려 올라가는 것처럼 위치와 Y 스케일을 동시에 변경한다.
        _doorSequence.Append(
            _doorVisualTransform
                .DOMove(openPosition, _openDuration)
                .SetEase(_openEase));

        _doorSequence.Join(
            _doorVisualTransform
                .DOScale(openScale, _openDuration)
                .SetEase(_openEase));

        _doorSequence.AppendInterval(_stayOpenDuration);

        // 일정 시간이 지나면 다시 닫힌 상태로 복구한다.
        _doorSequence.Append(
            _doorVisualTransform
                .DOMove(_closedPosition, _closeDuration)
                .SetEase(_closeEase));

        _doorSequence.Join(
            _doorVisualTransform
                .DOScale(_closedScale, _closeDuration)
                .SetEase(_closeEase));

        _doorSequence.OnComplete(HandleCloseCompleted);

        Debug.Log("문 개방 시작");
    }

    private void HandleCloseCompleted()
    {
        _isAnimating = false;
        _isOpen = false;

        if (_doorCollider != null)
        {
            // 문이 완전히 닫힌 뒤 Collider를 다시 켠다.
            _doorCollider.enabled = true;
        }

        // 닫힌 상태를 정확히 보정한다.
        _doorVisualTransform.position = _closedPosition;
        _doorVisualTransform.localScale = _closedScale;

        Debug.Log("문 닫힘 완료");
    }

    private float GetDoorHeight()
    {
        if (_useManualDoorHeight)
        {
            return _manualDoorHeight;
        }

        if (_doorSpriteRenderer != null)
        {
            // SpriteRenderer 기준 월드 높이를 사용한다.
            return _doorSpriteRenderer.bounds.size.y;
        }

        if (_doorCollider != null)
        {
            // SpriteRenderer가 없으면 Collider 기준 월드 높이를 사용한다.
            return _doorCollider.bounds.size.y;
        }

        return _manualDoorHeight;
    }

    private void KillDoorSequence()
    {
        if (_doorSequence == null)
        {
            return;
        }

        _doorSequence.Kill();
        _doorSequence = null;
    }

    private void ShowMessage(string message)
    {
        _message = message;
        _messageEndTime = Time.time + 2.0f;
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(_message) || Time.time > _messageEndTime)
        {
            return;
        }

        Rect messageRect = new Rect(
            Screen.width * 0.5f - 180.0f,
            Screen.height * 0.22f,
            360.0f,
            40.0f);

        GUI.Box(messageRect, _message);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // ID 카드 번호는 1 이상으로 제한한다.
        _requiredKeyCardId = Mathf.Max(1, _requiredKeyCardId);

        // 문 높이와 Tween 시간은 너무 작지 않게 제한한다.
        _manualDoorHeight = Mathf.Max(0.1f, _manualDoorHeight);
        _openDuration = Mathf.Max(0.05f, _openDuration);
        _stayOpenDuration = Mathf.Max(0.0f, _stayOpenDuration);
        _closeDuration = Mathf.Max(0.05f, _closeDuration);
    }
#endif
}