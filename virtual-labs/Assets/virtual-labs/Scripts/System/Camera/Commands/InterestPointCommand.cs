using Cinemachine;

namespace Praxilabs.CameraSystem
{
    public class InterestPointCommand : CameraCommand
    {
        public CinemachineVirtualCamera interestPointCamera;

        /// <summary>
        /// Check if interestPoint is null to switch to it's child camera
        /// </summary>
        public override void Execute()
        {
            if (interestPointCamera == null)
                return;

            CameraManager.Instance.cameraInstance.SwitchActiveCamera(interestPointCamera);
        }

        public override void StopExecuting()
        {
            CameraManager.Instance.cameraInstance.SwitchBack();
        }
    }
}