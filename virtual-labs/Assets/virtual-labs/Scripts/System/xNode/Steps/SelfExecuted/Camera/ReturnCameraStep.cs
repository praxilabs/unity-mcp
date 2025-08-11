using Praxilabs.CameraSystem;
using UnityEngine;

[NodeTint("#901E3E"), CreateNodeMenu("Camera/Return Camera", 1)]
public class ReturnCameraStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;
    [Output] public NodeObject exit;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        ReturnToCameraSystem();
        Exit();
    }

    private void ReturnToCameraSystem()
    {
        if (CameraManager.Instance.tempCurrentCamera == null)
        {
            Debug.Log("There is no camera to return to");
            return;
        }

        CameraManager.Instance.currentCamera.Priority = 1;
        CameraManager.Instance.tempCurrentCamera.Priority = 0;
    }

    public override void Exit()
    {
        IgnoreStep();
        ReturnOriginalTag();
        RevertFreezeSettings();

        CameraManager.Instance.tempCurrentCamera = null;
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void RevertFreezeSettings()
    {
        UIManager.Instance.ToggleTargetUI(true);
        CameraManager.Instance.FreezeCameraMove(false);
        CameraManager.Instance.FreezeCameraRotate(false);
    }

    private void ReturnOriginalTag()
    {
        ToolsCameraTarget targetCameraParent = CameraManager.Instance.tempCurrentCamera.transform.parent.GetComponent<ToolsCameraTarget>();

        if (targetCameraParent == null) return;

        if(targetCameraParent.targetTag != "Untagged")
            targetCameraParent.tag = targetCameraParent.targetTag;
    }
}
