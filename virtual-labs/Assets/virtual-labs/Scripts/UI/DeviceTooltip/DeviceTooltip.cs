using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class DeviceTooltip : MonoBehaviour
{
    private const float SCREEN_EDGE_PADDING = 10f;
    private const float SCREEN_EDGE_HIDE_THRESHOLD = 50f;
    
    [Header("References")]
    [SerializeField] private GameObject _window;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private TextMeshProUGUI _deviceNameText;
    
    [Header("Settings")]
    [ColorUsage(true, true), SerializeField] private Color _tintColorForBlack;
    [ColorUsage(true, true), SerializeField] private Color _tintColorForWhite;
    [SerializeField] private float _worldSpaceOffset;
    [field: SerializeField] public bool IsHighlightGloballyOff {get; set;}
    [field: SerializeField] public bool IsTooltipGloballyOff {get; set;}
    private bool _isHighlightLocallyOff;
    private bool _isTooltipLocallyOff;
    private bool _shouldTintColorForWhite = false;
    private string _deviceName;
    private Vector3 _targetPosition;
    private RectTransform _windowRectTransform;
    private RectTransform _arrowRectTransform;
    private Camera _mainCamera;
    private bool _isVisible = false;
    private Vector3 _lastScreenPosition;
    private float _arrowHeight;
    private Coroutine _showCoroutine;
    private WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);
    private List<Renderer> _devicePartsRenderers = new List<Renderer>();
    
    private void Awake()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        _windowRectTransform = _window.GetComponent<RectTransform>();
        _arrowRectTransform = _arrow.GetComponent<RectTransform>();
        _mainCamera = Camera.main;
        
        // Calculate arrow height for positioning, divided by 4 to get positioned exactly
        _arrowHeight = _arrowRectTransform.rect.height * _arrowRectTransform.localScale.y / 4;
        
        // Make sure tooltip is hidden at start
        _window.SetActive(false);
        _arrow.SetActive(false);
        
        // Set device name
        if (_deviceNameText != null && !string.IsNullOrEmpty(_deviceName))
            _deviceNameText.text = _deviceName;
    }
    
    private void Update()
    {
        if (_isVisible)
        {
            UpdatePosition();
        }
    }

    public void Setup(Vector3 targetPosition, string deviceName, float worldSpaceOffset, List<Renderer> devicePartsRenderers, bool isFlashing, bool shouldTintColorForWhite = false)
    {
        _targetPosition = targetPosition;
        _deviceName = deviceName;
        _worldSpaceOffset = worldSpaceOffset;
        _devicePartsRenderers = devicePartsRenderers;
        _isHighlightLocallyOff = isFlashing;
        _shouldTintColorForWhite = shouldTintColorForWhite;
            
        if (_deviceNameText != null)
            _deviceNameText.text = _deviceName;
    }
    
    
    public void Show()
    {
        if (IsDeviceTooCloseToEdge())
        {
            Hide();
            return;
        }

        if(_showCoroutine != null)
            StopCoroutine(_showCoroutine);
        _showCoroutine = StartCoroutine(ShowCoroutine());
    }

    public void Hide()
    {
        if(_showCoroutine != null)
            StopCoroutine(_showCoroutine);
        
        DisableDeviceHighlight();

        // if(IsTooltipGloballyOff)
        //     return;

        _window.SetActive(false);
        _arrow.SetActive(false);
        _isVisible = false;
    }

    private IEnumerator ShowCoroutine()
    {
        yield return _waitForSeconds;
        
        EnableDeviceHighlight();

        if(IsTooltipGloballyOff)
            yield break;

        _window.SetActive(true);
        _arrow.SetActive(true);
        _isVisible = true;

        UpdatePosition();
    }

    private void EnableDeviceHighlight()
    {
        if(IsHighlightGloballyOff || _isHighlightLocallyOff)
            return;
        
        for(int i = 0; i < _devicePartsRenderers.Count; i++)
            TintColorApplier.SetTint(_devicePartsRenderers[i], _shouldTintColorForWhite ? _tintColorForWhite : _tintColorForBlack);
    }

    private void DisableDeviceHighlight()
    {
        // if(IsHighlightGloballyOff || _isHighlightLocallyOff)
        //     return;

        for(int i = 0; i < _devicePartsRenderers.Count; i++)
            TintColorApplier.ClearTint(_devicePartsRenderers[i]);
    }

    private void UpdatePosition()
    {
        if (_mainCamera == null)
            return;
        
        // Get device position in world space
        Vector3 devicePosition = _targetPosition;
        
        // Calculate the offset position in world space (up direction)
        Vector3 offsetPosition = devicePosition + Vector3.up * _worldSpaceOffset;
        
        // Convert world positions to screen positions
        Vector3 deviceScreenPos = _mainCamera.WorldToScreenPoint(devicePosition);
        Vector3 offsetScreenPos = _mainCamera.WorldToScreenPoint(offsetPosition);
        
        // Calculate screen space offset (this adapts to zoom level)
        float screenSpaceOffset = offsetScreenPos.y - deviceScreenPos.y;
        
        _lastScreenPosition = deviceScreenPos;
        
        // Check if device is behind camera
        if (deviceScreenPos.z < 0)
        {
            Hide();
            return;
        }
        
        // Check if device is too close to screen edge
        if (IsDeviceTooCloseToEdge())
        {
            Hide();
            return;
        }
        
        // Get window height (half height since pivot is at center)
        float windowHalfHeight = (_windowRectTransform.rect.height * _windowRectTransform.localScale.y) / 2;
        
        // Position window above the device with dynamic offset based on zoom level
        _windowRectTransform.position = new Vector3(
            deviceScreenPos.x,
            deviceScreenPos.y + screenSpaceOffset + windowHalfHeight + _arrowHeight/2,
            0
        );
        
        // Position arrow directly below window (connected to it)
        _arrowRectTransform.position = new Vector3(
            deviceScreenPos.x,
            _windowRectTransform.position.y - windowHalfHeight - _arrowHeight/2,
            0
        );
        
        // Make sure tooltip stays within screen bounds
        AdjustToScreenBounds();
    }
    
    private void AdjustToScreenBounds()
    {
        Vector3 windowPosition = _windowRectTransform.position;
        Vector2 windowSize = _windowRectTransform.rect.size;
        float windowHalfHeight = (windowSize.y * _windowRectTransform.localScale.y) / 2;
        float windowHalfWidth = (windowSize.x * _windowRectTransform.localScale.x) / 2;
        
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        float leftEdge = windowPosition.x - windowHalfWidth;
        float rightEdge = windowPosition.x + windowHalfWidth;
        float topEdge = windowPosition.y + windowHalfHeight;
        float bottomEdge = windowPosition.y - windowHalfHeight;
        
        Vector3 offset = Vector3.zero;
        
        // Check horizontal bounds
        if (leftEdge < SCREEN_EDGE_PADDING)
        {
            offset.x = SCREEN_EDGE_PADDING - leftEdge;
        }
        else if (rightEdge > screenWidth - SCREEN_EDGE_PADDING)
        {
            offset.x = (screenWidth - SCREEN_EDGE_PADDING) - rightEdge;
        }
        
        // Check vertical bounds
        if (topEdge > screenHeight - SCREEN_EDGE_PADDING)
        {
            offset.y = (screenHeight - SCREEN_EDGE_PADDING) - topEdge;
        }
        else if (bottomEdge < SCREEN_EDGE_PADDING)
        {
            offset.y = SCREEN_EDGE_PADDING - bottomEdge;
        }
        
        // Apply offset to window
        _windowRectTransform.position += offset;
        
        // Always keep arrow X-aligned with device if possible
        float arrowX = _lastScreenPosition.x;
        
        // But constrain arrow X position to stay within window bounds
        float minArrowX = windowPosition.x + offset.x - windowHalfWidth + (_arrowRectTransform.rect.width * _arrowRectTransform.localScale.x) / 2;
        float maxArrowX = windowPosition.x + offset.x + windowHalfWidth - (_arrowRectTransform.rect.width * _arrowRectTransform.localScale.x) / 2;
        
        arrowX = Mathf.Clamp(arrowX, minArrowX, maxArrowX);
        
        // Keep arrow attached to bottom of window
        _arrowRectTransform.position = new Vector3(
            arrowX,
            _windowRectTransform.position.y - windowHalfHeight - _arrowHeight/2,
            _arrowRectTransform.position.z
        );
    }
    
    private bool IsDeviceTooCloseToEdge()
    {
        if ( _mainCamera == null)
            return true;
            
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(_targetPosition);
        
        // Check if device is outside screen or too close to edges
        return screenPos.x < SCREEN_EDGE_HIDE_THRESHOLD ||
               screenPos.x > Screen.width - SCREEN_EDGE_HIDE_THRESHOLD ||
               screenPos.y < SCREEN_EDGE_HIDE_THRESHOLD ||
               screenPos.y > Screen.height - SCREEN_EDGE_HIDE_THRESHOLD;
    }
}