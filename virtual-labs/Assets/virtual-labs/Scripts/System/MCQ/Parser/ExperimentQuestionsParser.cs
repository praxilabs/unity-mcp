using LocalizationSystem;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PraxiLabs.MCQ
{
    [RequireComponent(typeof(MCQWrapper))]
    public class ExperimentQuestionsParser : MonoBehaviour
    {
        private MCQWrapper _mcqWrapper;

        //Localization Variables
        private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
        private Dictionary<int, QuestionData> _questionsData = new Dictionary<int, QuestionData>();
        private Action _OnLanguageChangeDelegate;
        private bool _dataExists;

        private void Awake()
        {
            _mcqWrapper = GetComponent<MCQWrapper>();
        }

        private void OnEnable()
        {
            _OnLanguageChangeDelegate = () =>
            {
                if (!_dataExists) return;
                LoadMCQJson();
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

        private Dictionary<int, BaseInteractiveQuestion> CreateQuestionResponseMap(Dictionary<int, QuestionData> questionDataDict)
        {
            var questionResponseMap = new Dictionary<int, BaseInteractiveQuestion>();

            if (questionDataDict.Count > 0)
            {
                foreach (var keyValuePair in questionDataDict)
                {
                    BaseInteractiveQuestion interactiveQuestion = CreateQuestionFromData(questionDataDict[keyValuePair.Key]);
                    questionResponseMap[keyValuePair.Key] = interactiveQuestion;
                }
            }

            OrderDictionary(questionResponseMap);

            return questionResponseMap;
        }

        private BaseInteractiveQuestion CreateQuestionFromData(QuestionData questionData)
        {
            Question question = new Question(questionData);
            BaseInteractiveQuestion interactiveQuestion = CreateInteractiveQuestion(question);

            if (questionData.UnifiedData != null)
                HandleUnifiedData(questionData.UnifiedData, interactiveQuestion);

            return interactiveQuestion;
        }

        private BaseInteractiveQuestion CreateInteractiveQuestion(Question question)
        {
            if (question.ResponseType == QuestionResponseType.ObservationQuestion)
                return new ObservationQuestion(question);
            if (question.ResponseType == QuestionResponseType.SimpleFeedbackQuestion)
                return new SimpleFeedbackQuestion(question);
            if (question.ResponseType == QuestionResponseType.SpecificFeedbackQuestion)
                return new SpecificFeedbackQuestion(question);
            if (question.ResponseType == QuestionResponseType.UnifiedFeedbackQuestion)
                return new UnifiedFeedbackQuestion(question);

            Debug.LogError($"Invalid respone type: {question.ResponseType}, correct your json file");
            return null;
        }

        private void HandleUnifiedData(UnifiedData unifiedData, BaseInteractiveQuestion interactiveQuestion)
        {
            UnifiedFeedbackQuestion unifiedFeedbackQuestion = interactiveQuestion as UnifiedFeedbackQuestion;
            unifiedFeedbackQuestion.UnifiedData = unifiedData;
        }

        private Dictionary<int, BaseInteractiveQuestion> OrderDictionary(Dictionary<int, BaseInteractiveQuestion> dictionaryToOrder)
        {
            return dictionaryToOrder.OrderBy(keyValuePair => keyValuePair.Key)
                    .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        }

        #region Localization
        private void LoadJson()
        {
            _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.MCQ.ToString());

            if (!_dataExists) return;

            _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.MCQ.ToString()];
            LoadMCQJson();
        }

        public void LoadMCQJson()
        {
            if (!_dataExists) return;

            string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
            _questionsData = JsonConvert.DeserializeObject<Dictionary<int, QuestionData>>(currentJson);

            var questionResponseMap = CreateQuestionResponseMap(_questionsData);
            _mcqWrapper.QuestionResponseMap = new Dictionary<int, BaseInteractiveQuestion>(questionResponseMap);
            _mcqWrapper.UpdateCurrentQuestion();
        }
        #endregion
    }

    [System.Serializable]
    public class QuestionData
    {
        public string QuestionTxt { get; set; }
        public bool IsMultipleChoice { get; set; }
        public FeedbackTriggerType TriggerType { get; set; }
        public QuestionResponseType ResponseType { get; set; }
        public List<AnswerData> CorrectAnswer { get; set; } = new();
        public List<AnswerData> WrongAnswers { get; set; } = new();
        public UnifiedData UnifiedData { get; set; }
    }
}

