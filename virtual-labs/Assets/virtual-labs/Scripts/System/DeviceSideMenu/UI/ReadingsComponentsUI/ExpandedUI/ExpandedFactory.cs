using System;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class ExpandedFactory
    {
        private GameObject _expandedWindowPrefab;
        private Transform _parentTransform;
        public ExpandedFactory(GameObject expandedWindowPrefab, Transform parentTransform)
        {
            _expandedWindowPrefab = expandedWindowPrefab;
            _parentTransform = parentTransform;
        }

        public CameraViewExpandedUI CreateExpandedWindow(DeviceMenu deviceMenu, Texture texture, string deviceName, List<string> extraComponentNames, Action onHidingExpandedWindowCallback)
        {
            GameObject expandedWindowObject = GameObject.Instantiate(_expandedWindowPrefab, _parentTransform);
            expandedWindowObject.name = deviceName + "_CameraViewExpandedWindowCanvas";
            CameraViewExpandedUI expandedWindowUI = expandedWindowObject.GetComponentInChildren<CameraViewExpandedUI>();
            expandedWindowUI.Setup(deviceMenu, texture, deviceName, extraComponentNames, onHidingExpandedWindowCallback);

            return expandedWindowUI;
        }
    }
}