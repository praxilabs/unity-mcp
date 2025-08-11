using LocalizationSystem;
using Newtonsoft.Json;
using PraxiLabs.Tooltip;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LabelsController : MonoBehaviour
{
    [SerializeField] private DeviceSideType _deviceSideType;

    [SerializeField] private DeviceUI _deviceUI;
    [SerializeField] private BaseDeviceMenuBinding _deviceMenuBinding;
    [SerializeField] private GameObject _labelGizmoPrefab;

    [SerializeField] private DeviceSideLabelsInfo _frontLabelsInfo;
    [SerializeField] private DeviceSideLabelsInfo _backLabelsInfo;
    [SerializeField] private DeviceSideLabelsInfo _leftLabelsInfo;
    [SerializeField] private DeviceSideLabelsInfo _rightLabelsInfo;

    [SerializeField] private DeviceSideLabelsInfo _selectedLabelInfo;

    private GameObject _previousLabel;
    private bool _canUpdatePosition = false;
    private bool _showLabels = false;
    private bool _previousShowLabels;

    private UnityAction _closeClickDelegate;
    private UnityAction _frontSwitchClickDelegate;
    private UnityAction _backSwitchClickDelegate;
    private UnityAction _leftSwitchClickDelegate;
    private UnityAction _rightSwitchClickDelegate;
    private Action _OnLanguageChangeDelegate;
    private bool _dataExists;

    private Dictionary<int, UILabelData> _labelsData = new Dictionary<int, UILabelData>();
    private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();

    private WaitForSeconds _switchCameraTransitionDelay = new WaitForSeconds(SWITCH_CAMERA_TRANSITION_DELAY);
    private const float SWITCH_CAMERA_TRANSITION_DELAY = 2f;

    private void OnEnable()
    {
        _closeClickDelegate = () =>
        {
            SwitchSideType(DeviceSideType.FrontSide);
        };

        _frontSwitchClickDelegate = () => ForceSwitchSideType(DeviceSideType.FrontSide);
        _backSwitchClickDelegate = () => ForceSwitchSideType(DeviceSideType.BackSide);
        _leftSwitchClickDelegate = () => ForceSwitchSideType(DeviceSideType.LeftSide);
        _rightSwitchClickDelegate = () => ForceSwitchSideType(DeviceSideType.RightSide);

        _OnLanguageChangeDelegate = () =>
        {
            if (!_dataExists) return;
            LoadDeviceLabelsData();
        };

        _deviceUI.closeButton.onClick.AddListener(_closeClickDelegate);

        if (_deviceUI.cameraSideButtons.frontCameraButton != null)
            _deviceUI.cameraSideButtons.frontCameraButton.onClick.AddListener(_frontSwitchClickDelegate);
        if (_deviceUI.cameraSideButtons.backCameraButton != null)
            _deviceUI.cameraSideButtons.backCameraButton.onClick.AddListener(_backSwitchClickDelegate);
        if (_deviceUI.cameraSideButtons.leftCameraButton != null)
            _deviceUI.cameraSideButtons.leftCameraButton.onClick.AddListener(_leftSwitchClickDelegate);
        if (_deviceUI.cameraSideButtons.rightCameraButton != null)
            _deviceUI.cameraSideButtons.rightCameraButton.onClick.AddListener(_rightSwitchClickDelegate);
        LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
    }

    private void OnDisable()
    {
        _deviceUI.closeButton.onClick.RemoveListener(_closeClickDelegate);

        if (_deviceUI.cameraSideButtons.frontCameraButton != null)
            _deviceUI.cameraSideButtons.frontCameraButton.onClick.RemoveListener(_frontSwitchClickDelegate);
        if (_deviceUI.cameraSideButtons.backCameraButton != null)
            _deviceUI.cameraSideButtons.backCameraButton.onClick.RemoveListener(_backSwitchClickDelegate);
        if (_deviceUI.cameraSideButtons.leftCameraButton != null)
            _deviceUI.cameraSideButtons.leftCameraButton.onClick.RemoveListener(_leftSwitchClickDelegate);
        if (_deviceUI.cameraSideButtons.rightCameraButton != null)
            _deviceUI.cameraSideButtons.rightCameraButton.onClick.RemoveListener(_rightSwitchClickDelegate);
        LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
    }

    private IEnumerator Start()
    {
        SwitchSideType(DeviceSideType.FrontSide);
        yield return new WaitForSeconds(0.1f);
        _deviceMenuBinding?.AddListenerToDeviceMenuLabelsButton(() => ToggleLabelButtons(!_showLabels));
        LoadJson();
    }

    private void OnDestroy()
    {
        _deviceMenuBinding?.RemoveListenersFromDeviceMenuLabelsButton();
    }

    private void Update()
    {
        if (_canUpdatePosition && _selectedLabelInfo.labelGizmos.Count > 0)
        {
            for (int i = 0; i < _selectedLabelInfo.labelsPlaceHolderParent.childCount; i++)
                PositionLabel(_selectedLabelInfo.labelsPlaceHolderParent.GetChild(i), _selectedLabelInfo.labelGizmos[i]);
        }
    } 

    public void SetSideType(DeviceSideType deviceSideType)
    {
        _deviceSideType = deviceSideType;
        SetSelectedLabelInfo();
    }

    private void SwitchSideType(DeviceSideType deviceSideType)
    {
        _deviceSideType = deviceSideType;

        ToggleLabelButtons(false);
        SetSelectedLabelInfo();
    }

    private void ForceSwitchSideType(DeviceSideType deviceSideType)
    {
        StartCoroutine(ForceSwitchSideTypeCoroutine(deviceSideType));
    }

    private IEnumerator ForceSwitchSideTypeCoroutine(DeviceSideType deviceSideType)
    {
        SwitchSideType(deviceSideType);
        yield return _switchCameraTransitionDelay;
        if (_previousShowLabels)
            ToggleLabelButtons(true);
    }

    public void ToggleLabelButtons(bool setActive)
    {
        _previousShowLabels = _showLabels;
        _showLabels = setActive;

        if (_showLabels)
            _canUpdatePosition = true;
        else
        {
            _canUpdatePosition = false;
            ClosePreviousLabel(null);
        }

        _selectedLabelInfo.labelParent.SetActive(setActive);
    }

    public void InstantiateLabelGizmos()
    {
        SetSelectedLabelInfo();

        if (_selectedLabelInfo.labelGizmos != null && _selectedLabelInfo.labelGizmos.Count > 0)
            return;

        for (int i = 0; i < _selectedLabelInfo.labelsPlaceHolderParent.childCount; i++)
        {
            RectTransform label = Instantiate(_labelGizmoPrefab, _selectedLabelInfo.labelParent.transform).GetComponent<RectTransform>();
            _selectedLabelInfo.labelGizmos.Add(label);
            PositionLabel(_selectedLabelInfo.labelsPlaceHolderParent.GetChild(i), label);
        }
    }

    private void SetSelectedLabelInfo()
    {
        switch (_deviceSideType)
        {
            case DeviceSideType.FrontSide:
                _selectedLabelInfo = _frontLabelsInfo;
                break;
            case DeviceSideType.BackSide:
                _selectedLabelInfo = _backLabelsInfo;
                break;
            case DeviceSideType.LeftSide:
                _selectedLabelInfo = _leftLabelsInfo;
                break;
            case DeviceSideType.RightSide:
                _selectedLabelInfo = _rightLabelsInfo;
                break;
        }
    }

    public void DestroyLabelGizmos()
    {
        SetSelectedLabelInfo();

        for (int i = 0; i < _selectedLabelInfo.labelGizmos.Count; i++)
        {
            RectTransform labelGizmo = _selectedLabelInfo.labelGizmos[i];
            DestroyImmediate(labelGizmo.gameObject);
        }

        _selectedLabelInfo.labelGizmos.Clear();

        switch (_deviceSideType)
        {
            case DeviceSideType.FrontSide:
                _frontLabelsInfo.labelGizmos.Clear();
                break;
            case DeviceSideType.BackSide:
                _backLabelsInfo.labelGizmos.Clear();
                break;
            case DeviceSideType.LeftSide:
                _leftLabelsInfo.labelGizmos.Clear();
                break;
            case DeviceSideType.RightSide:
                _rightLabelsInfo.labelGizmos.Clear();
                break;
        }
    }

    private void PositionLabel(Transform targetObject, RectTransform uiElement)
    {
        Vector3 targetPosition = targetObject.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPosition);

        // Convert the screen position to the local space of the canvas
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiElement.parent as RectTransform,
            screenPos,
            null,
            out localPos);

        // Adjust for the position of the parent object
        localPos += (Vector2)targetObject.localPosition;

        uiElement.localPosition = localPos;
    }

    public void ClosePreviousLabel(GameObject newLabel)
    {
        if (_previousLabel is not null && _previousLabel != newLabel)
            _previousLabel.transform.GetComponentInParent<LabelGizmo>().ToggleLabel(false);

        _previousLabel = newLabel;
    }

    public IEnumerator WaitTillLabelsShownCoroutine()
    {
        yield return new WaitUntil(
          delegate
          {
              if (_showLabels)
              {
                  EventsManager.Instance.Invoke(FunctionCallEvents.GoToNextStep);
                  return true;
              }
              return false;
          });
    }

    #region Localization
    private void LoadJson()
    {
        _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.DeviceLabels.ToString());

        if (!_dataExists) return;

        _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.DeviceLabels.ToString()];
        LoadDeviceLabelsData();
    }

    private void LoadDeviceLabelsData()
    {
        if (!_dataExists) return;

        string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
        _labelsData = JsonConvert.DeserializeObject<Dictionary<int, UILabelData>>(currentJson);

        UpdateText();
    }

    public void UpdateText()
    {
        UpdateUIText(_frontLabelsInfo);
        UpdateUIText(_backLabelsInfo);
        UpdateUIText(_leftLabelsInfo);
        UpdateUIText(_rightLabelsInfo);
    }

    private void UpdateUIText(DeviceSideLabelsInfo labelsInfo)
    {
        foreach (var label in labelsInfo.labelGizmos)
        {
            LabelGizmo labelGizmo = label.GetComponent<LabelGizmo>();

            labelGizmo.UpdateText(_labelsData[labelGizmo.labelID].labelName);
        }
    }
    #endregion
}

    [Serializable]
public class DeviceSideLabelsInfo
{
    public GameObject labelParent;
    public Transform labelsPlaceHolderParent;
    public List<RectTransform> labelGizmos = new List<RectTransform>();
}