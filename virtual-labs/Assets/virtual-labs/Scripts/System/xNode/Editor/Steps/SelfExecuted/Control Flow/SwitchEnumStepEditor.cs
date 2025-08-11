using System;
using System.Linq;
using UnityEngine;
using XNodeEditor;
using static XNode.Node;

[CustomNodeEditor(typeof(SwitchEnumStep))]
public class SwitchEnumStepEditor : StepNodeEditor
{
    public override void OnBodyGUI(XNode.Node currentNode)
    {
        if ((target as XNode.Node).groupCollapsed)
            return;

        serializedObject.Update();

        DrawNodeContent(currentNode);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawNodeContent(XNode.Node currentNode)
    {
        DrawNodeFields(currentNode);

        if (currentNode.nodeCollapsed)
        {
            SwitchEnumStep switchStatementStep = currentNode as SwitchEnumStep;

            if (GUILayout.Button("Generate Ports", GUILayout.Height(20), GUILayout.Width(130)))
                GeneratePorts(switchStatementStep);
            DrawPorts(currentNode);
        }

        DisplayNodeFieldsButton(currentNode);
    }

    private void GeneratePorts(SwitchEnumStep switchStep)
    {
        if (switchStep.enumTypeName == switchStep.previousEnum) return;

        switchStep.ClearDynamicPorts();

        Type enumType = AppDomain.CurrentDomain.GetAssemblies()
           .SelectMany(assembly => assembly.GetTypes())
           .FirstOrDefault(type => type.IsEnum && type.Name == switchStep.enumTypeName);

        if (enumType != null)
        {
            var enumValues = Enum.GetValues(enumType);

            foreach (var value in enumValues)
                switchStep.AddDynamicOutput(typeof(NodeObject), ConnectionType.Override, TypeConstraint.Strict, value.ToString());
        }
        else
            Debug.Log("<color=red>Couldn't find enum type with that name. </color>" + switchStep.enumTypeName);

        switchStep.previousEnum = switchStep.enumTypeName;
    }
}