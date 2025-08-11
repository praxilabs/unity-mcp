using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PraxiLabs.MCQ
{
    [System.Serializable]
    public class Question
    {
        [field: SerializeField] public QuestionResponseType ResponseType {get; set;}
        [field: SerializeField] public FeedbackTriggerType TriggerType {get; set;}
        [field: SerializeField, TextArea(2, 3)] public string QuestionTxt {get; set;}
        [field: SerializeField] public List<AnswerData> CorrectAnswer {get; set;} = new();
        [field: SerializeField] public List<AnswerData> WrongAnswers {get; set;} = new();
        [field: SerializeField] public bool IsMultipleChoice {get; set;}
        
        public Question(Question question)
        {
            ResponseType = question.ResponseType;
            TriggerType = question.TriggerType;
            QuestionTxt = question.QuestionTxt;
            CorrectAnswer = question.CorrectAnswer;
            WrongAnswers = question.WrongAnswers;
            IsMultipleChoice = question.IsMultipleChoice;
        }

        public Question(QuestionData questionData)
        {
            ResponseType = questionData.ResponseType;
            TriggerType = questionData.TriggerType;
            QuestionTxt = questionData.QuestionTxt;
            IsMultipleChoice = questionData.IsMultipleChoice;

            CorrectAnswer = questionData.CorrectAnswer.Select(data => new AnswerData
            {
                Answer = data.Answer,
                Explanation = data.Explanation
            }).ToList();

            WrongAnswers = questionData.WrongAnswers.Select(data => new AnswerData
            {
                Answer = data.Answer,
                Explanation = data.Explanation
            }).ToList();
        }

        public Question() {}
    }
}