# XNode Node Management Tools 🎯

**Tool Group**: XNode Node Management  
**Purpose**: Create, list, delete, and manage nodes in XNode graphs  
**Files**: `CreateXNodeNode.cs`, `ManageXNodeNode.cs`, `SetNodeAsFirstStep.cs`

Core tools for managing individual nodes within XNode graphs, including creation, deletion, positioning, and graph structure management.

## 🎯 Use Cases

- **Node Creation**: Add new nodes to experiment graphs
- **Node Discovery**: List available node types and existing nodes
- **Node Cleanup**: Remove unwanted or obsolete nodes
- **Graph Structure**: Set starting points and organize node positions
- **Node Validation**: Check node existence and graph integrity

## 🏗️ Implementation Details

### Node Creation (`create_xnode_node`)
```csharp
// Loads the target graph asset and finds the node type via reflection
NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
Type nodeType = FindNodeType(nodeTypeName);

// Creates the node and adds it as a sub-asset to the graph
Node newNode = graph.AddNode(nodeType);
newNode.position = new Vector2(posX, posY);
newNode.name = $"{nodeTypeName}_{newNode.GetInstanceID()}";

// Critical: Adds node as sub-asset to prevent orphaned assets
AssetDatabase.AddObjectToAsset(newNode, graph);
```

### Node Type Discovery (`list_available_node_types`)
```csharp
// Uses reflection to find all types inheriting from Node
var nodeTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => t.IsSubclassOf(typeof(Node)) && !t.IsAbstract)
    .Select(t => new { 
        name = t.Name, 
        fullName = t.FullName,
        assembly = t.Assembly.GetName().Name 
    });
```

### Node Deletion (`delete_xnode_node`)
```csharp
// Supports deletion by name or instance ID
Node nodeToDelete = ResolveNode(graph, nodeIdentifier);
if (nodeToDelete != null)
{
    graph.RemoveNode(nodeToDelete);
    AssetDatabase.RemoveObjectFromAsset(nodeToDelete);
    Object.DestroyImmediate(nodeToDelete);
}
```

### Node Positioning (`set_node_position`)
```csharp
// Updates node position and marks assets as dirty
Node node = graph.nodes.FirstOrDefault(n => n.name == nodeName);
if (node != null)
{
    node.position = new Vector2(positionX, positionY);
    EditorUtility.SetDirty(node);
    EditorUtility.SetDirty(graph);
}
```

### First Step Configuration (`set_node_as_first_step`)
```csharp
// Sets the entry point for graph execution
if (graph is StepsGraph stepsGraph)
{
    stepsGraph.firstStep = targetNode;
    EditorUtility.SetDirty(stepsGraph);
    AssetDatabase.SaveAssets();
}
```

### Python Side Integration
```python
# All tools follow the same pattern: parameter mapping and error handling
@mcp.tool()
def create_xnode_node(ctx: Context, graph_path: str, node_type_name: str, position_x: float = 0, position_y: float = 0):
    params = {
        "graphPath": graph_path,
        "nodeTypeName": node_type_name,
        "positionX": position_x,
        "positionY": position_y
    }
    connection = get_unity_connection()
    result = connection.send_command("create_xnode_node", params)
    return {"success": True, "data": result}
```

### System Integration
```csharp
// CommandRegistry.cs - All tools registered with consistent naming
{ "HandleCreateXNodeNode", CreateXNodeNode.HandleCommand },
{ "HandleManageXNodeNode", ManageXNodeNode.HandleCommand },
{ "HandleSetNodeAsFirstStep", SetNodeAsFirstStep.HandleCommand },

// UnityMcpBridge.cs - Command routing
"create_xnode_node" => CreateXNodeNode.HandleCommand(paramsObject),
"manage_xnode_node" => ManageXNodeNode.HandleCommand(paramsObject),
"set_node_as_first_step" => SetNodeAsFirstStep.HandleCommand(paramsObject),
```
