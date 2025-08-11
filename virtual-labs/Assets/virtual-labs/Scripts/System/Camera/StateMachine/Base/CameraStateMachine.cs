using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public enum CameraState
    {
        idle, lockRotation, lockPosition, freeze,
        birdEye, moveToDevice,
        rotate, zoom, move, reset,
        interestPoint, settingsMenu
    }

    public abstract class CameraStateMachine : StateBase
    {
        public CameraState currentStateName;
        protected CameraState _previousState;
        protected CameraCommand _cameraCommand;
        protected bool _isGoingToPrevState = false;

        public CameraStateMachine(CameraState previousState)
        {
            currentState = State.start;
            _previousState = previousState;
            CameraManager.Instance.stateRunner.executedStates.Add(previousState);
        }

        protected override void Start()
        {
            base.Start();
            AddEvents();
        }

        protected virtual void PrepareCommand() { }
        protected virtual void GoToNextState() { }

        protected override void Exit()
        {
            _isGoingToPrevState = false;
            base.Exit();
            RemoveEvents();
        }

        protected virtual void AddEvents()
        {
            CameraStateLoggingEvents cameraEvents = GameObject.FindObjectOfType<CameraStateLoggingEvents>();
            cameraEvents.AddCameraEvent(currentStateName);
        }

        protected virtual void RemoveEvents()
        {
            CameraStateLoggingEvents cameraEvents = GameObject.FindObjectOfType<CameraStateLoggingEvents>();
            cameraEvents.RemoveCameraEvent(currentStateName);
        }
    }
}