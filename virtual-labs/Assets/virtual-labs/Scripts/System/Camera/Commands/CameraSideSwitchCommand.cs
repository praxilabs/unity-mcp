using Cinemachine;
using Praxilabs.Input;
using System.Collections;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class CameraSideSwitchCommand : CameraCommand
    {
        private Cinemachine3rdPersonFollow _vCam3rdPersonFollow;
        private Coroutine _lerpCoroutine;
        private float _lerpDuration = 0.5f;
        private bool _leftSide;

        private void Initialize()
        {
            _vCam3rdPersonFollow = CameraManager.Instance.currentCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            _leftSide = _vCam3rdPersonFollow.CameraSide == 1 ? true : false;

            KeyboardInputManager.switchCameraSide.performed += context => HandleSwitchCameraSide();
        }

        public override void Execute()
        {
            Initialize();
        }

        private void HandleSwitchCameraSide()
        {
            if (_leftSide)
            {
                if (_lerpCoroutine != null)
                    CameraCoroutineManager.Instance.StopCoroutine(_lerpCoroutine);
                _lerpCoroutine = CameraCoroutineManager.Instance.StartCoroutine(CameraSideLerp(1, 0, _lerpDuration));
                _leftSide = false;
            }
            else
            {
                if (_lerpCoroutine != null)
                    CameraCoroutineManager.Instance.StopCoroutine(_lerpCoroutine);
                _lerpCoroutine = CameraCoroutineManager.Instance.StartCoroutine(CameraSideLerp(0, 1, _lerpDuration));
                _leftSide = true;
            }
        }

        private IEnumerator CameraSideLerp(float startValue, float endValue, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                _vCam3rdPersonFollow.CameraSide = Mathf.Lerp(startValue, endValue, elapsedTime / duration);

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            _vCam3rdPersonFollow.CameraSide = endValue;
        }

        public override void StopExecuting()
        {
            KeyboardInputManager.switchCameraSide.performed -= context => HandleSwitchCameraSide();
        }
    }
}