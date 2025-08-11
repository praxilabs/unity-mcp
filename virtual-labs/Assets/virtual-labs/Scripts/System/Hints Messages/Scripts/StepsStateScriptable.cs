using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Steps State SO", menuName = "Messages/Hints/Steps State")]
public class StepsStateScriptable : ScriptableObject
{
    [SerializeField] private List<StateTypes> stepsData = new List<StateTypes>();

    public List<StateTypes> StepsStatesData { get => stepsData; set => stepsData = value; }
}

public enum StateTypes
{
    Unfinished,
    Finished,
    Current
}
