using Praxilabs.Input;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class ZoomState : CameraStateMachine
    {
        private ZoomCommand _zoomCommand;
        private float _zoomInput;

        public ZoomState(float zoomInput, CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.zoom;
            _zoomInput = zoomInput;
            PrepareCommand();
        }

        protected override void Start()
        {
            base.Start();

            //Check if position is locked or not to execute
            if (CameraManager.Instance.stateRunner.canMove)
                _zoomCommand.Execute();
        }

        protected override void PrepareCommand()
        {
            _zoomCommand = new ZoomCommand();
            _zoomCommand.zoomInput = _zoomInput;
        }

        protected override void Execute()
        {
            if (IsZoomStopped())
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

        private bool IsZoomStopped()
        {
            return MouseInputManager.zoomAction.ReadValue<Vector2>().y == 0;
        }
    }
}