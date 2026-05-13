using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class MapCellRevealTrigger : MonoBehaviour
{
    [SerializeField] private MapDiscoveryState _mapDiscoveryState;
    [SerializeField] private Vector2Int _cellPosition;

    public Vector2Int CellPosition => _cellPosition;

    private void Awake()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();

        // 맵 Cell 감지는 Trigger로 처리한다.
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryEnterCell(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 플레이어가 시작부터 Trigger 안에 있는 경우를 보완한다.
        TryEnterCell(other);
    }

    private void TryEnterCell(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (_mapDiscoveryState == null)
        {
            Debug.LogWarning($"{nameof(MapCellRevealTrigger)}: MapDiscoveryState가 연결되지 않았습니다.");
            return;
        }

        // 이 Trigger 좌표를 플레이어 현재 Cell이자 발견 Cell로 등록한다.
        _mapDiscoveryState.EnterCell(_cellPosition);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Scene View에서 이 Trigger가 어떤 맵 좌표인지 확인하기 위한 Gizmo.
        Gizmos.color = new Color(0.2f, 0.8f, 1.0f, 0.5f);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
#endif
}