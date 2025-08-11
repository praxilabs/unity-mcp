using UnityEngine;

[NodeTint("#5F99AE"), CreateNodeMenu("UI/Messages/Middle Message (One Action)", 1)]
public class MiddleMessageOneActionsStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField] private int _messageID;

    [Output] public NodeObject messageButton;
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

        _middleMessage.ActionOneButton.onClick.AddListener(OnMessageButtonClick);
        _middleMessage.CloseButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnMessageButtonClick()
    {
        _middleMessage.ActionOneButton.onClick.AddListener(OnMessageButtonClick);
        XnodeStepsRunner.Instance.StepIsDone("messageButton");
    }

    private void OnCloseButtonClick()
    {
        _middleMessage.CloseButton.onClick.AddListener(OnCloseButtonClick);
        XnodeStepsRunner.Instance.StepIsDone("closeButton");
    }
}
