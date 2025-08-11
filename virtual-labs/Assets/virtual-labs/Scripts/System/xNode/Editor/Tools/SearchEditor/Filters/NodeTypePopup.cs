using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


public class NodeTypePopup : PopupWindowContent
{
    public static Dictionary<string, bool> _nodeTypeDictionary = new Dictionary<string, bool>();
    public static List<bool> _toggle = new List<bool>();

    private static Vector2 _scroll;
    private static List<string> _nodeTypes = new List<string>();
    private static bool _selectAll = true;

    public NodeTypePopup(List<string> _nodeType)
    {
        _nodeTypes = _nodeType;
    }

    /// <summary>
    /// Add nodeType and a bool value  to dictionary, to determine whether we selected the nodeType or not.
    /// This is for filtring
    /// </summary>
    private static void SetDictionary()
    {
        foreach (var nodeType in _nodeTypes)
        {
            if (!_nodeTypeDictionary.ContainsKey(nodeType))
            {
                if (_nodeTypeDictionary.Count == 0)
                {
                    _nodeTypeDictionary.Add("All", true);
                    _toggle.Add(true);
                }
                _nodeTypeDictionary.Add(nodeType, true);
                _toggle.Clear();

                for (int i = 0; i < _nodeTypeDictionary.Count; i++)
                {
                    _toggle.Add(true);
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
        if (_nodeTypes.Count > 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(200));

            CreateToggleList();

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

    /// <summary>
    /// Add a field for each node type inside the toggle list
    /// </summary>
    private static void CreateToggleList()
    {
        // Separate the "All" entry
        var firstEntry = _nodeTypeDictionary.FirstOrDefault(kv => kv.Key == "All");
        var sortedEntries = _nodeTypeDictionary
            .Where(kv => kv.Key != "All") // Exclude "All"
            .OrderBy(kv => kv.Key)        // Sort remaining keys
            .ToList();

        // Rebuild sorted dictionary while keeping "All" first
        _nodeTypeDictionary = new Dictionary<string, bool> { { firstEntry.Key, firstEntry.Value } };
        foreach (var entry in sortedEntries)
        {
            _nodeTypeDictionary.Add(entry.Key, entry.Value);
        }

        // Generate sorted key list for iteration
        List<string> sortedKeys = _nodeTypeDictionary.Keys.ToList();

        for (int i = 0; i < sortedKeys.Count; i++)
        {
            if (i == 0)
            {
                EditorGUI.BeginChangeCheck();
                _selectAll = EditorGUILayout.Toggle("All", _selectAll);
                if (EditorGUI.EndChangeCheck())
                {
                    _nodeTypeDictionary["All"] = _toggle[0];

                    for (int j = 0; j < sortedKeys.Count - 1; j++)
                    {
                        _toggle[j + 1] = _selectAll;
                        _nodeTypeDictionary[sortedKeys[j + 1]] = _toggle[j + 1];
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                _toggle[i] = EditorGUILayout.Toggle(sortedKeys[i], _toggle[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    _nodeTypeDictionary[sortedKeys[i]] = _toggle[i];
                }
            }
        }
    }


    /// <summary>
    /// returns a list of the selected node types
    /// </summary>
    /// <returns></returns>
    public static List<string> SelectedTypes()
    {
        List<string> Selected = new List<string>();

        foreach (var nodeType in _nodeTypeDictionary)
        {
            if (nodeType.Value)
            {
                Selected.Add(nodeType.Key);
            }
        }

        return Selected;
    }

    public static void UpdateLists(List<string> _nodeType)
    {
        if (_nodeType.Count != _nodeTypes.Count)
        {
            _nodeTypes = _nodeType;
            Refresh();
        }
    }

    public static void Refresh()
    {
        if (_nodeTypeDictionary.Count != _nodeTypes.Count + 1)
        {
            _nodeTypeDictionary.Clear();
            _toggle.Clear();

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