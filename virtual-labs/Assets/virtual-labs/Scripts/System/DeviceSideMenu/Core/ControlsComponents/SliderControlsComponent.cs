using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class SliderControlsComponent : ControlsComponent
    {
        [field: SerializeField] public string Symbol {get; set;}
        [field: SerializeField] public float MinValue {get; set;}
        [field: SerializeField] public float MaxValue {get; set;}
        [field: SerializeField] public bool ShouldDispayScreenValue {get; set;}

        public override ControlsComponentType GetControlsComponentType()
        {
            return ControlsComponentType.Slider;
        }
    }
}