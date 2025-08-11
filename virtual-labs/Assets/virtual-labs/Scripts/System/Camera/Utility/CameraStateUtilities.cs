using Cinemachine;
using Praxilabs.CameraSystem;
using UnityEngine;

public static class CameraStateUtilities 
{
    public static void HandleGoToInterestPoint(CameraStateMachine cameraState, CinemachineVirtualCamera stateVirtualCamera)
    {
        if(!NextStateHelper.TryGetInterestPoint(out GameObject hitTarget)) return;

        CinemachineVirtualCamera interestPointCamera = hitTarget.transform.parent.gameObject.GetComponentInChildren<CinemachineVirtualCamera>();

        if(!interestPointCamera) return;

        stateVirtualCamera.LookAt = interestPointCamera.LookAt;

        NextStateHelper.GoToInterestPoint(cameraState, hitTarget);
    }

    public static void HandleGoToSettingsMenu(CameraStateMachine cameraState, CinemachineVirtualCamera stateVirtualCamera)
    {
        stateVirtualCamera.LookAt = CameraManager.Instance.experimentCameras.settingsMenuCamera.LookAt;
        NextStateHelper.GoToSettingsMenu(cameraState);
    }
}