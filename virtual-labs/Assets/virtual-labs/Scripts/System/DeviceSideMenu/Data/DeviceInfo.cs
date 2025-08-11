using System;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DeviceInfo : MonoBehaviour 
    {
        [field: SerializeField] public string Name {get; set;}
        [field: SerializeField] public TabType TabTypes {get; set;}
        [field: SerializeReference] public List<ReadingsComponent> ReadingsComponents {get; private set;}= new List<ReadingsComponent>();
        [field: SerializeReference] public List<ControlsComponent> ControlsComponents {get; private set;}= new List<ControlsComponent>();
    }
}