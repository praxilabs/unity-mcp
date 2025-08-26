# XNode Node Management Tools 🎯

**Tool Group**: XNode Node Management  
**Purpose**: Create, list, delete, and manage nodes in XNode graphs  
**Files**: `CreateXNodeNode.cs`, `ManageXNodeNode.cs`, `SetNodeAsFirstStep.cs`

Core tools for managing individual nodes within XNode graphs, including creation, deletion, positioning, and graph structure management.

Keep in mind that the next shown code snippets are simplified code snippets for expalaination and they won't necessarily work. the working implementation is in the **[UnityMCPBridge](https://github.com/praxilabs/unity-mcp/tree/main/UnityMcpBridge/Editor/Tools)** if you want to check it out

## 🎯 Use Cases

- **Node Creation**: Add new nodes to experiment graphs
- **Node Discovery**: List available node types and existing nodes
- **Node Cleanup**: Remove unwanted or obsolete nodes
- **Graph Structure**: Set starting points and organize node positions
- **Node Validation**: Check node existence and graph integrity

## 🏗️ Implementation Details

### Node Creation (`create_xnode_node`)

```csharp            
var graph = ToolUtils.LoadNodeGraph(validation.GraphPath);

// Find the node type
Type nodeType = ToolUtils.FindNodeType(validation.NodeTypeName);

// Create the node
Node newNode = graph.AddNode(nodeType);

// Configure the node
ConfigureNewNode(newNode, validation.NodeTypeName, validation.PositionX, validation.PositionY);
```
<br>

```csharp
private static void ConfigureNewNode(Node newNode, string nodeTypeName, float posX, float posY)
{
    newNode.position = new Vector2(posX, posY);
    newNode.name = $"{nodeTypeName}_{newNode.GetInstanceID()}";

    // Add the node as a sub-asset to the graph
    if (!AssetDatabase.Contains(newNode))
    {
        AssetDatabase.AddObjectToAsset(newNode, newNode.graph);
    }
}
```
---

### Node Type Discovery (`list_available_node_types`)

```csharp

//we will go through the ToolUtils in the utility-tools.md
var nodeTypes = ToolUtils.GetAvailableNodeTypes();
```
---

### Node Deletion (`delete_xnode_node`)
```csharp
// Load and validate graph
var graph = ToolUtils.LoadNodeGraph(validation.GraphPath);

// Find the node
var node = ToolUtils.FindNodeByIdentifier(graph, validation.NodeIdentifier, validation.IdentifierType);

// Delete the node
var deletionResult = DeleteSingleNode(graph, node, validation.NodeIdentifier);
```
<br>

```csharp
private static (bool Success, object NodeInfo, object ErrorInfo) DeleteSingleNode(NodeGraph graph, Node node, string nodeIdentifier)
{
    string nodeName = node.name;
    int nodeId = node.GetInstanceID();
    
    graph.RemoveNode(node);
    
    // Remove the node as a sub-asset from the AssetDatabase
    if (AssetDatabase.Contains(node))
    {
        AssetDatabase.RemoveObjectFromAsset(node);
    }
    
    var nodeInfo = new {
        name = nodeName,
        id = nodeId,
        identifier = nodeIdentifier
    };
    
    return (true, nodeInfo, null);
}

```
---

### Node Deletion (`delete_multiple_nodes`)
```csharp
// Load and validate graph
var graph = LoadNodeGraph(validation.GraphPath);

// Process node deletions
var result = ProcessNodeDeletions(graph, validation.NodeIdentifiers, validation.IdentifierType);


```
<br>

```csharp
private static (int DeletedCount, int FailedCount, List<object> DeletedNodes, List<object> FailedNodes) ProcessNodeDeletions(NodeGraph graph, string[] nodeIdentifiers, string identifierType)
{
    var deletedNodes = new List<object>();
    var failedNodes = new List<object>();

    foreach (string nodeIdentifier in nodeIdentifiers)
    {
        var node = FindNodeByIdentifier(graph, nodeIdentifier, identifierType);
        
        if (node != null)
        {
            var deletionResult = DeleteSingleNode(graph, node, nodeIdentifier);
            if (deletionResult.Success)
            {
                deletedNodes.Add(deletionResult.NodeInfo);
            }
            else
            {
                failedNodes.Add(deletionResult.ErrorInfo);
            }
        }
        else
        {
            failedNodes.Add(new {
                identifier = nodeIdentifier,
                error = $"Node not found with {identifierType}: {nodeIdentifier}"
            });
        }
    }

    return (deletedNodes.Count, failedNodes.Count, deletedNodes, failedNodes);
}
```
---

