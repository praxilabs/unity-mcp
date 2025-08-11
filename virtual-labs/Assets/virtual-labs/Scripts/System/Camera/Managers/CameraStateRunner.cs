using System;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    /// <summary>Runs camera state machine and anything related to camera states </summary>
    public class CameraStateRunner : MonoBehaviour
    {
        public List<CameraState> executedStates = new List<CameraState>();

        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool canRotate = true;
        [HideInInspector] public bool canReset = true;

        public CameraStateMachine currentState;
        private bool _interestPointsSetActive;
        private bool _isStateRunning = false;

        private void Update()
        {
            if (_isStateRunning)
                currentState = currentState.Process() as CameraStateMachine;
        }

        public void StartCameraStateMachine()
        {
            currentState = new IdleState(CameraState.idle);
            CameraManager.Instance.cameraSwitcher.Refresh();

            ToggleCameraInteraction(true);
        }

        private void ToggleCameraInteraction(bool toggle)
        {
            if (currentState != null)
                _isStateRunning = toggle;
        }
    }
}