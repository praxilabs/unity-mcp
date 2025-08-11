using System.Collections;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class ZoomCommand : CameraCommand
    {
        public float zoomInput;

        private Transform _currentCamera;
        private CameraSettings _currentCameraSettings;
        private float _zoomSmoothness = 0.2f;
        private float _zoomInputSmoothingFactor = 0.5f;

        private Vector3 _targetPosition;
        private Coroutine _zoomCoroutine;

        public override void Execute()
        {
            GetCamera();
            Zoom();
        }

        private void GetCamera()
        {
            _currentCamera = CameraManager.Instance.currentCamera.transform;
            _currentCameraSettings = _currentCamera.GetComponent<CameraSettings>();
        }
        private void Zoom()
        {
            if (Mathf.Abs(zoomInput) < 0.01f) return; // Ignore negligible input

            // Smooth the zoom input for mouse wheel
            float smoothedZoomInput = Mathf.Lerp(0, Mathf.Clamp(zoomInput, -1f, 1f), _zoomInputSmoothingFactor);

            // Determine the forward direction of the camera
            Vector3 cameraForward = Camera.main.transform.forward;

            // Project the forward vector onto the horizontal plane
            cameraForward.y = 0;
            cameraForward.Normalize();

            // Calculate movement direction based on zoom input
            Vector3 moveDirection = cameraForward * smoothedZoomInput * _currentCameraSettings.zoomSpeed * Time.deltaTime;
            Vector3 potentialNewPosition = _currentCamera.localPosition + moveDirection;

            // Calculate the distance moved in the forward direction
            float distanceMoved = Vector3.Dot(potentialNewPosition - _currentCameraSettings.initialPosition, cameraForward);

            // Clamp the movement within the zoom range
            distanceMoved = Mathf.Clamp(distanceMoved, _currentCameraSettings.maxBackwardDistance, _currentCameraSettings.maxForwardDistance);

            // Set the target position based on the clamped distance
            _targetPosition = _currentCameraSettings.initialPosition + cameraForward * distanceMoved;

            // Stop any existing zoom coroutine and start a new one
            if (_zoomCoroutine != null)
                CameraCoroutineManager.Instance.StopCoroutine(_zoomCoroutine);

            _zoomCoroutine = CameraCoroutineManager.Instance.StartCoroutine(SmoothZoomToTarget());
        }

        private IEnumerator SmoothZoomToTarget()
        {
            while (Vector3.Distance(_currentCamera.localPosition, _targetPosition) > 0.01f)
            {
                _currentCamera.localPosition = Vector3.MoveTowards(
                    _currentCamera.localPosition,
                    _targetPosition,
                    _zoomSmoothness * Time.deltaTime // consistent speed, frame-independent
                );
                yield return null;
            }

            // Snap to the target position to avoid tiny floating-point differences
            _currentCamera.localPosition = _targetPosition;
        }
    }
}