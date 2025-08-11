using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.Timekeeping.Timer
{
    public class Timer : TimeController
    {
        private TimeSpan _initialTime;
        private TimeSpan _remainingTime;

        public event Action OnTimerEnded;
        public event Action<bool> OnPlayButtonInteractabilityChanged;

        public void Setup(float initialSpeedFactor, List<float> speedFactors, TimeSpan initialTime)
        {
            base.Setup(initialSpeedFactor, speedFactors);
            _initialTime = initialTime;
            _remainingTime = initialTime;
            CheckPlayButtonInteractability(); // Retains the play button check
        }

        private void Update()
        {
            UpdateTime(); // Call the shared time update logic
        }

        protected override void UpdateTimeInternal()
        {
            if (_remainingTime.TotalSeconds > 0)
            {
                _remainingTime -= TimeSpan.FromSeconds(Time.deltaTime * _currentSpeedFactor);
                if (_remainingTime.TotalSeconds <= 0)
                {
                    _remainingTime = TimeSpan.Zero;
                    Stop();
                    OnTimerEnded?.Invoke();
                }
            }
        }

        protected override TimeSpan GetCurrentTime()
        {
            return _remainingTime;
        }

        public double GetRemainingTimeInSeconds()
        {
            return _remainingTime.TotalSeconds;
        }

        public float GetInitialTimeInSeconds()
        {
            return (float)_initialTime.TotalSeconds;
        }

        public void Reset()
        {
            Stop();
            _remainingTime = _initialTime;
            InvokeTimeUpdate(_remainingTime);
        }

        public void Skip()
        {
            _remainingTime = new TimeSpan(0, 0, 1);
            InvokeTimeUpdate(_remainingTime);
        }

        public void SetTimerTime(TimeSpan newTime)
        {
            _initialTime = newTime;
            _remainingTime = newTime;
            InvokeTimeUpdate(_remainingTime);
            CheckPlayButtonInteractability();
        }

        private void CheckPlayButtonInteractability()
        {
            bool canPlay = _remainingTime.TotalSeconds > 0;
            OnPlayButtonInteractabilityChanged?.Invoke(canPlay);
        }
    }
}
