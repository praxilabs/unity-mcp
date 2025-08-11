using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeSearchEditor : SearchEditorBase
{
    private StepsGraph graph { get { return _graph != null ? _graph : _graph = TargetGraph as StepsGraph; } }
    private StepsGraph _graph;
    private int selected = 0;

    public static UnityEngine.Object TargetGraph;

    private GUIStyle _searchButtonStyle;
    private GUIStyle SearchButtonStyle=>
        _searchButtonStyle??= StyleHelperxNode.Style(300f, 20f, "#F6F5F2", false, "#272727", new RectOffset(0, 0, 20, 20));

    /// <summary>
    /// Display and set window size
    /// </summary>
    public static void DisplaySearchWindow()
    {
        NodeSearchEditor finder = (NodeSearchEditor)GetWindow(typeof(NodeSearchEditor), false, "Current Graph Search");
        finder.minSize = new Vector2(400, 300);
        finder.Show();
    }

    /// <summary>
    /// Start searching in all nodes, or in nodes inside selected group 
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("\n Search For Tools, Functions or Classes in current graph\n", EditorStyles.boldLabel);
        _element = EditorGUILayout.TextField("Item To Find", _element);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Search", SearchButtonStyle))
        {
            if (graph.groups.Count > 0 && selected != 0)
                SearchStart(CastToStepsNode(graph.groups[selected - 1].nodes));
            else
                SearchStart(CastToStepsNode(graph.nodes));
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GroupMenu();
        ShowResults();
    }

    /// <summary>
    /// gets groups in current graph and displays them in menu
    /// note: if more than 1 group has the same name, only one 
    /// of them will be displayed (this is a limitation in EditorGUILayout.Popup)
    /// </summary>
    public void GroupMenu()
    {
        graph.GetGroups();
        if (graph.groups.Count > 0)
        {
            List<string> groupsList = new List<string>();
            groupsList.Add("None");

            foreach (var group in graph.groups)
            {
                groupsList.Add(group.name.ToString());
            }
            selected = EditorGUILayout.Popup("Select Group", selected, groupsList.ToArray());
        }
    }

    /// <summary>
    /// Start searching for nodes in current graph, and ignores nodes of type NodeGroup
    /// </summary>
    /// <param name="nodes">list of nodes inside this graph</param>
    public void SearchStart(List<StepNode> nodes)
    {
        _execludedFields.Clear();
        _execludedNodes.Clear();
        base._nodes.Clear();
        foreach (var node in nodes)
        {
            SearchFields(node);
        }
    }
}