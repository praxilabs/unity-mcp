using UnityEngine;

public class FirstPersonUIHolder : MonoBehaviour
{
    [SerializeField] private Canvas _cameraCanvas;
    [SerializeField] private Canvas _progressMapCanvas;
    [SerializeField] private Canvas _sideMenuCanvas;
    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _hintsCanvas;
    [SerializeField] private GameObject _oxi;

    public void ToggleFirstPersonUI(bool enable)
    {
        _cameraCanvas.enabled = enable; 
        _progressMapCanvas.enabled = enable;
        SettingsMenuUI.Instance.settingsBtn.gameObject.SetActive(enable);
        _hintsCanvas.enabled = enable;
        _oxi.SetActive(enable);

        if (enable)
            _oxi.GetComponent<HintsDisablerHelper>().ShowHints();
    }
    
    public void ToggleSideMenu(bool enable)
    {
        _sideMenuCanvas.enabled = enable;
    }
}