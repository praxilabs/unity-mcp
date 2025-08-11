using Cinemachine;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class RotateCommand : CameraCommand
    {
        public CinemachineVirtualCamera camera;
        public float verticalSpeed;
        public float horizontalSpeed;

        private CinemachinePOV _cameraPov;
        private CameraSettings _cameraData;

        public override void Execute()
        {
            Initialize();
            Rotate();
        }

        public override void StopExecuting()
        {
            _cameraPov.m_VerticalAxis.m_MaxSpeed = 0;
            _cameraPov.m_HorizontalAxis.m_MaxSpeed = 0;
        }

        private void Initialize()
        {
            if (_cameraPov == null)
                _cameraPov = camera.GetCinemachineComponent<CinemachinePOV>();
            if (camera is not null)
                _cameraData = camera.GetComponent<CameraSettings>();
        }

        private void Rotate()
        {
            if (_cameraData.canRotateVertically)
                _cameraPov.m_VerticalAxis.m_MaxSpeed = _cameraData.rotationSpeed;
            if (_cameraData.canRotateHorizontally)
                _cameraPov.m_HorizontalAxis.m_MaxSpeed = _cameraData.rotationSpeed;
        }

        public void Refresh()
        {
            Initialize();
            StopExecuting();
        }

        public void UIHorizontalRotate(float angle)
        {
            if (_cameraPov != null)
                _cameraPov.m_HorizontalAxis.Value += angle * Time.deltaTime;
        }

        public void ThirdPersonRotate(float direction)
        {
            float speed = _cameraData.rotationSpeed;
            float yaw = direction * speed * Time.deltaTime;

            if (CameraManager.Instance.cameraInstance.FollowTarget() != null)
                CameraManager.Instance.cameraInstance.FollowTarget().Rotate(Vector3.up, yaw, Space.World);
        }
    }
}