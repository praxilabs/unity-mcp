using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class InterestPointState : CameraStateMachine
    {
        private FirstPersonUIHolder _firstPersonUIHolder;
        private BirdEyeUIHolder _thirdPersonUIHolder;
        private CameraFirstPersonUI _cameraFirstPersonUI;

        private InterestPointCommand _interestPointCommand;

        public InterestPointState(CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.interestPoint;
        }

        protected override void Start()
        {
            ResolveObjects();
            base.Start();

            _thirdPersonUIHolder.ToggleBirdEyeUI(false);
            _firstPersonUIHolder.ToggleFirstPersonUI(true);
            _firstPersonUIHolder.ToggleSideMenu(true);

            PrepareCommand();
        }

        protected override void PrepareCommand()
        {
            _interestPointCommand = new InterestPointCommand();
            _interestPointCommand.interestPointCamera = CameraManager.Instance.currentCamera;

            if (_previousState is CameraState.idle or CameraState.birdEye or CameraState.moveToDevice)
                _interestPointCommand.Execute();
        }

        protected override void GoToNextState()
        {
            _isGoingToPrevState = true;

            nextState = NextStateHelper.GoToBirdEye(currentStateName);
            
            currentState = State.exit;
        }

        /// <summary>
        /// _isGoingToPrevState is made to not remove interestpoint camera from the stack in case of going to thirdperson state
        /// because if we remove it when we exist this state, then the next state after thirdperson will be idle or birdeye not
        /// interest point.
        /// </summary>
        protected override void Exit()
        {
            if (_isGoingToPrevState)
                _interestPointCommand.StopExecuting();
            base.Exit();
        }

        /// <summary>
        /// register events to switch to other states
        /// </summary>
        protected override void AddEvents()
        {
            base.AddEvents();
            CameraStateEvents.AddMouseEvents(this);
            CameraStateEvents.AddUIEvents(this);


            if (_cameraFirstPersonUI.goBackButton is not null && _cameraFirstPersonUI.goBackButton.gameObject.activeInHierarchy)
                _cameraFirstPersonUI.goBackButton.onClick.AddListener(() => GoBackToIdle());
        }

        /// <summary>
        /// unregister events
        /// </summary>
        protected override void RemoveEvents()
        {
            base.RemoveEvents();
            CameraStateEvents.RemoveMouseEvents(this);
            CameraStateEvents.RemoveUIEvents(this);
        }

        public void GoBackToIdle()
        {
            _isGoingToPrevState = true;

            _cameraFirstPersonUI.goBackButton.gameObject.SetActive(false);
            _cameraFirstPersonUI.interestPointButton.gameObject.SetActive(true);

            nextState = NextStateHelper.GoToIdle(currentStateName);
            currentState = State.exit;
            _cameraFirstPersonUI.goBackButton.onClick.RemoveListener(() => GoBackToIdle());
        }

        private void ResolveObjects()
        {
            _firstPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<FirstPersonUIHolder>();
            _thirdPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<BirdEyeUIHolder>();
            _cameraFirstPersonUI = ExperimentItemsContainer.Instance.Resolve<CameraFirstPersonUI>();
        }
    }
}