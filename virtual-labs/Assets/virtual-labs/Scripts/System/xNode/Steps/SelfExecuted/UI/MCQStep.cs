using System.Collections;
using System.Collections.Generic;
using PraxiLabs.MCQ;
using UnityEngine;

[NodeTint("#7D736A"), CreateNodeMenu("UI/MCQ", 2)]
public class MCQStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    public int questionNumber = -1;

    [Output] public NodeObject exit;

    private MCQWrapper _mcqWrapper;

    public override void PrepareStep()
    {
        base.PrepareStep();
        ResolveObjects();
        Execute();
    }

    public override void Execute()
    {
        if (questionNumber <= 0)
            Debug.LogError("Insert a quesiton number");
        else
        {
            _mcqWrapper.ShowQuestion(questionNumber);
            _mcqWrapper.GetCloseButton().onClick.AddListener(Exit);
        }
    }

    public override void Exit()
    {
        _mcqWrapper.GetCloseButton().onClick.RemoveListener(Exit);
        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _mcqWrapper = ExperimentItemsContainer.Instance.Resolve<MCQWrapper>();
    }
}
