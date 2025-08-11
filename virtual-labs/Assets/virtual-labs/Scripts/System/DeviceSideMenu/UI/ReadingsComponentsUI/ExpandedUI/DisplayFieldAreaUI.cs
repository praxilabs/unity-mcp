using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DisplayFieldAreaUI : MonoBehaviour 
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _displayText;
        [SerializeField] private TextMeshProUGUI _displayTextSign;

        public void InitializeTexts(DisplayFieldReadingsComponentUI displayFieldComponentUI)
        {
            Show();
            
            (string displayFieldLabel, string displayText, string displayTextSign) = displayFieldComponentUI.GetDisplayFieldTexts();
            
            _label.text = displayFieldLabel;
            _displayText.text = displayText;
            _displayTextSign.text = displayTextSign;
        }

        public void HandleReadingsUpdated(string displayText, string displayTextSign)
        {
            _displayText.text = float.Parse(displayText).ToString("F1");;
            _displayTextSign.text = displayTextSign;
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }
    }
}