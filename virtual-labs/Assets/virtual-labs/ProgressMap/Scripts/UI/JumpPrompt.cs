using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProgressMap.UI.ExpandedView
{

    public class JumpPrompt : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _rectTransform;
        [SerializeField] private RectTransform _promptPopup;
        [SerializeField] private UnityEngine.UI.Button _yesButton;
        [SerializeField] private UnityEngine.UI.Button _noButton;

        public UnityEngine.Events.UnityAction onYes;
        public UnityEngine.Events.UnityAction onNo;
        [SerializeField] private AnimationCurve _showCurve;
        [SerializeField] private AnimationCurve _hideCurve;
        [SerializeField] private float _alphaSpeed;
        //[SerializeField] private float _positionSpeed = 1000f;


        [SerializeField] private TMPro.TextMeshProUGUI _promptTitle;
        [SerializeField] private TMPro.TextMeshProUGUI _promptBody;
        private Vector2 _popupSizeDelta;
        private Vector2 _popupSizeZeroHeight;
        private float _showHideValue = 1;
        private void Awake()
        {

            _popupSizeDelta = _promptPopup.sizeDelta;
            _popupSizeZeroHeight = _popupSizeDelta;
            _popupSizeZeroHeight.y = 0;
            _promptPopup.sizeDelta = Vector2.zero;
            _rectTransform.alpha = 0;
            _showHideValue = 0;
        }

        private void ClearEvents()
        {
            onYes = null;
            onNo = null;
        }
        public void Confirm()
        {
            onYes?.Invoke();
            HidePrompt();
        }
        public void Cancel()
        {
            onNo?.Invoke();
            HidePrompt();
        }
        public void Prompt(char characterToReplace,int stageNumber, UnityEngine.Events.UnityAction onYes, UnityEngine.Events.UnityAction onNo)
        {
            this.onYes = onYes;
            this.onNo = onNo;
            _promptTitle.text.Replace(characterToReplace.ToString(), characterToReplace.ToString());
             _promptBody.text.Replace(characterToReplace.ToString(), characterToReplace.ToString());

            ShowPrompt();
        }
        [ContextMenu("Hide prompt")]
        public void HidePrompt()
        {
            StopAllCoroutines();
            StartCoroutine(nameof(HideAnimation));
        }
        [ContextMenu("Show prompt")]
        public void ShowPrompt()
        {
            _rectTransform.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(nameof(ShowAnimation));
        }

        private IEnumerator ShowAnimation()
        {
            while (_showHideValue!=1)
            {
                _showHideValue += _alphaSpeed;
                _showHideValue = Mathf.Min(_showHideValue, 1);
                _rectTransform.alpha = _showCurve.Evaluate(_showHideValue);
                _promptPopup.sizeDelta = Vector2.Lerp(_popupSizeZeroHeight, _popupSizeDelta, _showCurve.Evaluate(_showHideValue));
                _promptPopup.localPosition = Vector3.Lerp(Vector3.up * 1000f, Vector3.zero, _showCurve.Evaluate(_showHideValue));
                yield return null;
            }
        }

        private IEnumerator HideAnimation()
        {
            while (_showHideValue != 0)
            {
                _showHideValue -= _alphaSpeed;
                _showHideValue = Mathf.Max(_showHideValue, 0);
                _rectTransform.alpha = _hideCurve.Evaluate(_showHideValue);
                _promptPopup.sizeDelta = Vector2.Lerp(_popupSizeZeroHeight, _popupSizeDelta, _hideCurve.Evaluate(_showHideValue));
                _promptPopup.localPosition = Vector3.Lerp(Vector3.up * 1000f, Vector3.zero, _hideCurve.Evaluate(_showHideValue));
                yield return null;
            }
            _rectTransform.gameObject.SetActive(false);
        }
    }

}