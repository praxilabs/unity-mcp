using Praxilabs.CameraSystem;
using UnityEngine;

public class CameraStateLoggingEvents : MonoBehaviour
{
    public void AddCameraEvent(CameraState currentStateName)
    {
        EventsManager.Instance.AddListener(PhyLabDemoCameraEvents.EnteredNewState, LogStateEnter);
        EventsManager.Instance.AddListener(PhyLabDemoCameraEvents.ExitState, LogStateExit);
        EventsManager.Instance.Invoke(PhyLabDemoCameraEvents.EnteredNewState, currentStateName);
    }

    public void RemoveCameraEvent(CameraState currentStateName)
    {
        EventsManager.Instance.Invoke(PhyLabDemoCameraEvents.ExitState, currentStateName);
        EventsManager.Instance.RemoveListener(PhyLabDemoCameraEvents.EnteredNewState, LogStateEnter);
        EventsManager.Instance.RemoveListener(PhyLabDemoCameraEvents.ExitState, LogStateExit);
    }

    private void LogStateEnter(object currentStateName)
    {
        Debug.Log($"<color=#F3F8FF>State => {currentStateName} started!</color>");
    }

    private void LogStateExit(object currentStateName)
    {
        Debug.Log($"<color=#F3F8FF>State: {currentStateName} finished!</color>");
    }
}