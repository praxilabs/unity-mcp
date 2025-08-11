using LocalizationSystem;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PraxiLabs.Tooltip
{
    public class UITooltipManager : Singleton<UITooltipManager>
    {
        [SerializeField] private RectTransform _tooltip;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _fadeTime = 0.1f;

        private CanvasGroup _canvasGroup;
        private TextMeshProUGUI _textTMPro;
        private Coroutine _currentCoroutine;
      
        private Dictionary<int, UILabelData> _uiLabels = new Dictionary<int, UILabelData>();
        private Dictionary<string, string> _jsonDictionary= new Dictionary<string, string>();
        private int _currentLabelID;

        private Action _OnLanguageChangeDelegate;

        private void OnEnable()
        {
            _OnLanguageChangeDelegate = () =>
            {
                if (_jsonDictionary.Count == 0 || _uiLabels.Count == 0) return;
                LoadLabelJson();
            };
            LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
        }

        private void Start()
        {
            _canvasGroup = _tooltip.GetComponent<CanvasGroup>();
            _textTMPro = _tooltip.GetComponentInChildren<TextMeshProUGUI>();

            StartCoroutine(LoadJson());
        }

        public void ShowTooltip(int textID, RectTransform uiTransform)
        {
            UpdateTooltipPosition(uiTransform);
            if (_canvasGroup != null)
            {
                _textTMPro.text = _uiLabels[textID].labelName;

                if (_currentCoroutine != null)
                    StopCoroutine(_currentCoroutine);
                _currentCoroutine = StartCoroutine(UltimateClean.Utils.FadeIn(_canvasGroup, 1.0f, _fadeTime));
            }
        }

        public void HideTooltip()
        {
            if (_canvasGroup != null)
            {
                if (_currentCoroutine != null)
                    StopCoroutine(_currentCoroutine);
                if (_canvasGroup.alpha == 0f)
                    return;
                _currentCoroutine = StartCoroutine(UltimateClean.Utils.FadeOut(_canvasGroup, 0.0f, _fadeTime));
            }
        }

        private void UpdateTooltipPosition(RectTransform uiTransform)
        {
            //Get top center world position of the UI element
            Vector3[] corners = new Vector3[4];
            uiTransform.GetWorldCorners(corners);
            Vector3 topCenterWorld = (corners[1] + corners[2]) * 0.5f; // Top edge center

            //Get the correct camera from tooltip canvas
            Canvas tooltipCanvas = _tooltip.GetComponentInParent<Canvas>();
            Camera eventCamera = null;

            if (tooltipCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                eventCamera = tooltipCanvas.worldCamera;

            //Get screen position
            Vector3 screenPoint = eventCamera != null
                ? eventCamera.WorldToScreenPoint(topCenterWorld)
                : RectTransformUtility.WorldToScreenPoint(null, topCenterWorld);

            //Convert to local UI position
            RectTransform parentRect = _tooltip.parent as RectTransform;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                screenPoint,
                eventCamera,
                out localPoint
            );

            _tooltip.anchoredPosition = localPoint + _offset;
        }

        #region Localization
        private IEnumerator LoadJson()
        {
            yield return LocalizationLoader.Instance.LoadDiskJson(_jsonDictionary, LocalizationDataTypes.UILabels);
            LoadLabelJson();
        }

        public void LoadLabelJson()
        {
            if (_jsonDictionary.Count == 0 || _jsonDictionary.ContainsKey(LocalizationManager.Instance.CurrentLocale) == false) return;

            string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
            _uiLabels = JsonConvert.DeserializeObject<Dictionary<int, UILabelData>>(currentJson);

            UpdateText();
        }

        public void UpdateText()
        {
            if (_currentLabelID == 0) return;
            _textTMPro.text = _uiLabels[_currentLabelID].labelName;
        }
        #endregion
    }

    [Serializable]
    public class UILabelData
    {
        public string labelName;
    }
}
