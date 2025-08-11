using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Table.UI.Views
{
    public class TableTransitions : MonoBehaviour
    {
        #region fields
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private UnityEngine.UI.Image _bgImage;
        [SerializeField] private UnityEngine.UI.Image _bgImageBlur;
        [Header("Expand")]
        [SerializeField] private UnityEngine.UI.Image _expandBtnImage;
        [SerializeField] private Sprite _expandMaximizeIcon;
        [SerializeField] private Sprite _expandMinimizeIcon;
        [Header("Easing")]
        [SerializeField] private Ease _resizeEasing = Ease.OutCirc;
        [Header("Rects")]
        [SerializeField] private RectTransform _fullSizeRect;
        [SerializeField] private RectTransform _smallSizeRect;
        [SerializeField] private RectTransform _minimizedRect;
        [Header("PPU")]
        [SerializeField] private float _pixelPerUnitFullSize;
        [SerializeField] private float _pixelPerUnitSmallView;
        [SerializeField] private float _pixelPerUnitMinimizedView;
        [Header("Tween speeds")]
        [SerializeField] private float _expandCollapseSpeed = 600;
        [SerializeField] private float _expandCollapseAnchorSpeed = 600;
        [SerializeField] private float _fadeSpeed = 600;
        [Header("Canvas group")]
        [SerializeField] private CanvasGroup _tableCanvasGroup;
        [Header("______-_-_")]
        private DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> _sizeTween;
        private DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> _anchorMaxTween;
        private DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> _anchorMinTween;
        private DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> _anchorPosTween;
        private DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _expansionPixelTween;
        private View _view = View.Collapsed;
        private CanvasGroup _activeCanvasGroup = null;
        private bool _expandingToFull = false;


        public UnityEngine.Events.UnityEvent OnTableShown;
        public UnityEngine.Events.UnityEvent OnTableHidden;
        #endregion
        #region properties
        public float CurrentPixelPerUnit
        {
            get => _bgImage.pixelsPerUnitMultiplier;
            set
            {
                _bgImage.pixelsPerUnitMultiplier = value;
                _bgImageBlur.pixelsPerUnitMultiplier = value;
            }
        }
        #endregion

        #region Types Definitions
        public enum View
        {
            Collapsed,
            Small,
            Expanded
        }
        #endregion

        #region methods
        private void Start()
        {
            Initialize();
        }
        private void Initialize()
        {
            HideTableContent(null);
            _tableCanvasGroup.alpha = 0;

            _activeCanvasGroup = null;
            _tableCanvasGroup.gameObject.SetActive(false);

            _view = View.Collapsed;

            _rectTransform.sizeDelta = _minimizedRect.sizeDelta;
            OnTableHidden?.Invoke();
            gameObject.SetActive(false);

            CurrentPixelPerUnit = _pixelPerUnitMinimizedView;

            _rectTransform.anchorMin = _minimizedRect.anchorMin;
            _rectTransform.anchorMax = _minimizedRect.anchorMax;
            _rectTransform.anchoredPosition = _minimizedRect.anchoredPosition;
        }
        public void ExpandSmallView()
        {
            //print("expand hover");
            _view = View.Small;
            gameObject.SetActive(true);
            _activeCanvasGroup = _tableCanvasGroup;
            ResizeRect(
                _smallSizeRect.sizeDelta,
                _expandCollapseSpeed,
                _expandCollapseAnchorSpeed,
                _pixelPerUnitSmallView,
                _smallSizeRect.anchoredPosition,
                _smallSizeRect.anchorMin,
                _smallSizeRect.anchorMax,
                ShowTableContent
                );

            UpdateExpandIcon();
        }
        public void Collapse()
        {
            if (_expandingToFull)
                return;
            if (_view == View.Expanded)
            {
                ExpandSmallView();
                return;
            }
            if (_activeCanvasGroup != null)
            {
                if (_activeCanvasGroup == _tableCanvasGroup)
                    HideTableContent(Collapse);

                return;
            }

            //print("collapse");
            _view = View.Collapsed;
            ResizeRect(_minimizedRect.sizeDelta,
                _expandCollapseSpeed,
                _expandCollapseAnchorSpeed,
                _pixelPerUnitMinimizedView,
                _minimizedRect.anchoredPosition,
                _minimizedRect.anchorMin,
                _minimizedRect.anchorMax,
                ()=> { OnTableHidden?.Invoke(); gameObject.SetActive(false); });
        }
        public void ExpandFull()
        {
            //print("Expand full mini");
            _expandingToFull = true;
            _view = View.Expanded;
            _activeCanvasGroup = _tableCanvasGroup;
            gameObject.SetActive(true);
            _tableCanvasGroup.gameObject.SetActive(true);
            ResizeRect(
                _fullSizeRect.sizeDelta,
                _expandCollapseSpeed,
                _expandCollapseAnchorSpeed,
                _pixelPerUnitFullSize,
                _fullSizeRect.anchoredPosition,
                _fullSizeRect.anchorMin,
                _fullSizeRect.anchorMax,
                ShowTableContent
                );

            UpdateExpandIcon();
        }

        public void ToggleExpansion()
        {
            if(_view == View.Small)
                ExpandFull();
            else
                ExpandSmallView();
        }

        private void UpdateExpandIcon()
        {
            _expandBtnImage.sprite = _view == View.Small ? _expandMaximizeIcon : _expandMinimizeIcon;
        }

        private void ResizeRect(Vector2 targetSize,
            float resizeSpeed,
            float anchorSpeed,
            float targetPixelPerUnit,
            Vector2 anchorPos,
            Vector2 anchorMin,
            Vector2 anchorMax,
            TweenCallback onResizeComplete)
        {
            _sizeTween.Kill();
            _expansionPixelTween.Kill();

            _anchorMinTween.Kill();
            _anchorMaxTween.Kill();

            _anchorPosTween.Kill();

            _sizeTween = _rectTransform.DOSizeDelta(targetSize, resizeSpeed)
                .SetSpeedBased()
                .SetEase(_resizeEasing)
                .OnComplete(onResizeComplete);


            _expansionPixelTween = DOTween.To(() => CurrentPixelPerUnit,
                (x) => CurrentPixelPerUnit = x,
                targetPixelPerUnit,
                resizeSpeed)
                .SetSpeedBased()
                .SetEase(_resizeEasing)
                .OnComplete(onResizeComplete);

            _anchorMinTween = _rectTransform.DOAnchorMin(anchorMin, anchorSpeed).SetSpeedBased().SetEase(_resizeEasing);
            _anchorMaxTween = _rectTransform.DOAnchorMax(anchorMax, anchorSpeed).SetSpeedBased().SetEase(_resizeEasing);
            _anchorPosTween = _rectTransform.DOAnchorPos(anchorPos, resizeSpeed).SetSpeedBased().SetEase(_resizeEasing);


        }

        private void ShowTableContent()
        {
            _tableCanvasGroup.gameObject.SetActive(true);
            _tableCanvasGroup.DOFade(1, _fadeSpeed).SetSpeedBased().OnComplete(() => OnTableShown?.Invoke());
            _expandingToFull = false;
            
        }

        private void HideTableContent(System.Action action)
        {
            _tableCanvasGroup.DOFade(0, _fadeSpeed)
                .SetSpeedBased()
                .OnComplete(
                () =>
                {
                    _activeCanvasGroup = null;
                    _tableCanvasGroup.gameObject.SetActive(false);
                    action?.Invoke();
                }
                );
        }

        #endregion
    }
}