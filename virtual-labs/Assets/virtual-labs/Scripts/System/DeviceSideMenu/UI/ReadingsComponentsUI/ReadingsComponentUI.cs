using System;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public abstract class ReadingsComponentUI : MonoBehaviour
    {
        public string DeviceName {get; protected set;}
        [field: SerializeField] public string Name {get; protected set;}
        public virtual void InitializeData(ReadingsComponent readingsComponent) 
        {
            DeviceName = readingsComponent.DeviceName;
            Name = readingsComponent.Name;
        }
        public virtual void SetComponentValue(object obj) 
        {

        }

    }
}
