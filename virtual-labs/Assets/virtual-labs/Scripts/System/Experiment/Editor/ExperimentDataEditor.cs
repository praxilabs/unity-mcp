using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExperimentData))]
public class ExperimentDataEditor : Editor
{
    private ExperimentData _experimentData;

    private GUIStyle _updateCameraButtonStyle;
    private GUIStyle UpdateCameraButtonStyle => 
        _updateCameraButtonStyle ??= StyleHelperxNode.Style(200f, 20f, "#A67B5B", false, "#FFFFFF");
    private GUIStyle _darkBoxStyle;
    protected GUIStyle DarkBoxStyle =>
                _darkBoxStyle ??= StyleHelperxNode.SetBoxStyle(_darkBoxStyle, new Color(0.1f, 0.1f, 0.1f, 0.65f));

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical(DarkBoxStyle);
        GUILayout.Space(10);
        
        base.OnInspectorGUI();

        serializedObject.Update();

        _experimentData = (ExperimentData)target;

        UpdateCameraStagesButton();

        serializedObject.ApplyModifiedProperties();
        GUILayout.Space(10);
        EditorGUILayout.EndVertical();
    }

    private void UpdateCameraStagesButton()
    {
        if (GUILayout.Button("Update Camera Stages", UpdateCameraButtonStyle))
        {
            UpdateCameraStages();
        }
    }

    private void UpdateCameraStages()
    {
        if (_experimentData.experimentCameras == null) return;

        int stagesNumber = _experimentData.experimentStages.Count;

        foreach (InterestPointStagesToggle interestPointsToggle in _experimentData.experimentCameras.GetComponentsInChildren<InterestPointStagesToggle>())
        {
            interestPointsToggle.UpdateCameraStages(stagesNumber);
        }

        EditorUtility.SetDirty(_experimentData.experimentCameras);
        string assetPath = AssetDatabase.GetAssetPath(_experimentData.experimentCameras);

        if (!string.IsNullOrEmpty(assetPath))
        {
            AssetDatabase.SaveAssetIfDirty(_experimentData.experimentCameras);
        }
    }
}