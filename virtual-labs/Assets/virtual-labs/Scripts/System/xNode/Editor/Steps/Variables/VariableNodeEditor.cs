using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(VariableNode))]
    public class VariableNodeEditor : NodeEditor
    {
        protected VariableNode variableNode { get { return _variableNode != null ? _variableNode : _variableNode = target as VariableNode; } }
        private VariableNode _variableNode;

        public override void OnBodyGUI(XNode.Node currentNode)
        {
            if ((target as Node).groupCollapsed)
                return;

            serializedObject.Update();

            DisplayFields(currentNode);
            DisplayValueField();

            serializedObject.ApplyModifiedProperties();

            base.OnBodyGUI(currentNode);
        }

        public void DisplayFields(XNode.Node currentNode)
        {
            string[] excludes = { "m_Script", "stepId", "tooltip", "graph", "position", "ports", "inputPortColor", "outputPortColor" };
            SerializedProperty iterator = serializedObject.GetIterator();

            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                NodeEditorGUILayout.PropertyField(iterator, true);
            }
        }

        private void DisplayValueField()
        {
            SerializedProperty globalVariablesProperty = serializedObject.FindProperty("data");
            if (globalVariablesProperty != null)
            {
                // Draw the 'type' field to determine which type of value to display
                SerializedProperty typeProperty = globalVariablesProperty.FindPropertyRelative("type");

                // Draw the value field based on the type
                SerializedProperty intValue = globalVariablesProperty.FindPropertyRelative("intValue");
                SerializedProperty floatValue = globalVariablesProperty.FindPropertyRelative("floatValue");
                SerializedProperty stringValue = globalVariablesProperty.FindPropertyRelative("stringValue");
                SerializedProperty boolValue = globalVariablesProperty.FindPropertyRelative("boolValue");
                SerializedProperty colorValue = globalVariablesProperty.FindPropertyRelative("colorValue");
                SerializedProperty vector2Value = globalVariablesProperty.FindPropertyRelative("vector2Value");
                SerializedProperty vector3Value = globalVariablesProperty.FindPropertyRelative("vector3Value");
                SerializedProperty objectValue = globalVariablesProperty.FindPropertyRelative("objectValue");

                GlobalVariableTypes type = (GlobalVariableTypes)typeProperty.enumValueIndex;

                switch (type)
                {
                    case GlobalVariableTypes.Int:
                        EditorGUILayout.PropertyField(intValue, new GUIContent("Value"));
                        break;
                    case GlobalVariableTypes.Float:
                        EditorGUILayout.PropertyField(floatValue, new GUIContent("Value"));
                        break;
                    case GlobalVariableTypes.String:
                        EditorGUILayout.PropertyField(stringValue, new GUIContent("Value"));
                        break;
                    case GlobalVariableTypes.Bool:
                        EditorGUILayout.PropertyField(boolValue, new GUIContent("Value"));
                        break;
                    case GlobalVariableTypes.Color:
                        EditorGUILayout.PropertyField(colorValue, new GUIContent("Value"));
                        break;
                    case GlobalVariableTypes.Vector2:
                        EditorGUILayout.PropertyField(vector2Value, new GUIContent("Value"));
                        break;
                    case GlobalVariableTypes.Vector3:
                        EditorGUILayout.PropertyField(vector3Value, new GUIContent("Value"));
                        break;
                    case GlobalVariableTypes.Object:
                        EditorGUILayout.PropertyField(objectValue, new GUIContent("Value"));
                        break;
                    default:
                        EditorGUILayout.LabelField("Unknown type");
                        break;
                }
            }
            else
            {
                EditorGUILayout.LabelField("GlobalVariables property is not available");
            }
        }
    }
}