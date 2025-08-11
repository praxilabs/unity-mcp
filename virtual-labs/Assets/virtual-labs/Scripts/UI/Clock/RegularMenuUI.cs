using System;
using MenusBehaviors;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RegularMenuUI : RegularMenu, IPinnable
{
    public UnityEvent<bool> OnPinnedChanged;
    [SerializeField] private Button _pinButton;
    
    public bool IsPinned {get; set;}

    private void Start()
    {
        if (_pinButton != null)  _pinButton.onClick.AddListener(Pin);
    }

    private void OnDestroy() 
    {
        if (_pinButton != null)  _pinButton.onClick.RemoveAllListeners();
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

    private void UpdateButtonAction(Button button, UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}