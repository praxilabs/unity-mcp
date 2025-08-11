using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DropdownControlsComponentUI : ControlsComponentUI
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private TextMeshProUGUI dropdownLabelText;
        [SerializeField] private List<string> _dropdownOptions = new();
        [SerializeField] private bool _shouldConvertExponential;

        public override void SetLocalizedData(string label)
        {
            dropdownLabelText.text = label;
        }

        public override void InitializeData(ControlsComponent controlsComponent)
        {
            DropdownControlsComponent dropdownComponent = controlsComponent as DropdownControlsComponent;

            Name = dropdownComponent.Name;

            dropdownLabelText.text = dropdownComponent.ComponentLabel;

            _dropdownOptions = dropdownComponent.Options;

            _shouldConvertExponential = dropdownComponent.ConvertExponential;

            List<string> optionsFormatted = new();
            for(int i = 0; i < dropdownComponent.Options.Count; i++)
            {
                optionsFormatted.Add(FormatFloatToString(dropdownComponent.Options[i]));
                // options[i] = FormatFloatToString(dropdownComponent.Options[i]);
            }
            _dropdown.AddOptions(optionsFormatted);
        }

        public override void AddListenerToComponent(Action<object> onValueChangedCallback)
        {
            _dropdown.onValueChanged.AddListener((newIndex) => {
                // string selectedValue = _dropdown.options[newIndex].text;
                // float.TryParse(selectedValue, out float newValue);
                onValueChangedCallback(newIndex);
            });
        }

        public override void SetComponentValue(object data)
        {
            if(data is bool isTrue)
            {
                int dropdownIndex = isTrue ? 1 : 0;
                UpdateDropdown(dropdownIndex);
                return;
            }

            // Get the dropdown index based on the provided data(string/float)
            int index = GetDropdownIndexByOption(data);
                    
            if (index != -1)
            {
                UpdateDropdown(index);
            }
            else
            {
                Debug.LogWarning($"'{data}' not found in dropdown options.");
            }
        }

        public void SetInteractability(bool isInteractable)
        {
            _dropdown.interactable = isInteractable;
        }

        private void UpdateDropdown(int index)
        {
            _dropdown.SetValueWithoutNotify(index);
            _dropdown.RefreshShownValue(); // Refresh to display the selected option
        }

        private int GetDropdownIndexByOption(object data)
        {
            if (data is float desiredOption)
            {
                return GetDropdownIndex(desiredOption);
            }
            else if (data is string optionString)
            {
                return _dropdown.options.FindIndex(option => option.text == optionString);
            }
            return -1;
        }

        private int GetDropdownIndex(float value)
        {
            Debug.Log(_dropdownOptions.Count);
            for(int i = 0; i < _dropdownOptions.Count; i++)
            {
                if(Mathf.Approximately(float.Parse(_dropdownOptions[i]),value))
                {
                    return i;
                }
            }
            return -1;
        }

        // private string FormatFloat(float value)
        // {
        //     // Convert the float to a string using general formatting, to be able to check if it has scientific notation
        //     string valueStr = value.ToString("G");
            
        //     return FormatFloatToString(valueStr);
        // }

        private string FormatFloatToString(string valueStr)
        {
            if(valueStr.Contains("<")) // Has been formated already
                return valueStr;

            // Check if it contains 'e' for scientific notation
            if (valueStr.Contains("e") || valueStr.Contains("E"))
            {
                bool hasAlphaExcludingE = "abcdefghijklmnopqrstuvwxyz".Where(c => c != 'e').Any(c => valueStr.Contains(c)); // If true, then this is not a float
                if(hasAlphaExcludingE)
                    return valueStr;
                
                // Split into base and exponent parts
                string[] parts = valueStr.Split(new char[] { 'e', 'E' });
                string basePart = parts[0];

                // Parse the exponent part as an integer to remove leading zeros
                int exponent = int.Parse(parts[1]);

                // Check if base part is approximately 1 (for cases like 1.000000e-009)
                if (Mathf.Approximately(float.Parse(basePart), 1f))
                {
                    if(_shouldConvertExponential)
                    {
                        if(ConvertToMeasurement(exponent, out string measurementSign))
                        {
                            return $"10{measurementSign}";
                        }
                    }

                    return $"10<sup><size=28>{exponent}</size></sup>";
                }
                // Otherwise, display it as "Base^Exponent"
                else
                {
                    if(_shouldConvertExponential)
                    {
                        if(ConvertToMeasurement(exponent, out string measurementSign))
                        {
                            return $"{basePart}{measurementSign}";
                        }
                    }

                    return $"{basePart}<sup><size=28>{exponent}</size></sup>";
                }
            }
            
            // If no exponent, return the value as a regular string
            return valueStr;
        }

        private bool ConvertToMeasurement(int value, out string measurementSign)
        {
            switch(value)
            {
                case -3:
                    measurementSign = "mm";
                    return true;
                case -9:
                    measurementSign = "nm";
                    return true;
            }
            measurementSign = "";
            return false;
        }
    }
}