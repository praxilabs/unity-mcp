using UnityEditor;

namespace Praxilabs.DeviceSideMenu
{
    [CustomEditor(typeof(DeviceMenu))]
    public class DeviceMenuEditor : Editor
    {
        SerializedProperty _readingsComponentsProperty;
        SerializedProperty _controlsComponentsProperty;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space();

            if(_readingsComponentsProperty == null) _readingsComponentsProperty = serializedObject.FindProperty("_readingsComponents");
            if(_controlsComponentsProperty == null)  _controlsComponentsProperty = serializedObject.FindProperty("_controlsComponents");

            DrawNonEditableList(_readingsComponentsProperty);
            DrawNonEditableList(_controlsComponentsProperty);

            serializedObject.ApplyModifiedProperties();
        }

        // Helper method to draw a non-editable list of GameObjects
        private void DrawNonEditableList(SerializedProperty listProperty)
        {
            if (listProperty != null)
            {
                EditorGUI.BeginDisabledGroup(true); // Disable editing
                EditorGUILayout.PropertyField(listProperty, true); // Draw the list as usual
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.HelpBox("No components to display.", UnityEditor.MessageType.Info);
            }
        }
    }
}
