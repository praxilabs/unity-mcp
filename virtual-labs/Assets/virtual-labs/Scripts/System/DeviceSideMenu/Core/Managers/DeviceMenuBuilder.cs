using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DeviceMenuBuilder : MonoBehaviour 
    {
        public string DeviceMenuSuffix {get; private set; } = "_DeviceMenuUI";

        [Header("Parent Canvas")]
        [SerializeField] private Transform _canvasTransform;
        [Header("Presets")]
        [SerializeField] private GameObject _deviceMenuCanvasPrefab;
        [SerializeField] private DynamicComponentSpawner _tabDynamicComponentSpawner;
        private TabDataParser _tabDataParser = new TabDataParser();

        public Dictionary<string, DeviceMenu> BuildDeviceMenus(List<DeviceInfo> devicesInfo)
        {
            Dictionary<string, DeviceMenu> deviceMenusGODict = new Dictionary<string, DeviceMenu>();

            for(int i = 0; i < devicesInfo.Count; i++)
            {
                GameObject sideMenuCanvasGO = Instantiate(_deviceMenuCanvasPrefab, _canvasTransform);
                sideMenuCanvasGO.name = devicesInfo[i].Name + "_DeviceMenuCanvas";
                DeviceMenu deviceMenu = sideMenuCanvasGO.GetComponentInChildren<DeviceMenu>();
                deviceMenu.gameObject.name = devicesInfo[i].Name + DeviceMenuSuffix;

                deviceMenusGODict.Add(devicesInfo[i].Name, deviceMenu);

                SetupDeviceMenu(deviceMenu, devicesInfo[i]);
            }
            return deviceMenusGODict;
        }

#if UNITY_EDITOR
        public DeviceMenu BuildDeviceMenu(DeviceInfo deviceInfo)
        {
            if(_canvasTransform == null)
            {
                Debug.LogError("Assign a parent canvas!");
                return null;
            }

            GameObject sideMenuCanvasGO = (GameObject)PrefabUtility.InstantiatePrefab(_deviceMenuCanvasPrefab, _canvasTransform);
            sideMenuCanvasGO.name = deviceInfo.Name + "_DeviceMenuCanvas";
            DeviceMenu deviceMenu = sideMenuCanvasGO.GetComponentInChildren<DeviceMenu>();
            deviceMenu.gameObject.name = deviceInfo.Name + "_DeviceMenuUI";

            SetupDeviceMenu(deviceMenu, deviceInfo);
            return deviceMenu; // not needed in our case so far (might be helpful for future uses)
        }
#endif

        private void SetupDeviceMenu(DeviceMenu deviceMenu, DeviceInfo deviceInfo)
        {
            InsertReadingsTabData(deviceMenu, deviceInfo);
            InsertControlsTabData(deviceMenu, deviceInfo);

            deviceMenu.InitializeMenu(deviceInfo.Name, deviceInfo.TabTypes);
        }

        private void InsertReadingsTabData(DeviceMenu deviceMenu, DeviceInfo deviceInfo)
        {
            if((deviceInfo.TabTypes & TabType.Readings) != TabType.Readings) return;

            List<GameObject> readingsComponentsGameObjects = new List<GameObject>();

            // Loop through deviceInfoReadingsComponents list and create instance for each one of them
            foreach(ReadingsComponent readingsComponent in deviceInfo.ReadingsComponents)
            {
                string componentName = readingsComponent.GetType().Name;

                readingsComponent.DeviceName = deviceInfo.Name;
                
                // Get the component prefab by its name from the list of predefined components
                GameObject componentGO = _tabDynamicComponentSpawner.SpawnComponent(componentName);

                componentGO.name += "_" + readingsComponent.Name;
                readingsComponentsGameObjects.Add(componentGO);
            }

            _tabDataParser.InsertReadingsTabData(deviceMenu, deviceInfo, readingsComponentsGameObjects);
            deviceMenu.StoreReadingsComponents(readingsComponentsGameObjects); // Visualizing Dynamic Components - No Logic
        }

        private void InsertControlsTabData(DeviceMenu deviceMenu, DeviceInfo deviceInfo)
        {
            if((deviceInfo.TabTypes & TabType.Controls) != TabType.Controls) return;

            List<GameObject> controlsComponentsGameObjects = new List<GameObject>();

            // Loop through deviceInfoControlsComponents list and create instance for each one of them
            foreach(ControlsComponent controlsComponent in deviceInfo.ControlsComponents)
            {
                string componentName = controlsComponent.GetType().Name;
                // Get the component prefab by its name from the list of predefined components
                GameObject componentGO = _tabDynamicComponentSpawner.SpawnComponent(componentName);

                componentGO.name += "_" + controlsComponent.Name;
                controlsComponentsGameObjects.Add(componentGO);
            }

            _tabDataParser.InsertControlsTabData(deviceMenu, deviceInfo, controlsComponentsGameObjects);
            deviceMenu.StoreControlsComponents(controlsComponentsGameObjects); // Visualizing Dynamic Components - No Logic
        }
    }
}