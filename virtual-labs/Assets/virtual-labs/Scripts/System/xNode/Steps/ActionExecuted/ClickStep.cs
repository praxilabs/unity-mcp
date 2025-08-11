using UnityEngine;

[NodeTint("#B43F3F"), CreateNodeMenu("Click/Click Object", 0)]
public class ClickStep : ActionExecuted
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(Collider), "Clickable Target")]
    private RegistryItem _targetName;

    [HideInInspector] public GameObject clickableObject;

    [Output] public NodeObject exit;

    private string _targetParent => _targetName.prefabName;
    public string targetName => _targetName.childName;

    public override void PrepareStep()
    {
        if (!ignorePrepareStep)
            base.PrepareStep();

        PrepareTools();
    }

    public override void Execute(GameObject clickedOnObject)
    {
        if (!ignorePrepareStep)
            XnodeManager.Instance.CurrentStep = this;

        if (targetName == clickedOnObject.name)
        {
            Debug.Log($"Clicked {targetName}");
            Exit();
        }
    }

    public override void IgnoreStep()
    {
        if (clickableObject == null)
            ResolveObjects();

        ToolsFlashManager.Instance.StopFlashing(clickableObject);
        ToolsFlashManager.Instance.flashingTools.Remove(clickableObject);

        clickableObject.GetComponent<ClickableTool>().canClick = false;
    }

    public override void Exit()
    {
        IgnoreStep();
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void PrepareTools()
    {
        ResolveObjects();

        clickableObject.GetComponent<ClickableTool>().canClick = true;

        if (ToolsFlashManager.Instance.flashingTools.Contains(clickableObject))
            return;

        ToolsFlashManager.Instance.flashingTools.Add(clickableObject);
        ToolsFlashManager.Instance.StartFlashing(clickableObject);
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        clickableObject = ExperimentItemsContainer.Instance.Resolve(_targetParent, targetName);
    }
}