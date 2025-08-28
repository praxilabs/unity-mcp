using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;

namespace UnityMcpBridge.Editor.Tools
{
    public static class ManageConnectionsBetweenNodes
{
    // Args:
    // - graphPath: string (required)
    // - fromNode: string (name) or int (instance id) (required)
    // - toNode: string (name) or int (instance id) (required)
    // - fromPort: string (optional) → defaults to first Output port
    // - toPort: string (optional) → defaults to first Input port
    public static object CreateConnection(JObject args)
    {
        try
        {
            // Validate required parameters using ToolUtils
            var (isValidGraphPath, graphPath, graphPathError) = ToolUtils.ValidateRequiredString(args, "graphPath", "graphPath");
            if (!isValidGraphPath)
            {
                return ToolUtils.CreateErrorResponse(graphPathError);
            }

            // Load and validate graph using ToolUtils
            var (isValidGraph, graph, graphError) = ToolUtils.ValidateGraphPath(graphPath);
            if (!isValidGraph)
            {
                return ToolUtils.CreateErrorResponse(graphError);
            }

            if (graph.nodes == null || graph.nodes.Count == 0)
            {
                return ToolUtils.CreateErrorResponse("Graph has no nodes");
            }

            // Resolve endpoints using ToolUtils
            Node fromNode = ResolveNode(graph, args?["fromNode"]);
            Node toNode = ResolveNode(graph, args?["toNode"]);
            if (fromNode == null || toNode == null)
            {
                return ToolUtils.CreateErrorResponse("Could not resolve 'fromNode' or 'toNode' in the graph");
            }

            string fromPortName = ToolUtils.GetOptionalString(args, "fromPort");
            string toPortName = ToolUtils.GetOptionalString(args, "toPort");

            // Pick ports with sensible defaults
            NodePort fromPort = !string.IsNullOrEmpty(fromPortName)
                ? fromNode.GetOutputPort(fromPortName)
                : fromNode.DynamicPorts?.FirstOrDefault(p => p.IsOutput)
                    ?? fromNode.Ports.FirstOrDefault(p => p.IsOutput);

            NodePort toPort = !string.IsNullOrEmpty(toPortName)
                ? toNode.GetInputPort(toPortName)
                : toNode.DynamicPorts?.FirstOrDefault(p => p.IsInput)
                    ?? toNode.Ports.FirstOrDefault(p => p.IsInput);

            if (fromPort == null)
            {
                return ToolUtils.CreateErrorResponse($"No suitable output port found on '{fromNode.name}'");
            }
            if (toPort == null)
            {
                return ToolUtils.CreateErrorResponse($"No suitable input port found on '{toNode.name}'");
            }

            // Check if connection exists
            bool isConnected = fromPort.IsConnectedTo(toPort);
            string message;

            if (!isConnected)
            {
                fromPort.Connect(toPort);
                ToolUtils.SaveGraphChanges(graph);
                message = $"Connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}";
            }
            else
            {
                message = $"Ports already connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}";
            }

            return ToolUtils.CreateSuccessResponse(
                message,
                new
                {
                    graphPath,
                    action = "connect",
                    from = new { node = fromNode.name, port = fromPort.fieldName },
                    to = new { node = toNode.name, port = toPort.fieldName }
                }
            );
        }
        catch (Exception ex)
        {
            return ToolUtils.CreateErrorResponse($"Failed to connect nodes: {ex.Message}");
        }
    }

