using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperimentItemsRegistry", menuName = "Praxilabs/Experiment Data/Registry Data")]
public class ExperimentItemsRegistry : ScriptableObject
{
    public List<PrefabEntry> prefabRegisteries = new List<PrefabEntry>();
}

[Serializable]
public class PrefabEntry
{
    public string prefabName;
    public List<ChildEntry> prefabChildren = new List<ChildEntry>();
}

[Serializable]
public class ChildEntry
{
    public string childName;
    public List<string> childComponents = new List<string>();
}
