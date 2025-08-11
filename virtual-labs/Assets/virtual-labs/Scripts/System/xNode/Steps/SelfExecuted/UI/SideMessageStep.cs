using UnityEngine;

[NodeTint("#405D72"), CreateNodeMenu("UI/Messages/Side Message", 0)]
public class SideMessageStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    public int messageID;

    [Output] public NodeObject exit;

    private SideMessagesManager _sideMessagesManager;

    public override void PrepareStep()
    {
        base.PrepareStep();
        ResolveObjects();
        Execute();
    }

    public override void Execute()
    {
        _sideMessagesManager.OpenSideMessage(messageID);
        Exit();
    }

    public override void Exit()
    {
        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _sideMessagesManager = ExperimentItemsContainer.Instance.Resolve<SideMessagesManager>();
    }
}
