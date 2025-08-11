using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.UI;

public class ExploreHintPopupWindow : MonoBehaviour
{
    private const float WINDOW_Y_OFFSET = 30f;
    private const float OPEN_WINDOW_ANIMATION_DURATION = 0.3f;
    private const float SCREEN_EDGE_PADDING = 10f;

    [SerializeField] private GameObject _window;
    [SerializeField] private GameObject _arrowGO;
    [SerializeField] private TextMeshProUGUI _hintText;
    [SerializeField] private TextMeshProUGUI _exploredOfTotalHintsText;
    [SerializeField] private CleanButton _confirmBtn;
    private ExplorationHintsContainer _explorationHintsContainer;
    private RectTransform _windowRectTransform;
    private RectTransform _arrowRectTransform;
    private float _initialWindowPosY;
    private float _initialArrowPosY;
    private bool _isFirstTime = true;
    private string _hintTextString;
    private UnityEngine.Events.UnityAction _confirmBtnAction;

    public void Setup(ExplorationHintsContainer explorationHintsContainer, Action ResetFlashingAction, string hintText)
    {
        _explorationHintsContainer = explorationHintsContainer;
        _hintTextString = hintText;

        _confirmBtnAction = () =>
        {
            ResetFlashingAction();
            TogglePopupWindow(false);
            explorationHintsContainer.ToggleHintBtns(true);
            explorationHintsContainer.CheckIfCompletedGroupHints();
        };
        _confirmBtn.onClick.AddListener(_confirmBtnAction);

        _windowRectTransform = _window.GetComponent<RectTransform>();
        _arrowRectTransform = _arrowGO.GetComponent<RectTransform>();
        _initialWindowPosY = _windowRectTransform.anchoredPosition.y;
        _initialArrowPosY = _arrowRectTransform.anchoredPosition.y;
    }

    public void SetHintText(string hintText)
    {
        _hintTextString = hintText;
        _hintText.text = $"{_explorationHintsContainer.GetExploredHintsCount()}. {_hintTextString}";
    }

    public void TogglePopupWindow(bool enable)
    {
        if(_isFirstTime)
            InitializeHint();
        
        TogglePopupWindowAnimation(enable);
    }

    private void InitializeHint()
    {
        _isFirstTime = false;
        _hintText.text = $"{_explorationHintsContainer.GetExploredHintsCount()}. {_hintTextString}";
        SetHintsCountText();
    }

    public void SetHintsCountText()
    {
        _exploredOfTotalHintsText.text = _explorationHintsContainer.ProvideExploredOfTotalHints().ToString();
    }

    private void TogglePopupWindowAnimation(bool enable)
    {
        if(enable)
            OpenWindowAnimation();
        else
        {
            CloseWindowInstantly();
        }
    }

    private void OpenWindowAnimation()
    {
        float startWindowPosY = _initialWindowPosY + WINDOW_Y_OFFSET;
        float startArrowPosY = _initialArrowPosY + WINDOW_Y_OFFSET;

        _windowRectTransform.anchoredPosition = new Vector2(_windowRectTransform.anchoredPosition.x, startWindowPosY);
        _arrowRectTransform.anchoredPosition = new Vector2(_arrowRectTransform.anchoredPosition.x, startArrowPosY);

        _window.SetActive(true);
        _arrowGO.SetActive(true);

        MoveToScreenBounds();

        _windowRectTransform.DOAnchorPosY(_initialWindowPosY, OPEN_WINDOW_ANIMATION_DURATION)
            .SetEase(Ease.InOutQuad);
        _arrowRectTransform.DOAnchorPosY(_initialArrowPosY, OPEN_WINDOW_ANIMATION_DURATION)
            .SetEase(Ease.InOutQuad);
    }

    private void CloseWindowInstantly()
    {
        _window.SetActive(false);
        _arrowGO.SetActive(false);
    }

    private void MoveToScreenBounds()
    {
        Vector3 windowPosition = _windowRectTransform.position;
        Vector2 windowSize = _windowRectTransform.rect.size;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float leftEdge = windowPosition.x - windowSize.x / 2;
        float rightEdge = windowPosition.x + windowSize.x / 2;
        float topEdge = windowPosition.y + windowSize.y / 2;
        float bottomEdge = windowPosition.y - windowSize.y / 2;

        Vector3 newPosition = windowPosition;

        if (leftEdge < 0)
        {
            newPosition.x = windowPosition.x - leftEdge + SCREEN_EDGE_PADDING; // Move the window to the right
        }
        else if (rightEdge > screenWidth)
        {
            newPosition.x = windowPosition.x - (rightEdge - screenWidth) - SCREEN_EDGE_PADDING; // Move the window to the left
        }

        if (topEdge > screenHeight)
        {
            newPosition.y = windowPosition.y - (topEdge - screenHeight) - SCREEN_EDGE_PADDING; // Move the window down
        }
        else if (bottomEdge < 0)
        {
            newPosition.y = windowPosition.y + Mathf.Abs(bottomEdge) + SCREEN_EDGE_PADDING; // Move the window up
        }

        _windowRectTransform.position = newPosition;
    }
}
