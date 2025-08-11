using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.Timekeeping.Timer
{
    public class TimerUI : MonoBehaviour 
    {
        [Header("Slider")]
        [SerializeField] private Slider _slider;
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _deviceName;
        [SerializeField] private TextMeshProUGUI _timerText;
        [Header("Input Fields")]
        [SerializeField] private TMP_InputField _minutesInputField;
        [SerializeField] private TMP_InputField _secondsInputField;
        [Header("Buttons")]
        [SerializeField] private Button _speedFactorToggleBtn;
        [SerializeField] private Button _playPauseBtn;
        [SerializeField] private Button _skipBtn;
        [Header("Sprites")]
        [SerializeField] private Sprite _playSprite;
        [SerializeField] private Sprite _pauseSprite;
        [Header("Colors")]
        [SerializeField] private Color _enabledBtnColor;
        [SerializeField] private Color _disabledBtnColor;
        [Header("Tooltip")]
        [SerializeField] private GameObject _disabledBtnTooltip;
        [Header("Window")]
        [SerializeField] private RectTransform _window;

        private float _currentSpeedFactor;
        private Image _playPauseBtnFrame;
        private Image _playPauseBtnIcon;
        private Image _speedFactorToggleBtnIcon;
        private TextMeshProUGUI _speedFactorText;
        private TimeSpan _initialTime;
        private TimerHandler _timerHandler;
        private SpeedFactorsUI _speedFactorsUI;
        private bool _isSkipBtnRequired;

        public void UpdateUILocalization(string deviceName)
        {
            this._deviceName.text = deviceName;
        }

        public void Setup(float initialSpeedFactor, List<float> speedFactors, TimeSpan initialTime)
        {
            Reset();

            _currentSpeedFactor = initialSpeedFactor;
            _initialTime = initialTime;

            UpdateSlider(initialTime);
            UpdateInputFieldsText();
            
            SetupSpeedFactorsUI(speedFactors);
        }

        public void ShowTimer()
        {
            _window.localScale = Vector3.zero;

            gameObject.SetActive(true);

            _window.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack);
        }

        private void Awake()
        {
            _playPauseBtnFrame = _playPauseBtn.GetComponentInChildren<Image>();
            _playPauseBtnIcon = _playPauseBtn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != _playPauseBtn.GetComponent<Image>());
            _speedFactorToggleBtnIcon = _speedFactorToggleBtn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != _speedFactorToggleBtn.GetComponent<Image>());
            _speedFactorText = _speedFactorToggleBtn.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        private void Start()
        {
            _timerHandler = FindObjectOfType<TimerHandler>();
            InitializeSpeedFactorsUI();
            InitializeBtnsInputFields();
        }

        private void InitializeSpeedFactorsUI()
        {
            if(_speedFactorsUI == null)
            {
                _speedFactorsUI = GetComponentInChildren<SpeedFactorsUI>();
                _speedFactorsUI.OnSpeedFactorsMenuClosed += HandleSpeedFactorsMenuClosed;
            }
        }

        private void InitializeBtnsInputFields()
        {
            _speedFactorToggleBtn.onClick.AddListener(() => {
                ToggleFooter(false);
                _speedFactorsUI.SetSpeedFactorsMenuVisibility(true);
            });
            _playPauseBtn.onClick.AddListener(() => {
                _timerHandler.RequestPlayPause();
                UpdateInputFieldsEditability(false);
                UpdateInputFieldsVisibility(false);
            });
            _skipBtn.onClick.AddListener(() => {
                _timerHandler.RequestSkip();
            });
            _minutesInputField.onEndEdit.AddListener((value) => {
                if(GetInputFieldEditability(_minutesInputField))
                {
                    UpdateTimeFromInput(_timerHandler);
                    UpdateInputFieldsText();
                }
            });
            _secondsInputField.onEndEdit.AddListener((value) =>{
                if(GetInputFieldEditability(_secondsInputField))
                {
                    UpdateTimeFromInput(_timerHandler);
                    UpdateInputFieldsText();
                }
            });
        }

        private void SetupSpeedFactorsUI(List<float> speedFactors)
        {
            InitializeSpeedFactorsUI();
            _speedFactorsUI.Setup(speedFactors);
        }

        private void HandleSpeedFactorsMenuClosed()
        {
            ToggleFooter(true);
        }

        public void HandleTimerEnded()
        {
            gameObject.SetActive(false);
        }

        public void UpdateTimeDisplay(TimeSpan currentTime)
        {
            _timerText.text = currentTime.ToString("mm\\:ss");
            UpdateSlider(currentTime);
        }

        public void UpdatePlayPauseButtonSprite(bool isRunning)
        {
            _playPauseBtnIcon.sprite = isRunning ? _pauseSprite : _playSprite;
        }

        public void UpdatePlayButtonInteractability(bool isInteractable)
        {
            _playPauseBtn.interactable = isInteractable;
            UpdatePlayPauseButtonColor(isInteractable);
        }

        private void UpdatePlayPauseButtonColor(bool isInteractable)
        {
            _playPauseBtnFrame.color = isInteractable ? _enabledBtnColor : _disabledBtnColor;
        }

        public void UpdateInputFieldsVisibility(bool isVisible)
        {
            _minutesInputField.image.enabled = isVisible;
            _secondsInputField.image.enabled = isVisible;
        }

        public void UpdateSkippableButtonVisibility(bool isSkippable)
        {
            _skipBtn.gameObject.SetActive(isSkippable);
            if(isSkippable) _isSkipBtnRequired = true;
        }

        public void ToggleTooltipVisibility()
        {
            if(_playPauseBtn.IsInteractable()) return;
            SetDisabledBtnTooltipVisibility(!_disabledBtnTooltip.activeInHierarchy);
        }

        private bool GetInputFieldEditability(TMP_InputField inputField)
        {
            return !inputField.readOnly;
        }

        private void UpdateInputFieldsEditability(bool isEditable)
        {
            _minutesInputField.enabled = isEditable;
            _secondsInputField.enabled = isEditable;
        }

        private void UpdateTimeFromInput(TimerHandler timerHandler)
        {
            int minutes = Mathf.Abs(int.Parse(_minutesInputField.text));
            int seconds = Mathf.Abs(int.Parse(_secondsInputField.text));
            TimeSpan _inputTime = new TimeSpan(0, minutes, seconds);
            _initialTime = _inputTime;
            timerHandler.SetTimerTime(_inputTime);
        }

        private void UpdateSlider(TimeSpan currentTime)
        {
            if (_initialTime.TotalSeconds > 0)
            {
                _slider.value = 1 - (float)currentTime.TotalSeconds / (float)_initialTime.TotalSeconds;
            }
            else
            {
                _slider.value = 1;
            }
        }

        public void UpdateSpeedFactor(float newSpeedFactor)
        {
            _currentSpeedFactor = newSpeedFactor;
            SetSpeedFactorDisplay();
        }

        private void UpdateInputFieldsText()
        {
            _minutesInputField.text = Mathf.Abs(_initialTime.Minutes).ToString("00");
            _secondsInputField.text = Mathf.Abs(_initialTime.Seconds).ToString("00");
        }

        private void SetDisabledBtnTooltipVisibility(bool isVisible)
        {
            _disabledBtnTooltip.SetActive(isVisible);
        }

        private void SetSpeedFactorDisplay()
        {
            _speedFactorText.text = $"{_currentSpeedFactor}x";
            SwitchSpeedFactorIconToText(true);
        }

        private void SwitchSpeedFactorIconToText(bool isTrue)
        {
            _speedFactorText.gameObject.SetActive(isTrue);
            _speedFactorToggleBtnIcon.enabled = !isTrue;
        }

        private void ToggleFooter(bool isTrue)
        {
            _speedFactorToggleBtn.gameObject.SetActive(isTrue);
            _playPauseBtn.gameObject.SetActive(isTrue);
            if(_isSkipBtnRequired)
                UpdateSkippableButtonVisibility(isTrue);
        }

        private void Reset()
        {
            SetDisabledBtnTooltipVisibility(false);
            UpdateSkippableButtonVisibility(false);
            _isSkipBtnRequired = false;
            UpdateInputFieldsEditability(true);
            _minutesInputField.text = "00";
            _secondsInputField.text = "00";
            _timerText.text = "00:00";
            _playPauseBtnIcon.sprite = _playSprite;
            SwitchSpeedFactorIconToText(false);
            ToggleFooter(true);
            if(_speedFactorsUI != null)
                _speedFactorsUI.SetSpeedFactorsMenuVisibility(false);
        }

        private void OnDestroy() 
        {
            _speedFactorToggleBtn.onClick.RemoveAllListeners();
            _playPauseBtn.onClick.RemoveAllListeners();
            _skipBtn.onClick.RemoveAllListeners();
            _minutesInputField.onEndEdit.RemoveAllListeners();
            _secondsInputField.onEndEdit.RemoveAllListeners();
            if(_speedFactorsUI != null)
                _speedFactorsUI.OnSpeedFactorsMenuClosed -= HandleSpeedFactorsMenuClosed;
        }
    }
}