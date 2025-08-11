using UnityEngine;
using XNode;

[NodeWidth(260)]
[NodeTint("#313131"), CreateNodeMenu("")]
public class VariableNode : SelfExectutedStep
{
    [Get(ShowBackingValue.Never)] public NodeObject get;
    [Set(ShowBackingValue.Never)] public NodeObject set;

    [HideInInspector] public GlobalVariables data;

    private void OnValidate()
    {
        ApplyToSetPort();
        GetValue();
        UpdateAllVariables();
    }

    public override void PrepareStep()
    {
        ApplyToSetPort();
        GetValue();
        UpdateAllVariables();
        Execute();
    }

    public override void Execute()
    {
        XnodeStepsRunner.Instance.StepIsDone("set");
    }

    private void GetValue()
    {
        NodePort inputPort = GetInputPort("get");
        if (get == null)
            return;

        if (inputPort.IsConnected)
            ApplyFromGetPort(inputPort);
        else
            ApplyToSetPort();
    }

    private void ApplyFromGetPort(NodePort inputPort)
    {
        StepNode connectedNode = inputPort.Connection.node as StepNode;
        foreach (NodePort output in connectedNode.Outputs)
        {
            if (output.IsConnectedTo(inputPort))
            {
                NodeObject connectedNodeValue = output.GetOutputValue() as NodeObject;
                if (connectedNodeValue == null || connectedNodeValue.value == null)
                    continue;

                string connectedPortValueType = TypeHelper.GetFriendlyTypeName(connectedNodeValue.value.GetType());
                string inputValueType = TypeHelper.GetFriendlyTypeName(data.Value.GetType());
                if (connectedPortValueType == inputValueType)
                {
                    get.value = connectedNodeValue.value;
                    data.Value = get.value;
                    set.value = data.Value;
                }
            }
        }
    }

    private void ApplyToSetPort()
    {
        set.value = data.Value;
    }

    private void UpdateAllVariables()
    {
        ((StepsGraph)graph).UpdateGlobalVariableValue(data.name, data.Value);
    }

    public void UpdateVariableValue(object value)
    {
        data.Value = value;
    }

    public void Init(GlobalVariables customData)
    {
        NodeObject value = new NodeObject();
        data = customData;
        name = data.name;

        value.value = data.Value;
        set = value;
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == nameof(set))
        {
            // Directly assign the value from GlobalVariables to the output port
            if (get != null)
            {
                set.value = data.Value;
                return set;
            }
        }
        return null;
    }
}