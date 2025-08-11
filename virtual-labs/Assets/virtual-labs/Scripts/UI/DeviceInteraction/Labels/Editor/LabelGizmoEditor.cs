using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LabelGizmo))]
public class LabelGizmoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.Update(); // Sync serialized object

        DrawDefaultInspector();

        LabelGizmo labelGizmo = (LabelGizmo)target;

        InstantiateLabelsButton(labelGizmo);
        DestroyLabelsButton(labelGizmo);

        serializedObject.ApplyModifiedProperties(); // Apply changes and preserve
    }

    private void InstantiateLabelsButton(LabelGizmo labelGizmo)
    {
        if (GUILayout.Button("Instantiate Label", StyleHelperxNode.Style(null, 20, "#B17457", false, "#FFFFFF")))
        {
            labelGizmo.InstantiateLabels();

            // Mark the controller as dirty to ensure the changes persist
            MarkObjectAsDirty(labelGizmo);
        }
    }

    private void DestroyLabelsButton(LabelGizmo labelGizmo)
    {
        if (GUILayout.Button("Destroy Label", StyleHelperxNode.Style(null, 20, "#705C53", false, "#FFFFFF")))
        {
            labelGizmo.DestroyLabel();

            // Mark the controller as dirty to ensure the changes persist
            MarkObjectAsDirty(labelGizmo);
        }
    }

    private void MarkObjectAsDirty(LabelGizmo labelGizmo)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(labelGizmo); // Marks the object as dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(labelGizmo.gameObject.scene); // Mark scene dirty to ensure scene changes are saved
        }
#endif
    }
}
