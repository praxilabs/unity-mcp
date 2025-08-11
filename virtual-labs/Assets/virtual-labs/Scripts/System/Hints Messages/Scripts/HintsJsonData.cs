using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class HintsJsonData
{
    public List<HintsList> stepsData = new List<HintsList>();
    public List<HintsList> StepsData { get => stepsData; set => stepsData = value; }

    public void MapDictionaryToList(Dictionary<int, HintsList> _data)
    {
        List<HintsList> tmpList = _data.OrderBy(entry => entry.Key).Select(entry => entry.Value).ToList();
        stepsData = tmpList;
    }
}

[Serializable]
public class HintsList
{
    [SerializeField] private List<string> hints = new List<string>();
    [SerializeField] private List<string> teaseHints = new List<string>();
    [FormerlySerializedAs("stepState")][SerializeField] private StateTypes state;

    public StateTypes State
    {
        get => state;
        set => state = value;
    }

    public List<string> Hints { get => hints; set => hints = value; }
    public List<string> TeaseHints { get => teaseHints; set => teaseHints = value; }
}