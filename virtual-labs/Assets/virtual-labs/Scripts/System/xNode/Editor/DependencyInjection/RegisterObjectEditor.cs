using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RegisterObject))]
public class RegisterObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveDataButton();
    }

    private void SaveDataButton()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        RegisterObject registry = (RegisterObject)target;

        if (GUILayout.Button("Save Data", StyleHelperxNode.Style(200f, 25f, "#6C4E31", false, "#FFFFFF")))
        {
            registry.RegisterPreparation();
            EditorUtility.SetDirty(registry.registryData);
            EditorUtility.SetDirty(registry);

            if (PrefabUtility.IsPartOfPrefabInstance(registry))
                PrefabUtility.RecordPrefabInstancePropertyModifications(registry);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}