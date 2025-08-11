using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    [System.Serializable]
    public abstract class ReadingsComponent
    {
        public string DeviceName {get; set;}
        [field: SerializeField] public string Name {get; set;}
        
        public abstract ReadingsComponentType GetReadingsComponentType();
    }
}