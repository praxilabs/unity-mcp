using Cinemachine;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    /// <summary>WebGL version of CameraInstance </summary>
    public class CameraInstanceWebGL : CameraInstance
    {
        /// <summary>
        /// add new camera to stack and switch to it
        /// </summary>
        public override void SwitchActiveCamera(CinemachineVirtualCamera target)
        {
            CameraManager.Instance.cameraSwitcher.Switch(target);
        }

        /// <summary>
        /// Remove current camera from stack and switch to previous one
        /// </summary>
        public override void SwitchBack()
        {
            CameraManager.Instance.cameraSwitcher.SwitchBack();
        }

        /// <summary>
        /// Enable/Disable point of interests
        /// </summary>
        public override void InterestPointsSetActive(bool value)
        {
            foreach (GameObject interestPoint in CameraManager.Instance.experimentCameras.interestPointsEffect)
                interestPoint.SetActive(value);
        }

        /// <summary>
        /// Get current camera follow target
        /// </summary>
        /// <returns></returns>
        public override Transform FollowTarget()
        {
            return CameraManager.Instance.currentCamera.Follow;
        }
    }
}