using DG.Tweening;
using MenusBehaviors;
using Praxilabs.UIs;
using UnityEngine;
using UnityEngine.Events;

public class HintsAnimationManager : MonoBehaviour, IFullScreen, IPinnable
{
    [SerializeField] private HintsDisplay _hintsDisplayObject;
    [SerializeField] private HintsManager _uiManager;
    [SerializeField] private RectTransform _iconTransform;
    [SerializeField] private HintsResizerHelper _hintsResizerHelper;
    [SerializeField] private IconRotator _pinIconRotatorHelper;

    private Vector2 _initialAnchorPosition;
    private Vector3 initialMainScreenPosition = new Vector3(0, -474, 0);
    private bool isInitialized;

    public bool IsFullScreen { get; set; }
    public bool IsPinned { get; set; }

    

    private void OnEnable()
    {
        if (isInitialized) return;
        _initialAnchorPosition = _hintsDisplayObject.MainRectTransform.anchoredPosition;
        //initialMainScreenPosition = new Vector3(0, -474, 0);
        _hintsDisplayObject.MainRectTransform.anchoredPosition = _iconTransform.anchoredPosition;
        _hintsDisplayObject.MainRectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        isInitialized = true;
    }

    public void ShowHintBoxRect()
    {
        _hintsDisplayObject.MainRectTransform.anchoredPosition = _initialAnchorPosition;
        _hintsDisplayObject.MainRectTransform.localScale = Vector3.one;
        ToggleOxiButtonClose();
    }

    public void HideHintBoxRect()
    {
        if (IsFullScreen)
        {
            ToggleFullScreenOff();
            _hintsResizerHelper.ChangeFullScreenState();
            _hintsResizerHelper.AdjustSize();
            return;
        }

        if (IsPinned) return;

        CloseHintBox();
    }

    public void HideHintBoxRectWhilePinned()
    {
        if (IsFullScreen)
        {
            ToggleFullScreenOff();
            _hintsResizerHelper.ChangeFullScreenState();
            _hintsResizerHelper.AdjustSize();
            return;
        }
        CloseHintBox();
    }

    public void ForceHideHintBoxRect()
    {
        if (IsPinned) return;
        CloseHintBox();
    }

    public void ToggleFullScreenOn()
    {
        if (!IsFullScreen)
        {
            IsFullScreen = true;
            UpdateUIOnFullScreen(930, 4.25f, ToggleFullScreenOff);
            SetProgressBarHeight();
            _uiManager.UpdateMenuState(MenuState.FullScreen);
        }
    }

    public void ToggleFullScreenOff()
    {
        if (IsFullScreen)
        {
            IsFullScreen = false;



#if UNITY_EDITOR

            UpdateUIOnFullScreen(330, 1/4.25f, ActivateFullScreen);

#else

            UpdateUIOnFullScreen(330, 4.25f, ActivateFullScreen);

#endif

            _hintsDisplayObject.FullScreenButton.interactable = true;
            ResetProgressBarHeight();
            _uiManager.UpdateMenuState(MenuState.Default);
        }
    }

    public void SetProgressBarHeight()
    {
        _hintsResizerHelper.AdjustProgressBarHeight();
    }

    public void ResetProgressBarHeight()
    {
        if (IsFullScreen) return;
        _hintsResizerHelper.ResetProgressBarHeight();
    }

    private void CloseHintBox()
    {
        _hintsDisplayObject.MainRectTransform.anchoredPosition = _iconTransform.anchoredPosition;
        CloseHintBoxScaleDown();

        ToggleOxiButtonOpen();
    }

    private async void CloseHintBoxScaleDown()
    {
        #if UNITY_EDITOR

         Debug.Log("Unity Editor Hints");

        #else

          await _hintsDisplayObject.MainCanvasWebViewPrefab.WaitUntilInitialized();

        #endif

        _hintsDisplayObject.MainRectTransform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
    }

    private void UpdateUIOnFullScreen(float mainContainerHeight, float scrollViewHeight, UnityAction toggleAction)
    {

        _hintsDisplayObject.MainRectTransform.DOSizeDelta(
            new Vector2(_hintsDisplayObject.MainRectTransform.sizeDelta.x,mainContainerHeight), 0.0001f);
        _hintsDisplayObject.ScrollViewObject.DOSizeDelta(
            new Vector2(_hintsDisplayObject.ScrollViewObject.sizeDelta.x, _hintsDisplayObject.ScrollViewObject.sizeDelta.y * scrollViewHeight), 0.0001f);
        _hintsDisplayObject.MainRectTransform.localPosition = initialMainScreenPosition;
        _hintsDisplayObject.FullScreenButton.onClick.RemoveAllListeners();
        _hintsDisplayObject.FullScreenButton.onClick.AddListener(toggleAction);
    }

    public void ActivateFullScreen()
    {
        ToggleFullScreenOn();
    }

    public void Pin()
    {
        IsPinned = true;
        _pinIconRotatorHelper.RotateIcon();
    }

    public void Unpin()
    {
        IsPinned = false;
        _pinIconRotatorHelper.RotateIcon();
    }

    public void ToggleOxiButtonOpen()
    {
        _hintsDisplayObject.OxiButtonOpen.gameObject.SetActive(true);
        _hintsDisplayObject.OxiButtonClose.gameObject.SetActive(false);
    }
    public void ToggleOxiButtonClose()
    {
        _hintsDisplayObject.OxiButtonOpen.gameObject.SetActive(false);
        _hintsDisplayObject.OxiButtonClose.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        _hintsDisplayObject.MainRectTransform.anchoredPosition = _initialAnchorPosition;
        ToggleOxiButtonOpen();
    }

}
