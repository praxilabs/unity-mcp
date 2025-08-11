using System;

public class LabModeController : Singleton<LabModeController>
{
    public event Action<LabMode> OnModeChanged;

    public LabMode CurrentMode {get; private set;}

    public void SetLabMode(LabMode newLabMode)
    {
        CurrentMode = newLabMode;
        OnModeChanged?.Invoke(CurrentMode);
        HandleModeChange();
    }
    
    private void HandleModeChange()
    {
        switch(CurrentMode)
        {
            case LabMode.BenchMode:
                EnterBenchMode();
                break;
            case LabMode.BirdEyeMode:
                EnterBirdEyeMode();
                break;
            case LabMode.SettingsMenuMode:
                EnterSettingsMenuMode();
                break;
            case LabMode.DeviceMode:
                EnterDeviceMode();
                break;
            case LabMode.ExplorationMode:
                EnterExplorationMode();
                break;
            case LabMode.TutorialMode:
                EnterTutorialMode();
                break;
        }
    }
    
    private void EnterBenchMode()
    {
        DeviceTooltipManager.Instance.AdjustTooltipVisuals(false,false);
    }

    private void EnterBirdEyeMode()
    {
        DeviceTooltipManager.Instance.AdjustTooltipVisuals(true,true);
    }

    private void EnterSettingsMenuMode()
    {
        // Empty for now
    }

    private void EnterDeviceMode()
    {
        DeviceTooltipManager.Instance.AdjustTooltipVisuals(true,true);
    }

    private void EnterExplorationMode()
    {
        DeviceTooltipManager.Instance.AdjustTooltipVisuals(true,true);
    }

    private void EnterTutorialMode()
    {
        DeviceTooltipManager.Instance.AdjustTooltipVisuals(true,true);
    }
}
