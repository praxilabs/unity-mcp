using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Praxilabs.CameraSystem
{
    /// <summary>Holds reference for important camera scripts that is used by other classes </summary>
    public class CameraManager : Singleton<CameraManager>
    {
        public CameraInstance cameraInstance;
        public CameraSwitcher cameraSwitcher;
        public CameraStateRunner stateRunner;
        public EventSystem eventSystem;

        public CinemachineVirtualCamera birdEyeCamera;
        public CinemachineVirtualCamera currentCamera;
        [HideInInspector] public CinemachineVirtualCamera tempCurrentCamera;

        private CameraFirstPersonUI _cameraFirstPersonUI;

        private ExperimentCameras _experimentCameras;
        public ExperimentCameras experimentCameras
        {
            get
            {
                if (_experimentCameras == null)
                    _experimentCameras = ExperimentItemsContainer.Instance.Resolve<ExperimentCameras>();

                return _experimentCameras;
            }
        }

        private void Start()
        {
            ResolveObjects();
        }

        public void ExitCurrentState()
        {
            stateRunner.currentState.canExitState = true;
        }

        public void FreezeAll(bool enable)
        {
            stateRunner.canMove = !enable;
            stateRunner.canRotate = !enable;
            stateRunner.canReset = !enable;
        }

        public void FreezeCameraMove(bool enable)
        {
            stateRunner.canMove = !enable;
            stateRunner.canReset = !enable;
        }

        public void FreezeCameraRotate(bool enable)
        {
            stateRunner.canRotate = !enable;
        }

        public void ExitDeviceCameraState(CameraState cameraState)
        {
            if (stateRunner.currentState.currentStateName == CameraState.moveToDevice)
                ExitCurrentState();
        }

        public void GoToBirdEye()
        {
            _cameraFirstPersonUI.thirdPersonPOV.onClick.Invoke();
        }

        
        private void ResolveObjects()
        {
            _cameraFirstPersonUI = ExperimentItemsContainer.Instance.Resolve<CameraFirstPersonUI>();
        }
    }
}