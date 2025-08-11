using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeviceHoverDetector))]
public class DeviceHoverDetectorEditor : Editor
{
    private void OnSceneGUI()
    {
        DeviceHoverDetector detector = (DeviceHoverDetector)target;

        // Make a position handle for editing _targetPosition
        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(detector.TargetPosition, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(detector, "Move Target Position");
            detector.TargetPosition = newTargetPosition;
            EditorUtility.SetDirty(detector);
        }
    }
}
