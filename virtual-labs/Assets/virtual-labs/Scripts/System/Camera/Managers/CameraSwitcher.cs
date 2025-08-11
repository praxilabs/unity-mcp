using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class CameraSwitcher : MonoBehaviour
    {
        public Stack<CinemachineVirtualCamera> cameraStack = new Stack<CinemachineVirtualCamera>();
        //This is used for debugging only to view stack elements in inspector
        [SerializeField] private List<CinemachineVirtualCamera> _cameraList = new List<CinemachineVirtualCamera>();

        private bool _isFirstState = true;

        public void Refresh()
        {
            cameraStack.Clear();
            _cameraList.Clear();

            _isFirstState = true;

            Switch(CameraManager.Instance.experimentCameras.mainBenchCamera);
        }

        /// <summary>
        /// Add new camera to stack and switch to it
        /// </summary>
        /// <param name="target"></param>
        public void Switch(CinemachineVirtualCamera target)
        {
            if (cameraStack.Count > 0)
                cameraStack.Peek().Priority = 0;
            else
                _isFirstState = true;

            cameraStack.Push(target);
            _cameraList.Add(target);
            cameraStack.Peek().Priority = 1;

            UpdateCameraLookAt();

            CameraManager.Instance.currentCamera = cameraStack.Peek();
            ResetRotation(CameraManager.Instance.currentCamera);

             _isFirstState = false;
        }

        /// <summary>
        /// Remove camera from stack and switch to new peek
        /// </summary>
        public void SwitchBack()
        {
            cameraStack.Peek().Priority = 0;
            cameraStack.Pop();
            _cameraList.RemoveAt(_cameraList.Count - 1);
            cameraStack.Peek().Priority = 1;

            CameraManager.Instance.currentCamera = cameraStack.Peek();
        }

        private void ResetRotation(CinemachineVirtualCamera camera)
        {
            CinemachinePOV cameraPOV = camera.GetCinemachineComponent<CinemachinePOV>();

            if (cameraPOV == null) return;

            cameraPOV.m_VerticalAxis.Value = camera.GetComponent<CameraSettings>().verticalAxisInitialValue;
            cameraPOV.m_HorizontalAxis.Value = camera.GetComponent<CameraSettings>().horizontalAxisInitialValue;
        }

        /// <summary>
        /// Used to fix camera rotation
        /// </summary>
        private void UpdateCameraLookAt()
        {
            var prev = CameraManager.Instance.currentCamera;
            var nextCam = cameraStack.Peek();

            if (!_isFirstState)
            {
                if (PreviousStateChecker.IsPreviousBirdEye())
                {
                    prev.LookAt = nextCam.LookAt;
                }
            }
        }
    }
}