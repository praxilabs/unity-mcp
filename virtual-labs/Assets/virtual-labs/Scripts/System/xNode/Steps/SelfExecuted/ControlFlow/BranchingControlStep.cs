using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class BranchingControlStep : SelfExectutedStep, IProvideActionSteps
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;
    
    [Output] public NodeObject routes;
    [Output] public NodeObject exit;

    protected Action<string> _routeChosenDelegate;
    protected List<string> _performedStepsId = new List<string>();

    protected bool _isFirstRound = true;

    public override void PrepareStep()
    {
        base.PrepareStep();
        ResolveObjects();
        Execute();
    }

    protected abstract void FirstIteration();

    protected bool AreBranchesCompleted()
    {
        return _performedStepsId.Count == xNodeUtility.GetConnectedNodes(this, "routes").Count;
    }

    protected void UpdateAvailableSteps() 
    {
        List<StepNode> steps = xNodeUtility.GetConnectedNodes(this, "routes");
        List<StepNode> temp = new List<StepNode>();

        for (int i = 0; i < steps.Count; i++)
        {
            if (!_performedStepsId.Contains(steps[i].stepId))
                temp.Add(steps[i]);
        }
        XnodeStepsRunner.Instance.ProvideAvailableSteps(temp);
    }

    protected void RouteChoosen(string performedStepID)
    {
        _performedStepsId.Add(performedStepID);
        List<StepNode> steps = new List<StepNode>();
        steps = xNodeUtility.GetConnectedNodes(this, "routes");
        xNodeUtility.GetConnectedStepById(steps, performedStepID).StepIsPerformed -= _routeChosenDelegate;
    }

    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        if (from.node == this && from.fieldName == "routes" && !(to.node is ActionExecuted))
            from.Disconnect(to);
    }
}
