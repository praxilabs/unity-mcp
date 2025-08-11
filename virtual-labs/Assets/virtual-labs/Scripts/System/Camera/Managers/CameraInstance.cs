using Cinemachine;
using UnityEngine;

namespace Praxilabs.CameraSystem
{
    /// <summary>Replacement for instance of camera manager and holds functions that could be used in different platforms </summary>
    public abstract class CameraInstance
    {
        public virtual void SwitchActiveCamera(CinemachineVirtualCamera target) { }

        public virtual void SwitchBack() { }

        public virtual void InterestPointsSetActive(bool value) { }

        public virtual Transform FollowTarget() { return null; }
    }
}