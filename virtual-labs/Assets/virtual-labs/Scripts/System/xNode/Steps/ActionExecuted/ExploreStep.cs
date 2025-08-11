using UnityEngine;

[NodeTint("#5F378C"), CreateNodeMenu("Explore Step", 99)]
public class ExploreStep : ActionExecuted
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;
    [Output] public NodeObject exit;

    [SerializeField] private int _groupIndex;

    public override void PrepareStep()
    {
        ExplorationHandler.Instance.PrepareExplorationPopup(_groupIndex, EndStep);
        XnodeManager.Instance.CurrentStep = this;
    }

    private void EndStep()
    {
        Exit();
    }

    public override void Exit()
    {
        XnodeStepsRunner.Instance.StepIsDone();
    }
}
