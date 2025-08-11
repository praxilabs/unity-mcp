namespace Praxilabs.CameraSystem
{
    public class LockRotaionState : CameraStateMachine
    {
        public LockRotaionState(CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.lockRotation;
        }

        protected override void Start()
        {
            base.Start();
            PrepareCommand();
        }

        protected override void PrepareCommand()
        {
            _cameraCommand = new LockRotationCommand();
        }

        protected override void Execute()
        {
            _cameraCommand.Execute();

            GoToNextState();
        }

        protected override void GoToNextState()
        {
            if (PreviousStateChecker.IsPreviousIdle())
                nextState = NextStateHelper.GoToIdle(currentStateName);

            else if (PreviousStateChecker.IsPreviousInterestPoint())
                nextState = NextStateHelper.GoToInterestPoint(currentStateName);
            currentState = State.exit;
        }

        protected override void Exit()
        {
            base.Exit();
        }
    }
}