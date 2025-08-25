# Node Connection Tools 🔗

**Tool Group**: Node Connection Tools  
**Purpose**: Create connections between nodes in XNode graphs  
**File**: `MakeAConnectionBetweenNodes.cs` (4.3KB, 113 lines)

Connection management tools for linking nodes in XNode graphs, supporting different identification methods and port specification.

## 🎯 Use Cases

- **Graph Construction**: Build node-based experiment flows
- **Node Linking**: Connect nodes to create execution paths
- **Port Management**: Specify input/output ports for connections
- **Flexible Identification**: Connect nodes by name or instance ID
- **Connection Validation**: Ensure valid connections between compatible nodes

## 🏗️ Implementation Details

### Connection Creation (`make_connection_between_nodes`)
```csharp
public static class MakeAConnectionBetweenNodes
{
    public static object HandleCommand(JObject @params)
    {
        string graphPath = @params["graphPath"]?.ToString();
        JToken fromNode = @params["fromNode"];
        JToken toNode = @params["toNode"];
        string fromPort = @params["fromPort"]?.ToString();
        string toPort = @params["toPort"]?.ToString();

        try
        {
            // Load the graph and resolve nodes
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            Node fromNodeObj = ResolveNode(graph, fromNode);
            Node toNodeObj = ResolveNode(graph, toNode);

            // Find appropriate ports
            NodePort fromPortObj = FindOutputPort(fromNodeObj, fromPort);
            NodePort toPortObj = FindInputPort(toNodeObj, toPort);

            // Create the connection
            if (!fromPortObj.IsConnectedTo(toPortObj))
            {
                fromPortObj.Connect(toPortObj);
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
            }

            return new
            {
                success = true,
                message = $"Connected: {fromNodeObj.name}.{fromPortObj.fieldName} -> {toNodeObj.name}.{toPortObj.fieldName}"
            };
        }
        catch (Exception ex)
        {
            return new { success = false, error = $"Failed to connect nodes: {ex.Message}" };
        }
    }
}
```

### Node Resolution
```csharp
private static Node ResolveNode(NodeGraph graph, JToken token)
{
    if (token == null) return null;

    // Try by ID first
    if (token.Type == JTokenType.Integer)
    {
        int nodeId = token.ToObject<int>();
        return graph.nodes.FirstOrDefault(n => n.GetInstanceID() == nodeId);
    }

    // Try by name
    string nodeName = token.ToString();
    return graph.nodes.FirstOrDefault(n => n.name == nodeName);
}
```

### Port Discovery
```csharp
private static NodePort FindOutputPort(Node node, string portName)
{
    if (!string.IsNullOrEmpty(portName))
    {
        return node.GetOutputPort(portName);
    }

    // Default to first available output port
    return node.DynamicPorts?.FirstOrDefault(p => p.IsOutput)
        ?? node.Ports.FirstOrDefault(p => p.IsOutput);
}

private static NodePort FindInputPort(Node node, string portName)
{
    if (!string.IsNullOrEmpty(portName))
    {
        return node.GetInputPort(portName);
    }

    // Default to first available input port
    return node.DynamicPorts?.FirstOrDefault(p => p.IsInput)
        ?? node.Ports.FirstOrDefault(p => p.IsInput);
}
```

### Connection Validation
```csharp
// Check if ports are already connected
bool alreadyConnected = fromPort.IsConnectedTo(toPort);
if (alreadyConnected)
{
    return new
    {
        success = true,
        message = $"Ports already connected: {fromNode.name}.{fromPort.fieldName} -> {toNode.name}.{toPort.fieldName}"
    };
}
```

### Python Side Integration
```python
@mcp.tool()
def make_connection_between_nodes(
    ctx: Context,
    graph_path: str,
    from_node: str,
    to_node: str,
    from_port: str = None,
    to_port: str = None,
) -> Dict[str, Any]:
    try:
        params = {
            "graphPath": graph_path,
            "fromNode": from_node,
            "toNode": to_node,
            "fromPort": from_port,
            "toPort": to_port
        }
        params = {k: v for k, v in params.items() if v is not None}
        
        connection = get_unity_connection()
        result = connection.send_command("make_connection_between_nodes", params)
        return {"success": True, "data": result}
    except Exception as e:
        return {"success": False, "error": str(e)}

@mcp.tool()
def connect_nodes_by_name(
    ctx: Context,
    graph_path: str,
    from_node_name: str,
    to_node_name: str,
    from_port: str = None,
    to_port: str = None,
) -> Dict[str, Any]:
    # Convenience wrapper for name-based connections
    return make_connection_between_nodes(ctx, graph_path, from_node_name, to_node_name, from_port, to_port)

@mcp.tool()
def connect_nodes_by_id(
    ctx: Context,
    graph_path: str,
    from_node_id: int,
    to_node_id: int,
    from_port: str = None,
    to_port: str = None,
) -> Dict[str, Any]:
    # Convenience wrapper for ID-based connections
    return make_connection_between_nodes(ctx, graph_path, from_node_id, to_node_id, from_port, to_port)
```

### System Integration
```csharp
// CommandRegistry.cs
{ "HandleMakeAConnectionBetweenNodes", MakeAConnectionBetweenNodes.HandleCommand },

// UnityMcpBridge.cs
"make_connection_between_nodes" => MakeAConnectionBetweenNodes.HandleCommand(paramsObject),
```
