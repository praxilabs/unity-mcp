namespace Praxilabs.CameraSystem
{
    public class ResetState : CameraStateMachine
    {
        public ResetState(CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.reset;
        }

        protected override void Start()
        {
            base.Start();
            PrepareCommand();
        }

        protected override void PrepareCommand()
        {
            _cameraCommand = new ResetCommand();
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