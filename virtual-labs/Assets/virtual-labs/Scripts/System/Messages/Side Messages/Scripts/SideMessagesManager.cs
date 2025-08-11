using LocalizationSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;

public class SideMessagesManager : MonoBehaviour
{
    [Header("Side Message Prefabs")]
    [SerializeField] private GameObject _informativePrefab;
    [SerializeField] private GameObject _warningPrefab;
    [SerializeField] private GameObject _errorPrefab;

    [Header("Scrollable View Container")]
    [Space(10)]
    [SerializeField] private GameObject _scrollableContainer;

    private NotificationLauncher _launcher;

    //Localization Variables
    private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
    private Dictionary<int, SideMessageData> _messages = new Dictionary<int, SideMessageData>();
    private SideMessageData _currentMessage;
    private Action _OnLanguageChangeDelegate;
    private bool _dataExists;

    private void OnEnable()
    {
        _OnLanguageChangeDelegate = () =>
        {
            if (!_dataExists) return;
            LoadSideMessaegJson();
        };
        LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
    }
    void Start()
    {

        if (gameObject.GetComponent<NotificationLauncher>() == null)
        {
            _launcher = gameObject.AddComponent<NotificationLauncher>();
        }
        else
        {
            _launcher = GetComponent<NotificationLauncher>();
        }

        _launcher.ScrollableContainer = _scrollableContainer;
        LoadJson();
    }

    public void OpenSideMessage(MessageType messageType, SideMessageData MessageData)
    {
        if (_launcher != null)
        {
            SetLanucherData(messageType, MessageData);
            _launcher.LaunchNotificationInsideScrollableContainer();
        }
    }

    public void OpenSideMessage(MessageType messageType, SideMessageData MessageData, Vector2 offset)
    {
        if (_launcher != null)
        {
            SetLanucherData(messageType, MessageData);
            _launcher.LaunchNotificationInsideScrollableContainer();
        }
    }

    public void OpenSideMessage(int messageID)
    {
        _currentMessage = _messages[messageID];

        if (_launcher != null)
        {
            SetLanucherData(_currentMessage.MessageType, _currentMessage);
            _launcher.LaunchNotificationInsideScrollableContainerWithID(messageID);
        }
    }

    public void OpenSideMessage(int messageID, Vector2 offset)
    {
        _currentMessage = _messages[messageID];

        if (_launcher != null)
        {
            SetLanucherData(_currentMessage.MessageType, _currentMessage);
            _launcher.LaunchNotificationInsideScrollableContainer(messageID, offset);
        }
    }

    private void SetLanucherData(MessageType messageType, SideMessageData messageData)
    {
        _launcher.Prefab = GetMessagePrefab(messageType);
        _launcher.Title = messageData.Title;
        _launcher.Message = messageData.Message;
        _launcher.Type = messageData.Animation;
        _launcher.Duration = messageData.Duration;
    }

    private GameObject GetMessagePrefab(MessageType messageType)
    {
        GameObject messagePrefab = null;

        switch (messageType)
        {
            case MessageType.Informative:
                messagePrefab = _informativePrefab;
                break;
            case MessageType.Warning:
                messagePrefab = _warningPrefab;
                break;
            case MessageType.Error:
                messagePrefab = _errorPrefab;
                break;
        }

        return messagePrefab;
    }

    #region Localization
    private void LoadJson()
    {
        _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.SideMessages.ToString());

        if (!_dataExists) return;

        _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.SideMessages.ToString()];
        LoadSideMessaegJson();
    }

    public void LoadSideMessaegJson()
    {
        if (!_dataExists) return;

        string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
        _messages = JsonConvert.DeserializeObject<Dictionary<int, SideMessageData>>(currentJson);

        UpdateTextLocalization();
    }

    private void UpdateTextLocalization()
    {
        foreach (var message in _messages)
            _launcher.UpdateTextLocalization(message.Key);
    }
    #endregion

    [System.Serializable]
    public class SideMessageData
    {
        [field: SerializeField] public string Title { get; set; }
        [field: SerializeField] public string Message { get; set; }
        [field: SerializeField] public MessageType MessageType { get; set; }
        [field: SerializeField] public NotificationType Animation { get; set; }
        [field: SerializeField] public float Duration { get; set; }
    }
}