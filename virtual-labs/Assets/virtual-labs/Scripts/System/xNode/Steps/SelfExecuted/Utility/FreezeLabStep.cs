using UnityEngine;

[NodeTint("#7D6F07"), CreateNodeMenu("Utility/Freeze Lab Step", 0)]
public class FreezeLabStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [Header("Freeze Settings")]
    [SerializeField] private bool _freezeUI = false;
    [SerializeField] private bool _freezeCameraMove = false;
    [SerializeField] private bool _freezeCameraRotate = false;

    [Output] public NodeObject exit;


    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        ApplyFreezeSettings();
        Exit();
    }

    public override void Exit()
    {
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void ApplyFreezeSettings()
    {
        // Freeze UI
        if(_freezeUI)
            UIManager.Instance.ToggleTargetUI(false);
        else
            UIManager.Instance.ToggleTargetUI(true);

        // Freeze Camera Move
        if (_freezeCameraMove)
            Praxilabs.CameraSystem.CameraManager.Instance.FreezeCameraMove(true);
        else
            Praxilabs.CameraSystem.CameraManager.Instance.FreezeCameraMove(false);

        // Freeze Camera Rotate
        if (_freezeCameraRotate)
            Praxilabs.CameraSystem.CameraManager.Instance.FreezeCameraRotate(true);
        else
            Praxilabs.CameraSystem.CameraManager.Instance.FreezeCameraRotate(false);
    }
}
