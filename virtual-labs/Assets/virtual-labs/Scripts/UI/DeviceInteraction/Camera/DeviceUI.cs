using Praxilabs.DeviceSideMenu;
using UltimateClean;
using UnityEngine;
using UnityEngine.Events;

public class DeviceUI : MonoBehaviour
{
    public CleanButton closeButton;
    [SerializeField] private BaseDeviceMenuBinding _deviceMenuBinding;
    [SerializeField] private GameObject _deviceUICanvas;
    [SerializeField] private GameObject _sideCamerasParent;
    [SerializeField] private DeviceCameraPan _deviceCameraPan;
    [SerializeField] private DeviceCameraZoom _deviceCameraZoom;

    public CameraSideButtons cameraSideButtons;
    private UnityAction _closeClickDelegate;
    private DeviceSideController _deviceSideController;

    private void Start()
    {
        _deviceSideController = GetComponent<DeviceSideController>();
        _closeClickDelegate = () =>
        {
            ResetCameras();
            ToggleDeviceUI(false);
        };

        closeButton.onClick.AddListener(_closeClickDelegate);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(_closeClickDelegate);
    }

    public void ToggleDeviceUI(bool enable)
    {
        _deviceUICanvas.SetActive(enable);
        // _sideCamerasParent.SetActive(enable);
       _deviceMenuBinding?.ToggleDeviceMenu(enable);
    }

    private void ResetCameras()
    {
        foreach (var deviceSide in _deviceSideController.DeviceSides)
        {
            deviceSide.SetCameraPriorityWithoutNotify(-1);
            deviceSide.ToggleCameraIcon(true);
        }
        _deviceSideController.Reset();
    }

    public void ToggleCameraPan(bool enable)
    {
        _deviceCameraPan.canPan = enable;
    }

    public void ToggleCameraZoom(bool enable)
    {
        _deviceCameraZoom.canZoom = enable;
    }
}

[System.Serializable]
public struct CameraSideButtons
{
    public CleanButton frontCameraButton;
    public CleanButton backCameraButton;
    public CleanButton leftCameraButton;
    public CleanButton rightCameraButton;
}

public enum DeviceSideType
{
    FrontSide, BackSide, LeftSide, RightSide
}