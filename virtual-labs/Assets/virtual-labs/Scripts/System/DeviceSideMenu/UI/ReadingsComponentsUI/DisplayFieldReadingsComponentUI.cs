using System;
using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DisplayFieldReadingsComponentUI : ReadingsComponentUI
    {
        public event Action<string, string> OnReadingsUpdated;
        
        [SerializeField] private float _displayValue;
        [SerializeField] private string _displaySign;
        [SerializeField] private TextMeshProUGUI _displayFieldLabel;
        [SerializeField] private TextMeshProUGUI _displayText;
        [SerializeField] private TextMeshProUGUI _displayTextSign;

        public string DisplayFieldLabel => _displayFieldLabel.text;
        public string DisplayText => _displayText.text;
        public string DisplayTextSign => _displayTextSign.text;

        private void Start()
        {
            UpdateDisplayText(_displayValue);
            _displayTextSign.text = _displaySign;
            OnReadingsUpdated?.Invoke(_displayText.text, _displayTextSign.text);
        }

        public override void InitializeData(ReadingsComponent readingsComponent)
        {
            base.InitializeData(readingsComponent);

            DisplayFieldReadingsComponent displayFieldReadingsComponent = readingsComponent as DisplayFieldReadingsComponent;

            _displayFieldLabel.text = displayFieldReadingsComponent.Label;
            _displayTextSign.text = displayFieldReadingsComponent.DisplayTextSign;
        }

        public override void SetComponentValue(object obj)
        {
            float newValue = (float)obj;
            UpdateDisplayText(newValue);
            OnReadingsUpdated?.Invoke(_displayText.text, _displayTextSign.text);
        }

        public (string, string, string) GetDisplayFieldTexts()
        {
            return (_displayFieldLabel.text, _displayText.text, _displayTextSign.text);
        }

        private void UpdateDisplayText(float value)
        {
            _displayText.text = value.ToString("F1");
        }
    }
}