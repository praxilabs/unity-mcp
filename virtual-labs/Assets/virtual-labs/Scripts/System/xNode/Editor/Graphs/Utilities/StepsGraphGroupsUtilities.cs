using UnityEditor;
using UnityEngine;
using XNodeEditor;

public static class StepsGraphGroupsUtilities
{
    public static void GroupsUtilities(SerializedProperty groupsList, StepsGraph graph, int groupIndex, SerializedObject serializedObject, string groupName)
    {
        AddSeparator();

        GUILayout.Label("Groups", LabelStyle(16));
        GUILayout.Label("Search for all group nodes in graph", LabelStyle(12));

        GetGroups(groupsList, graph);
        CycleGroups(groupsList, graph, groupIndex, serializedObject);
        SearchGroupByName(groupName, graph, serializedObject);
    }

    private static void AddSeparator()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.Label("<----------------------------------------->");
        GUILayout.Space(10);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private static GUIStyle LabelStyle(int fontSize)
    {
        GUIStyle labelStyle;
        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = fontSize;
        labelStyle.normal.textColor = StyleHelperxNode.ConvertHexColor("#F8F4E1");

        return labelStyle;
    }

    private static void GetGroups(SerializedProperty groupsList, StepsGraph graph)
    {
        if (GUILayout.Button("Get Groups", StyleHelperxNode.Style(150f, 30f, "#6F4E37", false, "#FFFFFF")))
            graph.GetGroups();

        EditorGUILayout.PropertyField(groupsList, true);
        GUILayout.Space(EditorGUIUtility.singleLineHeight);
    }

    private static void CycleGroups(SerializedProperty groupsList, StepsGraph graph, int groupIndex, SerializedObject serializedObject)
    {
        GUILayout.Label("Cycle Groups", LabelStyle(12));

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("<<", StyleHelperxNode.Style(80f, 20f, "#A67B5B", false, "#FFFFFF")))
        {
            if (groupIndex == 0)
                groupIndex = groupsList.arraySize - 1;
            else
                groupIndex--;

            BrowseGraphGroups(graph, groupIndex, serializedObject);
        }

        // Add some space between the buttons
        GUILayout.Space(10);

        if (GUILayout.Button(">>", StyleHelperxNode.Style(80f, 20f, "#A67B5B", false, "#FFFFFF")))
        {
            if (groupIndex == groupsList.arraySize - 1)
                groupIndex = 0;
            else
                groupIndex++;

            BrowseGraphGroups(graph, groupIndex, serializedObject);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private static void SearchGroupByName(string groupName, StepsGraph graph, SerializedObject serializedObject)
    {
        GUILayout.Space(20);
        GUILayout.Label("Search for a group by name", LabelStyle(14));
        GUILayout.Space(10);
        groupName = EditorGUILayout.TextField("Group Name", groupName);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Find Group By Name", StyleHelperxNode.Style(200f, 35f, "#ECB176", false, "#000000")))
            GoToSpecificGroupNode(groupName, graph, serializedObject);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private static void BrowseGraphGroups(StepsGraph graph, int groupIndex, SerializedObject serializedObject)
    {
        NodeEditorWindow nodeEditorWindow = NodeEditorWindow.Open(serializedObject.targetObject as XNode.NodeGraph);

        if (graph.groups[groupIndex] != null)
            nodeEditorWindow.Home(graph.groups[groupIndex]);
        else
            Debug.LogError("can't find group node of index " + groupIndex + " , something went wrong");
    }

    private static void GoToSpecificGroupNode(string groupName, StepsGraph graph, SerializedObject serializedObject)
    {
        NodeEditorWindow nodeEditorWindow = NodeEditorWindow.Open(serializedObject.targetObject as XNode.NodeGraph);
        if (graph.groups.Find(group => string.Equals(group.name, groupName)))
            nodeEditorWindow.Home(graph.groups.Find(group => string.Equals(group.name, groupName)));
        else
            Debug.LogError("can't find group node of name " + groupName);
    }
}