using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class ControlsTabContent : TabContent
    {
        public override TabType TabType {get; protected set;} = TabType.Controls;

        private Dictionary<string, ControlsComponentUI> _controlsComponentUIDict = new Dictionary<string, ControlsComponentUI>();

        public override void UpdateData(DeviceInfo deviceInfo, List<GameObject> controlsComponentsGameObjects)
        {
            for(int i = 0; i < controlsComponentsGameObjects.Count; i++)
            {
                controlsComponentsGameObjects[i].transform.SetParent(_contentRect, false);

                if(!controlsComponentsGameObjects[i].TryGetComponent<ControlsComponentUI>(out ControlsComponentUI controlsComponentUI)) return;

                controlsComponentUI.InitializeData(deviceInfo.ControlsComponents[i]);

                _controlsComponentUIDict.Add(controlsComponentUI.Name, controlsComponentUI);
            }
        }

        public ControlsComponentUI GetControlsComponentUI(string componentName)
        {
            return _controlsComponentUIDict[componentName];
        }
    }
}
