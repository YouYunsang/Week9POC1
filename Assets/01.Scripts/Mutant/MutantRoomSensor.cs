using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class MutantRoomSensor : MonoBehaviour
{
    private Transform _currentPlayer;

    public event Action<Transform> PlayerEnteredRoom;
    public event Action<Transform> PlayerExitedRoom;

    private void Awake()
    {
        Collider2D sensorCollider = GetComponent<Collider2D>();

        // 방 감지는 Trigger Collider로 처리한다.
        sensorCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryEnterPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 플레이어가 게임 시작 시 이미 Trigger 안에 있는 경우를 보완한다.
        TryEnterPlayer(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (_currentPlayer == null || _currentPlayer != other.transform)
        {
            return;
        }

        // 플레이어가 변이체 방 감지 영역에서 나갔음을 알린다.
        PlayerExitedRoom?.Invoke(_currentPlayer);
        _currentPlayer = null;
    }

    private void TryEnterPlayer(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (_currentPlayer == other.transform)
        {
            return;
        }

        _currentPlayer = other.transform;

        // 플레이어가 변이체 방 감지 영역에 들어왔음을 알린다.
        PlayerEnteredRoom?.Invoke(_currentPlayer);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 변이체가 반응할 방 Trigger 범위를 표시한다.
        Gizmos.color = new Color(1.0f, 0.2f, 0.2f, 0.35f);

        Collider2D sensorCollider = GetComponent<Collider2D>();

        if (sensorCollider != null)
        {
            Gizmos.DrawWireCube(sensorCollider.bounds.center, sensorCollider.bounds.size);
        }
    }
#endif
}