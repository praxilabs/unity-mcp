using Cinemachine;
using CursorStates;
using System.Collections.Generic;
using UnityEngine;

public class DeviceSideController : MonoBehaviour
{
    [HideInInspector] public CinemachineVirtualCamera activeCamera;

    [field: SerializeField] public DeviceSide[] DeviceSides { get; private set; }
    [SerializeField] private DeviceCameraZoom _cameraZoom;
    private DeviceSide _activeDeviceSide;
    [SerializeField] private LabelsController _labelsController;

    private List<CursorStateHandler> _deviceSideCursorHandlers = new();
    private List<CursorStateHandler> _otherCursorHandlers = new();
    private const string DEVICE_SIDE_TAG = "Device";

    private void OnEnable()
    {
        ResolveObjects();
        CursorStatesManager.Instance.AddCursorHandler(this);
    }

    public void ToggleColliders(bool shouldEnable)
    {
        for (int i = 0; i < DeviceSides.Length; i++)
        {
            DeviceSides[i].ToggleColliders(shouldEnable);
        }
    }

    public void ToggleDeviceSide(bool shouldEnable)
    {
        for (int i = 0; i < DeviceSides.Length; i++)
        {
            DeviceSides[i].ToggleDeviceSide(shouldEnable);
        }
    }

    public void SetActiveSide(DeviceSide deviceSide)
    {
        if (_activeDeviceSide != null)
        {
            _activeDeviceSide.SetCameraPriority(0);
            _activeDeviceSide.ToggleCameraIcon(true);
        }
        if (_cameraZoom == null)
            ResolveObjects();

        _activeDeviceSide = deviceSide;
        _activeDeviceSide.SetCameraPriority(1);
        _activeDeviceSide.ToggleCameraIcon(false);
        if (_labelsController != null)
            _labelsController.SetSideType(_activeDeviceSide.DeviceSideType);
        else
            Debug.Log("<color:#B12C00>LabelsController is null</color>");
        activeCamera = _activeDeviceSide._sideCamera;
        _cameraZoom.SetDefaultValues();
    }

    public void ToggleAllCursorHandlers(bool enable)
    {
        ToggleDeviceSideCursorHandlers(enable);
        ToggleOtherCursorHandlers(enable);
    }

    public void ToggleDeviceSideCursorHandlers(bool enable)
    {
        foreach (CursorStateHandler cursorState in _deviceSideCursorHandlers)
        {
            cursorState.canHandleCursorState = enable;
        }
    }

    private void ToggleOtherCursorHandlers(bool enable)
    {
        foreach (CursorStateHandler cursorState in _otherCursorHandlers)
        {
            cursorState.canHandleCursorState = enable;
        }
    }

    public void Reset()
    {
        _activeDeviceSide.SetCameraPriority(0); // It is already handled by camera switcher system but this will let us invoke OnDeviceSideExit
        _activeDeviceSide = null;
        activeCamera = null;
    }

    private void OnDisable()
    {
        if(CursorStatesManager.Instance == null) return;
        CursorStatesManager.Instance.RemoveCursorHandler(this);
    }

    private void ResolveObjects()
    {
        if(DeviceSides == null)
        DeviceSides = GetComponentsInChildren<DeviceSide>();
        if(_cameraZoom == null)
        _cameraZoom = GetComponentInChildren<DeviceCameraZoom>();
        if(_labelsController == null)
        _labelsController = GetComponent<LabelsController>();

        CursorStateHandler[] cursorHandlers = transform.parent.GetComponentsInChildren<CursorStateHandler>();

        foreach (CursorStateHandler cursorState in cursorHandlers)
        {
            if (cursorState.gameObject.CompareTag(DEVICE_SIDE_TAG))
                _deviceSideCursorHandlers.Add(cursorState);
            else
                _otherCursorHandlers.Add(cursorState);
        }
    }
}