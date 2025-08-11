using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[DisallowMultipleComponent]
public abstract class MenusBase : MonoBehaviour
{
    
    protected static List<Canvas> _normalMenus = new List<Canvas>();
    

    private MenuState menuState;
    [SerializeField] private Canvas _menuCanvas;
    protected Canvas MenuCanvas { get => _menuCanvas; set => _menuCanvas = value; } 
    protected MenuState MenuState { get => menuState; set => menuState = value; }
    public abstract void Open();
    public void UpdateMenuState(MenuState state)
    {
        MenuState = state;
    }

}

public enum MenuState
{
    Default,
    FullScreen,
    Hidden,
    Pinned
}

