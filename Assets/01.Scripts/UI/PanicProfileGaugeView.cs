using UnityEngine;
using UnityEngine.UI;

public sealed class PanicProfileGaugeView : MonoBehaviour
{
    [SerializeField] private PanicViewModel _viewModel;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _panicFillImage;

    private void OnEnable()
    {
        if (_viewModel == null)
        {
            return;
        }

        // ViewModel의 표시 상태 변경 이벤트를 구독한다.
        _viewModel.ViewStateChanged += HandleViewStateChanged;
    }

    private void OnDisable()
    {
        if (_viewModel == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _viewModel.ViewStateChanged -= HandleViewStateChanged;
    }

    private void HandleViewStateChanged(PanicViewState viewState)
    {
        if (_canvasGroup != null)
        {
            // 프로필 UI는 HUD이므로 항상 표시한다.
            _canvasGroup.alpha = 1.0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        if (_panicFillImage != null)
        {
            // 패닉 게이지가 아래에서 위로 차오르게 한다.
            _panicFillImage.fillAmount = viewState.ProfileFillAmount;
        }
    }
}