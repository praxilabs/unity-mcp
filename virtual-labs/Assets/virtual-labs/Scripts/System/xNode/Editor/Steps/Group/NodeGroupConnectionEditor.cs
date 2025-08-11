using System.Linq;
using UnityEditor;
using XNode;

namespace XNodeEditor.NodeGroups
{
    public class NodeGroupConnectionEditor
    {
        public void DisplayPorts(SerializedObject serializedObject, Node target)
        {
            serializedObject.Update();
            IterateOverPorts(serializedObject);

            var dynamicPorts = target.DynamicPorts.Where(port => !NodeEditorGUILayout.IsDynamicPortListPort(port));
            foreach (var dynamicPort in dynamicPorts)
            {
                NodeEditorGUILayout.PortField(dynamicPort);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void IterateOverPorts(SerializedObject serializedObject)
        {
            string[] includes = { "entry", "exit" };
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (includes.Contains(iterator.name))
                {
                    NodeEditorGUILayout.PropertyField(iterator, false);
                }
            }
        }
    }
}