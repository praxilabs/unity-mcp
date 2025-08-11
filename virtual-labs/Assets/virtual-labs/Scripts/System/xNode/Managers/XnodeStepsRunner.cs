using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

public class XnodeStepsRunner : Singleton<XnodeStepsRunner>
{
    public List<StepNode> availableSteps = new List<StepNode>();

    private string stepGraphName;
    public bool isAutomated;

    [HideInInspector] public UnityEvent E_PrepareSteps;

    public void StepIsDone(string nodePortName = "exit")
    {
        StopAllCoroutines();
        BranchingIgnoreSteps();
        availableSteps.Clear();

        if (XnodeManager.Instance.CurrentStep.StepIsPerformed != null)
        {
            XnodeManager.Instance.CurrentStep.StepIsPerformed.Invoke(XnodeManager.Instance.CurrentStep.stepId);
            XnodeManager.Instance.CurrentStep.StepIsPerformed = null;
        }

        XnodeManager.Instance.executedSteps.Add(XnodeManager.Instance.CurrentStep.stepId);

        GetNextNode(nodePortName);
    }

    public void BranchingIgnoreSteps()
    {
        for (int i = 0; i < availableSteps.Count; i++)
        {
            if (availableSteps[i].stepId != XnodeManager.Instance.CurrentStep.stepId)
                availableSteps[i].IgnoreStep();
        }
    }

    public void ProvideAvailableSteps(List<StepNode> steps)
    {
        availableSteps.Clear();
        availableSteps = steps;

        List<StepNode> stepNodes = new List<StepNode>(steps);

        foreach (StepNode step in stepNodes)
        {
            step.ignorePrepareStep = true;
            step.PrepareStep();
        }
    }

    public void AddAdditionalSteps(List<StepNode> steps)
    {
        foreach (StepNode step in steps)
        {
            step.ignorePrepareStep = true;
            step.PrepareStep();
            availableSteps.Add(step);
        }
    }

    private void GetNextNode(string portName)
    {
        StepNode nextNode = null;
        NodePort port = XnodeManager.Instance.CurrentStep.GetOutputPort(portName);

        if (port.IsConnected)
        {
            if (port.ConnectionCount > 1)
                nextNode = port.Connection.node as StepNode;
            else
                nextNode = port.Connection.node as StepNode;
        }
        else if (XnodeManager.Instance.subGraphSteps.Count > 0)
            ExitCurrentSubgraph();
        else
        {
            ExperimentManager.Instance.ToggleEndMessage(true);
            Debug.Log("Experiment Finished");
        }

        if (nextNode != null)
        {
            XnodeManager.Instance.CurrentStep = nextNode;
            ExecuteNextStep();
        }
    }

    public void ExecuteNextStep()
    {
        XnodeManager.Instance.CurrentStep.PrepareStep();
    }

    public void ExitCurrentSubgraph()
    {
        if (XnodeManager.Instance.subGraphSteps.Count > 0)
            XnodeManager.Instance.subGraphSteps[XnodeManager.Instance.subGraphSteps.Count - 1].Exit();
    }
}