using Praxilabs.Input;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Praxilabs.CameraSystem
{
    public class MoveCommand : CameraCommand
    {
        public static Action<float, float> OnMoving;
        private Vector3 _moveDirection;
        public float horizontalValue;
        public float verticalValue;
        public bool isUsingUI;
        
        private NavMeshAgent _agent;

        public override void Execute()
        {
            if (isUsingUI)
                GetInputValues(horizontalValue, verticalValue);
            else
                GetInputValues(AxisInputManager.horizontal.ReadValue<float>(), AxisInputManager.vertical.ReadValue<float>());

            MovementEquation();
            Move();
        }

        /// <summary>
        /// Get horiznotal/vertical axis values (WASD/arrow keys inputs)
        /// </summary>
        private void GetInputValues(float horizontal, float vertical)
        {
            horizontalValue = horizontal;
            verticalValue = vertical;
        }

        private void MovementEquation()
        {
            //using camera brain because its transform is the same as current active virtual camera
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // Project camera forward and right vectors onto horizontal plane
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Calculate movement direction based on camera projection and input
            _moveDirection = (cameraForward * verticalValue) + (cameraRight * horizontalValue);
            _moveDirection.Normalize();
        }

        private void Move()
        {
            _agent = CameraManager.Instance.currentCamera.GetComponent<NavMeshAgent>();

            if (_agent != null)
                NavmeshMovement();
            else
                CameraFollowMovement();
        }

        private void NavmeshMovement()
        {
            var movement = _moveDirection * CameraManager.Instance.currentCamera.GetComponent<CameraSettings>().movementSpeed * Time.deltaTime;
            Vector3 nextPosition = _agent.nextPosition + movement;

            NavMeshHit hit;
            bool hitBoundary = NavMesh.Raycast(_agent.nextPosition, nextPosition, out hit, NavMesh.AllAreas);

            if (hitBoundary)
            {
                float distanceToBoundary = Vector3.Distance(_agent.nextPosition, hit.position);

                if (distanceToBoundary < 0.1f)
                    return;
                else
                    _agent.Move(movement);
            }
            else
                _agent.Move(movement);
        }

        private void CameraFollowMovement()
        {
            var movement = _moveDirection * CameraManager.Instance.currentCamera.GetComponent<CameraSettings>().movementSpeed * Time.deltaTime;

            if (CameraManager.Instance.cameraInstance.FollowTarget() != null)
            {
                CameraManager.Instance.cameraInstance.FollowTarget().position += movement;
                OnMoving?.Invoke(horizontalValue, verticalValue);
            }
        }
    }
}