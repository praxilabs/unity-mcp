using System;
using System.Collections.Generic;
using UnityEngine;

public class StageEndMessageHolder : MonoBehaviour
{
    public List<StageEnMessageAndType> stageEndMessages = new List<StageEnMessageAndType>();
}

public enum StageEndMessageTypes
{
    Independent, DependsOnNextStage, MultiScenario
}

[Serializable]
public class StageEnMessageAndType
{
    public StageEndMessageBase stageEndMessage;
    public StageEndMessageTypes stageEndMessageType;
}
