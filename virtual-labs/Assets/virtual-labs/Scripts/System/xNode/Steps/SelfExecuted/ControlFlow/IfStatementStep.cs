using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeTint("#643843"), CreateNodeMenu("Control Flow/If Statement Step", 1)]
public class IfStatementStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [Output] public bool True;
    [Output] public bool False;

    private bool _portValue;

    public override void PrepareStep()
    {
        base.PrepareStep();
        GetInputPortValue("entry");
        Execute();
    }

    public override void Execute()
    {
        base.Execute();

        if (_portValue)
            XnodeStepsRunner.Instance.StepIsDone("True");
        else
            XnodeStepsRunner.Instance.StepIsDone("False");
    }

    private void GetInputPortValue(string portName)
    {
        NodeObject value = (NodeObject)GetConnectedInputPortValue(this, portName);

        _portValue = (bool)value.value;
    }
}
