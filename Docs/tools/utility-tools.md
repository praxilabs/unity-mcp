# Utility Tools 🛠️

**Tool Group**: Utility Tools  
**Purpose**: Basic utility functions for testing and asset creation  
**Files**: `Print.cs`, `CreateScriptableObject.cs`, `ToolUtils.cs`

Simple utility tools for system verification and basic asset creation operations.

## 🎯 Use Cases

- **System Verification**: Test Unity MCP Bridge connectivity
- **Asset Creation**: Create ScriptableObject assets programmatically
- **Debugging**: Verify tool execution and response handling
- **Basic Operations**: Simple operations for testing and validation
- **Shared Utilities**: Common functions used across all tools

## 🏗️ Implementation Details

### System Verification (`print`)
```csharp
public static class Print
{
    public static object HandleCommand(JObject args)
    {
        // Extract value from args, defaulting to "Hello, World!" if not provided
        string valueToPrint = args["value"]?.ToString() ?? "Hello, World!";
        
        Debug.Log($"Print: {valueToPrint}");
        
        return new
        {
            success = true,
            message = $"Value '{valueToPrint}' printed to console",
            timestamp = System.DateTime.Now.ToString()
        };
    }
}
```
---

### ScriptableObject Creation (`create_scriptable_object`)
```csharp

// Create the ScriptableObject
ScriptableObject asset = CreateScriptableObjectInstance(scriptableObjectType);

// Ensure unique filename
assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

// Create the asset
AssetDatabase.CreateAsset(asset, assetPath);
AssetDatabase.SaveAssets();
AssetDatabase.Refresh();
```
<br>

```csharp
private static ScriptableObject CreateScriptableObjectInstance(string typeName)
{
    // Get the type from the assembly
    Type type = Type.GetType(typeName);
    if (type == null)
    {
        // Try to find the type in all loaded assemblies
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName);
            if (type != null) break;
        }
    }
    // Create instance
    return ScriptableObject.CreateInstance(type);   
}
```
---

### Python Side Integration
```python

//Print function Python Side logic.

@mcp.tool()
def print(ctx: Context, value: Optional[str] = None) -> Dict[str, Any]:
    """Prints a custom message or 'Hello, World!' to Unity's console.

    Args:
        value: Optional custom message to print. If not provided, defaults to 'Hello, World!'.

    Returns:
        Dictionary with results ('success', 'message', 'data').
    """
    try:
        connection = get_unity_connection()
        
        # Prepare arguments for Unity
        args = {}
        if value is not None:
            args["value"] = value
        
        result = connection.send_command("print", args)
        
        return {
            "success": True,
            "message": f"Value '{value or 'Hello, World!'}' printed to Unity console",
            "data": result
        }
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to print message: {str(e)}"
        }


//Create Scriptable Object Python Side logic.

@mcp.tool()
def create_scriptable_object(
    ctx: Context,
    scriptable_object_type: str,
    folder: str = "Assets/Testing/"
) -> Dict[str, Any]:
    """
    Creates a new ScriptableObject of the specified type in the given folder.

    Args:
        scriptable_object_type: The type of ScriptableObject to create (e.g., "StepsGraph", "ExperimentData").
        folder: Folder path where the asset will be created.

    Returns:
        Dictionary with results ('success', 'message', 'assetPath', 'timestamp').
    """
    try:
        # Generate appropriate asset name based on the ScriptableObject type
        if scriptable_object_type.lower() in ["stepsgraph", "steps_graph"]:
            asset_name = "NewStepsGraph"
        elif scriptable_object_type.lower() in ["experimentdata", "experiment_data"]:
            asset_name = "NewExperimentData"
        elif scriptable_object_type.lower() in ["itemregistry", "item_registry"]:
            asset_name = "NewItemRegistry"
        else:
            # Default naming pattern: "New" + ScriptableObject type
            asset_name = f"New{scriptable_object_type}"
        
        # Ensure folder ends with /
        if not folder.endswith("/"):
            folder += "/"
        
        # Create full asset path
        asset_path = f"{folder}{asset_name}.asset"
        
        connection = get_unity_connection()
        params = {
            "scriptableObjectType": scriptable_object_type,
            "assetPath": asset_path
        }
        result = connection.send_command("create_scriptable_object", params)
        return result
    except Exception as e:
        return {
            "success": False,
            "error": f"Failed to create ScriptableObject: {str(e)}"
        }
```
---

### System Integration
```csharp
// CommandRegistry.cs
{ "Print", Print.HandleCommand },
{ "CreateScriptableObject", CreateScriptableObject.HandleCommand },

// UnityMcpBridge.cs
"print" => Print.HandleCommand(paramsObject),
"create_scriptable_object" => CreateScriptableObject.HandleCommand(paramsObject),
```
<br>

