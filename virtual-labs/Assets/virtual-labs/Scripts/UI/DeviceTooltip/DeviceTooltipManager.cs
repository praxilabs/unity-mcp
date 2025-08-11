using LocalizationSystem;
using Newtonsoft.Json;
using PraxiLabs.Tooltip;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceTooltipManager : Singleton<DeviceTooltipManager>
{
    [SerializeField] private DeviceTooltip _tooltipPrefab;

    //Localization Variables
    private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
    private Dictionary<int, UILabelData> _uiLabels = new Dictionary<int, UILabelData>();
    private Action _OnLanguageChangeDelegate;
    private bool _dataExists;

    private string deviceName;

    private void OnEnable()
    {
        _OnLanguageChangeDelegate = () =>
        {
            if (!_dataExists) return;
            LoadLabelJson();
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

    public void ShowTooltip(Vector3 targetPosition, int deviceID, float worldSpaceOffset, List<Renderer> devicePartsRenderers, bool isFlashing, bool shouldTintColorForWhite = false)
    {
        if(!_uiLabels.ContainsKey(deviceID)) return;

        if (_tooltipPrefab != null || _uiLabels.Count > 0)
        {
            deviceName = _uiLabels[deviceID].labelName;
            _tooltipPrefab.Setup(targetPosition, deviceName, worldSpaceOffset, devicePartsRenderers, isFlashing, shouldTintColorForWhite);
            _tooltipPrefab.Show();
        }
    }

    public void HideTooltip()
    {
        if (_tooltipPrefab != null)
        {
            _tooltipPrefab.Hide();
        }
    }

    public void AdjustTooltipVisuals(bool isTooltipGloballyOff, bool isHighlightGloballyOff)
    {
        _tooltipPrefab.IsTooltipGloballyOff = isTooltipGloballyOff;
        _tooltipPrefab.IsHighlightGloballyOff = isHighlightGloballyOff;
    }


    #region Localization
    private void LoadJson()
    {
        _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.DeviceTooltip.ToString());

        if (!_dataExists) return;

        _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.DeviceTooltip.ToString()];
        LoadLabelJson();
    }

    public void LoadLabelJson()
    {
        if (!_dataExists) return;

        string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
        _uiLabels = JsonConvert.DeserializeObject<Dictionary<int, UILabelData>>(currentJson);
    }
    #endregion
}