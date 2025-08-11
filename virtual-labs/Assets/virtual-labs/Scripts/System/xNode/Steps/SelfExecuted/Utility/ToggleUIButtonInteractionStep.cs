using UnityEngine;
using UnityEngine.UI;

[NodeTint("#3D0301"), CreateNodeMenu("Utility/Toggle UI Button Interaction", 1)]
public class ToggleUIButtonInteractionStep : ActionExecuted
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(Button), "Button")]
    private RegistryItem _buttonName;
    private string _buttonParent => _buttonName.prefabName;
    public string buttonName => _buttonName.childName;

    [SerializeField] private bool _enable;

    [Output] public NodeObject exit;

    private Button _uiButton;

    public override void PrepareStep()
    {
        base.PrepareStep();
        ResolveObjects();

        _uiButton.interactable = _enable;

        Exit();
    }

    public override void Exit()
    {
        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _uiButton = ExperimentItemsContainer.Instance.Resolve(_buttonParent, buttonName).GetComponent<Button>();
    }
}
