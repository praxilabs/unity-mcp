using UnityEngine;

[NodeTint("#664343"), CreateNodeMenu("GameObject Set Active", 1)]
public class GameObjectSetActiveStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;
    [SerializeField, RegistryDropdown(typeof(GameObject), "Game Object")]
    private RegistryItem _newToolName;
    [SerializeField] private bool _isActive = true;
    private string _newToolParent => _newToolName.prefabName;
    public string newToolName => _newToolName.childName;


    [Output] public NodeObject exit;

    private GameObject _newToolObject;

    public override void PrepareStep()
    {
        base.PrepareStep();
        ResolveObjects();
        Execute();
    }

    public override void Execute()
    {
        _newToolObject.SetActive(_isActive);
        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _newToolObject = ExperimentItemsContainer.Instance.Resolve(_newToolParent, newToolName);
    }
}
