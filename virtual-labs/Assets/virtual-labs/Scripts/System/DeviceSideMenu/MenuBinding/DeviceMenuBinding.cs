using System.Collections;
using Praxilabs.DeviceSideMenu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeviceMenuBinding : BaseDeviceMenuBinding
{
    [SerializeField] private string _deviceName;
    [SerializeField] private Vector2 _devicePos;
    [SerializeField] private Button _labelsButton;
    private WaitForSeconds _delayStart = new WaitForSeconds(3f); 

    private IEnumerator Start()
    {
        yield return _delayStart;
        SetDeviceMenuPosition();
    }

    public override object GetDeviceMenu()
    {
        var deviceMenu = DeviceMenuWrapper.Instance.GetDeviceMenu(_deviceName);
        return deviceMenu;
    }

    public override object GetDeviceMenuName()
    {
        return _deviceName;
    }

    public override void ToggleDeviceMenu(bool isVisible)
    {
        DeviceMenuWrapper.Instance.SetDeviceSideMenuVisibility(_deviceName, isVisible);
    }

    public override void AddListenerToDeviceMenuLabelsButton(UnityAction callbackAction)
    {
        _labelsButton.onClick.AddListener(callbackAction);
    }

    public override void RemoveListenersFromDeviceMenuLabelsButton()
    {
        _labelsButton.onClick.RemoveAllListeners();
    }

    protected override void SetDeviceMenuPosition()
    {
        if(_devicePos == Vector2.zero) return;

        DeviceMenuWrapper.Instance.SetDeviceMenuPosition(_deviceName, _devicePos);
    }
}