using UnityEngine;

[NodeTint("#735557"), CreateNodeMenu("Control Flow/First Then Step", 11)]
public class FirstThenStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [Output] public bool first;
    public bool _isFirstIteration;
    [Output] public bool then;


    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        base.Execute();

        if (_isFirstIteration)
        {
            _isFirstIteration = false;
            XnodeStepsRunner.Instance.StepIsDone("first");
        }
        else
            XnodeStepsRunner.Instance.StepIsDone("then");
    }
}
