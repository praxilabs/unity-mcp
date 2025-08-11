using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCircle : ProgressVisualizerBase
{
    [SerializeField] private Image[] _circleFill;
    [SerializeField] private RectTransform _endCircle;
    [SerializeField] private bool _clockWise = true;
    public override void UpdateVisualization()
    {
        for (int i = 0; i < _circleFill.Length; i++)
        {
            _circleFill[i].fillAmount = _totalProgress == 0 ? 0 : _progress / _totalProgress;
        }
        
        if (_endCircle != null)
            _endCircle.rotation = Quaternion.Euler(Vector3.forward * (_progress / _totalProgress) * 360 * (_clockWise ? -1 : 1));
    }
}
