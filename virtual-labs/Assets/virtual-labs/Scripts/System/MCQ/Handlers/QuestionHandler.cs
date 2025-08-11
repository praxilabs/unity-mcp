
namespace PraxiLabs.MCQ
{
    public abstract class QuestionHandler
    {
        protected QuestionHandler _nextHandler;

        public void SetNext(QuestionHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public abstract void Handle(BaseInteractiveQuestion question, AnswerUI answerUI, bool enable, MCQController controller);
    }
}