using System;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.Timekeeping.Stopwatch
{
    public class Stopwatch : TimeController
    {
        private TimeSpan _currentTime;

        public event Action<bool> OnResetInteractabilityChanged;
        public event Action<bool> OnResetVisibilityChanged;

        public void Setup(float initialSpeedFactor, List<float> speedFactors, bool showReset)
        {
            base.Setup(initialSpeedFactor, speedFactors);
            _currentTime = TimeSpan.Zero;
            ResetVisibility(showReset); // Retains the original ResetVisibility call
        }

        private void Update()
        {
            UpdateTime(); // Call the shared time update logic
        }

        protected override void UpdateTimeInternal()
        {
            _currentTime += TimeSpan.FromSeconds(Time.deltaTime * _currentSpeedFactor);
        }

        protected override TimeSpan GetCurrentTime()
        {
            return _currentTime;
        }

        public double GetCurrentTimeInSeconds()
        {
            return _currentTime.TotalSeconds;
        }

        public void Reset()
        {
            Stop();
            _currentTime = TimeSpan.Zero;
            InvokeTimeUpdate(_currentTime);
        }

        public void ResetVisibility(bool isVisible)
        {
            OnResetVisibilityChanged?.Invoke(isVisible);
        }

        public void ResetInteractability(bool isInteractable)
        {
            OnResetInteractabilityChanged?.Invoke(isInteractable);
        }
    }
}
