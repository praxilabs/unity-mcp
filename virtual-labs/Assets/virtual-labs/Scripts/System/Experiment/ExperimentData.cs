using System.Collections.Generic;
using UnityEngine;
using static Table.TablesManager;

[CreateAssetMenu(fileName = "ExperimentData", menuName = "Praxilabs/Experiments/ExperimentData", order = 0)]
public class ExperimentData : ScriptableObject
{
    [Tooltip("Used to get localization files")]
    public string experimentName;
    public bool hasTutorial;
    public LabTypes labType;
    public GameObject experimentCameras;

    public List<ExperimentStage> experimentStages;
    public List<StageTableEntry> tableStages;
}

public enum LabTypes
{
    Physics,
    PhysicsNoSink,
    Chemistry,
    Biology
}

[System.Serializable]
public class ExperimentStage
{
    public StageEndMessageTypes stageEndMessageType;
    public StepsGraph experimentGraph;
    public GameObject experimentTools;
}