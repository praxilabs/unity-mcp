using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class ConnectKnobToSliderComponent : MonoBehaviour
    {
        [SerializeField] private KnobUI knobUI;
        [SerializeField] private SliderControlsComponentUI sliderComponentUI;

        private void Start()
        {
            sliderComponentUI.AddListenerToComponent(HandleSliderComponentValueChanged); // Updates Knob Value

            ListenOnKnobValueChanged(); // Updates Slider UI

            if (sliderComponentUI.ShouldDisplayScreenValue)
            {
                knobUI.AddListenerToOutput(UpdateDisplayText); // Updates Display Text UI by Display Value
                UpdateDisplayText(knobUI.GetInitialDisplayScreenValue()); // Sets display text's initial value
            }

            // Sets slider's initial value
            sliderComponentUI.SetComponentValue(knobUI.Knob.RotationPercentage);
        }

        private void HandleSliderComponentValueChanged(object obj)
        {
            float newValue = (float)obj;
            knobUI.Knob.UpdateRotationPercentage(newValue);
        }

        private void ListenOnKnobValueChanged()
        {
            knobUI.Knob.OnKnobRotation += (value) => sliderComponentUI.SetComponentValue(value);
        }

        private void UpdateDisplayText(string displayText)
        {
            // -1 in order not to change the value of the slider value
            SliderData pair = new SliderData(-1, displayText, "");
            sliderComponentUI.SetComponentValue(pair);
        }

        public void SetSliderInteraction(bool toggle)
        {
            knobUI.Knob.SetKnobInteractability(toggle);
            sliderComponentUI.SetInteractive(toggle);
        }
    }
}
