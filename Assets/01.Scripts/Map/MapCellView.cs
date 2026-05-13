using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public sealed class MapCellView : MonoBehaviour
{
    [SerializeField] private Color _hiddenColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    [SerializeField] private Color _revealedColor = new Color(0.35f, 0.75f, 0.9f, 0.85f);

    private Image _image;
    private Vector2Int _cellPosition;

    public Vector2Int CellPosition => _cellPosition;

    private void Awake()
    {
        // 자기 Image 컴포넌트를 캐싱한다.
        _image = GetComponent<Image>();
    }

    public void Initialize(Vector2Int cellPosition)
    {
        // 이 UI Cell이 어떤 맵 좌표를 표시하는지 저장한다.
        _cellPosition = cellPosition;
        SetRevealed(false);
    }

    public void SetRevealed(bool isRevealed)
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }

        // 밝혀진 Cell만 보이게 한다.
        _image.color = isRevealed ? _revealedColor : _hiddenColor;
    }
}