    // Args:
    // - graphPath: string (required)
    // - fromNode: string (name) or int (instance id) (required)
    // - toNode: string (name) or int (instance id) (required)
    // - fromPort: string (optional) → defaults to first Output port
    // - toPort: string (optional) → defaults to first Input port
    public static object DeleteConnection(JObject args)
    {
        try
        {
            // Validate required parameters using ToolUtils
            var (isValidGraphPath, graphPath, graphPathError) = ToolUtils.ValidateRequiredString(args, "graphPath", "graphPath");
            if (!isValidGraphPath)
            {
                return ToolUtils.CreateErrorResponse(graphPathError);
            }

            // Load and validate graph using ToolUtils
            var (isValidGraph, graph, graphError) = ToolUtils.ValidateGraphPath(graphPath);
            if (!isValidGraph)
            {
                return ToolUtils.CreateErrorResponse(graphError);
            }

            if (graph.nodes == null || graph.nodes.Count == 0)
            {
                return ToolUtils.CreateErrorResponse("Graph has no nodes");
            }

            // Resolve endpoints using ToolUtils
            Node fromNode = ResolveNode(graph, args?["fromNode"]);
            Node toNode = ResolveNode(graph, args?["toNode"]);
            if (fromNode == null || toNode == null)
            {
                return ToolUtils.CreateErrorResponse("Could not resolve 'fromNode' or 'toNode' in the graph");
            }

            string fromPortName = ToolUtils.GetOptionalString(args, "fromPort");
            string toPortName = ToolUtils.GetOptionalString(args, "toPort");

            // Pick ports with sensible defaults
            NodePort fromPort = !string.IsNullOrEmpty(fromPortName)
                ? fromNode.GetOutputPort(fromPortName)
                : fromNode.DynamicPorts?.FirstOrDefault(p => p.IsOutput)
                    ?? fromNode.Ports.FirstOrDefault(p => p.IsOutput);

            NodePort toPort = !string.IsNullOrEmpty(toPortName)
                ? toNode.GetInputPort(toPortName)
                : toNode.DynamicPorts?.FirstOrDefault(p => p.IsInput)
                    ?? toNode.Ports.FirstOrDefault(p => p.IsInput);

            if (fromPort == null)
            {
                return ToolUtils.CreateErrorResponse($"No suitable output port found on '{fromNode.name}'");
            }
            if (toPort == null)
            {
                return ToolUtils.CreateErrorResponse($"No suitable input port found on '{toNode.name}'");
            }

            // Check if connection exists
            bool isConnected = fromPort.IsConnectedTo(toPort);
            string message;

            if (isConnected)
            {
                // Disconnect both directions to ensure complete removal
                fromPort.Disconnect(toPort);
                toPort.Disconnect(fromPort);
                
                ToolUtils.SaveGraphChanges(graph);
                message = $"Disconnected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}";
            }
            else
            {
                message = $"Ports not connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}";
            }

            return ToolUtils.CreateSuccessResponse(
                message,
                new
                {
                    graphPath,
                    action = "disconnect",
                    from = new { node = fromNode.name, port = fromPort.fieldName },
                    to = new { node = toNode.name, port = toPort.fieldName }
                }
            );
        }
        catch (Exception ex)
        {
            return ToolUtils.CreateErrorResponse($"Failed to disconnect nodes: {ex.Message}");
        }
    }

    private static Node ResolveNode(NodeGraph graph, JToken token)
    {
        if (token == null) return null;

        // Try by ID
        if (token.Type == JTokenType.Integer)
        {
            int id = token.ToObject<int>();
            return ToolUtils.FindNodeById(graph, id);
        }

        // Try by name
        string name = token.ToString();
        Node byExact = ToolUtils.FindNodeByName(graph, name);
        if (byExact != null) return byExact;

        // Case-insensitive fallback
        return graph.nodes.FirstOrDefault(n => n != null && string.Equals(n.name, name, StringComparison.OrdinalIgnoreCase));
    }

