using Praxilabs.Input;
using UnityEngine;

[NodeTint("#344C64"), CreateNodeMenu("Attach-Detach/Attach", 0)]
public class AttachStep : ActionExecuted
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(GameObject), "Follower")] 
    private RegistryItem _followerName;
    [SerializeField, RegistryDropdown(typeof(GameObject), "Receiver")] 
    private RegistryItem _receiverName;

    [HideInInspector] public GameObject followerObject;
    [HideInInspector] public GameObject receiverObject;

    [SerializeField] private Vector3 _attachPosition;
    [SerializeField] private bool _hasRotation;
    [SerializeField] private Vector3 _followerRotation;

    [Output] public NodeObject exit;

    private string _followerParent => _followerName.prefabName;
    public string followerName => _followerName.childName;
    private string _receiverParent => _receiverName.prefabName;
    public string receiverName => _receiverName.childName;

    public bool justDrag;

    public override void PrepareStep()
    {
        if (!ignorePrepareStep)
            base.PrepareStep();

        PrepareTools();
    }

    public override void Execute()
    {
        if (!ignorePrepareStep)
            XnodeManager.Instance.CurrentStep = this;

        if (!justDrag)
        {
            followerObject.tag = "Untagged";

            DragDropManager.Instance.Drop(false);

            followerObject.transform.localPosition = _attachPosition;

            if (_hasRotation)
                followerObject.transform.eulerAngles = _followerRotation;

            Debug.Log($"Attached {followerName} to {receiverName}");
            Exit();
        }
        else
        {
            followerObject.GetComponent<DraggableObject>().resetPositionOnDrop = true;
            Exit();
        }
    }

    public override void IgnoreStep()
    {
        if (followerObject == null || receiverObject == null)
            ResolveObjects();

        ToolsFlashManager.Instance.StopFlashing(followerObject);
        ToolsFlashManager.Instance.StopFlashing(receiverObject);
        ToolsFlashManager.Instance.flashingTools.Remove(followerObject);
        ToolsFlashManager.Instance.flashingTools.Remove(receiverObject);

        followerObject.GetComponent<AttachableTool>().canAttach = false;
    }

    public override void Exit()
    {
        IgnoreStep();
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void PrepareTools()
    {
        ResolveObjects();

        if (!ToolsFlashManager.Instance.flashingTools.Contains(followerObject))
            ToolsFlashManager.Instance.flashingTools.Add(followerObject);
        if (!ToolsFlashManager.Instance.flashingTools.Contains(receiverObject))
            ToolsFlashManager.Instance.flashingTools.Add(receiverObject);

        followerObject.GetComponent<AttachableTool>().canAttach = true;

        ToolsFlashManager.Instance.StartFlashing(followerObject);
        ToolsFlashManager.Instance.StartFlashing(receiverObject);
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();

        followerObject = ExperimentItemsContainer.Instance.Resolve(_followerParent, followerName);
        receiverObject = ExperimentItemsContainer.Instance.Resolve(_receiverParent, receiverName);
    }
}