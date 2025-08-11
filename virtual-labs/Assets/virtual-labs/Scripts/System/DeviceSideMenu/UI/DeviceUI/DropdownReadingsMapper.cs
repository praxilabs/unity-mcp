using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Praxilabs.DeviceSideMenu
{
    public class DropdownReadingsMapper : MonoBehaviour
    {
        [SerializeField] private GameEventScriptableObject _onDropdownValueChanged;
        [SerializeField] private DropdownControlsComponentUI _dropdownComponentUI;

        private void Start() 
        {
            _dropdownComponentUI.AddListenerToComponent(HandleDropdownComponentValueChanged); // Updates knob's value
        }

        private void HandleDropdownComponentValueChanged(object obj)
        {
            int newIndex = (int)obj;
            _onDropdownValueChanged?.RaiseEvent(newIndex);
        }

        public void ListenOnControllerValueChanged(object data)
        {
            _dropdownComponentUI?.SetComponentValue(data);
        }

        public void SetDropdownInteractability(bool isInteractable)
        {
            _dropdownComponentUI.SetInteractability(isInteractable);
        }
    }
}
