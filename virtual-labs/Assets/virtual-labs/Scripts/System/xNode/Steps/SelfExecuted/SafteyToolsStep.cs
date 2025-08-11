using UnityEngine;

[NodeTint("#3A3960"), CreateNodeMenu("UI/SafetyTools", 5)]
public class SafteyToolsStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    public GameObject safetyToolsPrefab;

    [Output] public NodeObject exit;

    private GameObject _safetyToolsObject;
    private SafetyToolsManager _safetyManager;

    public override void PrepareStep()
    {
        if (!ignorePrepareStep)
            base.PrepareStep();

        OpenSafetyTools();
        AddEvent();
    }

    public override void Exit()
    {
        _safetyManager.GoToLabButton.onClick.RemoveListener(Exit);

        CloseSafetyTools();
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void OpenSafetyTools()
    {
        _safetyToolsObject = Instantiate(safetyToolsPrefab);
        _safetyManager = _safetyToolsObject.GetComponent<SafetyToolsManager>();
    }

    private void AddEvent()
    {
        _safetyManager.GoToLabButton.onClick.AddListener(Exit);
    }

    private void CloseSafetyTools()
    {
        Destroy(_safetyToolsObject);
    }
}
