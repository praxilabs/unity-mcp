using Praxilabs.CameraSystem;
using UnityEngine;

public class LabModeMediator : MonoBehaviour
{
    private void OnEnable()
    {
        BirdEyeState.OnEnterBirdEyeState += HandleEnterBirdEyeState;
        IdleState.OnEnterIdleState += HandleEnterIdleState;
        MoveToDeviceState.OnEnterDeviceState += HandleEnterDeviceState;
        SettingsMenuState.OnEnterSettingsMenuState += HandleEnterSettingsMenuState;
    }

    private void OnDisable()
    {
        BirdEyeState.OnEnterBirdEyeState -= HandleEnterBirdEyeState;
        IdleState.OnEnterIdleState -= HandleEnterIdleState;
        MoveToDeviceState.OnEnterDeviceState -= HandleEnterDeviceState;
        SettingsMenuState.OnEnterSettingsMenuState -= HandleEnterSettingsMenuState;
    }

    private void HandleEnterBirdEyeState()
    {
        SetLabMode(LabMode.BirdEyeMode);
    }

    private void HandleEnterIdleState()
    {
        SetLabMode(LabMode.BenchMode);
    }

    private void HandleEnterDeviceState()
    {
        SetLabMode(LabMode.DeviceMode);
    }

    private void HandleEnterSettingsMenuState()
    {
        SetLabMode(LabMode.SettingsMenuMode);
    }

    private void SetLabMode(LabMode labMode)
    {
        LabModeController.Instance.SetLabMode(labMode);
    }
}
