using UnityEngine;
using UnityEngine.UI;

public class HintsResizerHelper : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup _mainContainer;
    [SerializeField] private LayoutElement _progressBar;

    private float _defaultProgressbarHeightRatio = 1.1f;
    private float _FullScreenProgressbarHeightRatio = 1f;
    private float _defSpacing;
    private int _defTopPadding;

    private bool isFullScreen;

    private void Awake()
    {
        _defSpacing = _mainContainer.spacing;
        _defTopPadding = _mainContainer.padding.top;
    }

    private void OnEnable()
    {
        UpdateSpacingAndPadding();
    }
    private void OnDisable()
    {

        _mainContainer.spacing = _defSpacing;
        _mainContainer.padding.top = _defTopPadding;

    }
    public void AdjustSize()
    {
        if (gameObject.activeSelf) UpdateSpacingAndPadding();
    }
    public void ChangeFullScreenState()
    {
        isFullScreen = !isFullScreen;
    }
    private void UpdateSpacingAndPadding()
    {


        if (isFullScreen)
        {
            _mainContainer.spacing = -75f;
            _mainContainer.padding.top = 15;
        }
        else
        {
            _mainContainer.spacing = _defSpacing;
            _mainContainer.padding.top = _defTopPadding;
        }
    }
    public void AdjustProgressBarHeight()
    {
        _progressBar.flexibleHeight = _FullScreenProgressbarHeightRatio;
    }
    public void ResetProgressBarHeight()
    {
        _progressBar.flexibleHeight = _defaultProgressbarHeightRatio;
    }

}