## 🛠️ ToolUtils - Shared Utility Functions

#### Graph Management
```csharp
// Load a NodeGraph from path
public static NodeGraph LoadNodeGraph(string graphPath)
{
    return AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
}

// Validate graph path and load graph
public static (bool IsValid, NodeGraph Graph, string ErrorMessage) ValidateGraphPath(string graphPath)
{
    var graph = LoadNodeGraph(graphPath);
    return (graph != null, graph, null);
}

// Save graph changes
public static void SaveGraphChanges(NodeGraph graph)
{
    EditorUtility.SetDirty(graph);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
}
```
---

#### Node Management
```csharp
// Find node by identifier (name or ID)
public static Node FindNodeByIdentifier(NodeGraph graph, string nodeIdentifier, string identifierType = "name")
{
    if (identifierType == "id")
    {
        int instanceId = int.Parse(nodeIdentifier);
        return graph.nodes.FirstOrDefault(n => n.GetInstanceID() == instanceId);
    }
    return graph.nodes.FirstOrDefault(n => n.name == nodeIdentifier);
}

// Get first step node from StepsGraph
public static Node GetFirstStepNode(NodeGraph graph)
{
    SerializedObject so = new SerializedObject(graph);
    SerializedProperty firstStepProp = so.FindProperty("firstStep");
    return firstStepProp.objectReferenceValue as Node;
}
```
---

#### Node Type Management
```csharp
// Find node type by name
public static Type FindNodeType(string nodeTypeName)
{
    Type[] nodeTypes = NodeEditorReflection.GetDerivedTypes(typeof(Node));
    return nodeTypes.FirstOrDefault(t => t.Name == nodeTypeName);
}

// Get all available node types
public static object[] GetAvailableNodeTypes()
{
    Type[] nodeTypes = NodeEditorReflection.GetDerivedTypes(typeof(Node));
    return nodeTypes
        .Where(type => !type.IsAbstract)
        .Select(type => new
        {
            typeName = type.Name,
            fullName = type.FullName,
            assembly = type.Assembly.GetName().Name
        })
        .ToArray();
}
```
---

#### Node Information
```csharp
// Create node information object
public static object CreateNodeInfo(Node node, Node firstStepNode = null)
{
    return new
    {
        name = node.name,
        type = node.GetType().Name,
        instanceId = node.GetInstanceID(),
        position = new { x = node.position.x, y = node.position.y },
        isFirstStep = (firstStepNode != null && node.GetInstanceID() == firstStepNode.GetInstanceID())
    };
}

// Get all nodes in graph as information objects
public static List<object> GetGraphNodesInfo(NodeGraph graph)
{
    var firstStepNode = GetFirstStepNode(graph);
    var nodesList = new List<object>();
    
    foreach (var node in graph.nodes)
    {
        if (node != null)
        {
            nodesList.Add(CreateNodeInfo(node, firstStepNode));
        }
    }
    
    return nodesList;
}
```
---

#### Response Creation
```csharp
// Create standard error response
public static object CreateErrorResponse(string errorMessage)
{
    return new
    {
        success = false,
        error = errorMessage
    };
}

// Create standard success response
public static object CreateSuccessResponse(string message, object data = null)
{
    var response = new
    {
        success = true,
        message = message,
        timestamp = System.DateTime.Now.ToString()
    };

    if (data != null)
    {
        // Add data property to response
        var dynamicResponse = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
        dynamicResponse["success"] = response.success;
        dynamicResponse["message"] = response.message;
        dynamicResponse["timestamp"] = response.timestamp;
        dynamicResponse["data"] = data;
        return dynamicResponse;
    }

    return response;
}
```
---

#### Argument Validation
```csharp
// Validate required string argument
public static (bool IsValid, string Value, string ErrorMessage) ValidateRequiredString(JObject args, string key, string displayName = null)
{
    string value = args[key]?.ToString();
    if (string.IsNullOrEmpty(value))
    {
        return (false, null, $"Missing required argument: {displayName ?? key}");
    }
    return (true, value, null);
}

// Validate required integer argument
public static (bool IsValid, int Value, string ErrorMessage) ValidateRequiredInt(JObject args, string key, string displayName = null)
{
    var token = args[key];
    if (int.TryParse(token.ToString(), out int value))
    {
        return (true, value, null);
    }
    return (false, 0, $"Invalid integer value for {displayName ?? key}");
}

// Get optional float value with default
public static float GetOptionalFloat(JObject args, string key, float defaultValue = 0f)
{
    return args[key]?.ToObject<float>() ?? defaultValue;
}

// Get optional string value
public static string GetOptionalString(JObject args, string key, string defaultValue = null)
{
    return args[key]?.ToString() ?? defaultValue;
}
```
---

