using Praxilabs.Input;
using UnityEngine.Events;

namespace Praxilabs.CameraSystem
{
    public class SettingsMenuState : CameraStateMachine
    {
        public static event System.Action OnEnterSettingsMenuState;
        private UnityAction _returnFromSettingsDelegate;
        private UnityAction _openTutorialDelegate;

        private FirstPersonUIHolder _firstPersonUIHolder;
        private BirdEyeUIHolder _thirdPersonUIHolder;

        private bool _shouldReturnToMainBench;

        public SettingsMenuState(CameraState previousState) : base(previousState)
        {
            currentStateName = CameraState.settingsMenu;
        }

        protected override void Start()
        {
            base.Start();
            ResolveObjects();
           
            _firstPersonUIHolder.ToggleFirstPersonUI(false);
            _firstPersonUIHolder.ToggleSideMenu(true);

            _thirdPersonUIHolder.ToggleBirdEyeUI(false);
            SettingsMenuUI.Instance.ToggleSettingsMenu(true);

            PrepareCommand();
            OnEnterSettingsMenuState?.Invoke();
        }

        protected override void PrepareCommand()
        {
            _cameraCommand = new SettingsMenuCommand();

            _cameraCommand.Execute();
        }

        /// <summary>
        /// Go to idle state when we click T (settings menu input action)
        /// </summary>
        protected override void Execute()
        {
            if (KeyboardInputManager.settingMenu.WasPerformedThisFrame())
                GoToNextState();
        }

        protected override void GoToNextState()
        {
            if (PreviousStateChecker.IsPreviousIdle())
            {
                nextState = NextStateHelper.GoToIdle(currentStateName);
            }
            if (PreviousStateChecker.IsPreviousInterestPoint())
            {
                nextState = NextStateHelper.GoToInterestPoint(currentStateName);
            }            
            if (PreviousStateChecker.IsPreviousBirdEye())
            {
                nextState = NextStateHelper.GoToBirdEye(currentStateName);
            }            
            currentState = State.exit;
        }

        /// <summary>
        /// Remove settings menu camera from cameras stack and go to previous camera
        /// </summary>
        protected override void Exit()
        {
            if(!_shouldReturnToMainBench)
            {
                _cameraCommand.StopExecuting();
            }
            else
            {
                SettingsMenuCommand settingsMenuCommand = _cameraCommand as SettingsMenuCommand;
                settingsMenuCommand.SwitchToMainBench();
            }

            SettingsMenuUI.Instance.ToggleSettingsMenu(false);
            base.Exit();
        }

        private void GoToIdle()
        {
            _shouldReturnToMainBench = true;
            nextState = NextStateHelper.GoToIdle(currentStateName);
            currentState = State.exit;
        }

        protected override void AddEvents()
        {
            _returnFromSettingsDelegate = () => GoToNextState();
            _openTutorialDelegate = () => GoToIdle();
            SettingsMenuUI.Instance.returnFromSettingsBtn.onClick.AddListener(_returnFromSettingsDelegate);
            SettingsMenuUI.Instance.openTutorialBtn.onClick.AddListener(_openTutorialDelegate);
        }

        protected override void RemoveEvents()
        {
            SettingsMenuUI.Instance.returnFromSettingsBtn.onClick.RemoveListener(_returnFromSettingsDelegate);
            SettingsMenuUI.Instance.openTutorialBtn.onClick.RemoveListener(_openTutorialDelegate);
        }

        private void ResolveObjects()
        {
            _firstPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<FirstPersonUIHolder>();
            _thirdPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<BirdEyeUIHolder>();
        }
    }
}
