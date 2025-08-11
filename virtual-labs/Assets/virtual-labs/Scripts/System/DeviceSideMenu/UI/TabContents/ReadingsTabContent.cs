using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class ReadingsTabContent : TabContent
    {
        public override TabType TabType {get; protected set;} = TabType.Readings;

        private Dictionary<string, ReadingsComponentUI> _readingsComponentUIDict = new Dictionary<string, ReadingsComponentUI>();

        public override void UpdateData(DeviceInfo deviceInfo, List<GameObject> readingsComponentsGameObjects)
        {
            for(int i = 0; i < readingsComponentsGameObjects.Count; i++)
            {
                readingsComponentsGameObjects[i].transform.SetParent(_contentRect, false);

                if(!readingsComponentsGameObjects[i].TryGetComponent<ReadingsComponentUI>(out ReadingsComponentUI readingsComponentUI)) return;

                readingsComponentUI.InitializeData(deviceInfo.ReadingsComponents[i]);

                _readingsComponentUIDict.Add(readingsComponentUI.Name, readingsComponentUI);
            }
        }

        public ReadingsComponentUI GetReadingsComponentUI(string componentName)
        {
            return _readingsComponentUIDict[componentName];
        }
    }
}
