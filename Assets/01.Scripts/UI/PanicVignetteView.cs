using UnityEngine;
using UnityEngine.UI;

public sealed class PanicVignetteView : MonoBehaviour
{
    [SerializeField] private PanicViewModel _viewModel;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _vignetteImage;

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
            // 비네트는 클릭을 막지 않는다.
            _canvasGroup.alpha = 1.0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        if (_vignetteImage == null)
        {
            return;
        }

        Color color = _vignetteImage.color;
        color.a = viewState.VignetteAlpha;

        // 패닉 비율에 따라 비네트 알파를 조정한다.
        _vignetteImage.color = color;
    }
}