using UnityEngine;

[NodeTint("#674188"), CreateNodeMenu("Loops/While loop", 1)]
public class WhileLoopStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [Get] public NodeObject isTrue;

    [Output] public NodeObject continueLoop;
    [Output] public NodeObject exit;
    private bool _conditionValue;

    public override void PrepareStep()
    {
        base.PrepareStep();
        GetInputPortValue("isTrue");
        Execute();
    }

    public override void Execute()
    {
        if (_conditionValue)
            XnodeStepsRunner.Instance.StepIsDone("continueLoop");
        else
            XnodeStepsRunner.Instance.StepIsDone();
    }

    private void GetInputPortValue(string portName)
    {
        NodeObject value = (NodeObject)GetConnectedInputPortValue(this, portName);

        _conditionValue = (bool)value.value;
    }
}
