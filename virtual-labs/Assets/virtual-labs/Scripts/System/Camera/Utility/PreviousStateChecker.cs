namespace Praxilabs.CameraSystem
{
    /// <summary>helper class that check previous states</summary>
    public static class PreviousStateChecker
    {
        #region Idle state
        public static bool IsPreviousIdle()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.idle;
        }
        #endregion

        #region Lock Rotation state
        public static bool IsPreviousLockRotation()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.lockRotation;
        }
        #endregion

        #region Lock Position state
        public static bool IsPreviousLockPosition()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.lockPosition;
        }
        #endregion

        #region Freeze state
        public static bool IsPreviousFreeze()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.freeze;
        }
        #endregion

        #region Bird Eye state
        public static bool IsPreviousBirdEye()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.birdEye;
        }
        #endregion

        #region Rotate state
        public static bool IsPreviousRotate()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.rotate;
        }
        #endregion

        #region Zoom state
        public static bool IsPreviousZoom()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.zoom;
        }
        #endregion

        #region Reset state
        public static bool IsPreviousReset()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.reset;
        }
        #endregion

        #region InterestPoint state
        public static bool IsPreviousInterestPoint()
        {
            return CameraManager.Instance.stateRunner.executedStates[CameraManager.Instance.stateRunner.executedStates.Count - 1] is CameraState.interestPoint;
        }
        #endregion
    }
}