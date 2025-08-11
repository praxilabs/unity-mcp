namespace PraxiLabs.MCQ
{
    [System.Serializable]
    public class ObservationQuestion : BaseInteractiveQuestion
    {
        public ObservationQuestion(Question question) : base(question) {}

        public override QuestionResponseType GetQuestionResponseType() => QuestionResponseType.ObservationQuestion;

        public override FeedbackData FeedbackData()
        {
            FeedbackData feedbackData = new();
            feedbackData.showExplanation = false;
            feedbackData.showUnifiedExplanation = false;
            return feedbackData;
        }
    }
}