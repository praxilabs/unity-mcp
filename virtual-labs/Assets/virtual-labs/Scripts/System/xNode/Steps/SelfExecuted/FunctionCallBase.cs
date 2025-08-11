using System.Reflection;
using UnityEngine;
using System;

[CreateNodeMenu("")]
public class FunctionCallBase : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(GameObject), "Target Object")]
    private RegistryItem _calledObjectName;

    [HideInInspector] public string previousCalledObjectName;

    [HideInInspector] public GameObject selectedChild;
    [HideInInspector] public string prefabName => _calledObjectName.prefabName;
    [HideInInspector] public string childName => _calledObjectName.childName;

    [HideInInspector] public string selectedComponent;
    [HideInInspector] public string selectedFunction;

    [HideInInspector] public SerializableParameter[] parameterValues = new SerializableParameter[0];
    [Output] public NodeObject exit;

    protected MonoBehaviour _selectedComponent;
    protected MethodInfo _selectedfunction;
    protected object[] _selectedParameters;

    protected object _methodReturnValue;
    protected NodeObject _outputValue;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        base.Execute();
        selectedChild = ExperimentItemsContainer.Instance.Resolve(prefabName, childName);
        InvokeSelectedFunction();
    }

    public void InvokeSelectedFunction()
    {
        if (!GetFunction())
            return;
#if UNITY_EDITOR
        InvokeFunction();
#else
        try
        {
            InvokeFunction();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error invoking function {_selectedfunction}: {ex.Message}");
        }
#endif
    }

    private bool GetFunction()
    {
        if (selectedComponent is null) return false;

        _selectedComponent = selectedChild.GetComponent(selectedComponent) as MonoBehaviour;
        _selectedfunction = GetFunctionByName();

        if (_selectedfunction is null) return false;

        _selectedParameters = new object[parameterValues.Length];
        for (int i = 0; i < parameterValues.Length; i++)
            _selectedParameters[i] = parameterValues[i].GetValue();

        return true;
    }

    private MethodInfo GetFunctionByName()
    {
        return _selectedComponent.GetType().GetMethod(selectedFunction,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    }

    protected virtual void InvokeFunction()
    {
        object methodReturnValue = _selectedfunction.Invoke(_selectedComponent, _selectedParameters);
        _outputValue = new NodeObject(methodReturnValue);

        Debug.Log($"Method {_selectedfunction} invoked successfully. Return Value: {methodReturnValue}");
        SetOutputPortValue();

        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void SetOutputPortValue()
    {
        exit = _outputValue;
    }

    public override object GetValue(XNode.NodePort port)
    {
        return exit;
    }

    public void ResetSelection()
    {
        selectedChild = null;
        selectedComponent = null;

        parameterValues = new SerializableParameter[0];
        previousCalledObjectName = prefabName;
    }
}