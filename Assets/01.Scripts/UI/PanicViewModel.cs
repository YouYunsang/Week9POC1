using System;
using UnityEngine;

public sealed class PanicViewModel : MonoBehaviour
{
    [SerializeField] private PlayerPanic _playerPanic;

    [Header("Vignette")]
    [SerializeField] private float _maxVignetteAlpha = 0f;

    public event Action<PanicViewState> ViewStateChanged;

    private void OnEnable()
    {
        if (_playerPanic == null)
        {
            return;
        }

        // 패닉 변경 이벤트를 구독한다.
        _playerPanic.PanicChanged += HandlePanicChanged;
        _playerPanic.ControlLossStarted += HandleControlLossChanged;
        _playerPanic.ControlLossEnded += HandleControlLossChanged;

        PublishViewState();
    }

    private void OnDisable()
    {
        if (_playerPanic == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _playerPanic.PanicChanged -= HandlePanicChanged;
        _playerPanic.ControlLossStarted -= HandleControlLossChanged;
        _playerPanic.ControlLossEnded -= HandleControlLossChanged;
    }

    private void HandlePanicChanged(float currentPanic, float maxPanic)
    {
        PublishViewState();
    }

    private void HandleControlLossChanged()
    {
        PublishViewState();
    }

    private void PublishViewState()
    {
        if (_playerPanic == null)
        {
            return;
        }

        float panicRatio = _playerPanic.PanicRatio;

        PanicViewState viewState = new PanicViewState(
            panicRatio,
            panicRatio,
            panicRatio * _maxVignetteAlpha,
            _playerPanic.IsControlLost);

        ViewStateChanged?.Invoke(viewState);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _maxVignetteAlpha = Mathf.Clamp01(_maxVignetteAlpha);
    }
#endif
}