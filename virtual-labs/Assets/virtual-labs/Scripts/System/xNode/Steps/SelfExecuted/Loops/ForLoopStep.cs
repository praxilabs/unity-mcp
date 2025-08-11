using UnityEngine;

[NodeTint("#3A1078"), CreateNodeMenu("Loops/For loop", 0)]
public class ForLoopStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;
    public int iterations = 0;

    [Output] public NodeObject continueLoop;
    [Output] public NodeObject exit;
    private int _counter;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        if (_counter < iterations)
        {
            _counter++;
            XnodeStepsRunner.Instance.StepIsDone("continueLoop");
        }
        else
        {
            _counter = 0;
            XnodeStepsRunner.Instance.StepIsDone();
        }
    }
}
