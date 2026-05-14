using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class MutantThreatPanicZone : MonoBehaviour
{
    [SerializeField] private MutantController _mutantController;

    private PlayerPanic _playerPanic;

    private void Awake()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();

        // 변이체 위협 패닉 영역은 Trigger로 처리한다.
        zoneCollider.isTrigger = true;
    }

    private void Update()
    {
        if (_mutantController == null || _playerPanic == null)
        {
            return;
        }

        if (_mutantController.State != MutantState.Threatening)
        {
            return;
        }

        // 변이체가 Threatening 상태이고 플레이어가 근처에 있으면 패닉을 지속 증가시킨다.
        _playerPanic.AddMutantThreatPanic(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        _playerPanic = other.GetComponent<PlayerPanic>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerPanic exitingPanic = other.GetComponent<PlayerPanic>();

        if (_playerPanic == exitingPanic)
        {
            _playerPanic = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 변이체 Threatening 상태에서 패닉을 증가시키는 범위를 표시한다.
        Gizmos.color = new Color(0.8f, 0.0f, 0.8f, 0.35f);

        Collider2D zoneCollider = GetComponent<Collider2D>();

        if (zoneCollider != null)
        {
            Gizmos.DrawWireCube(zoneCollider.bounds.center, zoneCollider.bounds.size);
        }
    }
#endif
}