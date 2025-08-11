using UnityEngine;

[NodeTint("#3D0301"), CreateNodeMenu("Utility/Toggle Device Colliders", 0)]
public class ToggleDeviceCollidersStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(DeviceSideController), "Device Collider")]
    private RegistryItem _toolName;

    private string _toolParent => _toolName.prefabName;
    public string toolName => _toolName.childName;
    public bool toggle;

    [Output] public NodeObject exit;

    private DeviceSideController _deviceSideController;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        _deviceSideController.ToggleColliders(toggle);
        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        GameObject toolGO = ExperimentItemsContainer.Instance.Resolve(_toolParent, toolName);
        _deviceSideController = toolGO.GetComponentInParent<DeviceSideController>();
    }
}