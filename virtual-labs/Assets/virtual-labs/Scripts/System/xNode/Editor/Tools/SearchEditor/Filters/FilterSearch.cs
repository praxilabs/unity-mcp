using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FilterSearch : EditorWindow
{
    public static StepsGraph CurrentGraph;
    public static List<StepsGraph> graphs = new List<StepsGraph>();
    public static List<string> nodeTypes = new List<string>();

    private static Rect _buttonRect;
    private static GUIStyle _selectButtonStyle;
    private static GUIStyle SelectButtonStyle => 
        _selectButtonStyle ??= StyleHelperxNode.Style(200f, 18f, "#272727", false, "#F5F5F5", new RectOffset(0, 0, 0, 5));

    /// <summary>
    /// Add select experiment label and menu button
    /// Used Popup window instead of PopUp to add multi selection in menu
    /// </summary>
    /// <param name="experiments">list of existing experiments</param>
    public static void SelectExperiment(List<string> experiments)
    {
        if (experiments.Count > 0)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(380));

            GUILayout.Label("Select Experiment", EditorStyles.boldLabel);

            if (GUILayout.Button("Select", SelectButtonStyle))
            {
                PopupWindow.Show(_buttonRect, new ExperimentsPopup(experiments));
            }

            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.Repaint) _buttonRect = GUILayoutUtility.GetLastRect();
        }
    }

    /// <summary>
    /// Add select graphs label and menu button
    /// Used Popup wi[SerializeField] [SerializeField] ndow instead of PopUp to add multi selection in menu
    /// </summary>
    /// <param name="experiments">list of existing experiments</param>
    /// <param name="graphs">list of graphs(to be filled)</param>
    public static void SelectGraph(List<string> experiments)
    {
        GetGraphs(experiments);

        EditorGUILayout.BeginHorizontal(GUILayout.Width(380));

        GUILayout.Label("Select Graph", EditorStyles.boldLabel);

        if (GUILayout.Button("Select", SelectButtonStyle))
        {
            PopupWindow.Show(_buttonRect, new GraphsPopup(graphs));
        }

        EditorGUILayout.EndHorizontal();

        if (Event.current.type == EventType.Repaint) _buttonRect = GUILayoutUtility.GetLastRect();
    }

    /// <summary>
    /// Add select graphs label and menu button
    /// Used Popup window instead of PopUp to add multi selection in menu
    /// </summary>
    /// <param name="nodeTypes">list of node types</param>
    /// <param name="graphs">list of current graphs</param>
    public static void SelectNodeType()
    {
        GetNodeTypes(nodeTypes, graphs);

        EditorGUILayout.BeginHorizontal(GUILayout.Width(380));

        GUILayout.Label("Select NodeType", EditorStyles.boldLabel);

        if (GUILayout.Button("Select", SelectButtonStyle))
        {
            PopupWindow.Show(_buttonRect, new NodeTypePopup(nodeTypes));
        }

        EditorGUILayout.EndHorizontal();

        if (Event.current.type == EventType.Repaint) _buttonRect = GUILayoutUtility.GetLastRect();
    }

    public static void UpdateLists()
    {
        GraphsPopup.UpdateLists(graphs);
        NodeTypePopup.UpdateLists(nodeTypes);
    }

    /// <summary>
    /// Get the selected graph from the graphs dictionary inisde ExperimentsPopup class
    /// </summary>
    /// <param name="experiments">list of existing experiments</param>
    /// <param name="graphs">list of graphs(to be filled)</param>
    private static void GetGraphs(List<string> experiments)
    {
        graphs.Clear();

        for (int i = 0; i < ExperimentsPopup.experimentsDictionary.Count - 1; i++)
        {
            if (ExperimentsPopup.experimentsDictionary[ExperimentsPopup.experimentNames[i]])
                graphs.AddRange(StepsGraphFinder.FindAllStepsGraphs(experiments[i]));
        }
    }

    /// <summary>
    /// Get node types from selected graphs
    /// </summary>
    /// <param name="nodeTypes">list of nodeTypes (to be filled or refreshed)</param>
    /// <param name="graphs">list of selected graphs</param>
    private static void GetNodeTypes(List<string> nodeTypes, List<StepsGraph> graphs)
    {
        nodeTypes.Clear();
        foreach (var _graph in graphs)
        {
            foreach (var node in _graph.nodes)
            {
                string typeName = node.GetType().Name;

                if (!nodeTypes.Contains(typeName))
                    nodeTypes.Add(typeName);
            }
        }
    }
}
