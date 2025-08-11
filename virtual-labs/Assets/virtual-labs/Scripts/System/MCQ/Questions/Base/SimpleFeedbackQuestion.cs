namespace PraxiLabs.MCQ
{
    [System.Serializable]
    public class SimpleFeedbackQuestion : BaseInteractiveQuestion
    {
        public SimpleFeedbackQuestion(Question question) : base(question) {}

        public override QuestionResponseType GetQuestionResponseType() => QuestionResponseType.SimpleFeedbackQuestion;

        public override FeedbackData FeedbackData()
        {
            FeedbackData feedbackData = new();
            feedbackData.showExplanation = false;
            feedbackData.showUnifiedExplanation = false;
            return feedbackData;
        }
    }
}