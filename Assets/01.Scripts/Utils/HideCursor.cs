using UnityEngine;

public sealed class HideCursor : MonoBehaviour
{
    private static HideCursor _instance;

    [SerializeField] private bool _hideCursorOnStart = true;

    private bool _shouldHideCursorOnResume;

    private void Awake()
    {
        // 씬 안에서 커서 제어용 인스턴스를 캐싱한다.
        _instance = this;
    }

    private void Start()
    {
        if (!_hideCursorOnStart)
        {
            return;
        }

        HideGameCursor();
        _shouldHideCursorOnResume = true;
    }

    public static void ShowCursorForUI()
    {
        if (_instance == null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        // OnGUI 버튼 조작을 위해 커서를 표시한다.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void HideCursorForGameplay()
    {
        if (_instance == null)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        // 마우스 조준은 유지해야 하므로 커서는 숨기되 잠그지 않는다.
        _instance.HideGameCursor();
    }

    private void HideGameCursor()
    {
        // 게임 플레이 중에는 커서를 숨긴다.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }
}