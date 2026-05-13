using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class RescueSignalLayerView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RescueSignalState _signalState;
    [SerializeField] private MapGridView _mapGridView;
    [SerializeField] private RectTransform _signalLayer;

    [Header("Prefab")]
    [SerializeField] private Image _signalImagePrefab;

    [Header("Visual")]
    [SerializeField] private float _signalSize = 18.0f;
    [SerializeField] private Color _signalColor = new Color(0.15f, 1.0f, 0.45f, 1.0f);

    [Header("Blink")]
    [SerializeField] private bool _useBlink = true;
    [SerializeField] private float _blinkSpeed = 4.0f;
    [SerializeField] private float _minAlpha = 0.25f;
    [SerializeField] private float _maxAlpha = 1.0f;

    private readonly List<Image> _spawnedSignalImages = new List<Image>();

    private void OnEnable()
    {
        if (_signalState == null)
        {
            return;
        }

        // 구조 신호 변경 이벤트를 구독한다.
        _signalState.SignalChanged += Refresh;
    }

    private void OnDisable()
    {
        if (_signalState == null)
        {
            return;
        }

        // 비활성화 시 이벤트 구독을 해제한다.
        _signalState.SignalChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (!_useBlink)
        {
            return;
        }

        // 구조 신호 아이콘을 깜빡거리게 만든다.
        ApplyBlinkEffect();
    }

    public void Refresh()
    {
        ClearSpawnedSignals();

        if (_signalState == null || _mapGridView == null || _signalLayer == null || _signalImagePrefab == null)
        {
            return;
        }

        foreach (RescueSignalRecord signal in _signalState.ActiveSignals)
        {
            DrawSignal(signal);
        }
    }

    private void DrawSignal(RescueSignalRecord signal)
    {
        if (signal.SignalType == RescueSignalType.MutantWithoutSignal)
        {
            return;
        }

        Image signalImage = Instantiate(_signalImagePrefab, _signalLayer);
        RectTransform signalRectTransform = signalImage.GetComponent<RectTransform>();

        // Corpse와 Mutant는 같은 색과 같은 아이콘으로 표시한다.
        signalImage.color = _signalColor;
        signalImage.raycastTarget = false;

        // 해당 Cell 중앙에 구조 신호 아이콘을 표시한다.
        signalRectTransform.anchoredPosition =
            _mapGridView.ConvertCellCenterToAnchoredPosition(signal.CellPosition);

        signalRectTransform.sizeDelta = new Vector2(_signalSize, _signalSize);

        _spawnedSignalImages.Add(signalImage);
    }

    private void ApplyBlinkEffect()
    {
        float pulse = (Mathf.Sin(Time.time * _blinkSpeed) + 1.0f) * 0.5f;
        float alpha = Mathf.Lerp(_minAlpha, _maxAlpha, pulse);

        for (int i = _spawnedSignalImages.Count - 1; i >= 0; i--)
        {
            Image signalImage = _spawnedSignalImages[i];

            if (signalImage == null)
            {
                _spawnedSignalImages.RemoveAt(i);
                continue;
            }

            Color color = _signalColor;
            color.a = alpha;
            signalImage.color = color;
        }
    }

    private void ClearSpawnedSignals()
    {
        for (int i = _spawnedSignalImages.Count - 1; i >= 0; i--)
        {
            if (_spawnedSignalImages[i] != null)
            {
                Destroy(_spawnedSignalImages[i].gameObject);
            }
        }

        _spawnedSignalImages.Clear();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 신호 크기와 깜빡임 수치를 안전한 범위로 제한한다.
        _signalSize = Mathf.Max(1.0f, _signalSize);
        _blinkSpeed = Mathf.Max(0.0f, _blinkSpeed);
        _minAlpha = Mathf.Clamp01(_minAlpha);
        _maxAlpha = Mathf.Clamp01(_maxAlpha);

        if (_minAlpha > _maxAlpha)
        {
            _minAlpha = _maxAlpha;
        }
    }
#endif
}