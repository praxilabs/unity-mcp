using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[NodeTint("#AF1740"), CreateNodeMenu("Click/Click UI Button", 1)]
public class UIClickStep : ActionExecuted
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(Button), "Button")]
    private RegistryItem _buttonName;

    private string _buttonParent => _buttonName.prefabName;
    public string buttonName => _buttonName.childName;

    [Output] public NodeObject exit;

    private UnityAction _stepExitDelegate;
    private Button _uiButton;

    public override void PrepareStep()
    {
        if (!ignorePrepareStep)
            base.PrepareStep();
        ResolveObjects();

        if(_stepExitDelegate != null)
            _uiButton.onClick.RemoveListener(_stepExitDelegate);
            
        _stepExitDelegate = () => 
        {
            Exit();
        };
        _uiButton.onClick.AddListener(_stepExitDelegate);
    }

    public override void IgnoreStep()
    {
        _uiButton.onClick.RemoveListener(_stepExitDelegate);
    }

    public override void Exit()
    {
        IgnoreStep();

        if (ignorePrepareStep)
            XnodeManager.Instance.CurrentStep = this;

        if(XnodeManager.Instance.CurrentStep == this)
            XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _uiButton = ExperimentItemsContainer.Instance.Resolve(_buttonParent, buttonName).GetComponent<Button>();
    }
}