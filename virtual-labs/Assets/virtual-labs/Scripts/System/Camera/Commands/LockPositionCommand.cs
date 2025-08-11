namespace Praxilabs.CameraSystem
{
    public class LockPositionCommand : CameraCommand
    {
        /// <summary>
        /// Turn on/off canMove in camera manager to resume/stop camera movement
        /// </summary>
        public override void Execute()
        {
            CameraManager.Instance.stateRunner.canMove = !CameraManager.Instance.stateRunner.canMove;
        }
    }
}