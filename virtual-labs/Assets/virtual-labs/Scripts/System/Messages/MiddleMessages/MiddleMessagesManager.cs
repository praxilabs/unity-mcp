using LocalizationSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MiddleMessagesManager : Singleton<MiddleMessagesManager>
{
    [Header("Canvas")]
    [SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _background;
    [Header("Middle Messages - Two Action Btns")]
    [SerializeField] private GameObject _infoTwoActionPrefab;
    [SerializeField] private GameObject _warningTwoActionPrefab;
    [SerializeField] private GameObject _errorTwoActionPrefab;
    [Header("Middle Messages - One Action Btns")]
    [SerializeField] private GameObject _infoOneActionPrefab;
    [SerializeField] private GameObject _warningOneActionPrefab;
    [SerializeField] private GameObject _errorOneActionPrefab;

    private MiddleMessage _middleMessage;

    //Localization Variables
    private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
    private Dictionary<int, MiddleMessageData> _messages = new Dictionary<int, MiddleMessageData>();
    private MiddleMessageData _currentMessage;
    private Action _OnLanguageChangeDelegate;
    private bool _dataExists;
    private int _currentMessageID;

    public MiddleMessage MiddleMessage { get => _middleMessage; set => _middleMessage = value; }

    private void OnEnable()
    {
        _OnLanguageChangeDelegate = () =>
        {
            if (!_dataExists) return;
            LoadMiddleMessagesJson();
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

    public MiddleMessage OpenMiddleMessage(MessageType messageType, MiddleMessageType middleMessageType, MiddleMessageData middleMessageData)
    {
        GameObject messageInstance = Instantiate(GetMessagePrefab(), _canvas);
        _middleMessage = messageInstance.GetComponent<MiddleMessage>();

        _middleMessage.Setup(middleMessageData);

        ToggleBackground();

        return _middleMessage;
    }

    public MiddleMessage OpenMiddleMessageLocalization(int messageID)
    {
        _currentMessage = _messages[messageID];
        _currentMessageID = messageID;
        GameObject messageInstance = Instantiate(GetMessagePrefab(), _canvas);
        _middleMessage = messageInstance.GetComponent<MiddleMessage>();

        _middleMessage.Setup(_currentMessage);

        ToggleBackground();

        return _middleMessage;
    }

    public void ToggleBackground()
    {
        _background.SetActive(!_background.activeInHierarchy);
    }

    private GameObject GetMessagePrefab()
    {
        GameObject messagePrefab = null;

        if (_currentMessage.middleMessageType == MiddleMessageType.TwoAction)
        {
            switch (_currentMessage.messageType)
            {
                case MessageType.Informative:
                    messagePrefab = _infoTwoActionPrefab;
                    break;
                case MessageType.Warning:
                    messagePrefab = _warningTwoActionPrefab;
                    break;
                case MessageType.Error:
                    messagePrefab = _errorTwoActionPrefab;
                    break;
            }
        }
        else if (_currentMessage.middleMessageType == MiddleMessageType.OneAction)
        {
            switch (_currentMessage.messageType)
            {
                case MessageType.Informative:
                    messagePrefab = _infoOneActionPrefab;
                    break;
                case MessageType.Warning:
                    messagePrefab = _warningOneActionPrefab;
                    break;
                case MessageType.Error:
                    messagePrefab = _errorOneActionPrefab;
                    break;
            }
        }

        return messagePrefab;
    }

    #region Localization
    private void LoadJson()
    {
        _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.MiddleMessages.ToString());

        if (!_dataExists) return;

        _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.MiddleMessages.ToString()];
        LoadMiddleMessagesJson();
    }

    public void LoadMiddleMessagesJson()
    {
        if (!_dataExists) return;

        string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
        _messages = JsonConvert.DeserializeObject<Dictionary<int, MiddleMessageData>>(currentJson);

        UpdateTextLocalization();
    }

    private void UpdateTextLocalization()
    {
        if (_currentMessage == null) return;

        _currentMessage = _messages[_currentMessageID];
        MiddleMessage.SetUpText(_currentMessage);
    }
    #endregion
}

[System.Serializable]
public class MiddleMessageData
{
    [field: SerializeField] public string MessageTitle { get; set; }
    [field: SerializeField] public string TitleDescription { get; set; }
    [field: SerializeField] public string MessageHeader { get; set; }
    [field: SerializeField] public string Message { get; set; }
    [field: SerializeField] public string ActionOneText { get; set; }
    [field: SerializeField] public string ActionTwoText { get; set; }

    public MiddleMessageType middleMessageType;
    public MessageType messageType;
}

public enum MiddleMessageType
{
    OneAction,
    TwoAction
}
