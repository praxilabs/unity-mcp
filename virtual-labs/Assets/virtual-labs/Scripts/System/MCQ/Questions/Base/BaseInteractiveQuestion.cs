using System.Collections.Generic;
using UnityEngine;

namespace PraxiLabs.MCQ
{
    public abstract class BaseInteractiveQuestion : Question, IQuestionResponse
    {
        protected BaseInteractiveQuestion(Question question) : base(question) {}
        protected BaseInteractiveQuestion() {}

        public abstract QuestionResponseType GetQuestionResponseType();
        public virtual FeedbackData FeedbackData()
        {
            return null;
        }
    }
}