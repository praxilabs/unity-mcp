using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[NodeTint("#B43F3C"), CreateNodeMenu("Tools/Tools Inputfield", 0)]
public class ToolsInputField : ActionExecuted
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(Collider), "Tool")]
    private RegistryItem _targetName;

    [HideInInspector] public GameObject clickableObject;

    [Output] public NodeObject exit;
    [Input] public float desiredValueInput;

    private string _targetParent => _targetName.prefabName;
    public string targetName => _targetName.childName;

    public ToolsInputFieldManager.ToolsInputFieldUnits unit;
    public Vector3 uiPosition;
    private ToolsInputFieldManager _toolsInputfieldManager;


    public override void PrepareStep()
    {
        base.PrepareStep();
        PrepareTools();
    }

    public override void Execute(GameObject obj)
    {
       Debug.Log($"Value is Correct");
       Exit();
    }

    public override void Exit()
    {
        ToolsFlashManager.Instance.StopFlashing(clickableObject);
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void PrepareTools()
    {
        ResolveObjects();

        GameObject _toolsInputFieldCanvas = FindObjectOfType<ToolsInputFieldInstancer>().InstanceToolsInputCanvas();
        _toolsInputFieldCanvas.transform.SetParent(clickableObject.transform);

        _toolsInputfieldManager = _toolsInputFieldCanvas.GetComponentInChildren<ToolsInputFieldManager>();

        _toolsInputfieldManager.PredefinedValue = desiredValueInput;
        _toolsInputfieldManager.Unit = unit.ToString();
        _toolsInputfieldManager.Init(uiPosition);
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        clickableObject = ExperimentItemsContainer.Instance.Resolve(_targetParent, targetName);
    }
}