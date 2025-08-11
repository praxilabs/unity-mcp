using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ProgressMap.UI
{
    public class ProgressMapTransitions : MonoBehaviour
    {
        #region fields
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private UnityEngine.UI.Image _bgImage;
        [SerializeField] private UnityEngine.UI.Image _bgImageBlur;
        [Header("Easing")]
        [SerializeField] private Ease _resizeEasing = Ease.OutCirc;
        [Header("States data")]
        [SerializeField] private StateData _stateDataCollapsed;
        [SerializeField] private StateData _stateDataHover;
        [SerializeField] private StateData _stateDataExpanded;
        [Header("Tween speeds")]
        [SerializeField] private float _expandCollapseSpeed = 600;
        [SerializeField] private float _fadeSpeed = 600;
        [Header("______")]
        [SerializeField] private UnityEngine.UI.VerticalLayoutGroup _fullExpandedVerticalGroup;
        private DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> _expansionTween;
        private DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _expansionPixelTween;
        private View _view = View.collapsed;
        private CanvasGroup _activeCanvasGroup = null;
        private bool _expandingToFull = false;
        #endregion

        #region properties
        private float CurrentPixelPerUnit
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
        [System.Serializable]
        public class StateData
        {
            [field: SerializeField] public CanvasGroup CanvasGroup { get; set; }
            [field: SerializeField] public float PixelPerUnit { get; set; }
            [SerializeField] public Vector2 rectSize;
        }
        public enum View
        {
            collapsed,
            Hover,
            Expanded
        }
        #endregion

        #region methods

        public void ExpandHover()
        {
            if (_expandingToFull || _activeCanvasGroup == _stateDataExpanded.CanvasGroup)
                return;
            _view = View.Hover;

            ResizeRect(
                _stateDataHover.rectSize,
                _expandCollapseSpeed,
                _stateDataHover.PixelPerUnit,
                ShowExpandedHover
                );
            

        }
        public void Collapse()
        {
            if (_expandingToFull)
                return;
            if (_activeCanvasGroup != null)
            {
                if (_activeCanvasGroup == _stateDataHover.CanvasGroup)
                    HideExpandedHover(Collapse);
                if (_activeCanvasGroup == _stateDataExpanded.CanvasGroup)
                    HideExpandedFull(Collapse);
                return;
            }
            _view = View.collapsed;
            ResizeRect(_stateDataCollapsed.rectSize, _expandCollapseSpeed, _stateDataCollapsed.PixelPerUnit, ShowMini);
        }
        public void ExpandFull()
        {
            if (_activeCanvasGroup == _stateDataHover.CanvasGroup)
            {


                _expansionTween.Kill();
                HideExpandedHover(ExpandFull);
                return;
            }
            _expandingToFull = true;
            _view = View.Expanded;
            HideMini(null);
            _stateDataExpanded.CanvasGroup.gameObject.SetActive(true);
            ResizeRect(
                _stateDataExpanded.rectSize, 
                _expandCollapseSpeed, 
                _stateDataExpanded.PixelPerUnit, 
                ShowExpandedFull
                );
        }

        public void ResizeRect(Vector2 targetSize,
            float resizeSpeed,
            float targetPixelPerUnit,
            TweenCallback onResizeComplete)
        {
            _expansionTween.Kill();
            _expansionPixelTween.Kill();

            _expansionTween = _rectTransform.DOSizeDelta(targetSize, resizeSpeed)
                .SetSpeedBased()
                .SetEase(_resizeEasing)
                .OnComplete(onResizeComplete);


            _expansionPixelTween = DOTween.To(() => CurrentPixelPerUnit,
                (x) => CurrentPixelPerUnit = x,
                targetPixelPerUnit,
                resizeSpeed)
                .SetSpeedBased()
                .SetEase(_resizeEasing);
        }


        public void ToggleBGImage(bool toggle)
        {
            _bgImage.gameObject.SetActive(toggle);
            _bgImageBlur.gameObject.SetActive(toggle);
        }

        public void ShowMini()
        {
            _stateDataCollapsed.CanvasGroup.gameObject.SetActive(true);
            _stateDataCollapsed.CanvasGroup.DOFade(1, _fadeSpeed).SetSpeedBased(); 
        }

        public void HideMini(System.Action action)
        {
            _stateDataCollapsed.CanvasGroup.DOFade(0, _fadeSpeed)
                .SetSpeedBased()
                .OnComplete(
                () =>
                {
                    _stateDataCollapsed.CanvasGroup.gameObject.SetActive(false);
                    action?.Invoke();
                }
                );
        }

        private void ShowExpandedHover()
        {
            if (_activeCanvasGroup == _stateDataExpanded.CanvasGroup)
            {
                HideExpandedHover();
                return;
            }
            _activeCanvasGroup = _stateDataHover.CanvasGroup;
            _stateDataHover.CanvasGroup.gameObject.SetActive(true);
            _stateDataHover.CanvasGroup.DOFade(1, _fadeSpeed).SetSpeedBased();
        }

        private void HideExpandedHover()
        {
            switch (_view)
            {
                case View.Hover:
                    HideExpandedHover(Collapse);
                    break;
                default:
                    HideExpandedHover(null);
                    break;
            }

        }

        private void HideExpandedHover(System.Action action)
        {
            if (_view == View.Hover)
                _view = View.collapsed;
            _stateDataHover.CanvasGroup.DOFade(0, _fadeSpeed)
                .SetSpeedBased()
                .OnComplete(
                () =>
                { 
                    _stateDataHover.CanvasGroup.gameObject.SetActive(false);
                    if(_activeCanvasGroup==_stateDataHover.CanvasGroup)
                        _activeCanvasGroup = null;
                    action?.Invoke(); 
                }
                );
        }

        private void ShowExpandedFull()
        {

            _activeCanvasGroup = _stateDataExpanded.CanvasGroup;
            _stateDataExpanded.CanvasGroup.DOFade(1, _fadeSpeed).SetSpeedBased()
                .OnComplete(() => _expandingToFull = false);
        }

        public void HideExpandedFull() => HideExpandedFull(Collapse);

        private void HideExpandedFull(System.Action action)
        {
            if (_view == View.Expanded)
                _view = View.collapsed;

            _stateDataExpanded.CanvasGroup.DOFade(0, _fadeSpeed).SetSpeedBased()
                .OnComplete(
                () => 
                { 
                    _stateDataExpanded.CanvasGroup.gameObject.SetActive(false); 
                    _activeCanvasGroup = null;
                    action?.Invoke();
                }
                );
            
        }
        private void Update()
        {
            UpdateExpandedSize();
        }
        public void UpdateExpandedSize()
        {
            /// prefered height plus top, bottum padding
            float newHeight = _fullExpandedVerticalGroup.preferredHeight + 18 + 15;
            if (newHeight == _stateDataExpanded.rectSize.y || newHeight == (18 + 15))
                return;
            _stateDataExpanded.rectSize.y = newHeight;
            if (_view == View.Expanded)
            {
                ExpandFull();
            }
        }

        public float GetFadeSpeed() => _fadeSpeed;
        public bool IsExpandedToFull() => _view == View.Expanded;
        #endregion
    }
}