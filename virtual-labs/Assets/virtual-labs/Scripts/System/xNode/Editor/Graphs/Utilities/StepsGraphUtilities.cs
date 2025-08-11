using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

public static class StepsGraphUtilities
{
    public static void GraphUtilities(SerializedObject serializedObject, StepsGraph graph, string element)
    {
        UpdateVariablesValue(graph);
        VerifyConnections(graph);

        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        FindNode(serializedObject, graph, element);
    }

    private static void UpdateVariablesValue(StepsGraph graph)
    {
        if (GUILayout.Button("Update Variables", StyleHelperxNode.Style(150f, 30f, "#6F4E37", false, "#FFFFFF")))
        {
            // Example of displaying and updating the values in the graph
            foreach (var globalValue in graph.globalVariables)
            {
                foreach (var node in graph.nodes)
                {
                    if (node is VariableNode)
                    {
                        VariableNode variableNode = node as VariableNode;
                        if (globalValue.name == variableNode.data.name
                            && globalValue.Value != variableNode.data.Value)
                            variableNode.data.Value = globalValue.Value;
                    }

                }
            }
        }
    }

    private static void VerifyConnections(StepsGraph graph)
    {
        if (GUILayout.Button("Verify Connections", StyleHelperxNode.Style(null, 20f, "#6F4E37", false, "#FFFFFF")))
            graph.VerifyConnections();
    }

    private static void FindNode(SerializedObject serializedObject, StepsGraph graph, string element)
    {
        element = EditorGUILayout.TextField("Node To Find", element);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Find Node", StyleHelperxNode.Style(200f, 20f, "#A67B5B", false, "#FFFFFF")))
        {
            NodeEditorWindow nodeEditorWindow = NodeEditorWindow.Open(serializedObject.targetObject as XNode.NodeGraph);
            StepNode stepNode = graph.GetStepById(element);
            if (stepNode != null)
            {
                nodeEditorWindow.Home((Node)stepNode);
            }
            else
            {
                Debug.Log("can't find node with that id");
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}