using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class KnobUI : MonoBehaviour
    {
        public EnhancedKnobController Knob => enhancedKnobController;
        [SerializeField] private EnhancedKnobController enhancedKnobController;
        // [SerializeField] private DisplayScreen _displayScreen;

        public void AddListenerToOutput(Action<string> HandleOutputChanged)
        {
            //_displayScreen.OnDisplayValueChanged += (displayTxt) => HandleOutputChanged(displayTxt);
            // enhancedKnobController.OnKnobRotation += (value) => {HandleOutputChanged(float.Parse(displayText.text));};
        }

        public string GetInitialDisplayScreenValue()
        {
            return "_displayScreen.DisplayScreenText.text;";
        }        
    }
}
