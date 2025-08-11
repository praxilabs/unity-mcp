using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(StepNode))]
    public class StepNodeEditor : NodeEditor
    {
        protected StepNode stepNode => _stepNode != null ? _stepNode : _stepNode = target as StepNode;
        private StepNode _stepNode;

        private GUIStyle _darkBoxStyle;
        protected GUIStyle DarkBoxStyle =>
                   _darkBoxStyle ??= StyleHelperxNode.SetBoxStyle(_darkBoxStyle, new Color(0.1f, 0.1f, 0.1f, 0.65f));

        private GUIStyle _displayNodeFieldsButtonStyle;
        protected GUIStyle DisplayNodeFieldsButtonStyle =>
                    _displayNodeFieldsButtonStyle ??= StyleHelperxNode.Style(GetWidth() - 30, 15, "#00000000", true, "FFFFFF");

        private static readonly HashSet<string> _excludedProperties = new()
        {
            "m_Script", "stepId", "graph", "position", "ports", "inputPortColor", "outputPortColor", "tooltip"
        };

        public override void OnCreate()
        {
            base.OnCreate();

            if (stepNode.stepId == "s")
                stepNode.stepId = xNodeUtility.BuildID();
        }

        public override string GetHeaderTooltip()
        {
            return stepNode.tooltip;
        }

        public override void OnBodyGUI(Node currentNode)
        {
            if (currentNode.groupCollapsed || currentNode.isGroup) return;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawNodeFields(currentNode);
            DisplayNodeFieldsButton(currentNode);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            base.OnBodyGUI(currentNode);
        }

        public void DisplayNodeFieldsButton(Node currentNode)
        {
            string buttonTextureName = currentNode.nodeCollapsed ? "up_arrow" : "down_arrow";

            if (GUILayout.Button(StyleHelperxNode.SetBackground(buttonTextureName), DisplayNodeFieldsButtonStyle))
            {
                currentNode.nodeCollapsed = !currentNode.nodeCollapsed;
            }
        }

        public void DrawNodeFields(Node currentNode)
        {
            if (currentNode.isGroup) return;

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            if (currentNode.nodeCollapsed)
            {
                EditorGUILayout.BeginVertical(DarkBoxStyle);
                GUILayout.Space(2);

                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;
                    if (_excludedProperties.Contains(iterator.name)) continue;
                    NodeEditorGUILayout.PropertyField(iterator, true);
                }

                GUILayout.Space(2);
                EditorGUILayout.EndVertical();
                GUI.color = Color.white;
            }
            else
            {
                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;
                    if (currentNode.GetPort(iterator.name) != null)
                        NodeEditorGUILayout.PropertyField(iterator, true);
                }
            }
        }

        public void DrawPorts(Node currentNode)
        {
            var ports = currentNode.DynamicPorts;
            foreach (var port in ports)
            {
                if (!NodeEditorGUILayout.IsDynamicPortListPort(port))
                    NodeEditorGUILayout.PortField(port);
            }
        }

        public override int GetWidth()
        {
            return stepNode.width;
        }

        public override void SetWidth(int width)
        {
            stepNode.width = width;
        }

        public override int GetWidthOriginal()
        {
            return stepNode.originalWidth;
        }

        public override void SetWidthOriginal(int width)
        {
            stepNode.originalWidth = width;
        }
    }
}
