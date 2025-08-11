using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgressVisualizerBase : MonoBehaviour
{
    protected float _progress;
    protected float _totalProgress;
    /// <summary>
    /// Progress units done from total. for example \'3\' steps from 100.
    /// Automatically calls <see cref="UpdateVisualization"/>
    /// Make sure to set <see cref="TotalProgress"/>
    /// </summary>
    public float Progress 
    {
        set { _progress = value; UpdateVisualization(); }
        get { return _progress; }
    }
    public void SetProgress(float progress)
    {
        this.Progress = progress;
    }
    public float TotalProgress {
        set { _totalProgress = value; UpdateVisualization(); }
        get => _totalProgress;
    }

    public abstract void UpdateVisualization();
}
