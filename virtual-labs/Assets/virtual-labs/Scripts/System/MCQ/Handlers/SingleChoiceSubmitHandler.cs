namespace PraxiLabs.MCQ
{
    public class SingleChoiceSubmitHandler : QuestionHandler
    {
        public override void Handle(BaseInteractiveQuestion question, AnswerUI answerUI, bool enable, MCQController controller)
        {
            if (!question.IsMultipleChoice && question.TriggerType == FeedbackTriggerType.SubmitFeedback)
            {
                controller.SetConfirmButtonInteractability(true);

                if (enable)
                {
                    controller.IsCorrectAnswer = controller.CheckAnswer(answerUI);
                    controller.CurrentAnswer.Add(answerUI.GetAnswerData());
                    controller.ChosenAnswerUI = answerUI;
                }
                else
                {
                    controller.CurrentAnswer.Remove(answerUI.GetAnswerData());
                }
            }
            else
            {
                _nextHandler?.Handle(question, answerUI, enable, controller);
            }
        }
    }
}
