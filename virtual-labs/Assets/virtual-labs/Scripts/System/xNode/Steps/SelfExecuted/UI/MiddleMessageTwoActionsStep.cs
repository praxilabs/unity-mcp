using UnityEngine;

[NodeTint("#5F99AE"), CreateNodeMenu("UI/Messages/Middle Message (Two Actions)", 2)]
public class MiddleMessageTwoActionsStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField] private int _messageID;

    [Output] public NodeObject buttonOne;
    [Output] public NodeObject buttonTwo;
    [Output] public NodeObject closeButton;

    private MiddleMessage _middleMessage;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        _middleMessage = MiddleMessagesManager.Instance.OpenMiddleMessageLocalization(_messageID);

        _middleMessage.ActionOneButton.onClick.AddListener(OnButtonOneClick);
        _middleMessage.ActionTwoButton.onClick.AddListener(OnButtonTwoClick);
        _middleMessage.CloseButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnButtonOneClick()
    {
        _middleMessage.ActionOneButton.onClick.AddListener(OnButtonOneClick);
        XnodeStepsRunner.Instance.StepIsDone("buttonOne");
    }

    private void OnButtonTwoClick()
    {
        _middleMessage.ActionTwoButton.onClick.AddListener(OnButtonTwoClick);
        XnodeStepsRunner.Instance.StepIsDone("buttonTwo");
    }

    private void OnCloseButtonClick()
    {
        _middleMessage.CloseButton.onClick.AddListener(OnCloseButtonClick);
        XnodeStepsRunner.Instance.StepIsDone("closeButton");
    }
}