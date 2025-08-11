using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeTint("#B98A4E"), CreateNodeMenu("Control Flow/OR Step", 0)]
public class ORStep : ActionExecuted, IProvideActionSteps
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [Output] public NodeObject routes;
    [Output] public NodeObject exit;

    private Action<string> _routeChosenDelegate;

    private bool _isRouteChosen = false;

    public override void PrepareStep()
    {
        ResolveObjects();
        Execute();
    }

    public override void Execute()
    {
        if(!_isRouteChosen)
            PrepareAvailableSteps();
        else
            Exit();
    }

    public override void Exit()
    {
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void PrepareAvailableSteps()
    {
        List<StepNode> steps = xNodeUtility.GetConnectedNodes(this, "routes");

        _routeChosenDelegate = RouteChoosen;

        foreach (var step in steps)
        {
            step.StepIsPerformed += _routeChosenDelegate;
        }

        XnodeStepsRunner.Instance.AddAdditionalSteps(steps);
    }

    private void RouteChoosen(string performedStepID)
    {
        _isRouteChosen = true;
        List<StepNode> steps = xNodeUtility.GetConnectedNodes(this, "routes");
        xNodeUtility.GetConnectedStepById(steps, performedStepID).StepIsPerformed -= _routeChosenDelegate;
    }

    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        if (from.node == this && from.fieldName == "routes" && !(to.node is ActionExecuted))
            from.Disconnect(to);
    }
}