using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class KeyCardDoor : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private int _interactionPriority = 30;

    [Header("Key")]
    [SerializeField] private int _requiredKeyCardId = 1;

    [Header("Door")]
    [SerializeField] private Transform _doorVisualTransform;
    [SerializeField] private Transform _openedTargetTransform;
    [SerializeField] private Collider2D _doorCollider;
    [SerializeField] private float _openDuration = 0.8f;
    [SerializeField] private Ease _openEase = Ease.OutCubic;

    private bool _isOpen;
    private bool _isOpening;
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
            // 별도 Visual Transform이 없으면 자기 자신을 이동 대상으로 사용한다.
            _doorVisualTransform = transform;
        }
    }

    public bool CanInteract(InteractionContext context)
    {
        if (_isOpen || _isOpening)
        {
            return false;
        }

        if (context.PlayerCondition == null || !context.PlayerCondition.CanInteract)
        {
            return false;
        }

        return context.PlayerInventory != null;
    }

    public void Interact(InteractionContext context)
    {
        if (!CanInteract(context))
        {
            return;
        }

        if (!context.PlayerInventory.HasKeyCard(_requiredKeyCardId))
        {
            // ID 카드가 없으면 OnGUI 잠김 메시지를 표시한다.
            ShowMessage($"ID 카드 {_requiredKeyCardId}가 필요하다.");
            return;
        }

        OpenDoor();
    }

    public string GetInteractionPrompt()
    {
        return _isOpen ? "열린 문" : $"E: ID 카드 {_requiredKeyCardId} 문";
    }

    private void OpenDoor()
    {
        if (_openedTargetTransform == null)
        {
            Debug.LogWarning($"{nameof(KeyCardDoor)}: Opened Target Transform이 연결되지 않았습니다.");
            return;
        }

        _isOpening = true;

        // 문이 위로 올라가며 열리는 연출을 실행한다.
        _doorVisualTransform
            .DOMove(_openedTargetTransform.position, _openDuration)
            .SetEase(_openEase)
            .OnComplete(HandleOpenCompleted);

        Debug.Log($"ID 카드 {_requiredKeyCardId} 문 개방 시작");
    }

    private void HandleOpenCompleted()
    {
        _isOpening = false;
        _isOpen = true;

        if (_doorCollider != null)
        {
            // 문이 완전히 열린 뒤 통과 가능하도록 Collider를 비활성화한다.
            _doorCollider.enabled = false;
        }

        Debug.Log($"ID 카드 {_requiredKeyCardId} 문 개방 완료");
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

        // 문 열림 시간은 너무 작지 않게 제한한다.
        _openDuration = Mathf.Max(0.05f, _openDuration);
    }
#endif
}