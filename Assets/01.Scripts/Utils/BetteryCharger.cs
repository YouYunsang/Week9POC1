using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class BatteryCharger : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private int _interactionPriority = 15;

    [Header("Charge")]
    [SerializeField] private float _chargeDuration = 1.2f;

    [Header("Visual")]
    [SerializeField] private Color _usedColor = new Color(0.35f, 0.35f, 0.35f, 1.0f);

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    private PlayerBattery _currentBattery;
    private PlayerCondition _currentCondition;
    private PlayerMovement _currentMovement;

    private bool _isUsed;
    private bool _isWindowOpen;
    private bool _isCharging;
    private bool _isCompletionMessageOpen;

    private float _chargeTimer;
    private string _message;
    private Rect _windowRect = new Rect(40.0f, 80.0f, 420.0f, 220.0f);

    public int InteractionPriority => _interactionPriority;

    private void Awake()
    {
        Collider2D chargerCollider = GetComponent<Collider2D>();

        // 충전기는 상호작용 감지용 Trigger로 사용한다.
        chargerCollider.isTrigger = true;

        // 사용 완료 색상 변경을 위해 SpriteRenderer를 캐싱한다.
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    private void Update()
    {
        if (!_isCharging)
        {
            return;
        }

        TickCharging(Time.deltaTime);
    }

    public bool CanInteract(InteractionContext context)
    {
        if (_isCharging || _isWindowOpen)
        {
            return false;
        }

        if (context.PlayerCondition == null || !context.PlayerCondition.CanInteract)
        {
            return false;
        }

        return context.PlayerBattery != null && context.PlayerMovement != null;
    }

    public void Interact(InteractionContext context)
    {
        if (!CanInteract(context))
        {
            return;
        }

        _currentBattery = context.PlayerBattery;
        _currentCondition = context.PlayerCondition;
        _currentMovement = context.PlayerMovement;

        if (_isUsed)
        {
            OpenMessageWindow("이미 사용한 충전기다.");
            return;
        }

        if (_currentBattery.IsFull)
        {
            OpenMessageWindow("이미 배터리가 가득 찼다.");
            return;
        }

        OpenConfirmWindow();
    }

    public string GetInteractionPrompt()
    {
        if (_isUsed)
        {
            return "E: 사용 완료된 충전기";
        }

        return "E: 충전기 사용";
    }

    private void OpenConfirmWindow()
    {
        _message = "충전기를 사용하면 배터리 1칸이 회복된다.\n이 충전기는 한 번만 사용할 수 있다.";
        _isWindowOpen = true;
        _isCompletionMessageOpen = false;

        // 확인 UI가 열린 즉시 플레이어 조작을 고정한다.
        SetPlayerInteractionLock(true);

        // OnGUI 버튼 클릭을 위해 마우스를 표시한다.
        HideCursor.ShowCursorForUI();
    }

    private void OpenMessageWindow(string message)
    {
        _message = message;
        _isWindowOpen = true;
        _isCompletionMessageOpen = true;

        // 메시지 UI가 열린 동안도 플레이어를 고정한다.
        SetPlayerInteractionLock(true);

        // OnGUI 버튼 클릭을 위해 마우스를 표시한다.
        HideCursor.ShowCursorForUI();
    }

    private void StartCharging()
    {
        _isWindowOpen = false;
        _isCharging = true;
        _isCompletionMessageOpen = false;
        _chargeTimer = 0.0f;

        // 버튼 UI는 닫혔으므로 마우스를 다시 숨긴다.
        HideCursor.HideCursorForGameplay();

        // 충전 중에도 이동과 부력 차단은 유지한다.
        SetPlayerInteractionLock(true);
    }

    private void TickCharging(float deltaTime)
    {
        _chargeTimer += deltaTime;

        if (_chargeTimer < _chargeDuration)
        {
            return;
        }

        CompleteCharging();
    }

    private void CompleteCharging()
    {
        _isCharging = false;
        _isUsed = true;

        if (_currentBattery != null)
        {
            // 배터리 한 칸, 즉 최대량의 20%를 회복한다.
            _currentBattery.Recharge(_currentBattery.GetOneCellAmount());
        }

        if (_spriteRenderer != null)
        {
            // 사용 완료 상태를 어두운 색으로 표시한다.
            _spriteRenderer.color = _usedColor;
        }

        _message = "충전 완료. 배터리 1칸 회복.";
        _isWindowOpen = true;
        _isCompletionMessageOpen = true;

        // 완료 메시지 버튼을 누를 수 있도록 커서를 다시 표시한다.
        HideCursor.ShowCursorForUI();
    }

    private void CloseWindow()
    {
        _isWindowOpen = false;
        _isCompletionMessageOpen = false;

        // UI가 닫히면 플레이어 조작을 복구한다.
        SetPlayerInteractionLock(false);

        // 게임 플레이 상태로 돌아가므로 커서를 숨긴다.
        HideCursor.HideCursorForGameplay();

        ClearCurrentPlayerReferences();
    }

    private void SetPlayerInteractionLock(bool isLocked)
    {
        if (_currentCondition != null)
        {
            // UI 또는 충전 중에는 이동과 상호작용을 막는다.
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

        // UI 또는 충전 중에는 부력을 끄고, 종료 시 다시 켠다.
        _currentMovement.SetBuoyancyEnabled(!isLocked);
    }

    private void ClearCurrentPlayerReferences()
    {
        _currentBattery = null;
        _currentCondition = null;
        _currentMovement = null;
    }

    private void OnGUI()
    {
        if (_isCharging)
        {
            DrawChargingWindow();
            return;
        }

        if (_isWindowOpen)
        {
            // 충전기 확인/메시지 창을 표시한다.
            _windowRect = GUI.Window(GetInstanceID(), _windowRect, DrawWindow, "충전기");
        }
    }

    private void DrawWindow(int windowId)
    {
        GUILayout.Space(8.0f);
        GUILayout.Label(_message, GUILayout.Height(80.0f));
        GUILayout.Space(8.0f);

        if (!_isUsed && !_isCompletionMessageOpen && _currentBattery != null && !_currentBattery.IsFull)
        {
            if (GUILayout.Button("충전 시작", GUILayout.Height(36.0f)))
            {
                StartCharging();
            }
        }

        if (GUILayout.Button("닫기", GUILayout.Height(32.0f)))
        {
            CloseWindow();
        }

        GUI.DragWindow();
    }

    private void DrawChargingWindow()
    {
        float progressRatio = _chargeDuration <= 0.0f
            ? 1.0f
            : Mathf.Clamp01(_chargeTimer / _chargeDuration);

        Rect boxRect = new Rect(
            Screen.width * 0.5f - 180.0f,
            Screen.height * 0.5f - 60.0f,
            360.0f,
            120.0f);

        Rect barBackgroundRect = new Rect(
            boxRect.x + 24.0f,
            boxRect.y + 64.0f,
            boxRect.width - 48.0f,
            18.0f);

        Rect barFillRect = new Rect(
            barBackgroundRect.x,
            barBackgroundRect.y,
            barBackgroundRect.width * progressRatio,
            barBackgroundRect.height);

        // OnGUI로 임시 충전 진행 UI를 표시한다.
        GUI.Box(boxRect, "충전 중...");
        GUI.Box(barBackgroundRect, string.Empty);
        GUI.Box(barFillRect, string.Empty);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 충전 시간은 너무 짧지 않게 제한한다.
        _chargeDuration = Mathf.Max(0.1f, _chargeDuration);
    }
#endif
}