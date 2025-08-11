using System.Collections;
using UnityEngine;

[NodeTint("#077A7D"), CreateNodeMenu("Animation Step", 1)]
public class AnimationStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField, RegistryDropdown(typeof(Animator), "Animator")]
    private RegistryItem _targetName;

    [SerializeField] private string _animationTriggerName;
    [SerializeField] private float _numberOfRounds = 1;
    [SerializeField] private bool _dontCallEnd = false;

    [Header("Freeze Settings")]
    [SerializeField] private bool _freezeUI = true;
    [SerializeField] private bool _freezeCameraMove = false;
    [SerializeField] private bool _freezeCameraRotate = false;

    [Output] public NodeObject exit;

    private string _targetParent => _targetName.prefabName;
    public string targetName => _targetName.childName;
    private Animator _animator;

    public override void PrepareStep()
    {
        base.PrepareStep();

        ResolveObjects();
        ApplyFreezeSettings();
        Execute();
    }

    private void ApplyFreezeSettings()
    {
        if (_freezeUI)
            UIManager.Instance.ToggleTargetUI(false);
        if (_freezeCameraMove)
            Praxilabs.CameraSystem.CameraManager.Instance.FreezeCameraMove(true);
        if (_freezeCameraRotate)
            Praxilabs.CameraSystem.CameraManager.Instance.FreezeCameraRotate(true);
    }

    private void RevertFreezeSettings()
    {
        if (_freezeUI)
            UIManager.Instance.ToggleTargetUI(true);
        if (_freezeCameraMove)
            Praxilabs.CameraSystem.CameraManager.Instance.FreezeCameraMove(false);
        if (_freezeCameraRotate)
            Praxilabs.CameraSystem.CameraManager.Instance.FreezeCameraRotate(false);
    }

    public override void Execute()
    {
        float animationTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        XnodeCoroutineManager.Instance.StartCoroutine(RunAnimationCoroutine(animationTime));
    }

    private IEnumerator RunAnimationCoroutine(float animationTime)
    {
        _animator.SetTrigger(_animationTriggerName);
        yield return new WaitForSeconds(animationTime * _numberOfRounds);

        if (!_dontCallEnd)
            _animator.SetTrigger("End");

        Exit();
    }

    public override void Exit()
    {
        RevertFreezeSettings();
        base.Exit();
        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _animator = ExperimentItemsContainer.Instance.Resolve(_targetParent, targetName).GetComponent<Animator>();
    }
}
