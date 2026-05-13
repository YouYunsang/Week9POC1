using System.Collections.Generic;
using System.Text;
using UnityEngine;

public sealed class KeyCardDebugView : MonoBehaviour
{
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private bool _isVisible = true;

    private readonly StringBuilder _stringBuilder = new StringBuilder();

    private void OnGUI()
    {
        if (!_isVisible || _playerInventory == null)
        {
            return;
        }

        IReadOnlyCollection<int> keyCardIds = _playerInventory.GetOwnedKeyCardIds();

        _stringBuilder.Clear();
        _stringBuilder.AppendLine("KeyCards");

        if (keyCardIds.Count <= 0)
        {
            _stringBuilder.AppendLine("- 없음");
        }
        else
        {
            foreach (int keyCardId in keyCardIds)
            {
                _stringBuilder.AppendLine($"- ID 카드 {keyCardId}");
            }
        }

        Rect rect = new Rect(30.0f, 130.0f, 180.0f, 90.0f);

        // 현재 보유한 KeyCard를 OnGUI로 표시한다.
        GUI.Box(rect, _stringBuilder.ToString());
    }
}