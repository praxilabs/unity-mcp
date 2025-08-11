using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNodeEditor;
using static XNode.Node;

[CustomNodeEditor(typeof(SwitchStep))]
public class SwitchStepEditor : StepNodeEditor
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
            SwitchStep switchStep = currentNode as SwitchStep;

            if (GUILayout.Button("Generate Ports", GUILayout.Height(20), GUILayout.Width(130)))
                GeneratePorts(switchStep);
            DrawPorts(currentNode);
        }

        DisplayNodeFieldsButton(currentNode);
    }

    private void GeneratePorts(SwitchStep switchStep)
    {
        if (switchStep.currentCases == switchStep.previousCases) return;

        List<string> newCases = switchStep.currentCases.Except(switchStep.previousCases).ToList();
        List<string> removedCases = switchStep.previousCases.Except(switchStep.currentCases).ToList();

        foreach (string removedCase in removedCases)
            switchStep.RemoveDynamicPort(removedCase);

        foreach (string newCase in newCases)
            switchStep.AddDynamicOutput(typeof(NodeObject), ConnectionType.Override, TypeConstraint.Strict, newCase);

        switchStep.previousCases = new List<string>(switchStep.currentCases);
    }
}