using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GraphsPopup : PopupWindowContent
{
    public static Dictionary<string, bool> graphsDictionary = new Dictionary<string, bool>();
    public static List<bool> toggledGraphs = new List<bool>();

    private static List<StepsGraph> _graphs = new List<StepsGraph>();
    private static Vector2 _scroll;
    private static bool _selectAll = true;

    public GraphsPopup(List<StepsGraph> _graphs)
    {
        GraphsPopup._graphs = _graphs;
    }

    /// <summary>
    /// Add graphs and a bool value  to dictionary, to determine whether we selected the graph or not.
    /// This is for filtring
    /// </summary>
    private static void SetDictionary()
    {
        foreach (var graph in _graphs)
        {
            if (!graphsDictionary.ContainsKey(graph.name))
            {
                if (graphsDictionary.Count == 0)
                {
                    graphsDictionary.Add("All", true);
                    toggledGraphs.Add(true);
                }
                graphsDictionary.Add(graph.name, true);
                toggledGraphs.Clear();

                for (int i = 0; i < graphsDictionary.Count; i++)
                {
                    toggledGraphs.Add(true);
                }
            }
        }
    }

    /// <summary>
    /// Draw the scroll area and toggle list
    /// </summary>
    /// <param name="rect"></param>
    public override void OnGUI(Rect rect)
    {
        if (_graphs.Count > 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(200));

            CreateToggleList();

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

    /// <summary>
    /// Add a field for each graph inside the toggle list
    /// </summary>
    private static void CreateToggleList()
    {
        for (int i = 0; i < graphsDictionary.Count; i++)
        {
            if (i == 0)
            {
                EditorGUI.BeginChangeCheck();
                _selectAll = EditorGUILayout.Toggle("All", _selectAll);
                if (EditorGUI.EndChangeCheck())
                {
                    graphsDictionary["All"] = toggledGraphs[0];
                    for (int j = 0; j < graphsDictionary.Count - 1; j++)
                    {
                        toggledGraphs[j + 1] = _selectAll;
                        graphsDictionary[_graphs[j].name] = toggledGraphs[j + 1];
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                toggledGraphs[i] = EditorGUILayout.Toggle(_graphs[i - 1].name, toggledGraphs[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    graphsDictionary[_graphs[i - 1].name] = toggledGraphs[i];
                }
            }
        }
    }

    /// <summary>
    /// returns a list of the selected graphs
    /// </summary>
    /// <returns></returns>
    public static List<StepsGraph> SelectedTypes()
    {
        List<StepsGraph> Selected = new List<StepsGraph>();

        foreach (var graph in graphsDictionary)
        {
            if (graph.Value)
            {
                for (int i = 0; i < _graphs.Count; i++)
                {
                    if (_graphs[i].name == graph.Key)
                    {
                        Selected.Add(_graphs[i]);
                        continue;
                    }
                }
            }
        }

        return Selected;
    }

    public static void UpdateLists(List<StepsGraph> _graphs)
    {
        if (_graphs.Count != GraphsPopup._graphs.Count)
        {
            GraphsPopup._graphs = _graphs;
            Refresh();
        }
    }

    public static void Refresh()
    {
        if (graphsDictionary.Count != _graphs.Count + 1)
        {
            graphsDictionary.Clear();
            toggledGraphs.Clear();

            SetDictionary();
            CreateToggleList();
        }
    }

    /// <summary>
    /// Triggers when the toggle menu is oppened
    /// </summary>
    public override void OnOpen()
    {
        Refresh();
        SetDictionary();
    }
}