using Praxilabs.Timekeeping.Timer;
using System;
using System.Collections.Generic;
using UnityEngine;

[NodeTint("#4A4947"), CreateNodeMenu("UI/Timer", 0)]
public class TimerStep : SelfExectutedStep
{
    [Input(ShowBackingValue.Never)] public NodeObject entry;

    [SerializeField] private float _initialSpeedFactor;
    [SerializeField] private List<float> _speedFactors = new List<float>();
    [SerializeField] private TimeSpanWrapper _time;
    [SerializeField] private bool _isSkippable;

    [Output] public NodeObject exit;

    private event Action OnTimerEndDelegate;

    private TimerHandler _timeHandeler;

    public override void PrepareStep()
    {
        base.PrepareStep();

        ResolveObjects();
        Execute();
    }

    public override void Execute()
    {
        TimeSpan timeSpan = _time.TimeSpanValue;
        _timeHandeler.Setup(_initialSpeedFactor, _speedFactors, timeSpan, _isSkippable);
        _timeHandeler.PlayInstantly();
        OnTimerEndDelegate += () => Exit();
        _timeHandeler.timer.OnTimerEnded += OnTimerEndDelegate;
    }

    public override void Exit()
    {
        _timeHandeler.timer.OnTimerEnded -= OnTimerEndDelegate;
        XnodeStepsRunner.Instance.StepIsDone();
    }

    public override void ResolveObjects()
    {
        base.ResolveObjects();
        _timeHandeler = ExperimentItemsContainer.Instance.Resolve<TimerHandler>();
    }
}

[Serializable]
public class TimeSpanWrapper
{
    [Range(0, 23)] public int hours;
    [Range(0, 59)] public int minutes;
    [Range(0, 59)] public int seconds;

    public TimeSpan TimeSpanValue
    {
        get { return new TimeSpan(hours, minutes, seconds); }
        set
        {
            hours = value.Hours;
            minutes = value.Minutes;
            seconds = value.Seconds;
        }
    }
}