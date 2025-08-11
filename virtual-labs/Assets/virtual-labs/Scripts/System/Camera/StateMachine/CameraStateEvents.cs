using Praxilabs.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Praxilabs.CameraSystem
{
    public static class CameraStateEvents
    {
        // Define delegates for each action
        private static System.Action<InputAction.CallbackContext> _leftPressActionDelegate;
        private static System.Action<InputAction.CallbackContext> _rightPressActionDelegate;
        private static System.Action<InputAction.CallbackContext> _zoomActionDelegate;
        private static System.Action<InputAction.CallbackContext> _resetActionDelegate;

        private static UnityAction _birdEyeBtnDelegate;
        private static UnityAction _zoominBtnDelegate;
        private static UnityAction _zoomoutBtnDelegate;
        private static UnityAction _resetBtnDelegate;
        private static UnityAction _settingsBtnDelegate;

        private static FirstPersonUIHolder _firstPersonUIHolder;
        private static BirdEyeUIHolder _thirdPersonUIHolder;
        private static CameraFirstPersonUI _cameraFirstPersonUI;

        public static void PrepareEvents()
        {
            ResolveObjects();
        }

        public static void AddMouseEvents(CameraStateMachine state)
        {
            // Initialize delegates and assign actions
            _leftPressActionDelegate = context => NextStateHelper.MoveToDevice(state);
            _rightPressActionDelegate = context => NextStateHelper.GoToRotation(state);
            _zoomActionDelegate = context => NextStateHelper.GoToZoom(state, MouseInputManager.zoomAction.ReadValue<Vector2>().y, false);
            _resetActionDelegate = context => GoToReset(state);

            // Add listeners
            MouseInputManager.leftPressAction.performed += _leftPressActionDelegate;
            MouseInputManager.rightPressAction.performed += _rightPressActionDelegate;
            MouseInputManager.zoomAction.performed += _zoomActionDelegate;
            MouseInputManager.resetAction.performed += _resetActionDelegate;
        }

        public static void RemoveMouseEvents(CameraStateMachine state)
        {
            // Remove listeners using the same delegate instances
            MouseInputManager.leftPressAction.performed -= _leftPressActionDelegate;
            MouseInputManager.rightPressAction.performed -= _rightPressActionDelegate;
            MouseInputManager.zoomAction.performed -= _zoomActionDelegate;
            MouseInputManager.resetAction.performed -= _resetActionDelegate;
        }

        public static void AddUIEvents(CameraStateMachine state)
        {
            _birdEyeBtnDelegate = () => NextStateHelper.GoToBirdEye(state);
            _zoominBtnDelegate = () => NextStateHelper.GoToZoom(state, 1, true);
            _zoomoutBtnDelegate = () => NextStateHelper.GoToZoom(state, -1, true);
            _resetBtnDelegate = () => NextStateHelper.GoToReset(state);
            _settingsBtnDelegate = () => NextStateHelper.GoToSettingsMenu(state);

            _cameraFirstPersonUI.thirdPersonPOV.onClick.AddListener(_birdEyeBtnDelegate);
            _cameraFirstPersonUI.zoomin.onClick.AddListener(_zoominBtnDelegate);
            _cameraFirstPersonUI.zoomout.onClick.AddListener(_zoomoutBtnDelegate);
            _cameraFirstPersonUI.reset.onClick.AddListener(_resetBtnDelegate);

            SettingsMenuUI.Instance.settingsBtn.onClick.AddListener(_settingsBtnDelegate);
        }

        public static void RemoveUIEvents(CameraStateMachine state)
        {
            // Remove UI event listeners
            _cameraFirstPersonUI.thirdPersonPOV.onClick.RemoveListener(_birdEyeBtnDelegate);
            _cameraFirstPersonUI.zoomin.onClick.RemoveListener(_zoominBtnDelegate);
            _cameraFirstPersonUI.zoomout.onClick.RemoveListener(_zoomoutBtnDelegate);
            _cameraFirstPersonUI.reset.onClick.RemoveListener(_resetBtnDelegate);

            SettingsMenuUI.Instance.settingsBtn.onClick.RemoveListener(_settingsBtnDelegate);
        }

        private static void GoToReset(CameraStateMachine state)
        {
            if (IsPointerOverUI() || !CameraManager.Instance.stateRunner.canReset)
                return;
            else
                NextStateHelper.GoToReset(state);
        }

        private static bool IsPointerOverUI()
        {
            return CameraManager.Instance.eventSystem.IsPointerOverGameObject();
        }

        private static void ResolveObjects()
        {
            _cameraFirstPersonUI = ExperimentItemsContainer.Instance.Resolve<CameraFirstPersonUI>();
        }
    }
}
