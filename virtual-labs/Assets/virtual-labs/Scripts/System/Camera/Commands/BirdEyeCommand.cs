namespace Praxilabs.CameraSystem
{
    public class BirdEyeCommand : CameraCommand
    {
        public override void Execute()
        {
            CameraManager.Instance.cameraInstance.SwitchActiveCamera(CameraManager.Instance.birdEyeCamera);
        }

        public override void StopExecuting()
        {
            CameraManager.Instance.cameraInstance.SwitchBack();
        }
    }
}