using Cinemachine;
using Praxilabs.Input;
using UltimateClean;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Praxilabs.CameraSystem
{
    public class IdleState : CameraStateMachine
    {
        public static event System.Action OnEnterIdleState;
        private System.Action<InputAction.CallbackContext> _birdEyeDelegate;
        private System.Action<InputAction.CallbackContext> _settingsMenuDelegate;
        private UnityAction _leftInterestPointClickDelegate;
        private UnityAction _rightInterestPointClickDelegate;

        private FirstPersonUIHolder _firstPersonUIHolder;
        private BirdEyeUIHolder _thirdPersonUIHolder;
        private CameraFirstPersonUI _cameraFirstPersonUI;
        private InterestPointCommand _interestPointCommand;

        private bool _ignoreGoBack;

        public IdleState(CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.idle;
        }

        protected override void Start()
        {
            ResolveObjects();
            base.Start();

            _thirdPersonUIHolder.ToggleBirdEyeUI(false);
            _firstPersonUIHolder.ToggleFirstPersonUI(true);
            _firstPersonUIHolder.ToggleSideMenu(true);
            _cameraFirstPersonUI.CheckNearbyInterestPoints();

            PrepareCommand();
            OnEnterIdleState?.Invoke();
        }

        protected override void PrepareCommand()
        {
            _interestPointCommand = new InterestPointCommand();
            _interestPointCommand.interestPointCamera = CameraManager.Instance.experimentCameras.mainBenchCamera;

            if (_previousState is CameraState.idle or CameraState.birdEye or CameraState.moveToDevice)
                _interestPointCommand.Execute();
        }

        protected override void Exit()
        {
            base.Exit();
            if (nextState is not RotateState && nextState is not ZoomState && nextState is not InterestPointState && nextState is not ResetState)
                _cameraFirstPersonUI.DisableInterestPointsButons();
        }

        /// <summary>
        /// register events to witch to other states
        /// </summary>
        protected override void AddEvents()
        {
            base.AddEvents();
            CameraStateEvents.PrepareEvents();
            CameraStateEvents.AddMouseEvents(this);

            AddIdleEvents();
            CameraStateEvents.AddUIEvents(this);
        }

        /// <summary>
        /// unregister events
        /// </summary>
        protected override void RemoveEvents()
        {
            base.RemoveEvents();
            CameraStateEvents.RemoveMouseEvents(this);

            RemoveIdleEvents();
            CameraStateEvents.RemoveUIEvents(this);
        }

        private void AddIdleEvents()
        {
            _birdEyeDelegate = context => NextStateHelper.GoToBirdEye(this);
            _settingsMenuDelegate = context => CameraStateUtilities.HandleGoToSettingsMenu(this, CameraManager.Instance.currentCamera);

            _leftInterestPointClickDelegate = () => SwitchToInterestPoint(_cameraFirstPersonUI.rightInterestPointButton, _cameraFirstPersonUI.leftInterestPointButton, _cameraFirstPersonUI.leftInterestPoint);
            _rightInterestPointClickDelegate = () => SwitchToInterestPoint(_cameraFirstPersonUI.leftInterestPointButton, _cameraFirstPersonUI.rightInterestPointButton, _cameraFirstPersonUI.rightInterestPoint);

            KeyboardInputManager.settingMenu.performed += _settingsMenuDelegate;

            if (_cameraFirstPersonUI.leftInterestPoint is not null)
                _cameraFirstPersonUI.leftInterestPointButton.onClick.AddListener(_leftInterestPointClickDelegate);
            if (_cameraFirstPersonUI.rightInterestPoint is not null)
                _cameraFirstPersonUI.rightInterestPointButton.onClick.AddListener(_rightInterestPointClickDelegate);
        }

        private void RemoveIdleEvents()
        {
            KeyboardInputManager.settingMenu.performed -= _settingsMenuDelegate;

            if (_cameraFirstPersonUI.leftInterestPoint is not null)
                _cameraFirstPersonUI.leftInterestPointButton.onClick.RemoveListener(_leftInterestPointClickDelegate);
            if (_cameraFirstPersonUI.rightInterestPoint is not null)
                _cameraFirstPersonUI.rightInterestPointButton.onClick.RemoveListener(_rightInterestPointClickDelegate);
        }

        private void SwitchToInterestPoint(CleanButton goBackButton, CleanButton interestPointButton, CinemachineVirtualCamera interstPoint)
        {
            _ignoreGoBack = true;
            _cameraFirstPersonUI.goBackButton = goBackButton;
            _cameraFirstPersonUI.interestPointButton = interestPointButton;

            goBackButton.gameObject.SetActive(true);
            interestPointButton.gameObject.SetActive(false);

            nextState = NextStateHelper.GoToInterestPoint(currentStateName);
            currentState = State.exit;
        }

        private void ResolveObjects()
        {
            _firstPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<FirstPersonUIHolder>();
            _thirdPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<BirdEyeUIHolder>();
            _cameraFirstPersonUI = ExperimentItemsContainer.Instance.Resolve<CameraFirstPersonUI>();
        }
    }
}