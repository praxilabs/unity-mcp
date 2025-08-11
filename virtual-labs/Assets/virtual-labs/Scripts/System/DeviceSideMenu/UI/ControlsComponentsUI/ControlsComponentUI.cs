using System;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public abstract class ControlsComponentUI : MonoBehaviour
    {
        [field: SerializeField] public string Name {get; protected set;}

        public abstract void InitializeData(ControlsComponent controlsComponent);

        public virtual void AddListenerToComponent(Action<object> onValueChangedCallback)
        {

        }

        public virtual void SetComponentValue(object obj)
        {
            
        }

        public virtual void SetLocalizedData(string label) { }
    }
}

