using Praxilabs.Input;

namespace Praxilabs.CameraSystem
{
    public class FreezeState : CameraStateMachine
    {
        public FreezeState(CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.freeze;
        }

        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// press freeze input action to unfreeze and go back to idle state
        /// </summary>
        protected override void Execute()
        {
            if (KeyboardInputManager.freeze.WasPressedThisFrame())
            {
                if (PreviousStateChecker.IsPreviousIdle())
                    nextState = NextStateHelper.GoToIdle(currentStateName);

                else if (PreviousStateChecker.IsPreviousInterestPoint())
                    nextState = NextStateHelper.GoToInterestPoint(currentStateName);

                currentState = State.exit;
            }
        }

        protected override void Exit()
        {
            base.Exit();
        }
    }
}