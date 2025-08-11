using Praxilabs.Input;

namespace Praxilabs.CameraSystem
{
    public class RotateState : CameraStateMachine
    {
        private RotateCommand _rotateCommand;

        public RotateState(CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.rotate;

            PrepareCommand();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void PrepareCommand()
        {
            _rotateCommand = new RotateCommand();
            _rotateCommand.camera = CameraManager.Instance.currentCamera;
            _rotateCommand.Refresh();
        }

        protected override void Execute()
        {
            _rotateCommand.Execute();

            if (IsRotationStopped())
                GoToNextState();
        }

        protected override void GoToNextState()
        {
            if (PreviousStateChecker.IsPreviousIdle())
                nextState = NextStateHelper.GoToIdle(currentStateName);

            else if (PreviousStateChecker.IsPreviousBirdEye())
                nextState = NextStateHelper.GoToBirdEye(currentStateName);

            else if (PreviousStateChecker.IsPreviousInterestPoint())
                nextState = NextStateHelper.GoToInterestPoint(currentStateName);

            currentState = State.exit;
        }

        protected override void Exit()
        {
            _rotateCommand.StopExecuting();
            base.Exit();
        }

        private bool IsRotationStopped()
        {
            return MouseInputManager.rightPressAction.WasReleasedThisFrame();
        }
    }
}