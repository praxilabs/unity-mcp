using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNode;

public static class RenameToolsEditor
{
    public static string _element;
    private static string _newName = "enter new name";
    private static bool _canRename = false;

    /// <summary>
    /// Create an input field for the new name
    /// </summary>
    public static void ShowRenameField()
    {
        if (_canRename)
        {
            _newName = EditorGUILayout.TextField(_newName, GUILayout.Width(200));
        }
    }

    /// <summary>
    /// Create a rename button, that will open the text field and rename buttons
    /// </summary>
    public static void StartRenaming()
    {
        if (GUILayout.Button("Rename", StyleHelperxNode.Style(200f, 18f, "#272727", false, "#F5F5F5", new RectOffset(0, 0, 0, 5))))
        {
            _canRename = !_canRename;
        }
    }

    /// <summary>
    /// Create a rename all button, that will update the name inside all nodes
    /// </summary>
    /// <param name="nodes"></param>
    public static void RenameAllBTN(List<StepNode> nodes)
    {
        if (!_canRename) return;

        if (GUILayout.Button("Rename All", StyleHelperxNode.Style(200f, 18f, "#272727", false, "#F5F5F5", new RectOffset(0, 0, 0, 5))))
        {
            foreach (var node in nodes)
            {
                RenameFields(node, _newName);
            }
        }
    }

    /// <summary>
    /// Display rename button next to each node button if the rename button is clicked
    /// </summary>
    /// <param name="node"></param>
    public static void Rename(Node node)
    {
        if (!_canRename) return;

        if (GUILayout.Button("rename", StyleHelperxNode.Style(100f, 18f, "#272727", false, "#F5F5F5")))
        {
            RenameFields(node, _newName);
        }
    }

    /// <summary>
    /// rename fields inside nodes
    /// </summary>
    /// <param name="selectedNode"></param>
    /// <param name="newName"></param>
    private static void RenameFields(Node selectedNode, string newName)
    {
        foreach (var field in selectedNode.GetType().GetFields())
        {
            string fieldName = field.Name;
            if (selectedNode.GetType().GetField(fieldName) != null)
            {
                var value = field.GetValue(selectedNode);

                if (value != null)
                {
                    if (value is IList)
                    {
                        RenameListFields(value as IList);
                    }
                    else if (value.ToString() == _element)
                    {
                        field.SetValue(selectedNode, newName);
                    }
                }
            }
        }
    }

    /// <summary>
    /// rename fields inside lists
    /// </summary>
    /// <param name="list"></param>
    private static void RenameListFields(IList list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].ToString() == _element)
            {
                list[i] = _newName;
            }
            else
            {
                FieldInfo[] itemFields = list[i].GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);

                foreach (FieldInfo item in itemFields)
                {
                    if (item.GetValue(list[i]).ToString() == _element)
                    {
                        item.SetValue(list[i], _newName);
                    }
                }
            }
        }
    }
}