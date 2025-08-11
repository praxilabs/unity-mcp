using Newtonsoft.Json;
using UnityEngine;

namespace PraxiLabs.MCQ
{
    [System.Serializable]
    public class UnifiedFeedbackQuestion : BaseInteractiveQuestion
    {
        public UnifiedFeedbackQuestion(Question question) : base(question) {}

        [field: SerializeField] public UnifiedData UnifiedData {get; set;}

        public override QuestionResponseType GetQuestionResponseType() => QuestionResponseType.UnifiedFeedbackQuestion;

        public override FeedbackData FeedbackData()
        {
            FeedbackData feedbackData = new();
            feedbackData.showExplanation = false;
            feedbackData.showUnifiedExplanation = true;
            return feedbackData;
        }
    }

    [System.Serializable]
    public class UnifiedData
    {
        [JsonProperty("CorrectExplanation")]
        [field: SerializeField, TextArea(1, 3)] public string CorrectExplanation {get; set;}
        [JsonProperty("IncorrectExplanation")]
        [field: SerializeField, TextArea(1, 3)] public string IncorrectExplanation {get; set;}
    }
}