using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.DeviceSideMenu
{
    public class DeviceMenuWrapper : Singleton<DeviceMenuWrapper> 
    {
        [SerializeField] private DeviceSideMenusManager _deviceSideMenusManager;

        private void Start()
        {
            _addToDontDestroyOnLoad = false;
        }

        public void RegisterDeviceMenu(DeviceMenu deviceMenu)
        {
            _deviceSideMenusManager.RegisterDeviceMenu(deviceMenu);
        }

        public void UnregisterDeviceMenu(DeviceMenu deviceMenu)
        {
            _deviceSideMenusManager.UnregisterDeviceMenu(deviceMenu);      
        }

        public void SetDeviceSideMenuVisibility(string deviceName, bool isVisible, TabType? tabType = null)
        {
            DeviceMenu deviceMenu = GetDeviceMenu(deviceName);

            deviceMenu.SetDeviceSideMenuVisibility(isVisible, tabType);
        }
        public void SetDeviceMenuPosition(string deviceName, Vector2 position)
        {
            DeviceMenu deviceMenu = GetDeviceMenu(deviceName);

            deviceMenu.SetDeviceMenuPosition(position);
        }

        public void EnforceDeviceMenuTabSelection(string deviceName, TabType tabType)
        {
            DeviceMenu deviceMenu = GetDeviceMenu(deviceName);
            deviceMenu.EnforceTabSelection(tabType);
        }

        public void EnforceAllDeviceMenusTabSelection(TabType tabType)
        {
            List<DeviceMenu> deviceMenus = _deviceSideMenusManager.GetDeviceMenus();
            
            foreach(DeviceMenu deviceMenu in deviceMenus)
            {
                deviceMenu.EnforceTabSelection(tabType);
            }
        }

        public void EnforceAllDeviceMenusTabSelectionString(string tabTypeString) // Temp until we implement enum selection in function call node (xnode), added "String" to show up in xnode dropdown
        {
            TabType tabType = GetTabType(tabTypeString);
            EnforceAllDeviceMenusTabSelection(tabType);
        }

        public DeviceMenu GetDeviceMenu(string deviceName)
        {
            return _deviceSideMenusManager.GetDeviceMenu(deviceName);
        }

        private TabType GetTabType(string tabTypeString) // Temp until we implement enum selection in function call node (xnode)
        {
            switch(tabTypeString)
            {
                case "Controls":
                    return TabType.Controls;
                case "Readings":
                    return TabType.Readings;
                case "SafetyProcedures":
                    return TabType.SafetyProcedures;
                case "Description":
                default:
                    return TabType.Description;
            }
        }


        // public void AddListenerToControlsComponent(string deviceName, string componentName, Action<object> onValueChangedCallback)
        // {
        //     ControlsComponentUI controlsComponentUI = GetTabControlsComponentUI(deviceName, componentName);

        //     controlsComponentUI.AddListenerToComponent(onValueChangedCallback);
        // }

        // public void SetComponentValue(string deviceName, string componentName, object obj)
        // {
        //     ControlsComponentUI controlsComponentUI = GetTabControlsComponentUI(deviceName, componentName);

        //     controlsComponentUI.SetComponentValue(obj);
        // }

        public ControlsComponentUI GetTabControlsComponentUI(string deviceName, string componentName)
        {
            TabContent tabContent = GetTabContent(deviceName, TabType.Controls);


            ControlsTabContent controlsTabContent = tabContent as ControlsTabContent;
            ControlsComponentUI controlsComponentUI = controlsTabContent.GetControlsComponentUI(componentName);

            return controlsComponentUI;
        }
        
        public ReadingsComponentUI GetTabReadingsComponentUI(string deviceName, string componentName)
        {
            TabContent tabContent = GetTabContent(deviceName, TabType.Readings);


            ReadingsTabContent readingsTabContent = tabContent as ReadingsTabContent;
            ReadingsComponentUI readingsComponentUI = readingsTabContent.GetReadingsComponentUI(componentName);

            return readingsComponentUI;
        }

        private TabContent GetTabContent(string deviceName, TabType tabType)
        {
            DeviceMenu sideMenu = _deviceSideMenusManager.GetDeviceMenu(deviceName);

            TabContent tabContent = sideMenu.GetTabContent(tabType);

            return tabContent;
        }
    }
}