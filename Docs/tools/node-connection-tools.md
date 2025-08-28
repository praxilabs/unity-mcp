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

---

## 🗑️ Connection Deletion

### Overview
Connection deletion tools allow you to remove connections between nodes in XNode graphs, supporting various deletion strategies and identification methods.

### Deletion Methods

#### 1. **Delete Specific Connection** (`delete_connection_between_nodes`)
Removes a specific connection between two nodes.

**Parameters:**
- `graph_path`: Path to the NodeGraph asset (required)
- `from_node`: Source node name (string) or instance ID (int) (required)
- `to_node`: Target node name (string) or instance ID (int) (required)
- `from_port`: Output port name on source node (optional, defaults to first output port)
- `to_port`: Input port name on target node (optional, defaults to first input port)

**Example:**
```python
# Delete connection from "ClickStep" to "DelayStep"
delete_connection_between_nodes(
    graph_path="Assets/Testing/Graphs/MyGraph.asset",
    from_node="ClickStep",
    to_node="DelayStep"
)
```

#### 2. **Delete All Connections From Node** (`delete_all_connections_from_node`)
Removes all output connections from a specific node.

**Parameters:**
- `graph_path`: Path to the NodeGraph asset (required)
- `node`: Node name (string) or instance ID (int) (required)
- `port`: Output port name (optional, deletes all connections from this port)

**Example:**
```python
# Delete all output connections from "ClickStep"
delete_all_connections_from_node(
    graph_path="Assets/Testing/Graphs/MyGraph.asset",
    node="ClickStep"
)
```

#### 3. **Delete All Connections To Node** (`delete_all_connections_to_node`)
Removes all input connections to a specific node.

**Parameters:**
- `graph_path`: Path to the NodeGraph asset (required)
- `node`: Node name (string) or instance ID (int) (required)
- `port`: Input port name (optional, deletes all connections to this port)

**Example:**
```python
# Delete all input connections to "DelayStep"
delete_all_connections_to_node(
    graph_path="Assets/Testing/Graphs/MyGraph.asset",
    node="DelayStep"
)
```

#### 4. **Delete All Connections In Graph** (`delete_all_connections_in_graph`)
Removes all connections in the entire graph.

**Parameters:**
- `graph_path`: Path to the NodeGraph asset (required)

**Example:**
```python
# Delete all connections in the graph
delete_all_connections_in_graph(
    graph_path="Assets/Testing/Graphs/MyGraph.asset"
)
```

### Convenience Wrappers

#### Delete by Name (`delete_connection_by_name`)
```python
@mcp.tool()
def delete_connection_by_name(
    ctx: Context,
    graph_path: str,
    from_node_name: str,
    to_node_name: str,
    from_port: str = None,
    to_port: str = None
) -> Dict[str, Any]:
    """
    Deletes a connection between two nodes using their names.
    
    Convenience wrapper for delete_connection_between_nodes using node names.
    """
```

#### Delete by ID (`delete_connection_by_id`)
```python
@mcp.tool()
def delete_connection_by_id(
    ctx: Context,
    graph_path: str,
    from_node_id: int,
    to_node_id: int,
    from_port: str = None,
    to_port: str = None
) -> Dict[str, Any]:
    """
    Deletes a connection between two nodes using their instance IDs.
    
    Convenience wrapper for delete_connection_between_nodes using node IDs.
    """
```

### Implementation Details

#### Unity Side (C#)
```csharp
// Delete specific connection
NodePort fromPort = fromNode.GetOutputPort(fromPortName ?? fromNode.OutputPorts.First().fieldName);
NodePort toPort = toNode.GetInputPort(toPortName ?? toNode.InputPorts.First().fieldName);

if (fromPort.IsConnectedTo(toPort))
{
    fromPort.Disconnect(toPort);
    ToolUtils.SaveGraphChanges(graph);
}

// Delete all connections from node
foreach (NodePort outputPort in fromNode.OutputPorts)
{
    outputPort.ClearConnections();
}
ToolUtils.SaveGraphChanges(graph);

// Delete all connections in graph
foreach (Node node in graph.nodes)
{
    foreach (NodePort port in node.Ports)
    {
        port.ClearConnections();
    }
}
ToolUtils.SaveGraphChanges(graph);
```

#### Python Side Integration
```python
@mcp.tool()
def delete_connection_between_nodes(
    ctx: Context,
    graph_path: str,
    from_node: Union[str, int],
    to_node: Union[str, int],
    from_port: str = None,
    to_port: str = None
) -> Dict[str, Any]:
    """
    Deletes a connection between two nodes in a NodeGraph.
    """
    try:
        params = {
            "graphPath": graph_path,
            "fromNode": from_node,
            "toNode": to_node
        }
        
        if from_port is not None:
            params["fromPort"] = from_port
        if to_port is not None:
            params["toPort"] = to_port
        
        connection = get_unity_connection()
        result = connection.send_command("delete_connection_between_nodes", params)
        return result
        
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to delete connection between nodes: {str(e)}"
        }
```

### Use Cases

- **Graph Cleanup**: Remove unwanted or incorrect connections
- **Node Isolation**: Disconnect nodes before repositioning or modifying
- **Graph Reset**: Clear all connections to start fresh
- **Error Correction**: Remove connections that cause validation errors
- **Performance Optimization**: Remove unnecessary connections that slow down graph execution

### Best Practices

1. **Verify Before Deletion**: Check if connections exist before attempting deletion
2. **Use Specific Ports**: Specify port names when multiple ports exist to avoid ambiguity
3. **Graph Validation**: Validate graph integrity after bulk deletions
4. **Backup Strategy**: Consider backing up graph state before major connection changes
5. **Incremental Deletion**: Use specific deletion methods rather than clearing entire graphs when possible
