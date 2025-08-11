using System.Collections;
using UnityEngine;

[NodeTint("#982B1C"), CreateNodeMenu("Click/Rotate Knob", 1)]
public class RotateKnobStep : ActionExecuted
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [HideInInspector] public bool isExecuted = false;

    [SerializeField, RegistryDropdown(typeof(EnhancedKnobController), "Knob")]
    private RegistryItem _knobName;

    private string _knobParent => _knobName.prefabName;
    public string knobName => _knobName.childName;

    public float knobAngle;
    public float delayTime = 1.5f;

    private GameObject _knobObject;
    private EnhancedKnobController _knob;

    [Output] public NodeObject exit;

    public override void PrepareStep()
    {
        if (!ignorePrepareStep)
            base.PrepareStep();

        PrepareTools();
    }

    public override void Execute()
    {
        if (isExecuted) return;

        if (!ignorePrepareStep)
            XnodeManager.Instance.CurrentStep = this;

        isExecuted = true;
        
        XnodeCoroutineManager.Instance.StartCoroutine(ValidateKnobValue());
    }

    private IEnumerator ValidateKnobValue()
    {
        while (true)
        {
            if (_knob.AccumulatedAngle == knobAngle)
            {
                Exit();
                yield break;
            }
            yield return new WaitForSeconds(delayTime);
        }
    }

    public override void IgnoreStep()
    {
        _knob.canRotateFromStep = false;
        StopFlashing();
    }

    public override void Exit()
    {
        isExecuted = false;
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void PrepareTools()
    {
        ResolveObjects();
        _knob.canRotateFromStep = true;
        if (ToolsFlashManager.Instance.flashingTools.Contains(_knobObject))
            return;

        if (ToolsFlashManager.Instance.flashingTools.Contains(_knobObject))
            return;

        ToolsFlashManager.Instance.flashingTools.Add(_knobObject);
        ToolsFlashManager.Instance.StartFlashing(_knobObject);
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();

        _knobObject = ExperimentItemsContainer.Instance.Resolve(_knobParent, knobName);

        if (_knobObject.GetComponent<EnhancedKnobController>() is null)
        {
            Debug.LogError($"{_knobObject} doesn't have EnhancedKnobController component");
            return;
        }

        _knob = _knobObject.GetComponent<EnhancedKnobController>();
    }

    public void StopFlashing()
    {
        ToolsFlashManager.Instance.StopFlashing(_knobObject);
        ToolsFlashManager.Instance.flashingTools.Remove(_knobObject);
    }
}