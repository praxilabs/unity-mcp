using System;
using System.Collections.Generic;
using UnityEngine;

public class XnodeManager : Singleton<XnodeManager>
{
    public StepsGraph runningGraph;
    [SerializeField] private StepNode _currentStep;
    public StepNode CurrentStep
    {
        get { return _currentStep; }
        set
        {
            if (_currentStep != null && _currentStep != value && !isAutomated)
                _currentStep.IgnoreStep();

            _currentStep = value;
        }
    }

    public bool isAutomated;

    public List<string> executedSteps = new List<string>();
    public List<StepNode> subGraphSteps = new List<StepNode>();

    private Dictionary<string, StepsGraph> _copiedGraphs = new Dictionary<string, StepsGraph>();
    private Action _prepareForNewStageAction;

    private void OnEnable()
    {
        _prepareForNewStageAction = PrepareForNewStage;

        ExperimentManager.OnStageStart += _prepareForNewStageAction;
    }

    private void OnDisable()
    {
        ExperimentManager.OnStageStart -= _prepareForNewStageAction;
    }

    /// <summary>
    /// we use getStepyByID to get the first step instead of assigning it directly, because this way we'll
    /// get the first step in the copied graph, if we assign it directly we'll get the first step in the original
    /// </summary>
    public void StartGraph(StepsGraph currentGraph)
    {
        if (_copiedGraphs.ContainsKey(currentGraph.graphID))
            runningGraph = _copiedGraphs[currentGraph.graphID];
        else
        {
            runningGraph = currentGraph.Copy() as StepsGraph;
            _copiedGraphs.Add(currentGraph.graphID, runningGraph);
        }

        _currentStep = runningGraph.GetStepById(currentGraph.firstStep.stepId);
        XnodeStepsRunner.Instance.ExecuteNextStep();
    }

    private void PrepareForNewStage()
    {
        _copiedGraphs.Clear();
        executedSteps.Clear();
        subGraphSteps.Clear();
        XnodeStepsRunner.Instance.availableSteps.Clear();
    }
}