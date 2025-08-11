using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class RandomDisplayFieldExpandedWindowTest : MonoBehaviour
    {
        [SerializeField] private string _deviceName;
        [SerializeField] private List<string> _componentNames;


        private void Update() 
        {
            if(UnityEngine.Input.GetKeyDown(KeyCode.Z))
            {
                UpdateReadings();
            }  
        }

        private void UpdateReadings()
        {
            foreach(string componentName in _componentNames)
            {
                ReadingsComponentUI readingsComponentUI = 
                    DeviceMenuWrapper.Instance.GetTabReadingsComponentUI(_deviceName, componentName);
            
                readingsComponentUI.SetComponentValue(Random.Range(50f,200f));
            }
        }
    }
}
