using Cinemachine;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class MoveToDeviceCommand : CameraCommand
    {
        public DeviceSide deviceSide;
        public CinemachineVirtualCamera deviceCamera;
        private DeviceSideController _deviceSideController;

        /// <summary>
        /// Check if interestPoint is null to switch to it's child camera
        /// </summary>
        public override void Execute()
        {
            if (!HasCinemachine()) return;
            ResolveObjects();

            CameraManager.Instance.cameraInstance.SwitchActiveCamera(deviceCamera);
            _deviceSideController.SetActiveSide(deviceSide);

            deviceSide.ToggleCameraIcon(false);
            _deviceSideController.ToggleDeviceSide(true);

            CursorStatesManager.Instance.ToggleCursorHandlers(false, _deviceSideController);
        }

        public override void StopExecuting()
        {
            _deviceSideController.ToggleDeviceSide(false);
            CameraManager.Instance.cameraInstance.SwitchBack();
            CursorStatesManager.Instance.ToggleCursorHandlers(true);
        }

        private bool HasCinemachine()
        {
            deviceCamera = deviceSide._sideCamera;
            return deviceCamera;
        }

        private void ResolveObjects()
        {
            _deviceSideController = deviceSide.GetComponentInParent<DeviceSideController>();
        }
    }
}