using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DeviceSideMenusManager : MonoBehaviour 
    {
        [SerializeField] private DeviceMenuBuilder _deviceMenuBuilder;
        private Dictionary<string, DeviceMenu> _deviceMenuDict = new Dictionary<string, DeviceMenu>();
        private string _deviceMenuName;

        private void Start()
        {
            ExperimentManager.OnStageStart += ClearDeviceMenuDictionary;
        }

        public void RegisterDeviceMenu(DeviceMenu deviceMenu)
        {
            _deviceMenuName = GetDeviceMenuName(deviceMenu);

            if(_deviceMenuDict.ContainsKey(_deviceMenuName)) return;
            _deviceMenuDict.Add(_deviceMenuName, deviceMenu);
        }

        public void UnregisterDeviceMenu(DeviceMenu deviceMenu)
        {
            _deviceMenuDict.Remove(deviceMenu.name);   
        }

        public DeviceMenu GetDeviceMenu(string deviceName)
        {
            return _deviceMenuDict[deviceName];
        }

        public List<DeviceMenu> GetDeviceMenus()
        {
            return _deviceMenuDict.Values.ToList();
        }

        private string GetDeviceMenuName(DeviceMenu deviceMenu)
        {
            string deviceMenuName = deviceMenu.name;
            deviceMenuName = deviceMenuName.Substring(0, deviceMenuName.Length - _deviceMenuBuilder.DeviceMenuSuffix.Length);
            return deviceMenuName;
        }

        private void ClearDeviceMenuDictionary()
        {
            _deviceMenuDict.Clear();
        }

        private void OnDestroy()
        {
            ExperimentManager.OnStageStart -= ClearDeviceMenuDictionary;
        }
    }
}