using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class ExtraExpandedUI : MonoBehaviour 
    {
        [Header("Extras Section")]
        [SerializeField] private List<DisplayFieldAreaUI> _displayFieldAreas;
        
        private const int MAX_EXTRA_COMPONENTS = 4;

        private List<DisplayFieldReadingsComponentUI> _displayFieldComponentUIs = new List<DisplayFieldReadingsComponentUI>();

        private List<Action<string, string>> _readingsUpdatedHandlers = new List<Action<string, string>>();   

        protected void ExtraInitialization(string deviceName, List<string> extraComponentNames)
        {
            int extraComponentsCount = extraComponentNames.Count;
            if(extraComponentsCount > MAX_EXTRA_COMPONENTS)
            {
                extraComponentsCount = MAX_EXTRA_COMPONENTS;
            }

            for(int i = 0; i < extraComponentsCount; i++)
            {
                int capturedIndex = i;

                ReadingsComponentUI readingsComponentUI = 
                    DeviceMenuWrapper.Instance.GetTabReadingsComponentUI(deviceName, extraComponentNames[i]);
                
                DisplayFieldReadingsComponentUI displayFieldComponentUI = readingsComponentUI as DisplayFieldReadingsComponentUI;

                void readingsUpdatedHandler(string displayText, string sign) => HandleReadingsUpdated(capturedIndex, displayText, sign);
                displayFieldComponentUI.OnReadingsUpdated += readingsUpdatedHandler;
                _readingsUpdatedHandlers.Add(readingsUpdatedHandler);

                InitializeTexts(i, displayFieldComponentUI);
                _displayFieldComponentUIs.Add(displayFieldComponentUI);
            }
        }

        private void InitializeTexts(int index, DisplayFieldReadingsComponentUI displayFieldComponentUI)
        {
            _displayFieldAreas[index].InitializeTexts(displayFieldComponentUI);
        }

        private void HandleReadingsUpdated(int index, string displayText, string displayTextSign)
        {
            _displayFieldAreas[index].HandleReadingsUpdated(displayText, displayTextSign);
        }

        protected virtual void OnDestroy()
        {
            for(int i = 0; i < _displayFieldComponentUIs.Count; i++)
            {
                _displayFieldComponentUIs[i].OnReadingsUpdated -= _readingsUpdatedHandlers[i];
            }
        }
    }
}