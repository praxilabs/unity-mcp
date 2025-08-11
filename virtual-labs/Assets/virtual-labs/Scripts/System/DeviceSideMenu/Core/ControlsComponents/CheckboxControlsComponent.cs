using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class CheckboxControlsComponent : ControlsComponent
    {
        [field: SerializeField] public bool IsTrue {get; set;}

        public override ControlsComponentType GetControlsComponentType()
        {
            return ControlsComponentType.Dropdown;
        }
    }
}