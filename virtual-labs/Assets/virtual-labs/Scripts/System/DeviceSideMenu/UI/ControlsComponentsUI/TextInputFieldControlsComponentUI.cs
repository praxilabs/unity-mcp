using System;
using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class TextInputFieldControlsComponentUI : ControlsComponentUI
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _textInputFieldLabelText;
        
        public override void InitializeData(ControlsComponent controlsComponent)
        {
            TextInputFieldControlsComponent textInputFieldControlsComponent = controlsComponent as TextInputFieldControlsComponent;

            Name = textInputFieldControlsComponent.Name;

            _textInputFieldLabelText.text = textInputFieldControlsComponent.ComponentLabel;
        }

        public override void AddListenerToComponent(Action<object> onValueChangedCallback)
        {
            _inputField.onValueChanged.AddListener((newValue) => {
                onValueChangedCallback(newValue);
            });
        }

        public override void SetLocalizedData(string label) 
        { 
            _textInputFieldLabelText.text = label;
        }
    }
}