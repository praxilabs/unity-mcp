using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeTint("#5F374B"), CreateNodeMenu("Control Flow/Composite Step", 0)]
public class CompositeStep : BranchingControlStep
{
    public bool resetSwitch;
    public bool executeOnce;

    public override void Execute()
    {
        XnodeManager.Instance.CurrentStep = this;

        if (_isFirstRound)
            FirstIteration();
        else
        {
            if (!AreBranchesCompleted())
                UpdateAvailableSteps();
            else
            {
                UpdateAvailableSteps();
                if (resetSwitch)
                    Reset();

                XnodeStepsRunner.Instance.StepIsDone();
            }
        }

        if (executeOnce)
            Reset();
    }

    protected override void FirstIteration()
    {
        List<StepNode> steps = xNodeUtility.GetConnectedNodes(this, "routes");
        _routeChosenDelegate = RouteChoosen;
        for (int i = 0; i < steps.Count; i++)
        {
            steps[i].StepIsPerformed += _routeChosenDelegate;
        }
        _isFirstRound = false;
        XnodeStepsRunner.Instance.ProvideAvailableSteps(steps);
    }

    private void Reset()
    {
        _isFirstRound = true;
        _performedStepsId.Clear();
    }
}