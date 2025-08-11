using Praxilabs.Input;
using UnityEngine;

[NodeTint("#384B70"), CreateNodeMenu("Attach-Detach/Detach", 1)]
public class DetachStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(GameObject), "Follower")]
    private RegistryItem _followerName;
    private string _followerParent => _followerName.prefabName;
    public string followerName => _followerName.childName;

    [Output] public NodeObject exit;
    
    private GameObject _followerObject;

    public override void PrepareStep()
    {
        base.PrepareStep();
        ResolveObjects();
        Execute();
    }

    public override void Execute()
    {
        _followerObject.GetComponent<DraggableObject>().resetPositionOnDrop = true;
        _followerObject.GetComponent<DraggableObject>().ResetPosition();
        _followerObject.GetComponent<DraggableObject>().ResetRotation();

        _followerObject.tag = "Draggable";

        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _followerObject = ExperimentItemsContainer.Instance.Resolve(_followerParent, followerName);
    }
}
