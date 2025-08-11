using System.Collections.Generic;
using Praxilabs.DeviceSideMenu;
using UnityEngine;

public class DeviceMenuExternalButtonCreator : MonoBehaviour
{
    [SerializeField] private DeviceMenuExternalButton _deviceMenuBtnPrefab;
    [SerializeField] private Transform _deviceMenuContainer;
    [SerializeField] private BaseDeviceMenuBinding _baseDeviceMenuBinding;
    private List<DeviceMenu> _deviceMenuList = new();
    private List<string> _deviceNameList = new();

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        object deviceMenuObj = _baseDeviceMenuBinding.GetDeviceMenu();
        if(deviceMenuObj is List<DeviceMenu> deviceMenus)
        {
            _deviceMenuList = deviceMenus;
        }
        else if(deviceMenuObj is DeviceMenu deviceMenu)
        {
            _deviceMenuList.Add(deviceMenu);
        }
        
        object deviceNameObj = _baseDeviceMenuBinding.GetDeviceMenuName();
        if(deviceNameObj is List<string> deviceNames)
        {
            _deviceNameList = deviceNames;
        }
        else if(deviceNameObj is string deviceName)
        {
            _deviceNameList.Add(deviceName);
        }


        for(int i = 0; i < _deviceMenuList.Count; i++)
        {
            DeviceMenuExternalButton deviceMenuBtnInstance = Instantiate(_deviceMenuBtnPrefab, _deviceMenuContainer);
            deviceMenuBtnInstance.Setup(_deviceMenuList[i]);
            deviceMenuBtnInstance.name = _deviceNameList[i] + "Btn";
        }
    }
}
