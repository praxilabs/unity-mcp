namespace PraxiLabs.MCQ
{
    public class SingleChoiceImmediateHandler : QuestionHandler
    {
        public override void Handle(BaseInteractiveQuestion question, AnswerUI answerUI, bool enable, MCQController controller)
        {
            if (!question.IsMultipleChoice && question.TriggerType == FeedbackTriggerType.ImmediateFeedback)
            {
                if (enable)
                {
                    controller.IsCorrectAnswer = controller.CheckAnswer(answerUI);
                    controller.CurrentAnswer.Add(answerUI.GetAnswerData());
                    controller.ChosenAnswerUI = answerUI;
                    controller.DisplayFeedback();

                    controller.SetConfirmButtonInteractability(controller.IsCorrectAnswer);
                }
                else
                {
                    controller.CurrentAnswer.Remove(answerUI.GetAnswerData());
                    answerUI.ResetVisuals();
                }
            }
            else
            {
                _nextHandler?.Handle(question, answerUI, enable, controller);
            }
        }
    }
}