using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimmEffect
{
    public class DimmEffectManager : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private UnityEngine.UI.RawImage _rawImage;

        [SerializeField] private Camera _dimmCamera;
        [SerializeField] private GameObject _dimmOverlayObject;
        [SerializeField] private Color _dimmColor = new(0, 0, 0, 0.1f);
        [SerializeField] private Vector2Int _screenResolution = Vector2Int.zero;

        private RenderTexture _renderTexture;
        private Dictionary<GameObject, int> _beforeActivationLayers;

        private bool _setupDone = false;
        [ContextMenu("Setup")]
        public void Setup()
        {
            if (_setupDone)
                return;

            _setupDone = true;

            if (_screenResolution == Vector2Int.zero)
                _screenResolution = new Vector2Int(Screen.width, Screen.height);
          
#if UNITY_EDITOR
            _screenResolution = Vector2Int.RoundToInt( GetMainGameViewSize());
#endif

            _renderTexture = new RenderTexture(
                _screenResolution.x,
                _screenResolution.y,
                (int)UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt);

            #region Camera
            _dimmCamera.backgroundColor = _dimmColor;
            _dimmCamera.clearFlags = CameraClearFlags.SolidColor;
            _dimmCamera.targetTexture = _renderTexture;
            _dimmCamera.cullingMask = _layerMask;
            
            #endregion
            _rawImage.texture = _renderTexture;

            _beforeActivationLayers = new Dictionary<GameObject, int>();

        }
#if UNITY_EDITOR
        private static Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            print($"Type of Res {Res.GetType()}");
            return (Vector2)Res;
        }
#endif
        public void AddToFocus(GameObject gameObject)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                AddToFocus(gameObject.transform.GetChild(i).gameObject);
            }

            if (_beforeActivationLayers.ContainsKey(gameObject))
                return;

            _beforeActivationLayers.Add(gameObject, gameObject.layer);
            gameObject.layer = (int)Mathf.Log(_layerMask.value, 2);
        }
        public void RemoveFromFocus(GameObject gameObject)
        {
            if (!_beforeActivationLayers.ContainsKey(gameObject))
                return;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                RemoveFromFocus(gameObject.transform.GetChild(i).gameObject);
            }

            gameObject.layer = _beforeActivationLayers[gameObject];
            _beforeActivationLayers.Remove(gameObject);
        }
        [ContextMenu("Activate")]
        public void Activate()
        {
            _dimmCamera.gameObject.SetActive(true);
            CanvasResize();
            _dimmOverlayObject.SetActive(true);
        }

        [ContextMenu("DeActivate")]
        public void DeActivate()
        {
            _dimmCamera.gameObject.SetActive(false);
            _dimmOverlayObject.SetActive(false);
        }

        public void UpdateDimmCameraFOV(CinemachineVirtualCamera vCam)
        {
            _dimmCamera.fieldOfView = vCam.m_Lens.FieldOfView;
        }
        [SerializeField]
        private UnityEngine.UI.CanvasScaler _canvasScaler;
        private void CanvasResize()
        {
            Debug.Log($"Dimmer canvas resize, screen size ({Screen.width}x{Screen.height})");
            _canvasScaler.referenceResolution = new(Screen.width, Screen.height);
            Debug.Log($"Dimmer canvas modified canvas resolution, ({_canvasScaler.referenceResolution})");
            //_canvasScaler.matchWidthOrHeight = Mathf.Abs(Screen.width - _canvasScaler.referenceResolution.x) > Mathf.Abs(Screen.height - _canvasScaler.referenceResolution.y) ? 0 : 1;
        }
    }
}