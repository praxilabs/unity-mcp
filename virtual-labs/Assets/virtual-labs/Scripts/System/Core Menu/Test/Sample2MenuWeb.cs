using MenusBehaviors;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Sample2MenuWeb : MenuWithWebview , IPlaySound , IFullScreen
{
    protected override void OnEnable()
    {
        Debug.Log("On Enable Web Menu Sample");
        base.OnEnable();
    }

    [SerializeField] private Button _playSoundButton , _fullScreenButton;

    public bool IsFullScreen { get; set; }
    public bool IsPinned { get ; set;}

    private void Start()
    {
        OpenMenu();
        if (_playSoundButton != null) _playSoundButton.onClick.AddListener(PlaySound);
        if (_fullScreenButton != null) _fullScreenButton.onClick.AddListener(ActivateFullScreen);
    }
    public void OpenMenu()
    {
        Debug.Log("Open + " + transform.root.name);
        UpdateMenuState(MenuState.Default);
    }
    public void PlaySound()
    {
        Debug.Log("Play Audio From + " + transform.root.name);
        UpdateButtonAction(_playSoundButton, PauseSound);
    }
    public void PauseSound()
    {
        Debug.Log("Pause Audio From + " + transform.root.name);
        UpdateButtonAction(_playSoundButton, PlaySound);
    }
    public void ActivateFullScreen()
    {
        Debug.Log("FullScreen on From + " + transform.root.name);
        UpdateMenuState(MenuState.FullScreen);
        IsFullScreen = true;
    }
    private void UpdateButtonAction(Button _Button, UnityAction myAction)
    {
        _Button.onClick.RemoveAllListeners();
        _Button.onClick.AddListener(myAction);
    }

}
