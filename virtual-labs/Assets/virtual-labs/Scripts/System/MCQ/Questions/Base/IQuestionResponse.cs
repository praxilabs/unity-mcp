namespace PraxiLabs.MCQ
{
    public interface IQuestionResponse
    {
        public FeedbackData FeedbackData();
    }

    public enum QuestionResponseType
    {
        ObservationQuestion, SimpleFeedbackQuestion, SpecificFeedbackQuestion, UnifiedFeedbackQuestion
    }

    public class FeedbackData
    {
        public bool showExplanation;
        public bool showUnifiedExplanation;
    }
}