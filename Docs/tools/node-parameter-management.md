# Node Parameter Management Tools ⚙️

**Tool Group**: Node Parameter Management  
**Purpose**: Set, get, and list parameters for nodes in XNode graphs  
**File**: `ManageNodeParameters.cs` (31KB, 886 lines)

Advanced parameter management system for XNode graphs, supporting complex data types, registry references, and dynamic parameter configuration.

## 🎯 Use Cases

- **Parameter Configuration**: Set node parameters for experiment flows
- **Dynamic Configuration**: Update parameters at runtime
- **Registry Integration**: Configure parameters that reference registry objects
- **Parameter Discovery**: List available parameters for nodes
- **Complex Data Types**: Handle Vector3, Color, and custom data types
- **Validation**: Validate parameter values and types

## 🏗️ Implementation Details

### Parameter Setting (`set_node_parameter`)
```csharp
// Loads the graph and finds the target node
NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
Node node = graph.nodes.FirstOrDefault(n => n.name == nodeName);

// Uses reflection to find the parameter field/property
var fieldInfo = node.GetType().GetField(parameterName, 
    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

// Converts JSON value to correct type and sets it
object convertedValue = ConvertJTokenToType(parameterValue, fieldInfo.FieldType);
fieldInfo.SetValue(node, convertedValue);

// Marks assets as dirty and saves
EditorUtility.SetDirty(node);
EditorUtility.SetDirty(graph);
AssetDatabase.SaveAssets();
```

### Parameter Retrieval (`get_node_parameter`)
```csharp
// Similar to setting but retrieves the current value
var fieldInfo = node.GetType().GetField(parameterName, 
    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

object currentValue = fieldInfo.GetValue(node);
return new { 
    success = true, 
    parameterName = parameterName,
    parameterValue = currentValue,
    parameterType = fieldInfo.FieldType.Name 
};
```

### Parameter Listing (`list_node_parameters`)
```csharp
// Uses reflection to get all serializable fields
var fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
    .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
    .Select(f => new {
        name = f.Name,
        type = f.FieldType.Name,
        value = f.GetValue(node),
        isPublic = f.IsPublic
    });
```

### Complex Type Conversion
```csharp
private static object ConvertJTokenToType(JToken token, Type targetType)
{
    if (targetType == typeof(Vector3))
    {
        var array = token as JArray;
        return new Vector3(
            array[0].ToObject<float>(),
            array[1].ToObject<float>(),
            array[2].ToObject<float>()
        );
    }
    else if (targetType == typeof(RegistryItem))
    {
        var obj = token as JObject;
        return new RegistryItem
        {
            prefabName = obj["prefabName"]?.ToString(),
            childName = obj["childName"]?.ToString()
        };
    }
    
    return token.ToObject(targetType);
}
```

### Python Side Integration
```python
@mcp.tool()
def manage_node_parameters(
    ctx: Context,
    action: str,
    graph_path: str,
    node_name: str,
    parameter_name: str = None,
    parameter_value: Any = None,
) -> Dict[str, Any]:
    params = {
        "action": action,
        "graphPath": graph_path,
        "nodeName": node_name,
        "parameterName": parameter_name,
        "parameterValue": parameter_value
    }
    params = {k: v for k, v in params.items() if v is not None}
    
    connection = get_unity_connection()
    result = connection.send_command("manage_node_parameters", params)
    return {"success": True, "data": result}
```

### System Integration
```csharp
// CommandRegistry.cs
{ "HandleManageNodeParameters", ManageNodeParameters.HandleCommand },

// UnityMcpBridge.cs
"manage_node_parameters" => ManageNodeParameters.HandleCommand(paramsObject),
```
