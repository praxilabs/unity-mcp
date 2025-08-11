using System.Collections;
using System.Collections.Generic;
using Praxilabs.DeviceSideMenu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeviceMenuMultipleBinding : BaseDeviceMenuBinding
{
    [SerializeField] private List<string> _deviceName;
    [SerializeField] private List<Vector2> _devicePos;
    // [SerializeField] private List<Button> _labelsButtons = new();
    private WaitForSeconds _delayStart = new WaitForSeconds(3f); 

    private IEnumerator Start()
    {
        yield return _delayStart;
        SetDeviceMenuPosition();
    }

    public override object GetDeviceMenu()
    {
        List<DeviceMenu> deviceMenus = new();
        for(int i = 0; i < _deviceName.Count; i++)
        {
            var deviceMenu = DeviceMenuWrapper.Instance.GetDeviceMenu(_deviceName[i]);
            deviceMenus.Add(deviceMenu);
        }
        return deviceMenus;
    }

    public override object GetDeviceMenuName()
    {
        return _deviceName;
    }

    public override void ToggleDeviceMenu(bool isVisible)
    {
        for(int i = 0; i < _deviceName.Count; i++)
        {
            DeviceMenuWrapper.Instance.SetDeviceSideMenuVisibility(_deviceName[i], isVisible);
        }
    }
    
    public override void AddListenerToDeviceMenuLabelsButton(UnityAction callbackAction)
    {
        // for(int i = 0; i < _labelsButtons.Count; i++)
        // {
        //     _labelsButtons[i].onClick.AddListener(callbackAction);
        // }
    }

    public override void RemoveListenersFromDeviceMenuLabelsButton()
    {
        // for(int i = 0; i < _labelsButtons.Count; i++)
        // {
        //     _labelsButtons[i].onClick.RemoveAllListeners();
        // }
    }

    protected override void SetDeviceMenuPosition()
    {
        for(int i = 0; i < _deviceName.Count; i++)
        {
            if(_devicePos[i] == Vector2.zero) continue;
            DeviceMenuWrapper.Instance.SetDeviceMenuPosition(_deviceName[i], _devicePos[i]);
        }
    }
}