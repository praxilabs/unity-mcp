using System.Collections.Generic;
using XNode;

namespace Praxilabs.xNode.Editor
{
    public static class HidePortsOnPortSelect
    {
        private static List<NodePort> ignoredPorts = new List<NodePort>();
        private static NodePort activePort;

        public static void HidePortsOnSelect(StepsGraph currentGraph, NodePort hoveredPort)
        {
            NodePort prevValue = activePort;
            activePort = hoveredPort;

            if (prevValue == activePort)
                return;

            if (activePort is not null)
            {
                GetIgnoredPorts(activePort);
                HidePortsHelper.HidePorts(currentGraph, ignoredPorts);
            }
            else if (HidePortsHelper.IsPortsHidden(currentGraph))
            {
                RestorePortsColor(currentGraph);
            }

        }

        public static void GetIgnoredPorts(NodePort hoveredPort)
        {
            NodePort entryPort = hoveredPort.node.GetInputPort("entry");
            NodePort exitPort = hoveredPort.node.GetOutputPort("exit");

            if (entryPort is not null && hoveredPort.direction is NodePort.IO.Input)
            {
                foreach (var connection in entryPort.connections)
                {
                    ignoredPorts.Add(connection.node.GetOutputPort("exit"));
                }
            }
            else if (exitPort is not null && hoveredPort.direction is NodePort.IO.Output)
            {
                foreach (var connection in exitPort.connections)
                {
                    ignoredPorts.Add(connection.node.GetInputPort("entry"));
                }
            }
        }

        public static void GetIgnoredPortsV2(NodePort hoveredPort)
        {
            var nodeInputPorts = hoveredPort.node.Inputs;
            var nodeOutputPorts = hoveredPort.node.Outputs;


            if (nodeInputPorts is not null && hoveredPort.direction is NodePort.IO.Input)
            {
                foreach (var inputPort in nodeInputPorts)
                {
                    foreach (var connection in inputPort.connections)
                    {
                        ignoredPorts.Add(connection.node.GetOutputPort("exit"));
                    }
                }
            }
            else if (nodeOutputPorts is not null && hoveredPort.direction is NodePort.IO.Output)
            {
                foreach (var outputPort in nodeOutputPorts)
                {
                    foreach (var connection in outputPort.connections)
                    {
                        ignoredPorts.Add(connection.node.GetInputPort("entry"));
                    }
                }
            }
        }

        private static void RestorePortsColor(StepsGraph currentGraph)
        {
            HidePortsHelper.RestorePortsColor(currentGraph, ignoredPorts);
            ignoredPorts.Clear();
            currentGraph.originalInputColors.Clear();
            currentGraph.originalOutputColors.Clear();
        }
    }
}