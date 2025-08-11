using System.Collections;
using System.Collections.Generic;
using MenusBehaviors;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Vuplex.WebView;

public class SettingsWebViewPanelUI : MenuWithWebview, IPinnable
{
    [SerializeField] private UnityEvent<bool> OnPinnedChanged;
    [SerializeField] private Button _pinButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private bool _displayVideo;
    
    public bool IsPinned {get; set;}

    private CanvasWebViewPrefab _canvasWebviewPrefab;

    private void Awake()
    {
        if(_displayVideo) _canvasWebviewPrefab = GetComponentInChildren<CanvasWebViewPrefab>();
    }

    private void Start()
    {
        if (_pinButton != null)  _pinButton.onClick.AddListener(Pin);
        if (_backButton != null && _displayVideo)  _backButton.onClick.AddListener(StopVideo);
    }

    protected override void OnDestroy() 
    {
        if (_pinButton != null)  _pinButton.onClick.RemoveAllListeners();
        if (_backButton != null && _displayVideo)  _backButton.onClick.RemoveListener(StopVideo);
    }

    public void Pin()
    {
        UpdateButtonAction(_pinButton, Unpin);
        UpdateMenuState(MenuState.Pinned);
        IsPinned = true;
        OnPinnedChanged?.Invoke(IsPinned);
    }

    public void Unpin()
    {
        UpdateButtonAction(_pinButton, Pin);
        UpdateMenuState(MenuState.Default);
        IsPinned = false;
        OnPinnedChanged?.Invoke(IsPinned);
    }

    private void StopVideo()
    {
        // JavaScript to pause all videos on the webpage
        string jsCode = @"
            var videos = document.getElementsByTagName('video');
            for (var i = 0; i < videos.length; i++) {
                videos[i].pause();
            }";

        // Inject JavaScript into the WebView
        _canvasWebviewPrefab.WebView.ExecuteJavaScript(jsCode);
    }

    private void UpdateButtonAction(Button button, UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
