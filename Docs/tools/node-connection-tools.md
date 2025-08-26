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

Node fromNode = ResolveNode(graph, args?["fromNode"]);
Node toNode = ResolveNode(graph, args?["toNode"]);


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


// Connect if not already
bool alreadyConnected = fromPort.IsConnectedTo(toPort);
if (!alreadyConnected)
{
fromPort.Connect(toPort);
ToolUtils.SaveGraphChanges(graph);
}



```
---

### Node Resolution
```csharp
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
```
---

### Python Side Integration
```python
@mcp.tool()
def make_connection_between_nodes(
    ctx: Context,
    graph_path: str,
    from_node: Union[str, int],
    to_node: Union[str, int],
    from_port: str = None,
    to_port: str = None
) -> Dict[str, Any]:
    """
    Creates a connection between two nodes in a NodeGraph.

    Args:
        graph_path: Path to the NodeGraph asset (required).
        from_node: Source node name (string) or instance ID (int) (required).
        to_node: Target node name (string) or instance ID (int) (required).
        from_port: Output port name on source node (optional, defaults to first output port).
        to_port: Input port name on target node (optional, defaults to first input port).

    Returns:
        Dictionary with results including success status, message, and connection details.
    """
    try:
        # Prepare parameters for Unity command
        params = {
            "graphPath": graph_path,
            "fromNode": from_node,
            "toNode": to_node
        }
        
        # Add optional port parameters if provided
        if from_port is not None:
            params["fromPort"] = from_port
        if to_port is not None:
            params["toPort"] = to_port
        
        connection = get_unity_connection()
        result = connection.send_command("make_connection_between_nodes", params)
        return result
        
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to make connection between nodes: {str(e)}"
        }

@mcp.tool()
def connect_nodes_by_name(
    ctx: Context,
    graph_path: str,
    from_node_name: str,
    to_node_name: str,
    from_port: str = None,
    to_port: str = None
) -> Dict[str, Any]:
    """
    Creates a connection between two nodes using their names.
    
    Convenience wrapper for make_connection_between_nodes using node names.

    Args:
        graph_path: Path to the NodeGraph asset.
        from_node_name: Name of the source node.
        to_node_name: Name of the target node.
        from_port: Output port name (optional).
        to_port: Input port name (optional).

    Returns:
        Dictionary with connection results.
    """
    return make_connection_between_nodes(
        ctx, graph_path, from_node_name, to_node_name, from_port, to_port
    )

@mcp.tool()
def connect_nodes_by_id(
    ctx: Context,
    graph_path: str,
    from_node_id: int,
    to_node_id: int,
    from_port: str = None,
    to_port: str = None
) -> Dict[str, Any]:
    """
    Creates a connection between two nodes using their instance IDs.
    
    Convenience wrapper for make_connection_between_nodes using node IDs.

    Args:
        graph_path: Path to the NodeGraph asset.
        from_node_id: Instance ID of the source node.
        to_node_id: Instance ID of the target node.
        from_port: Output port name (optional).
        to_port: Input port name (optional).

    Returns:
        Dictionary with connection results.
    """
    return make_connection_between_nodes(
        ctx, graph_path, from_node_id, to_node_id, from_port, to_port
    )
```

### System Integration
```csharp
// CommandRegistry.cs
{ "MakeConnectionBetweenNodes", MakeAConnectionBetweenNodes.HandleCommand },

// UnityMcpBridge.cs
"make_connection_between_nodes" => MakeAConnectionBetweenNodes.HandleCommand(paramsObject),
```
