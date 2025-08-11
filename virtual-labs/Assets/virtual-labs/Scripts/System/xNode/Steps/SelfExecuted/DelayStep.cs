using System.Collections;
using UnityEngine;

[NodeTint("#424242"), CreateNodeMenu("Delay", 5)]
public class DelayStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField] private float timeToWait;

    [Output] public NodeObject exit;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        XnodeCoroutineManager.Instance.StartCoroutine(DelayCoroutine(timeToWait));
    }

    IEnumerator DelayCoroutine(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        XnodeStepsRunner.Instance.StepIsDone();
    }
}
