using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class TextInputFieldControlsComponent : ControlsComponent
    {
        public override ControlsComponentType GetControlsComponentType()
        {
            return ControlsComponentType.TextInputField;
        }
    }   
}