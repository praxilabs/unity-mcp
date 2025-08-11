using UnityEngine;

[NodeTint("#3D0301"), CreateNodeMenu("Utility/Toggle Collider", 0)]
public class ToggleColliderStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(Collider), "Tool Collider")]
    private RegistryItem _toolName;

    private string _toolParent => _toolName.prefabName;
    public string toolName => _toolName.childName;
    public bool toggle;

    [Output] public NodeObject exit;

    private Collider _toolCollider;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        _toolCollider.enabled = toggle;
        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _toolCollider = ExperimentItemsContainer.Instance.Resolve(_toolParent, toolName).GetComponent<Collider>();
    }
}
