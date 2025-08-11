using DimmEffect;
using System.Linq;
using Table;
using Table.UI.Views;
using UnityEngine;
using UnityEngine.Events;

namespace Praxilabs.CameraSystem
{
    public class MoveToDeviceState : CameraStateMachine
    {
        public static event System.Action OnEnterDeviceState;
        private FirstPersonUIHolder _firstPersonUIHolder;
        private MoveToDeviceCommand _toDeviceCommand;
        private DimmEffectManager _dimmEffectManager;
        
        private DeviceUI _deviceUI;

        private TablesManager _tablesManager;

        private bool _canExitState;
        private UnityAction OnCloseDelegate;

        /// <summary>
        /// add device to command to switch to its camera
        /// </summary>
        public MoveToDeviceState(DeviceSide deviceSide, CameraState previousState) : base(previousState)
        {

            currentStateName = CameraState.moveToDevice;
            _toDeviceCommand = new MoveToDeviceCommand();
            _toDeviceCommand.deviceSide = deviceSide;
        }

        protected override void Start()
        {
            base.Start();

            ResolveObjects();
            ToggleTable(false);
            OpenDeviceUI();
            _toDeviceCommand.Execute();
            ActivateDimmEffect();
            OnEnterDeviceState?.Invoke();
        }

        protected override void Execute()
        {
            if (CanExitState() || canExitState)
                GoToNextState();
        }

        protected override void GoToNextState()
        {
            if (PreviousStateChecker.IsPreviousInterestPoint())
                nextState = NextStateHelper.GoToInterestPoint(currentStateName);
            if (PreviousStateChecker.IsPreviousIdle())
                nextState = NextStateHelper.GoToIdle(currentStateName);

            currentState = State.exit;
        }

        protected override void Exit()
        {
            DeactivateDimmEffect();
            CloseDeviceUI();

            _toDeviceCommand.StopExecuting();

            base.Exit();
        }

        private bool CanExitState()
        {
            return _canExitState;
        }

        private void OpenDeviceUI()
        {
            _firstPersonUIHolder.ToggleFirstPersonUI(false);
            _firstPersonUIHolder.ToggleSideMenu(false);

            _toDeviceCommand.deviceSide.DeviceSideController.ToggleColliders(false);

            // _deviceCollider = _toDeviceCommand.device.GetComponent<Collider>();
            // _deviceCollider.enabled = false;

            // if (_toDeviceCommand.device.GetComponent<DeviceUI>())
            //     _deviceUI = _toDeviceCommand.device.GetComponent<DeviceUI>();
            // else
            //     _deviceUI = _toDeviceCommand.device.GetComponentInParent<DeviceUI>();

            _deviceUI = _toDeviceCommand.deviceSide.DeviceUI;
            _deviceUI.ToggleDeviceUI(true);

            OnCloseDelegate = () => _canExitState = true;

            _deviceUI.closeButton.onClick.AddListener(OnCloseDelegate);
        }

        private void CloseDeviceUI()
        {
            if (canExitState)
                _deviceUI.closeButton.onClick.Invoke();
            // _deviceCollider.enabled = true;
            _toDeviceCommand.deviceSide.DeviceSideController.ToggleColliders(true);
            _deviceUI.closeButton.onClick.RemoveListener(OnCloseDelegate);
        }

        private void ResolveObjects()
        {
            _dimmEffectManager = ExperimentItemsContainer.Instance.Resolve<DimmEffectManager>();
            _firstPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<FirstPersonUIHolder>();
            _tablesManager = ExperimentItemsContainer.Instance.Resolve<TablesManager>();
        }

        private void ToggleTable(bool toggle)
        {
            _tablesManager.ToggleTable(toggle);
        }

        #region Dimm effect functions
        private void ActivateDimmEffect()
        {
            _dimmEffectManager.Setup();
            _dimmEffectManager.Activate();
            _dimmEffectManager.UpdateDimmCameraFOV(_toDeviceCommand.deviceCamera);

            SetLayerName("DimmingEffect"); 
        }
        
        private void DeactivateDimmEffect()
        {
            _dimmEffectManager.DeActivate();
            SetLayerName("Default");
        }

        private void SetLayerName(string layerName)
        {
            Transform deviceParent = _toDeviceCommand.deviceSide.DeviceUI.transform.parent;
            // Transform deviceParent = _toDeviceCommand.device.transform.parent;
            var childObjects = deviceParent.GetComponentsInChildren<Transform>()
                           .Select(t => t.gameObject)
                           .Where(go => go.GetComponents<Renderer>().Length > 0)
                           .ToArray();
            for (int i = 0; i < childObjects.Length; i++)
            {
                //if (deviceParent.GetChild(i).GetComponent<Renderer>())
                //    deviceParent.GetChild(i).gameObject.layer = LayerMask.NameToLayer(layerName);
                childObjects[i].layer = LayerMask.NameToLayer(layerName);
            }
        }
        #endregion
    }
}