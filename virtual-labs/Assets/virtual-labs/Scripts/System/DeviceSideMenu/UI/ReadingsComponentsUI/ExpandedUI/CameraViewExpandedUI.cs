using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.DeviceSideMenu
{
    public class CameraViewExpandedUI : ExtraExpandedUI 
    {
        [SerializeField] private RawImage _cameraView;
        [SerializeField] private Button _hideBtn;
        [SerializeField] private IconRotator _pinIconRotator;
        private Action _onHidingExpandedWindowCallback;
        private bool _isPinned;
        private DeviceMenu _deviceMenu;
        private RegularMenuUI _regularMenuUI;

        private void Start()
        {
            _regularMenuUI = GetComponent<RegularMenuUI>();
            _regularMenuUI.OnPinnedChanged.AddListener(HandlePinnedChanged);
        }

        public void Setup(DeviceMenu deviceMenu, Texture texture, string deviceName, List<string> extraComponentNames, Action onHidingExpandedWindowCallback)
        {
            RenderTexture renderTexture = texture as RenderTexture;
            SetCameraView(renderTexture);

            if(extraComponentNames != null && extraComponentNames.Count > 0)
            {
                ExtraInitialization(deviceName, extraComponentNames);
            }

            _onHidingExpandedWindowCallback = onHidingExpandedWindowCallback;
            
            _hideBtn.onClick.AddListener(Hide);
            
            _deviceMenu = deviceMenu;
            _deviceMenu.OnTryingVisibilityChange += HandleTryingVisibilityChange;
        }

        public void SetCameraView(RenderTexture renderTexture)
        {
            _cameraView.texture = renderTexture;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            SetPin(false);
            gameObject.SetActive(false);
            _onHidingExpandedWindowCallback?.Invoke();
        }

        public bool IsVisible()
        {
            return gameObject.activeInHierarchy;
        }

        private void HandlePinnedChanged(bool isPinned)
        {
            SetPin(isPinned);
        }

        private void SetPin(bool isVisible)
        {
            _isPinned = isVisible;
            _pinIconRotator.RotateIcon(isVisible);
        }

        private void HandleTryingVisibilityChange(bool isVisible)
        {
            if(!isVisible & !_isPinned)
            {
                Hide();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _deviceMenu.OnTryingVisibilityChange -= HandleTryingVisibilityChange;
            _regularMenuUI.OnPinnedChanged.RemoveListener(HandlePinnedChanged);
        }
    }
}
