using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ExperimentsPopup : PopupWindowContent
{
    public static Dictionary<string, bool> experimentsDictionary = new Dictionary<string, bool>();
    public static List<string> experimentNames = new List<string>();
    public static List<bool> toggle = new List<bool>();

    private static List<string> _experiments = new List<string>();
    private static protected Vector2 _scroll;
    private static bool _selectAll = true;

    public ExperimentsPopup(List<string> _experiments)
    {
        ExperimentsPopup._experiments = _experiments;
    }

    /// <summary>
    /// Add experiments and a bool value  to dictionary, to determine whether we selected the experiments or not.
    /// This is for filtring
    /// </summary>
    private static void SetDictionary()
    {
        foreach (var exp in _experiments)
        {
            string expName = StepsGraphFinder.FilterGraphName(exp);

            if (!experimentsDictionary.ContainsKey(expName))
            {
                if (experimentsDictionary.Count == 0)
                {
                    experimentsDictionary.Add("All", true);
                    toggle.Add(true);
                }
                experimentsDictionary.Add(expName, true);
                experimentNames.Add(expName);
                toggle.Add(true);
            }
        }
    }

    /// <summary>
    /// Draw the scroll area and toggle list
    /// </summary>
    /// <param name="rect"></param>
    public override void OnGUI(Rect rect)
    {
        if (_experiments.Count > 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(200));

            CreateToggleList();

            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }


    /// <summary>
    /// Add a field for each experiment inside the toggle list
    /// and clear graphs, and lists
    /// </summary>
    private static void CreateToggleList()
    {
        for (int i = 0; i < experimentsDictionary.Count; i++)
        {
            if (i == 0)
            {
                EditorGUI.BeginChangeCheck();
                _selectAll = EditorGUILayout.Toggle("All", _selectAll);
                if (EditorGUI.EndChangeCheck())
                {
                    experimentsDictionary["All"] = toggle[0];
                    for (int j = 0; j < experimentsDictionary.Count - 1; j++)
                    {
                        toggle[j + 1] = _selectAll;


                        experimentsDictionary[experimentNames[j]] = toggle[j + 1];
                    }
                    FilterSearch.UpdateLists();
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                toggle[i] = EditorGUILayout.Toggle(experimentNames[i - 1], toggle[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    experimentsDictionary[experimentNames[i-1]] = toggle[i];
                    FilterSearch.UpdateLists();
                }
            }
        }
    }

    public static void UpdateLists(List<string> _experiments)
    {
        ExperimentsPopup._experiments = _experiments;
        SetDictionary();
        CreateToggleList();
    }

    /// <summary>
    /// Triggers when the toggle menu is oppened
    /// </summary>
    public override void OnOpen()
    {
        SetDictionary();
    }
}