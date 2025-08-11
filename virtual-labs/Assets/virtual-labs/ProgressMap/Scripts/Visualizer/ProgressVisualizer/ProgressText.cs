using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressText : ProgressVisualizerBase
{
    [SerializeField, Tooltip("Text before progress number, like \'overall\'")] private string _prefix;
    [SerializeField, Tooltip("Text after progress number, like %")] private string _postfix = "%";
    [SerializeField] private TMPro.TextMeshProUGUI _progressText;
    [SerializeField] private int _roundDecimals = 2;
    [SerializeField] private bool _isLocalized = false;
    private LocalizedString _localizedPrefix;

    public override void UpdateVisualization()
    {
        if (_isLocalized)
        {
            _localizedPrefix = GetComponent<LocalizeStringEvent>().LocalizedString;
            _localizedPrefix.Refresh();
            _prefix = _localizedPrefix.Value;
        }

        _progressText.text = $"{_prefix}{System.Math.Round((_totalProgress == 0 ? 0 : _progress / _totalProgress) * 100, _roundDecimals).ToString()}{_postfix}";
    }

}
