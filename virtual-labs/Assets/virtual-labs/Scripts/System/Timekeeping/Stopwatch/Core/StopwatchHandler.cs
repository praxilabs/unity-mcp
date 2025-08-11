using System;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.Timekeeping.Stopwatch
{
    public class StopwatchHandler : MonoBehaviour 
    {
        [SerializeField] private Stopwatch _stopwatch;
        [SerializeField] private StopwatchUI _stopwatchUI;

        public void Setup(float initialSpeedFactor, List<float> speedFactors, bool showReset)
        {
            _stopwatch.Setup(initialSpeedFactor, speedFactors, showReset);
            _stopwatchUI.Setup(initialSpeedFactor, speedFactors);
        }

        public void Setup(float initialSpeedFactor, List<float> speedFactors, bool showReset,  bool isInteractable)
        {
            Setup(initialSpeedFactor, speedFactors, showReset);
            _stopwatch.ResetInteractability(isInteractable);
        }

        private void Start() 
        {
            _stopwatch.OnTimeUpdate += HandleTimeUpdate;
            _stopwatch.OnPlayPauseStateChanged += HandlePlayPauseStateChanged;
            _stopwatch.OnResetInteractabilityChanged += HandleResetInteractabilityChanged;
            _stopwatch.OnResetVisibilityChanged += HandleResetVisibilityChanged;
        }

        public void RequestSpeedChange(Action<int> onSpeedFactorChanged)
        {
            _stopwatch.UpdateSpeedFactor(onSpeedFactorChanged);
        }

        public void RequestResetStopwatch()
        {
            _stopwatch.Reset();
        }

        public void RequestPlayPause()
        {
            if(_stopwatch.IsRunning)
            {
                _stopwatch.Pause();
            }
            else
            {
                _stopwatch.Play();
            }
        }

        public void ResetInteractability(bool isInteractable)
        {
            _stopwatch.ResetInteractability(isInteractable);
        }

        private void HandleTimeUpdate(TimeSpan currentTime)
        {
            _stopwatchUI.UpdateTimeDisplay(currentTime);
        }

        private void HandlePlayPauseStateChanged(bool isRunning)
        {
            _stopwatchUI.UpdatePlayPauseButton(isRunning);
        }

        private void HandleResetInteractabilityChanged(bool isInteractable)
        {
            _stopwatchUI.ResetInteractability(isInteractable);
        }

        private void HandleResetVisibilityChanged(bool isVisible)
        {
            _stopwatchUI.ResetVisibility(isVisible);
        }

        private void OnDestroy()
        {
            _stopwatch.OnTimeUpdate -= HandleTimeUpdate;
            _stopwatch.OnPlayPauseStateChanged -= HandlePlayPauseStateChanged;
            _stopwatch.OnResetInteractabilityChanged -= HandleResetInteractabilityChanged;
            _stopwatch.OnResetVisibilityChanged -= HandleResetVisibilityChanged;
        }
    }
}