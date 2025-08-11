using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.DeviceSideMenu
{
    public class DeviceScreenReadingsComponentUI : ReadingsComponentUI
    {
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private RawImage _deviceScreen;

        private Camera _cam;

        private void Start()
        {
            SetDeviceScreen();
        }

        private void OnEnable()
        {
            if(_cam)
            {
                _cam.enabled = true;
            }
        }
        
        private void OnDisable()
        {
            if(_cam)
            {
                _cam.enabled = false;
            }
        }

        private void SetDeviceScreen()
        {
            _deviceScreen.texture = _renderTexture;
        }
    }
}