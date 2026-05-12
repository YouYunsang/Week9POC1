using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PlayerBattery))]
public sealed class PlayerFlashlight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D _flashlight;
    [SerializeField] private Collider2D _detectionCollider;

    [Header("Initial State")]
    [SerializeField] private bool _isOnAtStart = false;

    private PlayerBattery _battery;

    public bool IsOn { get; private set; }
    public Vector2 OriginPosition => transform.position;

    private void Awake()
    {
        // 같은 GameObject의 배터리 컴포넌트를 캐싱한다.
        _battery = GetComponent<PlayerBattery>();

        // 시작 상태에 맞춰 손전등을 초기화한다.
        SetFlashlightEnabled(_isOnAtStart && _battery.HasBattery);
    }

    private void Update()
    {
        if (!IsOn)
        {
            return;
        }

        float drainAmount = _battery.GetFlashlightDrainAmount(Time.deltaTime);

        if (_battery.TryConsume(drainAmount))
        {
            if (_battery.HasBattery)
            {
                return;
            }
        }

        // 배터리가 0이 되면 손전등을 자동으로 끈다.
        SetFlashlightEnabled(false);
        Debug.Log("배터리가 없어 손전등이 꺼졌습니다.");
    }

    public void Toggle()
    {
        if (IsOn)
        {
            // 켜져 있으면 배터리 상태와 무관하게 끌 수 있다.
            SetFlashlightEnabled(false);
            return;
        }

        if (!_battery.HasBattery)
        {
            // 배터리가 없으면 손전등을 켤 수 없다.
            Debug.Log("배터리가 없어 손전등을 켤 수 없습니다.");
            return;
        }

        SetFlashlightEnabled(true);
    }

    public void SetFlashlightEnabled(bool isEnabled)
    {
        IsOn = isEnabled;

        if (_flashlight != null)
        {
            // 실제 Light2D 시각 표현을 켜거나 끈다.
            _flashlight.enabled = IsOn;
        }

        if (_detectionCollider != null)
        {
            // 이후 패닉 억제 영역으로 사용할 수 있는 감지 Collider를 함께 제어한다.
            _detectionCollider.enabled = IsOn;
        }

        Debug.Log(IsOn ? "손전등 ON" : "손전등 OFF");
    }
}