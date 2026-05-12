using UnityEngine;
using UnityEngine.UI;

public sealed class BatteryGaugeView : MonoBehaviour
{
    [SerializeField] private BatteryViewModel _viewModel;
    [SerializeField] private Image[] _batteryCellImages;

    private void OnEnable()
    {
        if (_viewModel == null)
        {
            return;
        }

        // ViewModel의 UI 상태 변경 이벤트를 구독한다.
        _viewModel.CellFillAmountsChanged += HandleCellFillAmountsChanged;
    }

    private void OnDisable()
    {
        if (_viewModel == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _viewModel.CellFillAmountsChanged -= HandleCellFillAmountsChanged;
    }

    private void HandleCellFillAmountsChanged(float[] fillAmounts)
    {
        if (_batteryCellImages == null)
        {
            return;
        }

        int count = Mathf.Min(_batteryCellImages.Length, fillAmounts.Length);

        for (int i = 0; i < count; i++)
        {
            if (_batteryCellImages[i] == null)
            {
                continue;
            }

            // 각 배터리 칸의 채움 정도를 UI에 반영한다.
            _batteryCellImages[i].fillAmount = fillAmounts[i];
        }
    }
}