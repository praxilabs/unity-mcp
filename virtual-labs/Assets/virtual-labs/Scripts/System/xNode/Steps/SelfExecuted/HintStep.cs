using Praxilabs.UIs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeTint("#424242"), CreateNodeMenu("Hint", 5)]
public class HintStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [Output] public NodeObject exit;

    [SerializeField] private int stepNumber;

    private HintsJsonHelper _helper;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        _helper = FindObjectOfType<HintsJsonHelper>(true);

        if (_helper == null)
        {
            XnodeStepsRunner.Instance.StepIsDone();
        }
        else
        {
            _helper.OpenHintsWithStep(stepNumber);
            XnodeStepsRunner.Instance.StepIsDone();
        }
    }

}
