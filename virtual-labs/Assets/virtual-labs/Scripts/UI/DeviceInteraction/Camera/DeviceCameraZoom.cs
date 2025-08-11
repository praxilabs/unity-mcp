using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeviceCameraZoom : MonoBehaviour
{
    [SerializeField] private Button _zoominButton;
    [SerializeField] private Button _zoomoutButton;
    [SerializeField] private Slider _zoomSlider;

    [SerializeField] private float _minZoom = -0.2f;
    [SerializeField] private float _maxZoom = 0.2f;

    [SerializeField] private DeviceSideController _deviceRef;
    private DeviceSideController _device
    {
        get
        {
            if (_deviceRef == null)
            {
                _deviceRef = GetComponentInParent<DeviceSideController>();
            }
            return _deviceRef;
        }
    }
    private CinemachineVirtualCamera _activeCamera;
    private Transform _cameraTransform;
    private Vector3 _initialCameraPosition;

    private UnityAction _zoominDelegate;
    private UnityAction _zoomoutDelegate;
    private Coroutine _zoomCoroutine;

    public bool isZooming;
    public bool canZoom = true;

    private void OnEnable()
    {
        if (_zoomSlider != null)
        {
            _zoominDelegate = () => _zoomSlider.value += 20;
            _zoomoutDelegate = () => _zoomSlider.value -= 20;

            _zoomSlider.onValueChanged.AddListener(HandleCameraZoom);
            _zoominButton.onClick.AddListener(_zoominDelegate);
            _zoomoutButton.onClick.AddListener(_zoomoutDelegate);
            HandleZoomFlag();
        }
    }

    private void OnDisable()
    {
        if (_zoomSlider != null)
        {
            _zoomSlider.onValueChanged.RemoveListener(HandleCameraZoom);
            _zoominButton.onClick.RemoveListener(_zoominDelegate);
            _zoomoutButton.onClick.RemoveListener(_zoomoutDelegate);
        }
    }

    private void Start()
    {
        SetDefaultValues();
    }

    private void HandleCameraZoom(float sliderValue)
    {
        if (_cameraTransform == null && !canZoom) return;

        StopZoomCoroutine();

        // Calculate the zoom factor based on the difference from 50
        if (sliderValue >= 50f)
        {
            float zoomFactor = (sliderValue - 50f) / 50f;
            float zoomDistance = Mathf.Lerp(0f, _maxZoom - _minZoom, zoomFactor);

            _zoomCoroutine= StartCoroutine(SmoothZoomToTarget(_initialCameraPosition + _cameraTransform.forward * zoomDistance, zoomFactor));
        }
        else if (sliderValue < 50f)
        {
            float zoomFactor = (50f - sliderValue) / 50f;
            float zoomDistance = Mathf.Lerp(0f, _minZoom - _maxZoom, zoomFactor);

            _zoomCoroutine = StartCoroutine(SmoothZoomToTarget(_initialCameraPosition + _cameraTransform.forward * zoomDistance, zoomFactor));
        }
    }

    private IEnumerator SmoothZoomToTarget(Vector3 targetPosition, float zoomFactor)
    {
        while (Vector3.Distance(_cameraTransform.localPosition, targetPosition) > 0.01f)
        {
            _cameraTransform.position = Vector3.MoveTowards(
                _cameraTransform.position,
                targetPosition,
                0.15f * Time.deltaTime
            );
            yield return null;
        }
        _cameraTransform.position = targetPosition;
    }

    public void SetDefaultValues()
    {
        StopZoomCoroutine();
        ResetZoom();

        _activeCamera = _device.activeCamera;
        if (_activeCamera == null) return;

        _cameraTransform = _activeCamera.transform;
        _initialCameraPosition = _cameraTransform.position;

        _zoomSlider.minValue = 0f;
        _zoomSlider.maxValue = 100f;
        _zoomSlider.value = 50f;
    }

    public void ResetZoom()
    { 
        if (_cameraTransform == null) return;
        _cameraTransform.position = _initialCameraPosition;
    }

    private void StopZoomCoroutine()
    {
        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);
    }

    /// <summary>
    /// Use event trigger to set the flag to true if pointer is down and false if pointer is up
    /// </summary>
    private void HandleZoomFlag()
    {
        EventTrigger trigger = _zoomSlider.gameObject.AddComponent<EventTrigger>();

        // PointerDown event
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDownEntry.callback.AddListener((data) => { isZooming = true; });
        trigger.triggers.Add(pointerDownEntry);

        // PointerUp event
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUpEntry.callback.AddListener((data) => { isZooming = false; });
        trigger.triggers.Add(pointerUpEntry);
    }
}
