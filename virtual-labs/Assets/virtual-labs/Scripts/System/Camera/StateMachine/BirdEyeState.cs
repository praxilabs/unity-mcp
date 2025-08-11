using Praxilabs.Input;
using UnityEngine.InputSystem;

namespace Praxilabs.CameraSystem
{
    public class BirdEyeState : CameraStateMachine
    {
        public static event System.Action OnEnterBirdEyeState;
        private static System.Action<InputAction.CallbackContext> _leftPressActionDelegate;
        private static System.Action<InputAction.CallbackContext> _rightPressActionDelegate;
        private FirstPersonUIHolder _firstPersonUIHolder;
        private BirdEyeUIHolder _thirdPersonUIHolder;

        public BirdEyeState(CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.birdEye;
        }

        protected override void Start()
        {
            ResolveObjects();
            base.Start();
            PrepareCommand();

            if (_previousState != CameraState.rotate)
            {
                _firstPersonUIHolder.ToggleFirstPersonUI(false);
                _firstPersonUIHolder.ToggleSideMenu(false);
                _thirdPersonUIHolder.ToggleBirdEyeUI(true);
            }
            CameraManager.Instance.cameraInstance.InterestPointsSetActive(true);
            OnEnterBirdEyeState?.Invoke();
        }

        protected override void PrepareCommand()
        {
            _cameraCommand = new BirdEyeCommand();

            if (_previousState is not CameraState.rotate)
                _cameraCommand.Execute();
        }

        private void ReturnToMainBench()
        {
            CameraManager.Instance.cameraSwitcher.Refresh();
            nextState = NextStateHelper.GoToIdle(currentStateName);
            currentState = State.exit;
        }

        /// <summary>
        /// Remove bird eye camera from cameras stack and go to previous camera
        /// </summary>
        protected override void Exit()
        {
            if (_isGoingToPrevState)
                _cameraCommand.StopExecuting();

            CameraManager.Instance.cameraInstance.InterestPointsSetActive(false);
            base.Exit();
        }

        protected override void AddEvents()
        {
            _thirdPersonUIHolder.firstPersonPOV.onClick.AddListener(ReturnToMainBench);
            base.AddEvents();
            _leftPressActionDelegate = context => CameraStateUtilities.HandleGoToInterestPoint(this, CameraManager.Instance.birdEyeCamera);
            _rightPressActionDelegate = context => NextStateHelper.GoToRotation(this);

            MouseInputManager.leftPressAction.performed += _leftPressActionDelegate;
            MouseInputManager.rightPressAction.performed += _rightPressActionDelegate;
        }

        protected override void RemoveEvents()
        {
            _thirdPersonUIHolder.firstPersonPOV.onClick.RemoveListener(ReturnToMainBench);
            base.RemoveEvents();

            MouseInputManager.leftPressAction.performed -= _leftPressActionDelegate;
            MouseInputManager.rightPressAction.performed -= _rightPressActionDelegate;
        }

        private void ResolveObjects()
        {
            _firstPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<FirstPersonUIHolder>();
            _thirdPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<BirdEyeUIHolder>();
        }
    }
}