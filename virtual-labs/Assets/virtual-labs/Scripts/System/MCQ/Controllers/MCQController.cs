using LocalizationSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PraxiLabs.MCQ
{
    [RequireComponent(typeof(UILayoutRefresher))]
    public class MCQController : MonoBehaviour 
    {
        private LocalizedString SingleChoiceQuestion = new LocalizedString();
        private LocalizedString MultipleChoiceQuestion = new LocalizedString();
        private LocalizedString SubmitFeedbackText = new LocalizedString();
        private LocalizedString ImmediateFeedbackText = new LocalizedString();

        private const float LayoutRefreshDelayDuration = 0.1f;
        private const float DisableMaskDelayDuration = 0.0002f;

        [Header("General Elements"), Space]
        [SerializeField] private TextMeshProUGUI _questionHeader;
        [SerializeField] private TextMeshProUGUI _question;
        // [SerializeField] private Mask _windowMask;
        [SerializeField] private GameObject _window;
        [SerializeField] private GameObject _backgroundOverlay;
        [SerializeField] private GameObject _singleChoicePanel;
        [SerializeField] private GameObject _multipleChoicePanel;
        private List<AnswerUI> _singleAnswerUIs;
        private List<AnswerUI> _multipleAnswerUIs;
        [SerializeField] private AnswerUI _answerPrefab;
        [SerializeField] private DynamicScrollView _singleChoiceDynamicScrollView;

        [Header("Button Elements"), Space]
        [SerializeField] private Button _confirmBtn;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Color _enabledBtnColor;
        [SerializeField] private Color _disabledBtnColor;

        [Header("Unified Elements"), Space]
        [SerializeField] private TextMeshProUGUI _unifiedExplanation;
        [SerializeField] private GameObject _unifiedContainer; // toggle it when needed
        [SerializeField] private Image _unifiedIcon; // turn it to red/green and back to white on reset
        [SerializeField] private Color _correctColor;
        [SerializeField] private Color _incorrectColor;

        private BaseInteractiveQuestion _currentQuestion;
        public List<AnswerData> CurrentAnswer = new List<AnswerData>();
        private List<AnswerData> _selectedQuestionAnswers = new List<AnswerData>();

        public AnswerUI ChosenAnswerUI {get; set;} // most probably will make it a list when I work on multiple choice questions
        public bool IsCorrectAnswer {get; set;}
        public Button CloseButton => _closeBtn; 

        private UILayoutRefresher _layoutRefresher;
        private DynamicToggleGroup _dynamicToggleGroup;

        private QuestionHandler _questionHandlerChain;
        private void Awake() 
        {
            InitializeHandlers();

            _singleAnswerUIs = _singleChoicePanel.GetComponentsInChildren<AnswerUI>(true).ToList();
            // _multipleAnswerUIs = _multipleChoicePanel.GetComponentsInChildren<AnswerUI>(true).ToList();
            _layoutRefresher = GetComponent<UILayoutRefresher>();
            _dynamicToggleGroup = _singleChoicePanel.GetComponentInChildren<DynamicToggleGroup>();
        }

        private void InitializeHandlers()
        {
            SingleChoiceSubmitHandler singleChoiceSubmitHandler = new SingleChoiceSubmitHandler();
            singleChoiceSubmitHandler.SetNext(new SingleChoiceImmediateHandler());//.SetNext(othertypeslikeMultipleChoiceSubmitAndImmediateHandlers)
            _questionHandlerChain = singleChoiceSubmitHandler;
        }

        private void Start()
        {
            foreach(AnswerUI answerUI in _singleAnswerUIs)
            {
               answerUI.Toggle.onValueChanged.AddListener(enable => HandleAnswerUIToggleUpdated(answerUI, enable));
            }
            _confirmBtn.onClick.AddListener(() => { 
                    DisplayFeedbackWithCorrectAnswer();
                    Confirm();
                    });
            _closeBtn.onClick.AddListener(Close);
            // foreach(AnswerUI answerUI in _multipleAnswerUIs)
            // {
            //    answerUI.Toggle.onValueChanged.AddListener(enable => HandleAnswerUIToggleUpdated(answerUI, enable));
            // }
            _unifiedContainer.SetActive(false);
        }

        private void OnDestroy()
        {
            foreach(AnswerUI answerUI in _singleAnswerUIs)
            {
               answerUI.Toggle.onValueChanged.RemoveAllListeners();
            }
            // foreach(AnswerUI answerUI in _multipleAnswerUIs)
            // {
            //    answerUI.Toggle.onValueChanged.RemoveAllListeners();
            // }
            _confirmBtn.onClick.RemoveAllListeners();
            _closeBtn.onClick.RemoveListener(Close);
        }

        private void HandleAnswerUIToggleUpdated(AnswerUI answerUI, bool enable)
        {
            _questionHandlerChain.Handle(_currentQuestion, answerUI, enable, this);
        }

        public void SetQuestion(BaseInteractiveQuestion question)
        {
            _currentQuestion = question;
            UpdateDisplay();
        }

        public void UpdateQuestion(BaseInteractiveQuestion question)
        {
            _currentQuestion = question;
        }

        private void Confirm()
        {
            SetAllToggleImageVisibility(false);
            SetAllToggleInteractability(false);
            SetConfirmButtonVisbilty(false);
            SetConfirmButtonInteractability(false);
            SetCloseButtonVisbilty(true);
        }

        private void Close()
        {
            // Close the whole panel
            foreach(AnswerUI answerUI in _singleAnswerUIs)
            {
               answerUI.Reset();
            }
            // foreach(AnswerUI answerUI in _multipleAnswerUIs)
            // {
            //    answerUI.Reset();
            // }
            SetCloseButtonVisbilty(false);
            SetConfirmButtonVisbilty(true);
            _backgroundOverlay.SetActive(false);
            _window.SetActive(false);
            _singleChoicePanel.SetActive(false);
            // _multipleChoicePanel.SetActive(false);
            _dynamicToggleGroup.Reset();
            _unifiedIcon.color = Color.white;
            _unifiedContainer.SetActive(false);
        }

        public void UpdateDisplay()
        {
            AddLocalizedData();
            _questionHeader.text = _currentQuestion.IsMultipleChoice ? MultipleChoiceQuestion.Value : SingleChoiceQuestion.Value;

            SetConfirmButtonInteractability(false);
            UpdateConfirmButtonText(_currentQuestion.TriggerType == FeedbackTriggerType.SubmitFeedback ? SubmitFeedbackText.Value : ImmediateFeedbackText.Value);
            SetConfirmButtonVisbilty(true);

            if(_currentQuestion.IsMultipleChoice)
            {
                // Update multiple choice panel
                // Show multiple choice panel
            }
            else
            {
                List<AnswerData> answers = GetRandomizedAnswers();
                _selectedQuestionAnswers = answers;
                AdjustAnswerUIs(_singleAnswerUIs, answers, _singleChoicePanel.transform);

                for (int i = 0; i < answers.Count; i++)
                {
                    _singleAnswerUIs[i].SetAnswerData(answers[i]);
                    _singleAnswerUIs[i].gameObject.SetActive(true);
                }

                _singleChoicePanel.SetActive(true);
            }

            // _windowMask.enabled = true;
            _backgroundOverlay.SetActive(true);
            _window.SetActive(true);

            // LayoutRefresh();
            StartLayoutRefreshCoroutine();
            // StartDisableWindowMaskCoroutine();
        }

        private void AddLocalizedData()
        {
            _question.text = _currentQuestion.QuestionTxt;

            MultipleChoiceQuestion.table = "MCQ_StaticData";
            MultipleChoiceQuestion.key = "MultipleChoiceQuestion";
            SingleChoiceQuestion.table = "MCQ_StaticData";
            SingleChoiceQuestion.key = "SingleChoiceQuestion";
            SubmitFeedbackText.table = "MCQ_StaticData";
            SubmitFeedbackText.key = "SubmitFeedbackText";
            ImmediateFeedbackText.table = "MCQ_StaticData";
            ImmediateFeedbackText.key = "ImmediateFeedbackText";

            RefreshLocalizedText();
        }

        public void UpdateText()
        {
            RefreshLocalizedText();
            UpdateUI();
            StartLayoutRefreshCoroutine();
        }

        private void RefreshLocalizedText()
        {
            MultipleChoiceQuestion.Refresh();
            SingleChoiceQuestion.Refresh();
            SubmitFeedbackText.Refresh();
            ImmediateFeedbackText.Refresh();
        }

        private void UpdateUI()
        {
            _question.text = _currentQuestion.QuestionTxt;

            List<AnswerData> tempList = new List<AnswerData>(_currentQuestion.CorrectAnswer);
            tempList.AddRange(_currentQuestion.WrongAnswers);
            tempList.Sort((a, b) => _selectedQuestionAnswers.IndexOf(a).CompareTo(_selectedQuestionAnswers.IndexOf(b)));

            _selectedQuestionAnswers = tempList;

            for (int i=0; i<_singleAnswerUIs.Count; i++)
            {
                _singleAnswerUIs[i].SetAnswerData(_selectedQuestionAnswers[i]);
            }

            _questionHeader.text = _currentQuestion.IsMultipleChoice ? MultipleChoiceQuestion.Value : SingleChoiceQuestion.Value;
            UpdateConfirmButtonText(_currentQuestion.TriggerType == FeedbackTriggerType.SubmitFeedback ? SubmitFeedbackText.Value : ImmediateFeedbackText.Value);
        }

        private List<AnswerData> GetRandomizedAnswers()
        {
            List<AnswerData> answers = new List<AnswerData>(_currentQuestion.CorrectAnswer);
            answers.AddRange(_currentQuestion.WrongAnswers);

            System.Random random = new System.Random();
            answers = answers.OrderBy(a => random.Next()).ToList();

            return answers;
        }

        private void AdjustAnswerUIs(List<AnswerUI> _answerUIList, List<AnswerData> answers, Transform parent)
        {
            if (answers.Count > _answerUIList.Count)
            {
                // Create new AnswerUI when needed
                int requiredNewAnswerUI = answers.Count - _answerUIList.Count;
                for (int i = 0; i < requiredNewAnswerUI; i++)
                {
                    var answerUIInstance = GameObject.Instantiate(_answerPrefab, parent);
                    _dynamicToggleGroup.AddToggle(answerUIInstance.Toggle);
                    _answerUIList.Add(answerUIInstance);
                    answerUIInstance.Toggle.onValueChanged.AddListener(enable => HandleAnswerUIToggleUpdated(answerUIInstance, enable));
                    _layoutRefresher.TargetList.Add(answerUIInstance.GetComponent<RectTransform>());
                }
            }
            else if (answers.Count < _answerUIList.Count)
            {
                // Disable some of the available AnswerUI when needed
                int exceedCount = _answerUIList.Count - answers.Count;
                for (int i = 0; i < exceedCount; i++)
                {
                    _answerUIList[_answerUIList.Count - 1 - i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < answers.Count; i++)
            {
                // Enable some/all of the available AnswerUI
                _answerUIList[i].gameObject.SetActive(true);
            }
        }

        public bool CheckAnswer(AnswerUI answerUI) // probably would change it to recieve chosenAnswerUI (list in future)
        {
            if(_currentQuestion.IsMultipleChoice)
            {
                // WIP
                return false;
            }
            else
            {
                return _currentQuestion.CorrectAnswer[0] == answerUI.GetAnswerData();
            }
        }

        private void DisplayFeedbackWithCorrectAnswer()
        {
            if(_currentQuestion.TriggerType == FeedbackTriggerType.ImmediateFeedback) return;
            
            DisplayFeedback();

            if(IsCorrectAnswer) return;

            DisplayCorrectAnswer();
        }

        private void DisplayCorrectAnswer()
        {
            if(_currentQuestion.IsMultipleChoice)
            {
                // WIP
            }
            else
            {
                FeedbackData feedbackData = _currentQuestion.FeedbackData();
                AnswerUI answerUI = null;
                for(int i = 0; i < _singleAnswerUIs.Count; i++)
                {
                    if(CheckAnswer(_singleAnswerUIs[i]))
                    {
                        answerUI = _singleAnswerUIs[i];
                        break;
                    }
                }
                
                answerUI.SetCorrectAnswer(feedbackData.showExplanation);
            }
        }

        public void DisplayFeedback()
        {
            FeedbackData feedbackData = _currentQuestion.FeedbackData();
            ChosenAnswerUI.DisplayFeedback(IsCorrectAnswer, feedbackData.showExplanation, StartLayoutRefreshCoroutine);

            if(feedbackData.showUnifiedExplanation)
            {
                DisplayUnifiedFeedback();
            }
        }

        private void DisplayUnifiedFeedback()
        {
            UnifiedFeedbackQuestion unifiedFeedbackQuestion = _currentQuestion as UnifiedFeedbackQuestion;
            if(unifiedFeedbackQuestion == null)
            {
                Debug.LogError("You are trying to show a unified explanation by relying on a non unified feedback question");
                return;
            }

            UnifiedData unifiedData = unifiedFeedbackQuestion.UnifiedData;

            _unifiedExplanation.text = IsCorrectAnswer ? unifiedData.CorrectExplanation : unifiedData.IncorrectExplanation;

            _unifiedIcon.color = IsCorrectAnswer ? _correctColor : _incorrectColor;

            _unifiedContainer.SetActive(true);

            StartLayoutRefreshCoroutine();
        }

        private void SetAllToggleInteractability(bool isInteractable)
        {
            if(_currentQuestion.IsMultipleChoice)
            {

            }
            else
            {
                foreach(var answerUI in _singleAnswerUIs)
                {
                    answerUI.SetToggleInteractability(isInteractable);
                }
            }
        }
        private void SetAllToggleImageVisibility(bool isVisible)
        {
            if(_currentQuestion.IsMultipleChoice)
            {

            }
            else
            {
                foreach(var answerUI in _singleAnswerUIs)
                {
                    if(ChosenAnswerUI == answerUI) continue;
                    answerUI.SetToggleImageVisibility(isVisible);
                }
            }
        }
        public void SetConfirmButtonInteractability(bool isInteractable)
        {
            _confirmBtn.interactable = isInteractable;
            _confirmBtn.GetComponent<Image>().color = isInteractable ? _enabledBtnColor : _disabledBtnColor;
        }

        private void SetConfirmButtonVisbilty(bool isVisible) => _confirmBtn.gameObject.SetActive(isVisible);
        private void SetCloseButtonVisbilty(bool isVisible) => _closeBtn.gameObject.SetActive(isVisible);
        private void UpdateConfirmButtonText(string targetText) => _confirmBtn.GetComponentInChildren<TextMeshProUGUI>().text = targetText;

        private void LayoutRefresh()
        {
            _singleChoiceDynamicScrollView.ContentUpdated();
            _layoutRefresher.Refresh();
            // LayoutRebuilder.ForceRebuildLayoutImmediate(_window.GetComponent<RectTransform>());
        }

        private void StartLayoutRefreshCoroutine() => StartCoroutine(LayoutRefreshDelay());
        private IEnumerator LayoutRefreshDelay()
        {
            yield return LayoutRefreshDelayDuration;
            LayoutRefresh();
        }

        // private void StartDisableWindowMaskCoroutine() => StartCoroutine(DisableWindowMaskCoroutine());
        // private IEnumerator DisableWindowMaskCoroutine()
        // {
        //     yield return DisableMaskDelayDuration;
        //     // _windowMask.enabled = false;
        // }
    }
}