    // Args:
    // - graphPath: string (required)
    // - node: string (name) or int (instance id) (required)
    // - port: string (optional) → deletes all connections from this port
    public static object DeleteAllConnectionsFromNode(JObject args)
    {
        try
        {
            var (isValidGraphPath, graphPath, graphPathError) = ToolUtils.ValidateRequiredString(args, "graphPath", "graphPath");
            if (!isValidGraphPath)
            {
                return ToolUtils.CreateErrorResponse(graphPathError);
            }

            var (isValidGraph, graph, graphError) = ToolUtils.ValidateGraphPath(graphPath);
            if (!isValidGraph)
            {
                return ToolUtils.CreateErrorResponse(graphError);
            }

            Node node = ResolveNode(graph, args?["node"]);
            if (node == null)
            {
                return ToolUtils.CreateErrorResponse("Could not resolve 'node' in the graph");
            }

            string portName = ToolUtils.GetOptionalString(args, "port");
            int deletedCount = 0;

            if (!string.IsNullOrEmpty(portName))
            {
                NodePort port = node.GetOutputPort(portName);
                if (port != null)
                {
                    deletedCount = port.ConnectionCount;
                    port.ClearConnections();
                }
            }
            else
            {
                foreach (NodePort port in node.Ports.Where(p => p.IsOutput))
                {
                    deletedCount += port.ConnectionCount;
                    port.ClearConnections();
                }
            }

            ToolUtils.SaveGraphChanges(graph);
            return ToolUtils.CreateSuccessResponse(
                $"Deleted {deletedCount} connections from node '{node.name}'",
                new { graphPath, node = node.name, deletedCount }
            );
        }
        catch (Exception ex)
        {
            return ToolUtils.CreateErrorResponse($"Failed to delete connections from node: {ex.Message}");
        }
    }

    // Args:
    // - graphPath: string (required)
    // - node: string (name) or int (instance id) (required)
    // - port: string (optional) → deletes all connections to this port
    public static object DeleteAllConnectionsToNode(JObject args)
    {
        try
        {
            var (isValidGraphPath, graphPath, graphPathError) = ToolUtils.ValidateRequiredString(args, "graphPath", "graphPath");
            if (!isValidGraphPath)
            {
                return ToolUtils.CreateErrorResponse(graphPathError);
            }

            var (isValidGraph, graph, graphError) = ToolUtils.ValidateGraphPath(graphPath);
            if (!isValidGraph)
            {
                return ToolUtils.CreateErrorResponse(graphError);
            }

            Node node = ResolveNode(graph, args?["node"]);
            if (node == null)
            {
                return ToolUtils.CreateErrorResponse("Could not resolve 'node' in the graph");
            }

            string portName = ToolUtils.GetOptionalString(args, "port");
            int deletedCount = 0;

            if (!string.IsNullOrEmpty(portName))
            {
                NodePort port = node.GetInputPort(portName);
                if (port != null)
                {
                    deletedCount = port.ConnectionCount;
                    port.ClearConnections();
                }
            }
            else
            {
                foreach (NodePort port in node.Ports.Where(p => p.IsInput))
                {
                    deletedCount += port.ConnectionCount;
                    port.ClearConnections();
                }
            }

            ToolUtils.SaveGraphChanges(graph);
            return ToolUtils.CreateSuccessResponse(
                $"Deleted {deletedCount} connections to node '{node.name}'",
                new { graphPath, node = node.name, deletedCount }
            );
        }
        catch (Exception ex)
        {
            return ToolUtils.CreateErrorResponse($"Failed to delete connections to node: {ex.Message}");
        }
    }

    // Args:
    // - graphPath: string (required)
    public static object DeleteAllConnectionsInGraph(JObject args)
    {
        try
        {
            var (isValidGraphPath, graphPath, graphPathError) = ToolUtils.ValidateRequiredString(args, "graphPath", "graphPath");
            if (!isValidGraphPath)
            {
                return ToolUtils.CreateErrorResponse(graphPathError);
            }

            var (isValidGraph, graph, graphError) = ToolUtils.ValidateGraphPath(graphPath);
            if (!isValidGraph)
            {
                return ToolUtils.CreateErrorResponse(graphError);
            }

            int totalDeleted = 0;
            foreach (Node node in graph.nodes.Where(n => n != null))
            {
                foreach (NodePort port in node.Ports)
                {
                    totalDeleted += port.ConnectionCount;
                    port.ClearConnections();
                }
            }

            ToolUtils.SaveGraphChanges(graph);
            return ToolUtils.CreateSuccessResponse(
                $"Deleted {totalDeleted} connections from graph",
                new { graphPath, deletedCount = totalDeleted }
            );
        }
        catch (Exception ex)
        {
            return ToolUtils.CreateErrorResponse($"Failed to delete all connections in graph: {ex.Message}");
        }
    }
}
}
