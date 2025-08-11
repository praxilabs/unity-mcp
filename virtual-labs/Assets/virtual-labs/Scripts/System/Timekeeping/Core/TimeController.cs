using System;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.Timekeeping
{
    public abstract class TimeController : MonoBehaviour
    {
        public event Action<TimeSpan> OnTimeUpdate;
        public event Action<bool> OnPlayPauseStateChanged;
        public event Action<float> OnSpeedFactorChanged;

        public bool IsRunning => _isRunning;
        protected bool _isRunning;
        protected float _currentSpeedFactor;
        protected List<float> _speedFactors;
        protected int _currentSpeedFactorIndex;

        public void Setup(float initialSpeedFactor, List<float> speedFactors)
        {
            _isRunning = false;
            _currentSpeedFactor = initialSpeedFactor;
            _speedFactors = speedFactors;
            _currentSpeedFactorIndex = GetSpeedFactorIndex(initialSpeedFactor);
        }

        protected void UpdateTime()
        {
            if (_isRunning)
            {
                // Derived classes will implement how to update time
                UpdateTimeInternal();
                OnTimeUpdate?.Invoke(GetCurrentTime());
            }
        }

        protected abstract void UpdateTimeInternal();
        protected abstract TimeSpan GetCurrentTime();

        public void Play()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                OnPlayPauseStateChanged?.Invoke(_isRunning);
            }
        }

        public void Pause()
        {
            if (_isRunning)
            {
                _isRunning = false;
                OnPlayPauseStateChanged?.Invoke(_isRunning);
            }
        }

        public void UpdateSpeedFactor(Action<int> onSpeedFactorChanged)
        {
            _currentSpeedFactorIndex++;
            if (_currentSpeedFactorIndex >= _speedFactors.Count)
                _currentSpeedFactorIndex = 0;

            _currentSpeedFactor = _speedFactors[_currentSpeedFactorIndex];
            onSpeedFactorChanged?.Invoke(_currentSpeedFactorIndex);
        }

        public void UpdateSpeedFactor(int newSpeedFactorIndex)
        {
            if(_currentSpeedFactorIndex == newSpeedFactorIndex)
                return;
            
            if (newSpeedFactorIndex >= _speedFactors.Count)
            {
                Debug.LogError($"The new speed factor index is out of bounds! its value: {newSpeedFactorIndex}, speed factors list count: {_speedFactors.Count}");
                return;
            }

            _currentSpeedFactorIndex = newSpeedFactorIndex;

            _currentSpeedFactor = _speedFactors[_currentSpeedFactorIndex];
            OnSpeedFactorChanged?.Invoke(_currentSpeedFactor);
        }

        protected void Stop()
        {
            _isRunning = false;
            OnPlayPauseStateChanged?.Invoke(false);
        }

        protected void InvokeTimeUpdate(TimeSpan time)
        {
            OnTimeUpdate?.Invoke(time);
        }

        protected int GetSpeedFactorIndex(float value)
        {
            if (_speedFactors == null) return 0; // returns the first index

            int index = _speedFactors.IndexOf(value);
            return index == -1 ? 0 : index; // returns the first index if not found
        }
    }
}
