using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;

public static class MakeAConnectionBetweenNodes
{
    // Args for make_connection_between_nodes:
    // - graphPath: string (required)
    // - fromNode: string (name) or int (instance id) (required)
    // - toNode: string (name) or int (instance id) (required)
    // - fromPort: string (optional) → defaults to first Output port
    // - toPort: string (optional) → defaults to first Input port
    // - action: string (optional) → "connect" or "disconnect" (defaults to "connect")
    //
    // Args for delete_connection_between_nodes:
    // - graphPath: string (required)
    // - fromNode: string (name) or int (instance id) (required)
    // - toNode: string (name) or int (instance id) (required)
    // - fromPort: string (optional) → defaults to first Output port
    // - toPort: string (optional) → defaults to first Input port
    //
    // Args for delete_all_connections_from_node:
    // - graphPath: string (required)
    // - node: string (name) or int (instance id) (required)
    // - port: string (optional) → defaults to all output ports
    //
    // Args for delete_all_connections_to_node:
    // - graphPath: string (required)
    // - node: string (name) or int (instance id) (required)
    // - port: string (optional) → defaults to all input ports
    //
    // Args for delete_all_connections_in_graph:
    // - graphPath: string (required)
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

            // Determine command type based on available parameters
            string commandType = DetermineCommandType(args);

            switch (commandType)
            {
                case "make_connection_between_nodes":
                case "delete_connection_between_nodes":
                    return HandleConnectionBetweenNodes(graph, args, commandType);
                case "delete_all_connections_from_node":
                    return HandleDeleteAllConnectionsFromNode(graph, args);
                case "delete_all_connections_to_node":
                    return HandleDeleteAllConnectionsToNode(graph, args);
                case "delete_all_connections_in_graph":
                    return HandleDeleteAllConnectionsInGraph(graph, args);
                default:
                    return ToolUtils.CreateErrorResponse($"Unknown command type: {commandType}");
            }
        }
        catch (Exception ex)
        {
            return ToolUtils.CreateErrorResponse($"Failed to handle connection command: {ex.Message}");
        }
    }

    private static string DetermineCommandType(JObject args)
    {
        // Check for specific command indicators
        if (args.ContainsKey("fromNode") && args.ContainsKey("toNode"))
        {
            string action = ToolUtils.GetOptionalString(args, "action") ?? "connect";
            return action.ToLower() == "disconnect" ? "delete_connection_between_nodes" : "make_connection_between_nodes";
        }
        else if (args.ContainsKey("node"))
        {
            // Check if it's a "from" or "to" operation based on context
            // For now, we'll need to infer from the calling context
            // This might need adjustment based on how the Python side calls these
            return "delete_all_connections_from_node"; // Default assumption
        }
        else
        {
            return "delete_all_connections_in_graph";
        }
    }

    private static object HandleConnectionBetweenNodes(NodeGraph graph, JObject args, string commandType)
    {
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

        bool isConnected = fromPort.IsConnectedTo(toPort);
        string message;

        if (commandType == "delete_connection_between_nodes")
        {
            if (isConnected)
            {
                fromPort.Disconnect(toPort);
                ToolUtils.SaveGraphChanges(graph);
                message = $"Disconnected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}";
            }
            else
            {
                message = $"Ports not connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}";
            }
        }
        else // make_connection_between_nodes
        {
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
        }

        return ToolUtils.CreateSuccessResponse(
            message,
            new
            {
                graphPath = args["graphPath"].ToString(),
                action = commandType == "delete_connection_between_nodes" ? "disconnect" : "connect",
                from = new { node = fromNode.name, port = fromPort.fieldName },
                to = new { node = toNode.name, port = toPort.fieldName }
            }
        );
    }

    private static object HandleDeleteAllConnectionsFromNode(NodeGraph graph, JObject args)
    {
        Node node = ResolveNode(graph, args?["node"]);
        if (node == null)
        {
            return ToolUtils.CreateErrorResponse("Could not resolve 'node' in the graph");
        }

        string portName = ToolUtils.GetOptionalString(args, "port");
        int connectionsDeleted = 0;

        if (!string.IsNullOrEmpty(portName))
        {
            // Delete connections from specific port
            NodePort port = node.GetOutputPort(portName);
            if (port == null)
            {
                return ToolUtils.CreateErrorResponse($"Output port '{portName}' not found on node '{node.name}'");
            }

            var connections = port.GetConnections().ToList();
            foreach (var connection in connections)
            {
                port.Disconnect(connection.node, connection.fieldName);
                connectionsDeleted++;
            }
        }
        else
        {
            // Delete all output connections
            foreach (var port in node.Ports.Where(p => p.IsOutput))
            {
                var connections = port.GetConnections().ToList();
                foreach (var connection in connections)
                {
                    port.Disconnect(connection.node, connection.fieldName);
                    connectionsDeleted++;
                }
            }
        }

        ToolUtils.SaveGraphChanges(graph);
        return ToolUtils.CreateSuccessResponse(
            $"Deleted {connectionsDeleted} connections from node '{node.name}'",
            new { graphPath = args["graphPath"].ToString(), node = node.name, connectionsDeleted }
        );
    }

    private static object HandleDeleteAllConnectionsToNode(NodeGraph graph, JObject args)
    {
        Node node = ResolveNode(graph, args?["node"]);
        if (node == null)
        {
            return ToolUtils.CreateErrorResponse("Could not resolve 'node' in the graph");
        }

        string portName = ToolUtils.GetOptionalString(args, "port");
        int connectionsDeleted = 0;

        if (!string.IsNullOrEmpty(portName))
        {
            // Delete connections to specific port
            NodePort port = node.GetInputPort(portName);
            if (port == null)
            {
                return ToolUtils.CreateErrorResponse($"Input port '{portName}' not found on node '{node.name}'");
            }

            var connections = port.GetConnections().ToList();
            foreach (var connection in connections)
            {
                connection.node.GetOutputPort(connection.fieldName).Disconnect(node, portName);
                connectionsDeleted++;
            }
        }
        else
        {
            // Delete all input connections
            foreach (var port in node.Ports.Where(p => p.IsInput))
            {
                var connections = port.GetConnections().ToList();
                foreach (var connection in connections)
                {
                    connection.node.GetOutputPort(connection.fieldName).Disconnect(node, port.fieldName);
                    connectionsDeleted++;
                }
            }
        }

        ToolUtils.SaveGraphChanges(graph);
        return ToolUtils.CreateSuccessResponse(
            $"Deleted {connectionsDeleted} connections to node '{node.name}'",
            new { graphPath = args["graphPath"].ToString(), node = node.name, connectionsDeleted }
        );
    }

    private static object HandleDeleteAllConnectionsInGraph(NodeGraph graph, JObject args)
    {
        int totalConnectionsDeleted = 0;

        foreach (var node in graph.nodes.Where(n => n != null))
        {
            foreach (var port in node.Ports)
            {
                var connections = port.GetConnections().ToList();
                foreach (var connection in connections)
                {
                    if (port.IsOutput)
                    {
                        port.Disconnect(connection.node, connection.fieldName);
                    }
                    else
                    {
                        connection.node.GetOutputPort(connection.fieldName).Disconnect(node, port.fieldName);
                    }
                    totalConnectionsDeleted++;
                }
            }
        }

        ToolUtils.SaveGraphChanges(graph);
        return ToolUtils.CreateSuccessResponse(
            $"Deleted {totalConnectionsDeleted} connections from graph",
            new { graphPath = args["graphPath"].ToString(), connectionsDeleted = totalConnectionsDeleted }
        );
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