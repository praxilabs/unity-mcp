using System;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.DeviceSideMenu
{
    public class CameraViewReadingsComponentUI : ExtraReadingsComponentUI
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private RawImage _cameraView;
        [SerializeField] private Button _expandViewBtn;
        [SerializeField] private GameObject _expandedWindowPrefab;

        private Vector2 _normalRenderTextureDimension = new Vector2(256, 256);
        private Vector2 _expandedRenderTextureDimension = new Vector2(600, 307);

        private CameraViewExpandedUI _expandedWindowUI;

        private DeviceMenu _deviceMenu;

        private void Awake()
        {
            _expandViewBtn.onClick.AddListener(ShowExpandedWindow);
        }

        private void Start()
        {
            _deviceMenu = GetComponentInParent<DeviceMenu>();
            _deviceMenu.OnVisibilityChanged += HandleCameraVisibility;
            SetCameraView();
        }

        private void OnEnable()
        {
            UpdateCameraVisibility(true);
        }

        private void OnDisable()
        {
            UpdateCameraVisibility(false);
        }

        private void HandleCameraVisibility(bool isVisible)
        {
            if(IsVisible())
                UpdateCameraVisibility(isVisible);
        }

        public override void InitializeData(ReadingsComponent readingsComponent)
        {
            base.InitializeData(readingsComponent);

            CameraViewReadingsComponent cameraViewComponent = readingsComponent as CameraViewReadingsComponent;
            
            extraComponentNames = cameraViewComponent.ExtraComponentNames;
        }

        private void SetCameraView()
        {
            _cameraView.texture = _renderTexture;
            
            // In case we decided to change the camera view during runtime for special cases
            if(_expandedWindowUI)
            {
                _expandedWindowUI.SetCameraView(_renderTexture);
            }
        }

        private void ShowExpandedWindow()
        {            
            if(_expandedWindowUI is null)
            {
                _expandedWindowUI = CreateExpandedWindow();
                _expandedWindowUI.Hide();
            }

            Hide();

            UpdateRenderTextureDimension(true);

            _expandedWindowUI.Show();

            UpdateCameraVisibility(true);
        }

        private CameraViewExpandedUI CreateExpandedWindow()
        {
            Transform sideMenusParentTransform = _deviceMenu.transform.parent.parent;
            ExpandedFactory cameraViewExpandedFactory;

            cameraViewExpandedFactory = new ExpandedFactory(_expandedWindowPrefab, sideMenusParentTransform);
                
            return cameraViewExpandedFactory.CreateExpandedWindow(_deviceMenu, _cameraView.texture, DeviceName, extraComponentNames, OnExpandedWindowHidden);
        }

        private void OnExpandedWindowHidden()
        {
            UpdateRenderTextureDimension(false);
            Show();
            if(!IsVisible())
            {
                UpdateCameraVisibility(false);
            }
        }

        private void UpdateRenderTextureDimension(bool isExpanded)
        {
            if(_renderTexture == null) return;
            
            _renderTexture.Release();
            if(isExpanded)
            {
                _renderTexture.width = (int)_expandedRenderTextureDimension.x;
                _renderTexture.height = (int)_expandedRenderTextureDimension.y;
            }
            else
            {
                _renderTexture.width = (int)_normalRenderTextureDimension.x;
                _renderTexture.height = (int)_normalRenderTextureDimension.y;
            }
            _renderTexture.Create();

            UpdateCameraRender();
        }

        private void UpdateCameraVisibility(bool isVisible)
        {
            if(_cam == null) return;

            if(isVisible)
            {
                _cam.enabled = true;
            }
            else
            {
                if(_expandedWindowUI == null)
                {
                    _cam.enabled = false;
                }
                else if(!_expandedWindowUI.IsVisible())
                {
                    _cam.enabled = false;
                }
            }
        }

        private void UpdateCameraRender()
        {
            if(_cam)
                _cam.Render();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private bool IsVisible()
        {
            return gameObject.activeInHierarchy;
        }

        private void OnDestroy() 
        {
            _expandViewBtn.onClick.RemoveAllListeners();
            UpdateRenderTextureDimension(false);
        }
    }
}