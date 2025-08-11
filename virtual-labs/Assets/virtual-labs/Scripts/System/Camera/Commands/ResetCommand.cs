using Cinemachine;
using Praxilabs.Input;
using System.Collections;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class ResetCommand : CameraCommand
    {
        private Transform _currentCamera;
        private CinemachinePOV _povCam;
        private Coroutine _resetCoroutine;

        private bool _inputBlocked = false;

        public override void Execute()
        {
            GetCamera();
            StartReset();
        }

        private void GetCamera()
        {
            _currentCamera = CameraManager.Instance.currentCamera.transform;
            _povCam = _currentCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();
        }

        private void StartReset()
        {
            CameraCoroutineManager.Instance.StopAllCoroutines();

            CameraManager.Instance.eventSystem.enabled = false;

            _resetCoroutine = CameraCoroutineManager.Instance.StartCoroutine(SmoothResetPositionAndRotation());
        }
        private IEnumerator SmoothResetPositionAndRotation()
        {
            float resetDuration = 0.5f; // Total duration for the reset
            float elapsedTime = 0f;

            Vector3 initialPosition = _currentCamera.localPosition;
            float initialHorizontalAxisValue = _povCam.m_HorizontalAxis.Value;

            while (elapsedTime < resetDuration)
            {
                if (IsInputDetected())
                {
                    CameraManager.Instance.eventSystem.enabled = true;
                    yield break;
                }

                float t = elapsedTime / resetDuration;

                _currentCamera.localPosition = Vector3.Lerp(initialPosition, _currentCamera.GetComponent<CameraSettings>().initialPosition, t);
                _povCam.m_HorizontalAxis.Value = Mathf.Lerp(initialHorizontalAxisValue, _currentCamera.GetComponent<CameraSettings>().horizontalAxisInitialValue, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _currentCamera.localPosition = _currentCamera.GetComponent<CameraSettings>().initialPosition;
            _povCam.m_HorizontalAxis.Value = _currentCamera.GetComponent<CameraSettings>().horizontalAxisInitialValue;

            CameraManager.Instance.eventSystem.enabled = true;
        }

        private bool IsInputDetected()
        {
            // Check if mouse is clicked or UI interaction is happening
            if (MouseInputManager.rightPressAction.WasPressedThisFrame()
                || MouseInputManager.mouseScrollWheel.WasPressedThisFrame())
            {
                return true;
            }
            return false;
        }
    }
}