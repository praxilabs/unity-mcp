using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.DeviceSideMenu
{
    public class SliderControlsComponentUI : ControlsComponentUI
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _displayText;
        [SerializeField] private TextMeshProUGUI _symbolText;
        [SerializeField] private TextMeshProUGUI _sliderLabelText;
        [SerializeField] private Button _plusButton, _minusButton;
        [SerializeField] private float _incrementValue = 0.1f;
        [SerializeField] private float _decrementValue = -0.1f;
        [field: SerializeField] public bool ShouldDisplayScreenValue {get; private set;}

        private void Awake() 
        {
            _plusButton.onClick.AddListener(PlusButton); 
            _minusButton.onClick.AddListener(MinusButton); 
        }

        private void OnDestroy() 
        {
            _plusButton.onClick.RemoveListener(PlusButton); 
            _minusButton.onClick.RemoveListener(MinusButton);    
        }

        public override void SetLocalizedData(string label)
        {
            _sliderLabelText.text = label;
        }

        public override void InitializeData(ControlsComponent controlsComponent)
        {
            SliderControlsComponent sliderComponent = controlsComponent as SliderControlsComponent;

            Name = sliderComponent.Name;

            _sliderLabelText.text = sliderComponent.ComponentLabel;
            _symbolText.text = sliderComponent.Symbol;
            _slider.minValue = sliderComponent.MinValue;
            _slider.maxValue = sliderComponent.MaxValue;

            ShouldDisplayScreenValue = sliderComponent.ShouldDispayScreenValue;

            if(ShouldDisplayScreenValue) return;

            // Text display slider's value if there is no display screen
            _slider.onValueChanged.AddListener((value) => UpdateDisplayText(value.ToString())); 
        }

        public override void AddListenerToComponent(Action<object> onValueChangedCallback)
        {
            _slider.onValueChanged.AddListener((newValue) => {
                onValueChangedCallback(1 - newValue);
            });
        }

        public override void SetComponentValue(object obj)
        {
            if(!ShouldDisplayScreenValue)
            {
                float newSliderValue = (float)obj;
                _slider.SetValueWithoutNotify(newSliderValue);
                UpdateDisplayText(newSliderValue.ToString());
            }
            else if(obj is float newSliderValue)
            {
                _slider.SetValueWithoutNotify(newSliderValue);
            }
            else
            {
                SliderData sliderData = (SliderData)obj;
                if(sliderData.sliderValue != -1)
                {
                    _slider.SetValueWithoutNotify(sliderData.sliderValue);
                }
                UpdateDisplayText(sliderData.displayText);
                
                if(sliderData.symbol != "")
                {
                    UpdateSymbolText(sliderData.symbol);
                }
            }
        }

        private void PlusButton()
        {
            _slider.value += _incrementValue;
        }
        private void MinusButton()
        {
            _slider.value += _decrementValue;
        }

        private void UpdateDisplayText(string displayText)
        {
            if(float.TryParse(displayText, out float displayValue))
            {
                // In case ShouldDisplayScreenValue is false, should be deleted if we will rely only on the display screen's value
                _displayText.text = displayValue.ToString("f2");
            }
            else
            {
                _displayText.text = displayText;
            }
        }
        private void UpdateSymbolText(string value)
        {
            _symbolText.text = value;
        }

        public void SetInteractive(bool toggle)
        {
            _slider.interactable = toggle;
            _plusButton.interactable = toggle;
            _minusButton.interactable = toggle;
        }
    }

    public class SliderData
    {
        public float sliderValue; 
        public string displayText;
        public string symbol;

        public SliderData(float sliderValue, string displayText, string symbol)
        {
            this.sliderValue = sliderValue;
            this.displayText = displayText;
            this.symbol = symbol;
        }
    }
}