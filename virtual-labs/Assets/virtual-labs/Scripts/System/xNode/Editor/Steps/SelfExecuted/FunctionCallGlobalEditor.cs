using System.Linq;
using System.Reflection;
using UnityEngine;
using XNode;

[CustomNodeEditor(typeof(FunctionCallGlobalStep))]
public class FunctionCallGlobalEditor : FunctionCallBaseEditor
{
    protected override void DisplayParameters()
    {
        if (_functionSelectionChanged)
            RefreshParameterPorts();

        ParameterInfo[] parameters = _selectedFunction.GetParameters();

        if (_functionCall.parameterValues == null || _functionCall.parameterValues.Length != parameters.Length)
            _functionCall.parameterValues = new SerializableParameter[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            ParameterInfo param = parameters[i];
            NodePort dynamicPort;
            string portName = param.Name;

            // Add a dynamic port for each parameter based on its type
            if (_functionCall.GetInputPort(portName) == null)
                dynamicPort = _functionCall.AddDynamicInput(typeof(NodeObject), XNode.Node.ConnectionType.Override, XNode.Node.TypeConstraint.Strict, portName);
            else
                dynamicPort = _functionCall.GetInputPort(portName);

            // Check the connected value's type and assign it to the parameter if types match
            if (dynamicPort.IsConnected)
            {
                // Fetch the connected port's value
                NodeObject nodeObject = dynamicPort.GetInputValue<NodeObject>();
                if (nodeObject != null && nodeObject.value != null)
                {
                    // Validate the type of the object inside NodeObject
                    string parameterTypeName = TypeHelper.GetFriendlyTypeName(param.ParameterType);
                    string connectedTypeName = TypeHelper.GetFriendlyTypeName(nodeObject.value.GetType());
                    if (parameterTypeName == connectedTypeName)
                        _functionCall.parameterValues[i].SetValue(nodeObject.value);
                    else
                        Debug.Log($"Type mismatch for parameter '{portName}'. Expected {parameterTypeName}, but got {connectedTypeName}.");
                }
            }
        }
    }

    private void RefreshParameterPorts()
    {
        foreach (var port in _functionCall.DynamicPorts.ToList())
            _functionCall.RemoveDynamicPort(port.fieldName);
    }
}