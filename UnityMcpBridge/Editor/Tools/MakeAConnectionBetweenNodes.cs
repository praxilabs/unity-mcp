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
            string graphPath = args?["graphPath"]?.ToString();
            if (string.IsNullOrEmpty(graphPath))
            {
                return new { success = false, error = "Missing required argument: graphPath" };
            }

            // Load graph
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new { success = false, error = $"Could not load NodeGraph at path: {graphPath}" };
            }
            if (graph.nodes == null || graph.nodes.Count == 0)
            {
                return new { success = false, error = "Graph has no nodes" };
            }

            // Resolve endpoints
            Node fromNode = ResolveNode(graph, args?["fromNode"]);
            Node toNode = ResolveNode(graph, args?["toNode"]);
            if (fromNode == null || toNode == null)
            {
                return new { success = false, error = "Could not resolve 'fromNode' or 'toNode' in the graph" };
            }

            string fromPortName = args?["fromPort"]?.ToString();
            string toPortName = args?["toPort"]?.ToString();

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
                return new { success = false, error = $"No suitable output port found on '{fromNode.name}'" };
            }
            if (toPort == null)
            {
                return new { success = false, error = $"No suitable input port found on '{toNode.name}'" };
            }

            // Connect if not already
            bool alreadyConnected = fromPort.IsConnectedTo(toPort);
            if (!alreadyConnected)
            {
                fromPort.Connect(toPort);
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
            }

            return new
            {
                success = true,
                message = alreadyConnected
                    ? $"Ports already connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}"
                    : $"Connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}",
                graphPath,
                from = new { node = fromNode.name, port = fromPort.fieldName },
                to = new { node = toNode.name, port = toPort.fieldName }
            };
        }
        catch (Exception ex)
        {
            return new { success = false, error = $"Failed to connect nodes: {ex.Message}" };
        }
    }

    private static Node ResolveNode(NodeGraph graph, JToken token)
    {
        if (token == null) return null;

        // Try by ID
        if (token.Type == JTokenType.Integer)
        {
            int id = token.ToObject<int>();
            return graph.nodes.FirstOrDefault(n => n != null && n.GetInstanceID() == id);
        }

        // Try by name
        string name = token.ToString();
        Node byExact = graph.nodes.FirstOrDefault(n => n != null && n.name == name);
        if (byExact != null) return byExact;

        // Case-insensitive fallback
        return graph.nodes.FirstOrDefault(n => n != null && string.Equals(n.name, name, StringComparison.OrdinalIgnoreCase));
    }
}