using UnityEngine;

[NodeTint("#B43F3F"), CreateNodeMenu("Tools/Tools Interactions Menu", 0)]
public class ToolsInteractionsMenuStep : ActionExecuted
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(Collider), "Tool")]
    private RegistryItem _targetName;

    [HideInInspector] public GameObject clickableObject;

    [Output] public NodeObject exit;

    private string _targetParent => _targetName.prefabName;
    public string targetName => _targetName.childName;


    public string ButtonToActivate;


    public override void PrepareStep()
    {
        if (!ignorePrepareStep)
            base.PrepareStep();

        PrepareTools();
    }

    public override void Execute(GameObject clickedOnObject)
    {
        if (!ignorePrepareStep) XnodeManager.Instance.CurrentStep = this;

        Debug.Log($"Clicked From Interactions Menu {clickedOnObject.name}");
        Exit();
    }

    public override void Exit()
    {
        XnodeStepsRunner.Instance.StepIsDone();
        Debug.Log($"Exited From Interactions Menu ");
    }

    private void PrepareTools()
    {
        ResolveObjects();

        clickableObject.GetComponent<ClickableTool>().canClick = true;
        clickableObject.GetComponentInChildren<ToolsInteractionsClickerHelper>().InitializeMenu(clickableObject.GetComponentInChildren<ToolsInteractionsMenu>(true), ButtonToActivate); //ButtonToEnable.ToString());

    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        clickableObject = ExperimentItemsContainer.Instance.Resolve(_targetParent, targetName);
    }

}

