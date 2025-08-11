using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : ProgressVisualizerBase
{
    [SerializeField] private UnityEngine.UI.Slider _slider;
    public override void UpdateVisualization()
    {
        _slider.value = _progress / _totalProgress;
    }
}
