using UnityEngine;

[NodeTint("#921A40"), CreateNodeMenu("Control Flow/Switch/Switch Enum Step", 1)]
public class SwitchEnumStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never, ConnectionType.Override)] public NodeObject entry;

    public string enumTypeName;
    [HideInInspector] public string previousEnum;
    private string _portValue;

    [Output] public bool Default;

    public override void PrepareStep()
    {
        base.PrepareStep();
        GetInputPortValue("entry");
        Execute();
    }

    public override void Execute()
    {
        base.Execute();

        if (this.GetOutputPort(_portValue) != null)
            XnodeStepsRunner.Instance.StepIsDone(_portValue);
        else
            XnodeStepsRunner.Instance.StepIsDone("Default");
    }

    private void GetInputPortValue(string portName)
    {
        NodeObject value = (NodeObject)GetConnectedInputPortValue(this, portName);

        _portValue = (string)value.value;
    }
}