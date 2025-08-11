using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

public class SearchEditorBase : EditorWindow
{
    protected List<StepNode> _nodes = new List<StepNode>();
    protected string _element;
    protected UnityEngine.Vector2 _scroll1;

    protected List<string> _graphNames = new List<string>();
    protected List<object> _execludedFields = new List<object>();
    protected List<object> _execludedNodes = new List<object>();

    /// <summary>
    /// Display Rename GUI
    /// Display the found nodes as buttons
    /// Display the scroll area in which the node buttons will be presented
    /// </summary>
    protected virtual void ShowResults()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _scroll1 = GUILayout.BeginScrollView(_scroll1, GUILayout.Height(350));

        RenameGUIFields();
        DisplayNodeButton();

        GUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Display Rename GUI
    /// </summary>
    protected virtual void RenameGUIFields()
    {
        RenameToolsEditor.StartRenaming();
        RenameToolsEditor.ShowRenameField();
        RenameToolsEditor.RenameAllBTN(_nodes);
    }

    /// <summary>
    /// Display graph label for each collection of nodes
    /// </summary>
    /// <param name="node"></param>
    protected virtual void DisplayGraphsName(Node node)
    {
        if (!_graphNames.Contains(node.graph.name))
        {
            _graphNames.Add(node.graph.name);
            GUILayout.Label(node.graph.name, StyleHelperxNode.Style(position.width, 25f, "#F5F5F5", false, "#000000", new RectOffset(0, 0, 20, 20)));
        }
    }

    /// <summary>
    /// Display the found nodes as buttons
    /// </summary>
    protected virtual void DisplayNodeButton()
    {
        _graphNames.Clear();

        var buttonWidth = position.width / 5 * 4 - 50;

        foreach (var node in _nodes)
        {
            DisplayGraphsName(node);

            using (var horizontal = new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(node.name, StyleHelperxNode.StyleNoBackGround(buttonWidth, 20f, node.NodeTint)))
                {
                    Selection.activeObject = node;
                    FocusOnNode(node);
                }

                RenameToolsEditor._element = _element;
                RenameToolsEditor.Rename(node);
            }
        }
    }

    /// <summary>
    /// search for entered text inside all node fields, and collections
    /// </summary>
    /// <param name="node"></param>
    protected virtual void SearchFields(StepNode node)
    {
        if (string.IsNullOrEmpty(_element))
            return;

        if (!_execludedNodes.Contains(node))
            _execludedNodes.Add(node);
        else
            return;

        _execludedFields.Clear();
        foreach (var member in node.GetType().GetMembers())
        {
            if (member == null)
                continue;

            // Skip members from the "XNode" namespace
            if (member.DeclaringType.Namespace == "XNode")
                continue;

            // Check if it's a field or property
            if (member is FieldInfo field)
            {
                // Field checks
                if (field.IsPublic ||
                    field.GetCustomAttribute<SerializeField>() != null ||
                    field.GetCustomAttribute<RegistryDropdownAttribute>() != null)
                {
                    var value = field.GetValue(node);
                    if (value == null)
                        continue;

                    FieldTypeChecker(value, node);
                }
            }
            else if (member is PropertyInfo property)
            {
                // Property checks
                if (property.GetGetMethod() != null ||
                    property.GetGetMethod().IsPublic ||
                    property.GetCustomAttribute<SerializeField>() != null ||
                    property.GetCustomAttribute<RegistryDropdownAttribute>() != null)
                {
                    var value = property.GetValue(node);
                    if (value == null)
                        continue;

                    FieldTypeChecker(value, node);
                }
            }
        }

    }

    //Recursive Search
    #region Recursive Search through N number of nested objects
    private void FieldTypeChecker(object objValue, StepNode node)
    {
        if (objValue is ICollection)
            ListFieldSearch(objValue as ICollection, node);

        else if (objValue is Enum)
        {
            if (objValue.ToString().ToLower().Contains(_element.ToLower()) && !_nodes.Contains(node))
                _nodes.Add(node);
        }
        else if (!IsPrimitiveValue(objValue as object))
            SearchFieldsRecursive(node, objValue as object);

        else if (objValue.ToString().ToLower().Contains(_element.ToLower()) && !_nodes.Contains(node))
            _nodes.Add(node);
    }

    private void SearchFieldsRecursive(StepNode node, object obj)
    {
        if (obj == null)
            return;

        if (obj is StepNode)
        {
            if (!_execludedNodes.Contains(obj))
                _execludedNodes.Add(obj);
            else
                return;
        }
        else
        {
            if (!_execludedFields.Contains(obj))
                _execludedFields.Add(obj);
            else
                return;
        }

        foreach (var field in obj.GetType().GetFields())
        {
            if (field == null)
                continue;

            if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
            {
                var value = field.GetValue(obj);

                if (value == null)
                    continue;

                FieldTypeChecker(value, node);
            }
        }
    }

    /// <summary>
    /// Search for entered text inside list fields
    /// </summary>
    /// <param name="list"></param>
    /// <param name="node"></param>
    protected virtual void ListFieldSearch(ICollection list, StepNode node)
    {
        foreach (var listItem in list)
        {
            FieldTypeChecker(listItem, node);
        }
    }
    #endregion


    /// <summary>
    /// Navigate to node if we click on the node button
    /// </summary>
    /// <param name="node"></param>
    protected virtual void FocusOnNode(Node node)
    {
        NodeEditorWindow nodeWindow = NodeEditorWindow.Open(node.graph);
        nodeWindow.Home(node);
    }
    private bool IsPrimitiveValue(object obj)
    {
        if (obj is int || obj is string || obj is float || obj is bool ||
            obj is long || obj is uint || obj is char || obj is decimal ||
            obj is UnityEngine.Vector2 || obj is UnityEngine.Vector3 || obj is UnityEngine.Vector4)
            return true;

        return false;
    }
    protected List<StepNode> CastToStepsNode(List<Node> nodes)
    {
        List<StepNode> steps = new List<StepNode>();
        List<string> skippedSteps = new List<string>();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is StepNode)
                steps.Add((StepNode)nodes[i]);
            else
            {
                string nodeType = nodes[i].GetType().PrettyName().ToString();
                if (!skippedSteps.Contains(nodeType))
                {
                    skippedSteps.Add(nodeType);
                    Debug.Log(nodeType + "<Color=orange> node is skipped from search</Color>");
                }
            }
        }
        return steps;
    }
}