using UltimateClean;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : Singleton<SettingsMenuUI>
{
    public CleanButton settingsBtn;
    public CleanButton returnFromSettingsBtn;
    public Button openTutorialBtn;  
    [SerializeField] private GameObject _settingsMenuPanel;

    public void ToggleSettingsMenu(bool enable)
    {
        _settingsMenuPanel.SetActive(enable);
    }
}
