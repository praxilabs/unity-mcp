using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;

public class GraphSearchEditor : SearchEditorBase
{
    private List<string> _experimentsPath = new List<string>();
    private List<string> _nodeTypes = new List<string>();
    private List<StepsGraph> _graphs = new List<StepsGraph>();

    private GUIStyle _searchButtonStyle;
    private GUIStyle SearchButtonStyle =>
        _searchButtonStyle ??= StyleHelperxNode.Style(300f, 20f, "#F6F5F2", false, "#272727", new RectOffset(0, 0, 20, 20));

    /// <summary>
    /// Display and set window size
    /// </summary>
    [MenuItem("Xnode/Search In Graphs")]
    static void CreateWindow()
    {
        EditorWindow window = GetWindow<GraphSearchEditor>();
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    /// <summary>
    /// Set window title
    /// Fetch all experiments
    /// </summary>
    void OnEnable()
    {
        string[] experimentTypeFolders = AssetDatabase.GetSubFolders("Assets/-AssetBundlesXnode");

        foreach (var folder in experimentTypeFolders)
            _experimentsPath.AddRange(AssetDatabase.GetSubFolders(folder).ToList());

        titleContent = new GUIContent("Search In Graphs");
    }

    /// <summary>
    /// Start searching in all nodes, after applying/selecting filters
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label("\n Search For Tool, Function or Class in any graph\n", EditorStyles.boldLabel);
        _element = EditorGUILayout.TextField("Item To Find", _element);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Search", SearchButtonStyle))
        {
            Debug.Log("<Color=blue>experiments.Count </Color>" + _experimentsPath.Count);
            ExperimentsPopup.UpdateLists(_experimentsPath);
            FilterSearch.UpdateLists();
            SearchStart();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        ShowDropDownMenus();
        ShowResults();
    }

    /// <summary>
    /// Display dropdown menu for experiments, graphs, and node types
    /// Note: you must select the experiments, graphs, and node types first before
    /// you start searching
    /// </summary>
    private void ShowDropDownMenus()
    {
        if (_experimentsPath.Count > 0)
        {
            FilterSearch.SelectExperiment(_experimentsPath);
            FilterSearch.SelectGraph(_experimentsPath);
            FilterSearch.SelectNodeType();
            _graphs = GraphsPopup.SelectedTypes();
            _nodeTypes = NodeTypePopup.SelectedTypes();
        }
    }

    /// <summary>
    /// Start searching inside selected graphs
    /// </summary>
    public void SearchStart()
    {
        _nodes.Clear();
        _execludedFields.Clear();
        Debug.Log("<Color=red>graphs.Count </Color>" + _graphs.Count);
        foreach (var _graph in _graphs)
        {
            GraphSearch(_graph);
        }
    }

    /// <summary>
    /// searching inside selected graph
    /// </summary>
    /// <param name="graph"></param>
    private void GraphSearch(NodeGraph graph)
    {
        _execludedNodes.Clear();
        List<StepNode> ListOfSteps = CastToStepsNode(graph.nodes);
        foreach (var _node in ListOfSteps)
        {
            NodeSearch(_node);
        }
    }

    /// <summary>
    /// searching inside selected node
    /// </summary>
    /// <param name="graph"></param>
    private void NodeSearch(StepNode node)
    {
        if (_nodeTypes.Contains(node.GetType().ToString()))
            SearchFields(node);
    }
}