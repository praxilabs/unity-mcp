using UnityEngine;

[NodeTint("#6E6E6E"), CreateNodeMenu("Debug/Log", 5)]
public class DebugLogStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;
    [Output] public NodeObject exit;

    public enum LogSeverity
    {
        Log,
        Warning,
        Error
    }

    [SerializeField] private LogSeverity _severity = LogSeverity.Log;
    [SerializeField, TextArea(3, 10)] private string _message = "Debug Log";

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        switch (_severity)
        {
            case LogSeverity.Log:
                Debug.Log(_message);
                break;
            case LogSeverity.Warning:
                Debug.LogWarning(_message);
                break;
            case LogSeverity.Error:
                Debug.LogError(_message);
                break;
        }

        XnodeStepsRunner.Instance.StepIsDone();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_message == null)
            throw new System.Exception($"{nameof(_message)} must not be null!");
    }
#endif
}
