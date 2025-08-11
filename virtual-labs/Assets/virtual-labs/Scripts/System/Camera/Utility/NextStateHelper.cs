using UnityEngine;
using static Praxilabs.StateBase;

namespace Praxilabs.CameraSystem
{
    /// <summary>helper class that have functions to go to next states</summary>
    public static class NextStateHelper
    {
        #region Idle state
        public static CameraStateMachine GoToIdle(CameraState currentStateName)
        {
            return new IdleState(currentStateName);
        }
        #endregion

        #region Lock Rotation state
        public static void GoToLockRotation(CameraStateMachine state)
        {
            state.nextState = new LockRotaionState(state.currentStateName);
            state.currentState = State.exit;
        }

        public static CameraStateMachine GoToLockRotation(CameraState currentStateName)
        {
            return new LockRotaionState(currentStateName);
        }
        #endregion

        #region Lock Position state
        public static void GoToLockPosition(CameraStateMachine state)
        {
            state.nextState = new LockPositionState(state.currentStateName);
            state.currentState = State.exit;
        }

        public static CameraStateMachine GoToLockPosition(CameraState currentStateName)
        {
            return new LockPositionState(currentStateName);
        }
        #endregion

        #region Freeze state
        public static void GoToFreeze(CameraStateMachine state)
        {
            state.nextState = new FreezeState(state.currentStateName);
            state.currentState = State.exit;
        }

        public static CameraStateMachine GoToFreeze(CameraState currentStateName)
        {
            return new FreezeState(currentStateName);
        }
        #endregion

        #region Bird Eye state
        public static void GoToBirdEye(CameraStateMachine state)
        {
            state.nextState = new BirdEyeState(state.currentStateName);
            state.currentState = State.exit;
        }

        public static CameraStateMachine GoToBirdEye(CameraState currentStateName)
        {
            return new BirdEyeState(currentStateName);
        }
        #endregion

        #region Settings Menu state
        public static void GoToSettingsMenu(CameraStateMachine state)
        {
            state.nextState = new SettingsMenuState(state.currentStateName);
            state.currentState = State.exit;
        }

        public static CameraStateMachine GoToSettingsMenu(CameraState currentStateName)
        {
            return new SettingsMenuState(currentStateName);
        }
        #endregion

        #region Rotate state
        public static void GoToRotation(CameraStateMachine state)
        {
            if (CameraManager.Instance.eventSystem.IsPointerOverGameObject())
                return;

            if (CameraManager.Instance.stateRunner.canRotate)
            {
                state.nextState = new RotateState(state.currentStateName);
                state.currentState = State.exit;
            }
        }

        public static CameraStateMachine GoToRotate(CameraState currentStateName)
        {
            return new RotateState(currentStateName);
        }
        #endregion

        #region Zoom state
        public static void GoToZoom(CameraStateMachine state, float zoomInput, bool ignoreUI)
        {
            if (!ignoreUI && CameraManager.Instance.eventSystem.IsPointerOverGameObject())
                return;

            state.nextState = new ZoomState(zoomInput, state.currentStateName);
            state.currentState = State.exit;
        }
        #endregion

        #region Reset state
        public static void GoToReset(CameraStateMachine state)
        {
            state.nextState = new ResetState(state.currentStateName);
            state.currentState = State.exit;
        }

        public static CameraStateMachine GoToReset(CameraState currentStateName)
        {
            return new ResetState(currentStateName);
        }
        #endregion

        #region InterestPoint state
        public static void GoToInterestPoint(CameraStateMachine state, GameObject hitTarget)
        {
            InterestPoint targetInterestPoint = hitTarget.GetComponent<InterestPoint>();
            if (targetInterestPoint == null) return;

            CameraManager.Instance.currentCamera = targetInterestPoint.poiCamera;
            switch (targetInterestPoint.poiType)
            {
                case InterestPointType.InterestPoint:
                    state.nextState = NextStateHelper.GoToInterestPoint(state.currentStateName);
                    break;
                case InterestPointType.MainBench:
                    state.nextState = NextStateHelper.GoToIdle(state.currentStateName);
                    break;
                case InterestPointType.SettingsMenu:
                    state.nextState = NextStateHelper.GoToSettingsMenu(state.currentStateName);
                    break;
            }

            state.currentState = State.exit;
        }

        public static CameraStateMachine GoToInterestPoint(CameraState currentStateName)
        {
            return new InterestPointState(currentStateName);
        }

        public static bool TryGetInterestPoint(out GameObject hitTarget)
        {
            RaycastHit hitInfo;
            hitTarget = GameHelper.GetClickedObjectWithTag(out hitInfo, "POI");

            return hitTarget;
        }
        #endregion

        #region Move To Device
        public static void MoveToDevice(CameraStateMachine state)
        {
            if (CameraManager.Instance.eventSystem.IsPointerOverGameObject())
                return;
            RaycastHit hitInfo;
            GameObject hitTarget = GameHelper.GetClickedObjectWithTag(out hitInfo, "Device");

            if (hitTarget)
            {
                DeviceSide deviceSide = hitTarget.GetComponent<DeviceSide>();
                state.nextState = new MoveToDeviceState(deviceSide, state.currentStateName);
                state.currentState = State.exit;
            }
        }
        #endregion
    }
}