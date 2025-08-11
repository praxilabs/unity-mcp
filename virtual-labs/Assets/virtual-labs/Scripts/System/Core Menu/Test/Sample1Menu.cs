using MenusBehaviors;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Sample1Menu : RegularMenu , IPlaySound , IFullScreen , IPinnable
{

    [SerializeField] private Button _playSoundButton, _pinButton, _fullScreenButton;

    public bool IsFullScreen { get; set; }
    public bool IsPinned { get ; set;}

    private void Start()
    {
        Open();
        if (_playSoundButton != null) _playSoundButton.onClick.AddListener(PlaySound);
        if (_pinButton != null)  _pinButton.onClick.AddListener(Pin);
        if (_fullScreenButton != null) _fullScreenButton.onClick.AddListener(ActivateFullScreen);
    }
    public override void Open()
    {
        Debug.Log("Open + " + transform.root.name);
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
    public void Pin()
    {
        Debug.Log("Pin From " + transform.root.name);
        UpdateButtonAction(_pinButton, Unpin);
        UpdateMenuState(MenuState.Pinned);
        IsPinned = true;
    }
    public void Unpin()
    {
        Debug.Log("Unpin From " + transform.root.name);
        UpdateButtonAction(_pinButton, Pin);
        UpdateMenuState(IsFullScreen ? MenuState.FullScreen : MenuState.Default);
        IsPinned = false;
    }
    private void UpdateButtonAction(Button _Button, UnityAction myAction)
    {
        _Button.onClick.RemoveAllListeners();
        _Button.onClick.AddListener(myAction);
    }

  
}
