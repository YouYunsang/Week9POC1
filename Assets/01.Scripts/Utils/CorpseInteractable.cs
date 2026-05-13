using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class CorpseInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private int _interactionPriority = 20;

    [Header("Corpse")]
    [SerializeField] private int _keyCardId = 1;
    [SerializeField] private string _corpseTitle = "선원 시체";
    [TextArea]
    [SerializeField]
    private string _investigationText =
        "구조 신호는 이 시체에서 나오고 있었다.\n살아있는 사람은 아니다.";

    private PlayerInventory _currentInventory;
    private PlayerCondition _currentCondition;
    private PlayerMovement _currentMovement;

    private bool _isInvestigated;
    private bool _isWindowOpen;
    private string _message;
    private float _messageEndTime;
    private Rect _windowRect = new Rect(40.0f, 80.0f, 420.0f, 260.0f);

    public int InteractionPriority => _interactionPriority;

    private void Awake()
    {
        Collider2D corpseCollider = GetComponent<Collider2D>();

        // 시체는 물리 충돌보다 상호작용 감지 대상이다.
        corpseCollider.isTrigger = true;
    }

    public bool CanInteract(InteractionContext context)
    {
        if (context.PlayerCondition == null || !context.PlayerCondition.CanInteract)
        {
            return false;
        }

        return context.PlayerInventory != null && context.PlayerMovement != null;
    }

    public void Interact(InteractionContext context)
    {
        if (!CanInteract(context))
        {
            return;
        }

        if (_isInvestigated)
        {
            // 이미 조사한 시체는 OnGUI 메시지만 표시한다.
            ShowMessage("이미 조사했다.");
            return;
        }

        _currentInventory = context.PlayerInventory;
        _currentCondition = context.PlayerCondition;
        _currentMovement = context.PlayerMovement;

        OpenInvestigationWindow();
    }

    public string GetInteractionPrompt()
    {
        return _isInvestigated ? "E: 이미 조사한 시체" : "E: 시체 조사";
    }

    private void OpenInvestigationWindow()
    {
        _isWindowOpen = true;

        // OnGUI 버튼 UI가 열리면 플레이어 조작을 고정한다.
        SetPlayerInteractionLock(true);

        // OnGUI 버튼 클릭을 위해 마우스를 표시한다.
        HideCursor.ShowCursorForUI();
    }

    private void CloseInvestigationWindow()
    {
        _isWindowOpen = false;

        // OnGUI 버튼 UI가 닫히면 플레이어 조작을 복구한다.
        SetPlayerInteractionLock(false);

        // 게임 플레이 상태로 돌아가므로 커서를 다시 숨긴다.
        HideCursor.HideCursorForGameplay();

        _currentInventory = null;
        _currentCondition = null;
        _currentMovement = null;
    }

    private void AcquireKeyCard()
    {
        if (_currentInventory == null)
        {
            CloseInvestigationWindow();
            return;
        }

        // 시체 조사 보상으로 ID 카드를 획득한다.
        bool isAdded = _currentInventory.AddKeyCard(_keyCardId);

        if (isAdded)
        {
            _isInvestigated = true;
            ShowMessage($"ID 카드 {_keyCardId} 획득");
        }

        CloseInvestigationWindow();
    }

    private void SetPlayerInteractionLock(bool isLocked)
    {
        if (_currentCondition != null)
        {
            // UI가 열린 동안 이동과 상호작용을 막는다.
            _currentCondition.SetMovementBlocked(isLocked);
            _currentCondition.SetInteractionBlocked(isLocked);
        }

        if (_currentMovement == null)
        {
            return;
        }

        if (isLocked)
        {
            // UI가 열린 즉시 입력 이동과 Sprint를 멈춘다.
            _currentMovement.SetMoveInput(Vector2.zero);
            _currentMovement.SetSprintState(false);
        }

        // UI가 열린 동안 부력을 끄고, 닫히면 다시 켠다.
        _currentMovement.SetBuoyancyEnabled(!isLocked);
    }

    private void ShowMessage(string message)
    {
        _message = message;
        _messageEndTime = Time.time + 2.0f;
    }

    private void OnGUI()
    {
        if (_isWindowOpen)
        {
            // OnGUI로 임시 조사 UI를 표시한다.
            _windowRect = GUI.Window(GetInstanceID(), _windowRect, DrawInvestigationWindow, _corpseTitle);
        }

        if (!string.IsNullOrEmpty(_message) && Time.time <= _messageEndTime)
        {
            DrawMessage();
        }
    }

    private void DrawInvestigationWindow(int windowId)
    {
        GUILayout.Space(8.0f);

        GUILayout.Label(_investigationText, GUILayout.Height(120.0f));

        GUILayout.Space(12.0f);

        if (GUILayout.Button($"ID 카드 {_keyCardId} 획득", GUILayout.Height(36.0f)))
        {
            AcquireKeyCard();
        }

        if (GUILayout.Button("닫기", GUILayout.Height(32.0f)))
        {
            CloseInvestigationWindow();
        }

        GUI.DragWindow();
    }

    private void DrawMessage()
    {
        Rect messageRect = new Rect(
            Screen.width * 0.5f - 160.0f,
            Screen.height * 0.18f,
            320.0f,
            40.0f);

        GUI.Box(messageRect, _message);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // ID 카드 번호는 1 이상으로 제한한다.
        _keyCardId = Mathf.Max(1, _keyCardId);
    }
#endif
}