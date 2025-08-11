using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[NodeTint("#40534C"), CreateNodeMenu("Function Call/Function Call Coroutine", 2)]
public class FunctionCallCoroutineStep : FunctionCallBase
{
    [SerializeField] private bool _canGoToNextStep = true;
    private UnityAction _exitInMiddleDelegate;

    public override void PrepareStep()
    {
        if (_exitInMiddleDelegate == null)
            _exitInMiddleDelegate = () => ExitInMiddle();
        _canGoToNextStep = true;
        EventsManager.Instance.AddListener(FunctionCallEvents.GoToNextStep, _exitInMiddleDelegate);
        base.PrepareStep();
    }

    public override void Exit()
    {
        EventsManager.Instance.RemoveListener(FunctionCallEvents.GoToNextStep, _exitInMiddleDelegate);
        XnodeStepsRunner.Instance.StepIsDone();
    }

    private void ExitInMiddle()
    {
        _canGoToNextStep = false;
    }

    protected override void InvokeFunction()
    {
        XnodeCoroutineManager.Instance.StartCoroutine(InvokeCoroutine());
    }

    private IEnumerator InvokeCoroutine()
    {
        if (_canGoToNextStep)
        {
            XnodeCoroutineManager.Instance.StartCoroutine((IEnumerator)_selectedfunction.Invoke(_selectedComponent, _selectedParameters));
            yield return ExecuteCoroutine();
            yield break;
        }

        Debug.Log("Coroutine invocation completed.");
    }

    private IEnumerator ExecuteCoroutine()
    {
        IEnumerator coroutine = (IEnumerator)_selectedfunction.Invoke(_selectedComponent, _selectedParameters);

        while (_canGoToNextStep)
            yield return coroutine.Current;
        Exit();
        Debug.Log("invoked");
    }
}