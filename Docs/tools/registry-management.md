
# This Page is Under Construction !!


<!--
# Registry Management Tools 📋

**Tool Group**: Registry Management  
**Purpose**: Manage and query experiment registry data and component information  
**File**: `ManageRegistryData.cs` (31KB, 865 lines)

Registry management tools for accessing experiment data, listing registry items, and retrieving component information for experiment configuration.

## 🎯 Use Cases

- **Registry Exploration**: List all registry items and parent-child relationships
- **Component Discovery**: Find available components and their methods
- **Experiment Configuration**: Access registry data for experiment setup
- **Data Validation**: Verify registry structure and component availability
- **Dynamic Configuration**: Use registry data for parameter configuration

## 🏗️ Implementation Details

### Registry Listing (`list_registry_all`)
```csharp
public static object HandleListRegistryAll(JObject @params)
{
    string graphPath = @params["graphPath"]?.ToString();
    
    // Load the StepsGraph and access its registry
    StepsGraph graph = AssetDatabase.LoadAssetAtPath<StepsGraph>(graphPath);
    if (graph?.registry == null)
    {
        return new { success = false, error = "Registry not found" };
    }

    // Flatten the registry into a list of all items
    var allItems = new List<object>();
    foreach (var parent in graph.registry.parents)
    {
        foreach (var child in parent.children)
        {
            allItems.Add(new
            {
                parentName = parent.prefabName,
                childName = child.childName,
                components = child.components?.Select(c => c.name).ToArray()
            });
        }
    }

    return new
    {
        success = true,
        items = allItems,
        count = allItems.Count
    };
}
```

### Parent Listing (`list_registry_parents`)
```csharp
// Lists all parent objects in the registry
var parents = graph.registry.parents.Select(p => new
{
    prefabName = p.prefabName,
    childCount = p.children?.Count ?? 0
}).ToList();
```

### Child Listing (`list_registry_children`)
```csharp
// Lists children of a specific parent
var parent = graph.registry.parents.FirstOrDefault(p => p.prefabName == parentName);
if (parent != null)
{
    var children = parent.children.Select(c => new
    {
        childName = c.childName,
        componentCount = c.components?.Count ?? 0
    }).ToList();
}
```

### Component Retrieval (`get_child_components`)
```csharp
// Gets components for a specific child
var child = parent.children.FirstOrDefault(c => c.childName == childName);
if (child != null)
{
    var components = child.components.Select(c => new
    {
        name = c.name,
        type = c.type,
        methods = c.methods?.Select(m => m.name).ToArray()
    }).ToList();
}
```

### Method Discovery (`get_component_methods`)
```csharp
public static object HandleGetComponentMethods(JObject @params)
{
    string componentTypeName = @params["componentTypeName"]?.ToString();
    
    // Use reflection to find the component type
    Type componentType = FindComponentType(componentTypeName);
    if (componentType == null)
    {
        return new { success = false, error = $"Component type '{componentTypeName}' not found" };
    }

    // Get all public methods
    var methods = componentType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(m => !m.IsSpecialName) // Exclude properties
        .Select(m => new
        {
            name = m.Name,
            returnType = m.ReturnType.Name,
            parameters = m.GetParameters().Select(p => new
            {
                name = p.Name,
                type = p.ParameterType.Name,
                isOptional = p.IsOptional
            }).ToArray()
        }).ToList();

    return new
    {
        success = true,
        componentType = componentTypeName,
        methods = methods,
        count = methods.Count
    };
}
```

### Type Discovery
```csharp
private static Type FindComponentType(string typeName)
{
    // Searches all loaded assemblies for the component type
    return AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .FirstOrDefault(type => 
            type.Name == typeName && 
            type.IsSubclassOf(typeof(MonoBehaviour)));
}
```

### Python Side Integration
```python
@mcp.tool()
def list_registry_all(ctx: Context, graph_path: str) -> Dict[str, Any]:
    try:
        params = {"graphPath": graph_path}
        connection = get_unity_connection()
        result = connection.send_command("list_registry_all", params)
        return {"success": True, "data": result}
    except Exception as e:
        return {"success": False, "error": str(e)}

@mcp.tool()
def get_component_methods(ctx: Context, component_type_name: str) -> Dict[str, Any]:
    try:
        params = {"componentTypeName": component_type_name}
        connection = get_unity_connection()
        result = connection.send_command("get_component_methods", params)
        return {"success": True, "data": result}
    except Exception as e:
        return {"success": False, "error": str(e)}
```

### System Integration
```csharp
// CommandRegistry.cs
{ "HandleManageRegistryData", ManageRegistryData.HandleCommand },

// UnityMcpBridge.cs
"list_registry_all" => ManageRegistryData.HandleCommand(paramsObject),
"list_registry_parents" => ManageRegistryData.HandleCommand(paramsObject),
"list_registry_children" => ManageRegistryData.HandleCommand(paramsObject),
"get_child_components" => ManageRegistryData.HandleCommand(paramsObject),
"get_component_methods" => ManageRegistryData.HandleCommand(paramsObject),
```

-->