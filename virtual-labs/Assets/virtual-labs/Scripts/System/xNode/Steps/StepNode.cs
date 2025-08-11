using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using XNode;

#if UNITY_EDITOR
#endif

[NodeWidth(300)]
public abstract class StepNode : Node
{
    public string stepId = "s";
    public string tooltip;

    public Action<string> StepIsPerformed;
    public Action<string> StepIsAutomated;

    [HideInInspector] public int width = 300;
    [HideInInspector] public int originalWidth = 300;
    [HideInInspector] public bool ignorePrepareStep = false;

    public string NodeTint
    {
        get
        {
            return GetNodeTintHex();
        }
    }

    public virtual void IgnoreStep() { }

    public virtual void PrepareStep()
    {
        ResolveObjects();
        XnodeManager.Instance.CurrentStep = this;
    }

    public virtual void Execute() { }

    public virtual void Execute(GameObject go) { }

    public virtual void Exit() { }

    public abstract void AutomateStep();

    public void SetStepToFirstStep()
    {
        StepsGraph currentGraph = graph as StepsGraph;
        currentGraph.firstStep = this;
    }

    public object GetConnectedInputPortValue(StepNode node, string portName)
    {
        // Get the input port from the node
        NodePort inputPort = node.GetInputPort(portName);

        if (inputPort != null && inputPort.Connection != null)
        {
            // Get the connected node's outport
            NodePort connectedOutPort = inputPort.Connection;

            // Get the value from the outport
            object value = connectedOutPort.GetOutputValue();

            return value;
        }

        return null;
    }

    /// <summary>
    /// Used to apply values to step variables
    /// </summary>
    public virtual void ApplyValues() { }

    public virtual void ResolveObjects()
    { }

    private string GetNodeTintHex(float lightenAmount = 0.9f, float saturationBoost = 15f)
    {
        var nodeType = GetType();
        var tintAttribute = nodeType.GetCustomAttributes(typeof(NodeTintAttribute), true).FirstOrDefault() as NodeTintAttribute;

        if (tintAttribute != null)
        {
            var color = tintAttribute.color;

            color = Color.Lerp(color, Color.white, lightenAmount);

            float hue, saturation, value;
            Color.RGBToHSV(color, out hue, out saturation, out value);
            saturation = Mathf.Clamp(saturation * saturationBoost, 0f, 1f);
            color = Color.HSVToRGB(hue, saturation, value);

            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        return "#FFFFFF";
    }
}