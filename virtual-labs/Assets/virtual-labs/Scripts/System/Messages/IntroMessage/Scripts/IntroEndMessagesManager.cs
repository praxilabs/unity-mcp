using DG.Tweening;
using LocalizationSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Praxilabs.UIs
{

    [RequireComponent(typeof(IntroEndMessagesDisplay))]
    public class IntroEndMessagesManager : MonoBehaviour
    {
        [HideInInspector] public bool isMessageOpened = false;
        [SerializeField] private EndMessagesIconsScriptable _endMessagesIconsSO;
        [SerializeField] private UnityEvent onGreetingMessageEnd;

        private IntroEndMessagesDisplay _introEndMessagesDisplayObject;

        private int _currentIndex;
        private int _currentMessageID;
        private float _defaultSegmentWidth = 45f;

        private Image _nextButtonImage;
        private Button _nextButton;

        //Localization Variables
        private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
        private Dictionary<int, IntroEndMessage> _messages = new Dictionary<int, IntroEndMessage>();
        private IntroEndMessage _currentMessage;
        private Action _OnLanguageChangeDelegate;
        private bool _dataExists;

        private void OnEnable()
        {

            _OnLanguageChangeDelegate = () =>
            {
                if (!_dataExists) return;
                LoadIntroEndJson();
            };
            LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
        }

        private void Start()
        {
            _introEndMessagesDisplayObject = GetComponent<IntroEndMessagesDisplay>();
            _nextButtonImage = _introEndMessagesDisplayObject.NextButton.transform.GetChild(0).GetComponent<Image>();
            _nextButton = _introEndMessagesDisplayObject.NextButton.GetComponent<Button>();
            LoadJson();
        }

        public void Open(int messageID)
        {
            if (isMessageOpened || _messages.Count == 0) return;
            _currentMessageID = messageID;
            isMessageOpened = true;
            _currentMessage = _messages[messageID];
            IntroEndMessageType msgType = _messages[messageID].messageType;

            if (msgType == IntroEndMessageType.End)
            {
                InitializeEndIcons();
            }

            gameObject.GetComponent<Canvas>().enabled = true;
            //gameObject.GetComponent<InteractionBlockerManager>().enabled = true;

            InitializeText(0);
            SetBackButtonToDisabled();
            InitializeButtonEvents();
            GenerateLineSegments();
            UpdateLineSegments();

        }
        public void Close()
        {
            gameObject.GetComponent<Canvas>().enabled = false;
            gameObject.GetComponent<InteractionBlockerManager>().DestroyTheBlocker();
            onGreetingMessageEnd?.Invoke();

            isMessageOpened = false;
            XnodeStepsRunner.Instance.StepIsDone();
        }
        private void InitializeText(int idx)
        {
            _introEndMessagesDisplayObject.Header.text = _currentMessage.Messages[idx].headerText;
            _introEndMessagesDisplayObject.Body.text = _currentMessage.Messages[idx].bodyText;
            _introEndMessagesDisplayObject.Footer.text = _currentMessage.Messages[idx].footerText;
        }
        private void InitializeButtonEvents()
        {
            _introEndMessagesDisplayObject.BackButton.GetComponent<Button>().onClick.AddListener(UpdateBackButtonStateWithMessagesCount);
            _nextButton.onClick.AddListener(UpdateNextButtonStateWithMessagesCount);
        }
        private void InitializeEndIcons()
        {
            _introEndMessagesDisplayObject.OxiImage.sprite = _endMessagesIconsSO.Oxi;
            _introEndMessagesDisplayObject.CupImage.sprite = _endMessagesIconsSO.Cup;

            _introEndMessagesDisplayObject.OxiImage.color = Color.white;
            _introEndMessagesDisplayObject.CupImage.color = Color.white;
        }
        private void SetBackButtonToDisabled()
        {
            _introEndMessagesDisplayObject.BackButton.interactable = false;
            _introEndMessagesDisplayObject.BackButton.image.color = _introEndMessagesDisplayObject.DisabledColor;
        }

        private void SetBackButtonToEnabled()
        {
            _introEndMessagesDisplayObject.BackButton.interactable = true;
            _introEndMessagesDisplayObject.BackButton.image.color = _introEndMessagesDisplayObject.ActiveColor;
        }
        private void SetNextButtonImage(Sprite image)
        {
            _nextButtonImage.sprite = image;
        }
        private void UpdateNextButtonAction(UnityAction action)
        {
            _nextButton.onClick.RemoveAllListeners();
            _nextButton.onClick.AddListener(action);
        }
        private void UpdateNextButtonStateWithMessagesCount()
        {
            if (_currentIndex < _currentMessage.Messages.Count)
            {
                _currentIndex++;
                SetBackButtonToEnabled();
                InitializeText(_currentIndex);
            }
            UpdateLineSegments();

        }
        private void UpdateBackButtonStateWithMessagesCount()
        {
            if (_currentIndex >= 0)
            {
                _currentIndex--;
                InitializeText(_currentIndex);

            }
            if (_currentIndex == 0)
            {
                SetBackButtonToDisabled();
            }

            SetNextButtonImage(_introEndMessagesDisplayObject.NextButtonInitialImage);
            UpdateNextButtonAction(UpdateNextButtonStateWithMessagesCount);
            UpdateLineSegments();
        }
        private void GenerateLineSegments()
        {
            for (int i = 0; i < _currentMessage.Messages.Count; i++)
            {
                GameObject tmp = Instantiate(_introEndMessagesDisplayObject.LineSegementPrefab);
                tmp.transform.SetParent(_introEndMessagesDisplayObject.LineSegementContainer.transform, false);
            }
        }
        private void UpdateLineSegments()
        {

            for (int i = 0; i < _introEndMessagesDisplayObject.LineSegementContainer.transform.childCount; i++)
            {
                RectTransform tmpRect = _introEndMessagesDisplayObject.LineSegementContainer.transform.GetChild(i).GetComponent<RectTransform>();
                float currentWidth = tmpRect.sizeDelta.x;


                if (i == _currentIndex)
                {
                    tmpRect.DOSizeDelta(new Vector2(currentWidth * 2.45f, tmpRect.sizeDelta.y), 0.5f);
                    tmpRect.gameObject.GetComponent<Image>().color = _introEndMessagesDisplayObject.ActiveColor;
                }
                else
                {
                    tmpRect.DOSizeDelta(new Vector2(_defaultSegmentWidth, tmpRect.sizeDelta.y), 0.5f);
                    tmpRect.gameObject.GetComponent<Image>().color = _introEndMessagesDisplayObject.DisabledColor;
                }

            }

            if (_currentIndex == _currentMessage.Messages.Count - 1)
            {
                SetNextButtonImage(_introEndMessagesDisplayObject.CloseImage);
                UpdateNextButtonAction(Close);
            }
        }

        #region Localization
        private void LoadJson()
        {
            _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.IntroEnd.ToString());

            if (!_dataExists) return;

            _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.IntroEnd.ToString()];
            LoadIntroEndJson();
        }

        public void LoadIntroEndJson()
        {
            if (!_dataExists) return;

            string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
            _messages = JsonConvert.DeserializeObject<Dictionary<int, IntroEndMessage>>(currentJson);

            if (isMessageOpened)
            {
                _currentMessage = _messages[_currentMessageID];
                InitializeText(_currentIndex);
            }
        }
        #endregion
    }

    public enum IntroEndMessageType
    {
        Intro,
        End
    }

}