using LocalizationSystem;
using Newtonsoft.Json;
using Praxilabs.DeviceSideMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeviceMenuTextDistributer : MonoBehaviour
{
    [SerializeField] private int _ID;
    [SerializeField] private string _deviceID;
    [SerializeField] private SafetyProceduresTabContent _safetyProceduresContent;
    [SerializeField] private DescriptionTabContent _descriptionContent;
    [SerializeField] private DeviceMenu _deviceMenu;

    private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
    private Dictionary<int, DeviceData> _devicesData = new Dictionary<int, DeviceData>();
    private Action _OnLanguageChangeDelegate;
    private bool _dataExists;

    private void OnEnable()
    {
        _OnLanguageChangeDelegate = () =>
        {
            if (!_dataExists) return;
            LoadDeviceMenuJson();
        };
        LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
    }

    private void Start()
    {
        LoadJson();
    }

    private void LoadJson()
    {
        _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.DeviceMenu.ToString());

        if (!_dataExists) return;

        _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.DeviceMenu.ToString()];
        LoadDeviceMenuJson();
    }

    public void LoadDeviceMenuJson()
    {
        if (!_dataExists) return;

        string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
        _devicesData = JsonConvert.DeserializeObject<Dictionary<int, DeviceData>>(currentJson);

        DistributeData();
    }

    public void DistributeData()
    {
        AddStaticData();
        AddControlsData();
    }

    public void AddStaticData()
    {
        DeviceData deviceData = _devicesData[_ID];
        _deviceMenu.UpdateDeviceName(deviceData.deviceName);
        _safetyProceduresContent.UpdateData(deviceData.safetyProceduresTitle, deviceData.safetyProceduresBody);
        _descriptionContent.UpdateData(deviceData.descriptionTitle, deviceData.descriptionBody);
    }

    public void AddControlsData()
    {
        DeviceData deviceData = _devicesData[_ID];

        foreach (DeviceControlData selectedDeviceControlData in deviceData.controls)
        {
            ControlsComponentUI matchingComponent = _deviceMenu.GetControlsComponents().FirstOrDefault(c => c.Name == selectedDeviceControlData.name);

            if (matchingComponent != null)
                matchingComponent.SetLocalizedData(selectedDeviceControlData.componentLabel);
        }
    }
}