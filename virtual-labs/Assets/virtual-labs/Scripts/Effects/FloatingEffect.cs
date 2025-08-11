using System;
using UnityEngine;

namespace Praxilabs.Effects
{
    public sealed class FloatingEffect : MonoBehaviour
    {
        private const float TwoPi = Mathf.PI * 2f;

        [SerializeField] private float _amplitude = 10f;
        [SerializeField] private float _frequency = 1f;
        [SerializeField] private float _phaseOffset = 0f;
        [SerializeField] private bool _useUnscaledTime = false;
        [SerializeField] private bool _useLocalSpaceFor3D = true;
        [SerializeField] private Vector3 _axis3D = Vector3.up;
        [SerializeField] private bool _uiUseAnchoredPosition = true;

        private RectTransform _rectTransform;
        private Transform _cachedTransform;
        private Vector2 _startAnchoredPosition;
        private Vector3 _startLocalPosition;
        private Vector3 _startWorldPosition;
        private Vector3 _axis3DNormalized;

        private void OnValidate()
        {
            if (_amplitude < 0f)
                throw new ArgumentOutOfRangeException(nameof(_amplitude), "Amplitude must be >= 0.");
            if (_frequency <= 0f)
                throw new ArgumentOutOfRangeException(nameof(_frequency), "Frequency must be > 0.");
        }

        private void Awake()
        {
            _cachedTransform = transform;
            _rectTransform = GetComponent<RectTransform>();
            _axis3DNormalized = _axis3D.sqrMagnitude > 0f ? _axis3D.normalized : Vector3.up;

            if (_rectTransform != null && _uiUseAnchoredPosition)
            {
                _startAnchoredPosition = _rectTransform.anchoredPosition;
            }
            else
            {
                _startLocalPosition = _cachedTransform.localPosition;
                _startWorldPosition = _cachedTransform.position;
            }
        }

        private void OnEnable()
        {
            if (_rectTransform != null && _uiUseAnchoredPosition)
            {
                _startAnchoredPosition = _rectTransform.anchoredPosition;
            }
            else
            {
                _startLocalPosition = _cachedTransform.localPosition;
                _startWorldPosition = _cachedTransform.position;
            }
        }

        private void OnDisable()
        {
            if (_rectTransform != null && _uiUseAnchoredPosition)
            {
                _rectTransform.anchoredPosition = _startAnchoredPosition;
            }
            else if (_useLocalSpaceFor3D)
            {
                _cachedTransform.localPosition = _startLocalPosition;
            }
            else
            {
                _cachedTransform.position = _startWorldPosition;
            }
        }

        private void Update()
        {
            float t = _useUnscaledTime ? Time.unscaledTime : Time.time;
            float offset = Mathf.Sin((t * _frequency * TwoPi) + _phaseOffset) * _amplitude;

            if (_rectTransform != null && _uiUseAnchoredPosition)
            {
                Vector2 p = _startAnchoredPosition;
                p.y = _startAnchoredPosition.y + offset;
                _rectTransform.anchoredPosition = p;
                return;
            }

            if (_useLocalSpaceFor3D)
            {
                _cachedTransform.localPosition = _startLocalPosition + (_axis3DNormalized * offset);
            }
            else
            {
                _cachedTransform.position = _startWorldPosition + (_axis3DNormalized * offset);
            }
        }
    }
}


