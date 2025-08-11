using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseDeviceMenuBinding : MonoBehaviour
{
    public abstract object GetDeviceMenu();
    public abstract object GetDeviceMenuName(); 
    public abstract void ToggleDeviceMenu(bool isVisible);
    public abstract void AddListenerToDeviceMenuLabelsButton(UnityAction action);
    public abstract void RemoveListenersFromDeviceMenuLabelsButton();
    protected abstract void SetDeviceMenuPosition();
}