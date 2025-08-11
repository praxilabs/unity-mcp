using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XNode;
using XNode.NodeGroups;

[CreateAssetMenu(menuName = "Praxilabs/xNode/Graphs/Steps Graph")]
public class StepsGraph : NodeGraph
{
    public List<ExperimentItemsRegistry> dataRegistries;

    [HideInInspector] public List<NodeGroup> groups = new List<NodeGroup>();
    [HideInInspector] public List<StepsGraph> runningGraphs = new List<StepsGraph>();
    [HideInInspector] public List<Color> originalInputColors = new List<Color>();
    [HideInInspector] public List<Color> originalOutputColors = new List<Color>();
    [HideInInspector] public Color hiddenPortsColor = new Color32(60, 60, 60, 30);
    public string graphID;

    public List<GlobalVariables> globalVariables = new List<GlobalVariables>();

    public StepNode firstStep;
    public bool isOpenedFromNode;

    public ExperimentItemsRegistry registryData { get { return GetFullRegistry(); } }

    private void OnEnable()
    {
        if (IsDuplicateId(graphID, this))
        {
            graphID = xNodeUtility.BuildID();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    private bool IsDuplicateId(string id, StepsGraph currentGraph)
    {
#if UNITY_EDITOR
        string currentPath = AssetDatabase.GetAssetPath(currentGraph);

        if(string.IsNullOrEmpty(currentPath)) return false;
        string folderPath = Path.GetDirectoryName(currentPath);

        if (string.IsNullOrEmpty(folderPath)) return false;

        string[] guids = AssetDatabase.FindAssets("t:StepsGraph", new[] { folderPath });
        int count = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var graph = AssetDatabase.LoadAssetAtPath<StepsGraph>(path);
            if (graph != null && graph.graphID == id)
            {
                if (graph != currentGraph)
                    count++;
            }
        }

        return count > 0;
#else
    return false;
#endif
    }

    private ExperimentItemsRegistry GetFullRegistry()
    {
        ExperimentItemsRegistry experimentItemsRegistry = ScriptableObject.CreateInstance<ExperimentItemsRegistry>();
        List<PrefabEntry> prefabEntries = new List<PrefabEntry>();
        foreach (var registry in dataRegistries)
        {
            prefabEntries.AddRange(registry.prefabRegisteries);
        }
        experimentItemsRegistry.prefabRegisteries = prefabEntries;
        return experimentItemsRegistry;
    }

    public void VerifyConnections()
    {
        nodes.RemoveAll(item => item == null);
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].VerifyConnections();
        }
    }

    public override Node CopyNode(Node original)
    {
        if (original is StepNode)
        {
            StepNode sn = (StepNode)base.CopyNode(original);
            sn.stepId = "s";
            return sn;
        }
        else
        {
            Node node = base.CopyNode(original);
            return node;
        }
    }

    public virtual StepNode GetStepById(string _stepID)
    {
        StepNode targetStep = null;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is not StepNode)
                continue;
            targetStep = (StepNode)nodes[i];
            if (targetStep.stepId == _stepID)
            {
                return targetStep;
            }
        }
        return null;
    }

    public void GetGroups()
    {
        groups.Clear();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetType() == typeof(XNode.NodeGroups.NodeGroup))
                groups.Add(nodes[i] as NodeGroup);
        }
    }

    public void UpdateGlobalVariableValue(string name, object value)
    {
        foreach (var globalVar in globalVariables)
        {
            if (globalVar.name == name)
                globalVar.Value = value;
        }

        // Update all nodes with the new value
        foreach (Node node in nodes)
        {
            if (node is VariableNode)
            {
                VariableNode variableNode = (VariableNode)node;
                if (variableNode.data.name == name)
                    variableNode.UpdateVariableValue(value);
            }
        }
    }
}
