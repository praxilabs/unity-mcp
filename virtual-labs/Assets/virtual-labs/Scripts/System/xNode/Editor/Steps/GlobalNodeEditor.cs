using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomEditor(typeof(StepNode), true)]
public class GlobalNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Edit graph", GUILayout.Height(40)))
        {
            SerializedProperty graphProp = serializedObject.FindProperty("graph");
            NodeEditorWindow nodeEditorWindow = NodeEditorWindow.Open(graphProp.objectReferenceValue as XNode.NodeGraph);
            nodeEditorWindow.Home(); // Focus selected node
        }

        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        GUILayout.Label("Raw data", "BoldLabel");


        // Now draw the node itself.
        DrawDefaultInspector();

        StepNode stepNode = (StepNode)target;
        if (GUILayout.Button("Set as first step", GUILayout.Height(30)))
        {
            stepNode.SetStepToFirstStep();
        }
        serializedObject.ApplyModifiedProperties();
    }
}