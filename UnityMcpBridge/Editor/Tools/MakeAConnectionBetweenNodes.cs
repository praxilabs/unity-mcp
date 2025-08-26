using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;

public static class MakeAConnectionBetweenNodes
{
    // Args:
    // - graphPath: string (required)
    // - fromNode: string (name) or int (instance id) (required)
    // - toNode: string (name) or int (instance id) (required)
    // - fromPort: string (optional) → defaults to first Output port
    // - toPort: string (optional) → defaults to first Input port
    public static object HandleCommand(JObject args)
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

            // Connect if not already
            bool alreadyConnected = fromPort.IsConnectedTo(toPort);
            if (!alreadyConnected)
            {
                fromPort.Connect(toPort);
                ToolUtils.SaveGraphChanges(graph);
            }

            string message = alreadyConnected
                ? $"Ports already connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}"
                : $"Connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}";

            return ToolUtils.CreateSuccessResponse(
                message,
                new
                {
                    graphPath,
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
}