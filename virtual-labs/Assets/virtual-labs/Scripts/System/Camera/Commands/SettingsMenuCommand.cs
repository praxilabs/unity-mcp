namespace Praxilabs.CameraSystem
{
    public class SettingsMenuCommand : CameraCommand
    {
        public override void Execute()
        {
            CameraManager.Instance.cameraInstance.SwitchActiveCamera(CameraManager.Instance.experimentCameras.settingsMenuCamera);
        }

        public override void StopExecuting()
        {
            CameraManager.Instance.cameraInstance.SwitchBack();
        }

        public void SwitchToMainBench()
        {
            CameraManager.Instance.cameraInstance.SwitchActiveCamera(CameraManager.Instance.experimentCameras.mainBenchCamera);
        }
    }
}