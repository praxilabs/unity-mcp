using Praxilabs.UIs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeTint("#424242"), CreateNodeMenu("Intro-End Messages", 5)]
public class IntroEndMessagesStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [Output] public NodeObject exit;

    // [SerializeField] private IntroEndMessageType messageType;
    [SerializeField] private int messageIndex;
    private IntroEndMessagesManager _introEndManager;

    public override void PrepareStep()
    {
        base.PrepareStep();
        Execute();
    }

    public override void Execute()
    {
        _introEndManager = FindObjectOfType<IntroEndMessagesManager>(true);

        if (_introEndManager == null)
        {
            XnodeStepsRunner.Instance.StepIsDone();
        }
        else
        {
            _introEndManager.Open(messageIndex);
        }
    }

}
