using Praxilabs.CameraSystem;
using UnityEngine;

[NodeTint("#901E3E"), CreateNodeMenu("Camera/Focus On Tool", 0)]
public class FocusOnToolStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(ToolsCameraTarget), "Target Tool")]
    private RegistryItem _targetName;

    [Header("Freeze Settings")]
    [SerializeField] private bool _freezeUI = true;
    [SerializeField] private bool _freezeCameraMove = false;
    [SerializeField] private bool _freezeCameraRotate = false;

    [Output] public NodeObject exit;

    private ToolsCameraTarget _targetTool;
    private string _targetParent => _targetName.prefabName;
    public string targetName => _targetName.childName;

    public override void PrepareStep()
    {
        base.PrepareStep();
        ResolveObjects();
        ApplyFreezeSettings();
        Execute();
    }

    public override void Execute()
    {
        GoToTool();
        ChangeTag();
        Exit();
    }

    private void GoToTool()
    {
        CameraManager.Instance.tempCurrentCamera = _targetTool.cameraTarget;

        CameraManager.Instance.currentCamera.Priority = 0;
        CameraManager.Instance.tempCurrentCamera.Priority = 1;
    }

    public override void Exit()
    {
        IgnoreStep();
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void ApplyFreezeSettings()
    {
        if (_freezeUI)
            UIManager.Instance.ToggleTargetUI(false);
        if (_freezeCameraMove)
            CameraManager.Instance.FreezeCameraMove(true);
        if (_freezeCameraRotate)
            CameraManager.Instance.FreezeCameraRotate(true);
    }

    private void ChangeTag()
    {
        if (_targetTool.targetTag != "Untagged")
            _targetTool.tag = "Untagged";
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _targetTool = ExperimentItemsContainer.Instance.Resolve(_targetParent, targetName).GetComponent<ToolsCameraTarget>();
    }
}
