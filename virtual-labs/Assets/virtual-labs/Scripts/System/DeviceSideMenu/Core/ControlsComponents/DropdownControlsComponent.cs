using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DropdownControlsComponent : ControlsComponent
    {
        [field: SerializeField] public List<string> Options {get; set;}
        [field: SerializeField] public bool ConvertExponential {get; set;}

        public override ControlsComponentType GetControlsComponentType()
        {
            return ControlsComponentType.Dropdown;
        }
    }
}