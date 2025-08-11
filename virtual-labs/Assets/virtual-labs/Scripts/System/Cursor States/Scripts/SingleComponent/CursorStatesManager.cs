using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorStatesManager : Singleton<CursorStatesManager>
{
    [SerializeField] private List<DeviceSideController> _deviceSideControllers = new List<DeviceSideController>();

    protected override void Awake()
    {
        _addToDontDestroyOnLoad = false;
        base.Awake();
    }

    public void ToggleCursorHandlers(bool enable, DeviceSideController targetDeviceSideController = null)
    {
        foreach(DeviceSideController deviceSideController in _deviceSideControllers)
        {
            if(deviceSideController == targetDeviceSideController)
            {
                targetDeviceSideController.ToggleDeviceSideCursorHandlers(enable);
                continue;
            }    
            deviceSideController.ToggleAllCursorHandlers(enable);
        }
    }

    public void AddCursorHandler(DeviceSideController deviceSideController)
    {
        _deviceSideControllers.Add(deviceSideController);
    }

    public void RemoveCursorHandler(DeviceSideController deviceSideController)
    {
        _deviceSideControllers.Remove(deviceSideController);
    }
}
