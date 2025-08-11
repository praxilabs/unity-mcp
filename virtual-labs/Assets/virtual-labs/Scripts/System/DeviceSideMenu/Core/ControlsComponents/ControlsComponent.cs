using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    [System.Serializable]
    public abstract class ControlsComponent
    {
        [field: SerializeField] public string Name {get; set;}
        public string ComponentLabel {get; set;}

        public abstract ControlsComponentType GetControlsComponentType();
    }
}