### Node Positioning (`set_node_position`)
```csharp
// Load and validate graph
var graph = LoadNodeGraph(validation.GraphPath);

// Find the node
var node = FindNodeByIdentifier(graph, validation.NodeId, "id");

SetNodePosition(node, validation.PositionX, validation.PositionY);
```
<br>

```csharp
private static void SetNodePosition(Node node, float posX, float posY)
{
    node.position = new Vector2(posX, posY);
}
```
---

### First Step Configuration (`set_node_as_first_step`)
```csharp
// Find target node
var targetNode = FindTargetNode(graph, args);

// Set first step node
if (!ToolUtils.SetFirstStepNode(graph, targetNode))
{
    //Return an Error Response.
}
```
<br>

```csharp
public static bool SetFirstStepNode(NodeGraph graph, Node targetNode)
{
    
    SerializedObject so = new SerializedObject(graph);
    SerializedProperty firstStepProp = so.FindProperty("firstStep");

    firstStepProp.objectReferenceValue = targetNode;

    so.ApplyModifiedProperties();
    
    return true;
}
```
---

### Node Listing (`list_available_nodes`)

```csharp
public static object ListGraphNodes(JObject args)


if (graph.nodes == null || graph.nodes.Count == 0)
{
    //return a success Response with an empty list.
}

// Get nodes information
var nodesList = ToolUtils.GetGraphNodesInfo(graph);

//return a success response with the graph nodes.        

```
---

### Python Side Integration
```python
@mcp.tool()
def create_xnode_node(
    ctx: Context,
    graph_path: str,
    node_type_name: str,
    position_x: float = 0,
    position_y: float = 0
) -> Dict[str, Any]:
    """
    Creates a new node in an existing xNode graph.

    Args:
        graph_path: Path to the NodeGraph asset (e.g., "Assets/Testing/Graphs/NewStepsGraph.asset").
        node_type_name: Name of the node type to create (e.g., "ClickStep", "DelayStep").
        position_x: X position of the node in the graph editor.
        position_y: Y position of the node in the graph editor.

    Returns:
        Dictionary with results ('success', 'message', 'nodeId', etc.).
    """
    try:
        connection = get_unity_connection()
        params = {
            "graphPath": graph_path,
            "nodeTypeName": node_type_name,
            "positionX": position_x,
            "positionY": position_y
        }
        result = connection.send_command("create_xnode_node", params)
        return result
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to create XNode node: {str(e)}"
        }
```
<br>

```python
@mcp.tool()
def delete_xnode_node(
    ctx: Context,
    graph_path: str,
    node_identifier: str,
    identifier_type: str = "name"
) -> Dict[str, Any]:
    """
    Deletes a node from an existing xNode graph.

    Args:
        graph_path: Path to the NodeGraph asset (e.g., "Assets/Testing/Graphs/NewStepsGraph.asset").
        node_identifier: Node name (string) or instance ID (int) to delete.
        identifier_type: How to identify the node - "name" or "id" (default: "name").

    Returns:
        Dictionary with results ('success', 'message', 'deletedNode', etc.).
    """
    try:
        connection = get_unity_connection()
        params = {
            "graphPath": graph_path,
            "nodeIdentifier": node_identifier,
            "identifierType": identifier_type
        }
        result = connection.send_command("delete_xnode_node", params)
        return result
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to delete XNode node: {str(e)}"
        }
```
<br>

