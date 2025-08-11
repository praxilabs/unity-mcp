using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.DeviceSideMenu
{
    public class CheckboxControlsComponentUI : ControlsComponentUI
    {
        [SerializeField] private Toggle checkbox;
        [SerializeField] private TextMeshProUGUI checkboxLabelText;

        public override void SetLocalizedData(string label)
        {
            checkboxLabelText.text = label;
        }

        public override void InitializeData(ControlsComponent controlsComponent)
        {
            CheckboxControlsComponent checkboxComponent = controlsComponent as CheckboxControlsComponent;

            Name = checkboxComponent.Name;

            checkboxLabelText.text = checkboxComponent.ComponentLabel;
            checkbox.isOn = checkboxComponent.IsTrue;
        }

        public override void AddListenerToComponent(Action<object> onValueChangedCallback)
        {
            checkbox.onValueChanged.AddListener((newValue) => {
                onValueChangedCallback(newValue);
            });
        }
        
        public override void SetComponentValue(object obj)
        {
            bool isChecked = (bool)obj;

            checkbox.isOn = isChecked;
        }
    }
}