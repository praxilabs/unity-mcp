namespace Praxilabs.CameraSystem
{
    public class LockRotationCommand : CameraCommand
    {
        /// <summary>
        /// Turn on/off canRotate in camera manager to resume/stop camera rotation
        /// </summary>
        public override void Execute()
        {
            CameraManager.Instance.stateRunner.canRotate = !CameraManager.Instance.stateRunner.canRotate;
        }
    }
}