```python
@mcp.tool()
def delete_multiple_nodes(
    ctx: Context,
    graph_path: str,
    node_identifiers: List[str],
    identifier_type: str = "name"
) -> Dict[str, Any]:
    """
    Deletes multiple nodes from an xNode graph.

    Args:
        graph_path: Path to the NodeGraph asset.
        node_identifiers: List of node names or IDs to delete.
        identifier_type: How to identify nodes - "name" or "id" (default: "name").

    Returns:
        Dictionary with results including success count and any failures.
    """
    try:
        connection = get_unity_connection()
        params = {
            "graphPath": graph_path,
            "nodeIdentifiers": node_identifiers,
            "identifierType": identifier_type
        }
        result = connection.send_command("delete_multiple_nodes", params)
        return result
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to delete multiple nodes: {str(e)}"
        }
```
<br>

```python
@mcp.tool()
def list_graph_nodes(
    ctx: Context,
    graph_path: str
) -> Dict[str, Any]:
    """
    Lists all nodes in a StepsGraph with their details.

    Args:
        graph_path: Path to the StepsGraph asset.

    Returns:
        Dictionary with list of nodes and their information.
    """
    try:
        connection = get_unity_connection()
        params = {
            "graphPath": graph_path
        }
        result = connection.send_command("list_graph_nodes", params)
        return result
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to list graph nodes: {str(e)}"
        }
```
<br>

```python
@mcp.tool()
def set_node_position(
    ctx: Context,
    graph_path: str,
    node_name: str,
    position_x: float,
    position_y: float
) -> Dict[str, Any]:
    """
    Sets the position of a specific node in an xNode graph.

    Args:
        graph_path: Path to the NodeGraph asset (e.g., "Assets/Testing/Graphs/NewStepsGraph.asset").
        node_name: Name of the node to modify (e.g., "ClickStep_-64856").
        position_x: X position of the node in the graph editor.
        position_y: Y position of the node in the graph editor.

    Returns:
        Dictionary with results ('success', 'message', 'data', etc.).
    """
    try:
        connection = get_unity_connection()
        
        # First, get the node's instance ID from the node name
        list_params = {
            "graphPath": graph_path
        }
        list_result = connection.send_command("list_graph_nodes", list_params)
        
        if not list_result.get("success", False):
            return {
                "success": False,
                "error": f"Failed to list graph nodes: {list_result.get('error', 'Unknown error')}"
            }
        
        # Find the node by name and get its instance ID
        node_id = None
        nodes = list_result.get("data", {}).get("nodes", [])
        for node in nodes:
            if node.get("name") == node_name:
                node_id = str(node.get("instanceId"))
                break
        
        if node_id is None:
            return {
                "success": False,
                "error": f"Node '{node_name}' not found in graph"
            }
        
        # Now set the position using the node ID
        params = {
            "graphPath": graph_path,
            "nodeId": node_id,
            "positionX": position_x,
            "positionY": position_y
        }
        result = connection.send_command("set_node_position", params)
        return result
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to set node position: {str(e)}"
        }
```
---

### System Integration
```csharp
// CommandRegistry.cs - All tools registered with consistent naming

{ "SetNodeAsFirstStep", ManageXNodeNode.SetNodeAsFirstStep },
{ "ListGraphNodes", ManageXNodeNode.ListGraphNodes },
{ "HandleCreateXNodeNode", ManageXNodeNode.CreateNode },
{ "HandleDeleteXNodeNode", ManageXNodeNode.DeleteNode },
{ "HandleDeleteMultipleNodes", ManageXNodeNode.DeleteMultipleNodes },
{ "HandleSetXNodeNodePosition", ManageXNodeNode.SetNodePosition },
{ "HandleListAvailableNodeTypes", ManageXNodeNode.HandleListAvailableNodeTypes },

// UnityMcpBridge.cs - Command routing

"create_xnode_node" => ManageXNodeNode.CreateNode(paramsObject),
"delete_xnode_node" => ManageXNodeNode.DeleteNode(paramsObject),
"delete_multiple_nodes" => ManageXNodeNode.DeleteMultipleNodes(paramsObject),
"set_node_position" => ManageXNodeNode.SetNodePosition(paramsObject),
"list_available_node_types" => ManageXNodeNode.HandleListAvailableNodeTypes(paramsObject),
"set_node_as_first_step" => ManageXNodeNode.SetNodeAsFirstStep(paramsObject),
"list_graph_nodes" => ManageXNodeNode.ListGraphNodes(paramsObject),
```
