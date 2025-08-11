using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class TabDataParser
    {
        public void InsertReadingsTabData(DeviceMenu deviceMenu, DeviceInfo deviceInfo, List<GameObject> readingsComponentsGameObjects)
        {
            ReadingsTabContent readingsTabContent = GetTabContent(deviceMenu, TabType.Readings) as ReadingsTabContent;
            readingsTabContent.UpdateData(deviceInfo, readingsComponentsGameObjects);
        }

        public void InsertControlsTabData(DeviceMenu deviceMenu, DeviceInfo deviceInfo, List<GameObject> controlsComponentsGameObjects)
        {
            ControlsTabContent controlsTabContent = GetTabContent(deviceMenu, TabType.Controls) as ControlsTabContent;
            controlsTabContent.UpdateData(deviceInfo, controlsComponentsGameObjects);
        }

        private TabContent GetTabContent(DeviceMenu deviceMenu, TabType tabType)
        {
            for(int i = 0; i < deviceMenu.TabContents.Count; i++)
            {
                if((tabType & deviceMenu.TabContents[i].TabType) == tabType)
                {
                    return deviceMenu.TabContents[i];
                }
            }
            Debug.LogWarning($"{tabType} <- the required tab is not found, returning the first tab in the given sideMenu.TabContents");
            return deviceMenu.TabContents[0];
        }
    }
}