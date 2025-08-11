using System.Collections.Generic;
using XNode;

namespace Praxilabs.xNode.Editor
{
    public static class HidePortsOnNodeSelect
    {
        private static List<NodePort> _ignoredPorts = new List<NodePort>();

        public static Node activeStep;

        public static void HidePortsOnSelect(StepsGraph currentGraph, bool isPortsHidden)
        {
            if (activeStep is not null && !isPortsHidden)
            {
                if (HidePortsHelper.IsPortsHidden(currentGraph))
                    RestorePortsColor(currentGraph);

                GetIgnoredPorts();
                HidePortsHelper.HidePorts(currentGraph, _ignoredPorts);
            }
            else if (HidePortsHelper.IsPortsHidden(currentGraph))
            {
                RestorePortsColor(currentGraph);
            }
        }

        private static void GetIgnoredPorts()
        {
            NodePort entryPort = activeStep.GetInputPort("entry");
            NodePort exitPort = activeStep.GetOutputPort("exit");

            if (entryPort is not null)
            {
                foreach (var connection in entryPort.connections)
                {
                    _ignoredPorts.Add(connection.node.GetOutputPort("exit"));
                }
            }

            if (exitPort is not null)
            {
                foreach (var connection in exitPort.connections)
                {
                    _ignoredPorts.Add(connection.node.GetInputPort("entry"));
                }
            }

            _ignoredPorts.Add(activeStep.GetInputPort("entry"));
            _ignoredPorts.Add(activeStep.GetOutputPort("exit"));
        }

        private static void RestorePortsColor(StepsGraph currentGraph)
        {
            HidePortsHelper.RestorePortsColor(currentGraph, _ignoredPorts);
            _ignoredPorts.Clear();
            currentGraph.originalInputColors.Clear();
            currentGraph.originalOutputColors.Clear();
        }
    }
}