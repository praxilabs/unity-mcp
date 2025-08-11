using LocalizationSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.Timekeeping.Timer
{
    public class TimerHandler : MonoBehaviour
    {
        public Timer timer;
        [SerializeField] private TimerUI _timerUI;

        private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
        private TimerData _timerData;
        private Action _OnLanguageChangeDelegate;
        private bool _dataExists;

        private void OnEnable()
        {
            _OnLanguageChangeDelegate = () =>
            {
                if (!_dataExists) return;
                LoadTimerJson();
            };

            timer.OnTimerEnded += HandleTimerEnded;
            timer.OnTimeUpdate += HandleTimeUpdate;
            timer.OnSpeedFactorChanged += HandleSpeedFactorChanged;
            timer.OnPlayPauseStateChanged += HandlePlayPauseStateChanged;
            timer.OnPlayButtonInteractabilityChanged += HandlePlayButtonInteractabilityChanged;
            LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
        }

        private void OnDisable()
        {
            timer.OnTimerEnded -= HandleTimerEnded;
            timer.OnTimeUpdate -= HandleTimeUpdate;
            timer.OnSpeedFactorChanged -= HandleSpeedFactorChanged;
            timer.OnPlayPauseStateChanged -= HandlePlayPauseStateChanged;
            timer.OnPlayButtonInteractabilityChanged -= HandlePlayButtonInteractabilityChanged;
            LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
        }

        private void Start()
        {
            LoadJson();
        }

        public void Setup(float initialSpeedFactor, List<float> speedFactors, TimeSpan? initialTime = null)
        {
            if (_timerUI == null) return;
            _timerUI.ShowTimer();

            TimeSpan timeToSet = initialTime ?? TimeSpan.Zero;
            timer.Setup(initialSpeedFactor, speedFactors, timeToSet);
            _timerUI.Setup(initialSpeedFactor, speedFactors, timeToSet);

            if (initialTime != null)
            {
                _timerUI.UpdateInputFieldsVisibility(false);
                _timerUI.UpdatePlayButtonInteractability(false);
                // Might add here PlayInstantly() - when needed
            }
            else
            {
                _timerUI.UpdateInputFieldsVisibility(true);
            }
        }

        public void Setup(float initialSpeedFactor, List<float> speedFactors, TimeSpan initialTime, bool isSkippable)
        {
            if (_timerUI == null) return;

            Setup(initialSpeedFactor, speedFactors, initialTime);

            _timerUI.UpdateSkippableButtonVisibility(isSkippable);
        }

        public void PlayInstantly()
        {
            timer.Play();
        }

        public void RequestSpeedChange(int newSpeedFactorIndex)
        {
            timer.UpdateSpeedFactor(newSpeedFactorIndex);
        }

        public void RequestPlayPause()
        {
            if (timer.IsRunning)
            {
                timer.Pause();
            }
            else
            {
                timer.Play();
            }
        }

        public void RequestSkip()
        {
            timer.Skip();
        }

        public void SetTimerTime(TimeSpan newTime)
        {
            timer.SetTimerTime(newTime);
        }

        public void SetPlayButtonInteractability(bool isInteractable)
        {
            _timerUI.UpdatePlayButtonInteractability(isInteractable);
        }

        private void HandleTimeUpdate(TimeSpan currentTime)
        {
            _timerUI.UpdateTimeDisplay(currentTime);
        }

        private void HandleSpeedFactorChanged(float newSpeedFactor)
        {
            _timerUI.UpdateSpeedFactor(newSpeedFactor);
        }

        private void HandlePlayPauseStateChanged(bool isRunning)
        {
            _timerUI.UpdatePlayPauseButtonSprite(isRunning);
        }

        private void HandlePlayButtonInteractabilityChanged(bool canPlay)
        {
            _timerUI.UpdatePlayButtonInteractability(canPlay);
        }

        private void HandleTimerEnded()
        {
            _timerUI.HandleTimerEnded();
        }

        #region Localization
        private void LoadJson()
        {
            _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.Timer.ToString());

            if (!_dataExists) return;

            _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.Timer.ToString()];
            LoadTimerJson();
        }

        public void LoadTimerJson()
        {
            if (!_dataExists) return;

            string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
            _timerData = JsonConvert.DeserializeObject<TimerData>(currentJson);

            UpdateUI(_timerData.deviceName);
        }

        public void UpdateUI(string deviceName)
        {
            _timerUI.UpdateUILocalization(deviceName);
        }
        #endregion
    }

    public class TimerData
    {
        public string deviceName;
    }
}