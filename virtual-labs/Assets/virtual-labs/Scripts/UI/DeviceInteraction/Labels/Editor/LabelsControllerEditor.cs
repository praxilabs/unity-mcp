using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LabelsController))]
public class LabelsControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedObject serializedObject = new SerializedObject(target);
        serializedObject.Update();

        DrawDefaultInspector();

        LabelsController controller = (LabelsController)target;

        InstantiateLabelsButton(controller);
        DestroyLabelsButton(controller);

        serializedObject.ApplyModifiedProperties();
    }

    private void InstantiateLabelsButton(LabelsController controller)
    {
        if (GUILayout.Button("Instantiate Label Gizmos", StyleHelperxNode.Style(null, 20, "#B17457", false, "#FFFFFF")))
        {
            controller.InstantiateLabelGizmos();

            MarkObjectAsDirty(controller);
        }
    }

    private void DestroyLabelsButton(LabelsController controller)
    {
        if (GUILayout.Button("Destroy Labels Gizmos", StyleHelperxNode.Style(null, 20, "#705C53", false, "#FFFFFF")))
        {
            controller.DestroyLabelGizmos();

            MarkObjectAsDirty(controller);
        }
    }

    /// <summary>
    /// Mark the controller as dirty to ensure the changes persist
    /// </summary>
    private void MarkObjectAsDirty(LabelsController controller)
    {
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(controller);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(controller.gameObject.scene);
        }
    }
}
