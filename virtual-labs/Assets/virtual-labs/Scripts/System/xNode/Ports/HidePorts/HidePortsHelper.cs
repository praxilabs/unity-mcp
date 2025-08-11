using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Praxilabs.xNode.Editor
{

    public static class HidePortsHelper
    {
        public static void HidePorts(StepsGraph currentGraph, List<NodePort> ignoredPorts)
        {
            foreach (Node node in currentGraph.nodes)
            {
                foreach (NodePort port in node.Ports)
                {
                    if (port.IsInput)
                        currentGraph.originalInputColors.Add(node.inputPortColor);
                    else if (port.IsOutput)
                        currentGraph.originalOutputColors.Add(node.outputPortColor);
                }
                node.inputPortColor = currentGraph.hiddenPortsColor;
                node.outputPortColor = currentGraph.hiddenPortsColor;
            }
        }

        public static bool IsPortsHidden(StepsGraph currentGraph)
        {
            return currentGraph.originalInputColors.Count > 0 || currentGraph.originalOutputColors.Count > 0;
        }

        public static void RestorePortsColor(StepsGraph currentGraph, List<NodePort> ignoredPorts)
        {
            foreach (Node node in currentGraph.nodes)
            {
                foreach (NodePort port in node.Ports)
                {
                    if (port.IsInput)
                    {
                        node.inputPortColor = currentGraph.originalInputColors[0];
                        currentGraph.originalInputColors.RemoveAt(0);
                    }
                    else if (port.IsOutput)
                    {
                        node.outputPortColor = currentGraph.originalOutputColors[0];
                        currentGraph.originalOutputColors.RemoveAt(0);
                    }
                }
            }
        }
    }
}