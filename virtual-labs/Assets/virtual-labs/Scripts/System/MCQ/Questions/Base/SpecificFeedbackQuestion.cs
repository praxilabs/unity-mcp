namespace PraxiLabs.MCQ
{
    [System.Serializable]
    public class SpecificFeedbackQuestion : BaseInteractiveQuestion
    {
        public SpecificFeedbackQuestion(Question question) : base(question) {}

        public override QuestionResponseType GetQuestionResponseType() => QuestionResponseType.SpecificFeedbackQuestion;

        public override FeedbackData FeedbackData()
        {
            FeedbackData feedbackData = new();
            feedbackData.showExplanation = true;
            feedbackData.showUnifiedExplanation = false;
            return feedbackData;
        }
    }
}