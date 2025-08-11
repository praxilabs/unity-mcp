using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomEditor(typeof(StepsGraph))]
public class StepsGraphInspector : Editor
{
    string Element;
    string groupName;
    private SerializedProperty groupsList;
    private int groupIndex = 0;

    private void OnEnable()
    {
        groupsList = serializedObject.FindProperty("groups");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        OpenGraphWindow();

        DrawDefaultInspector();

        StepsGraphUtilities.GraphUtilities(serializedObject, (StepsGraph)target, Element);
        StepsGraphGroupsUtilities.GroupsUtilities(groupsList, (StepsGraph)target, groupIndex, serializedObject, groupName);

        serializedObject.ApplyModifiedProperties();
    }

    private void OpenGraphWindow()
    {
        if (GUILayout.Button("Edit graph", StyleHelperxNode.Style(null, 35f, "#ECB176", false, "#000000")))
        {
            StepsGraph stepsGraph = (StepsGraph)target;
            NodeEditorWindow nodeEditorWindow = NodeEditorWindow.Open(serializedObject.targetObject as XNode.NodeGraph);
            if (stepsGraph.firstStep == null)
            {
                if (stepsGraph.nodes.Count > 0)
                    nodeEditorWindow.Home((Node)stepsGraph.nodes[0]);
            }
            else
                nodeEditorWindow.Home((Node)stepsGraph.firstStep);
        }
    }
}
