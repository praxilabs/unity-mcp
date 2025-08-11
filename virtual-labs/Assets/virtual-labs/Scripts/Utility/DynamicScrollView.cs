using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class DynamicScrollView : MonoBehaviour
{
    [Header("Height Limits")]
    [SerializeField] private float _defaultHeight = 150f;
    [SerializeField] private float _maxHeight = 300f;
    [SerializeField] private float _heightOffset = 5f;

    private RectTransform _scrollRect;
    private RectTransform _content;

    private void Awake()
    {
        _scrollRect = GetComponent<RectTransform>();
        _content = _scrollRect.GetComponent<ScrollRect>().content;
    }

    public void ContentUpdated()
    {
        UpdateScrollView();
    }

    private void UpdateScrollView()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);

        float contentHeight = _content.rect.height;

        // Adjust the height of the scroll view based on content height
        if (contentHeight <= _defaultHeight)
        {
            // Set to default height if content fits
            _scrollRect.sizeDelta = new Vector2(_scrollRect.sizeDelta.x, _defaultHeight + _heightOffset);
        }
        else if (contentHeight > _maxHeight)
        {
            // Set to max height if content exceeds max limit
            _scrollRect.sizeDelta = new Vector2(_scrollRect.sizeDelta.x, _maxHeight + _heightOffset);
        }
        else
        {
            // Adjust the scroll view height to fit the content
            _scrollRect.sizeDelta = new Vector2(_scrollRect.sizeDelta.x, contentHeight + _heightOffset);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect);
    }
}
