using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.Timekeeping.Stopwatch
{
    public class StopwatchUI : MonoBehaviour 
    {
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _stopwatchText;
        [Header("Buttons")]
        [SerializeField] private Button _speedFactorBtn;
        [SerializeField] private Button _resetBtn;
        [SerializeField] private Button _playPauseBtn;
        [Header("Sprites")]
        [SerializeField] private Sprite _playSprite;
        [SerializeField] private Sprite _pauseSprite;

        private StopwatchHandler _stopwatchHandler;
        private float _currentSpeedFactor;
        private List<float> _speedFactors; // Will also be used later when we implement the pop up menu to choose from the given speed factors
        private Image _playPauseBtnIcon;
        private Image _resetBtnIcon;
        private TextMeshProUGUI _speedFactorText;

        public void Setup(float initialSpeedFactor, List<float> speedFactors)
        {
            _currentSpeedFactor = initialSpeedFactor;
            _speedFactors = speedFactors;
            
            SetSpeedFactorDisplay();
        }

        private void Awake() 
        {
            _playPauseBtnIcon = _playPauseBtn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != _playPauseBtn.GetComponent<Image>());
            _resetBtnIcon = _resetBtn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != _resetBtn.GetComponent<Image>());
            _speedFactorText = _speedFactorBtn.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            _stopwatchHandler = FindObjectOfType<StopwatchHandler>();

            InitializeButtons();
        }

        private void InitializeButtons()
        {
            _speedFactorBtn.onClick.AddListener(() => {
                _stopwatchHandler.RequestSpeedChange(UpdateSpeedFactorDisplay);
            });
            _resetBtn.onClick.AddListener(() => {
                _stopwatchHandler.RequestResetStopwatch();
            });
            _playPauseBtn.onClick.AddListener(() => {
                _stopwatchHandler.RequestPlayPause();
            });
        }

        public void UpdateTimeDisplay(TimeSpan currentTime)
        {
            _stopwatchText.text = currentTime.ToString("hh\\:mm\\:ss");
        }

        public void UpdatePlayPauseButton(bool isRunning)
        {
            _playPauseBtnIcon.sprite = isRunning ? _pauseSprite : _playSprite;
        }

        public void ResetInteractability(bool isInteractable)
        {
            _resetBtn.interactable = isInteractable;
        }

        public void ResetVisibility(bool isVisible)
        {
            if(!isVisible)
            {
                _resetBtn.enabled = false;
                _resetBtn.image.enabled = false;
                _resetBtnIcon.enabled = false;
            }
            else
            {
                _resetBtn.enabled = true;
                _resetBtn.image.enabled = true;
                _resetBtnIcon.enabled = true;
            }
        }

        private void UpdateSpeedFactorDisplay(int newSpeedFactorIndex)
        {
            _currentSpeedFactor = _speedFactors[newSpeedFactorIndex];
            SetSpeedFactorDisplay();
        }

        private void SetSpeedFactorDisplay()
        {
            _speedFactorText.text = $"x{_currentSpeedFactor}";
        }
    }
}