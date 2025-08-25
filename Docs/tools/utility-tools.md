# Utility Tools 🛠️

**Tool Group**: Utility Tools  
**Purpose**: Basic utility functions for testing and asset creation  
**Files**: `PrintHelloWorld.cs`, `CreateScriptableObject.cs`

Simple utility tools for system verification and basic asset creation operations.

## 🎯 Use Cases

- **System Verification**: Test Unity MCP Bridge connectivity
- **Asset Creation**: Create ScriptableObject assets programmatically
- **Debugging**: Verify tool execution and response handling
- **Basic Operations**: Simple operations for testing and validation

## 🏗️ Implementation Details

### System Verification (`print_hello_world`)
```csharp
public static class PrintHelloWorld
{
    public static object HandleCommand(JObject @params)
    {
        // Simple test that prints to Unity console
        Debug.Log("Hello, World!");
        
        return new
        {
            success = true,
            message = "Hello, World! printed to Unity console",
            timestamp = System.DateTime.Now.ToString()
        };
    }
}
```

### ScriptableObject Creation (`create_scriptable_object`)
```csharp
public static class CreateScriptableObject
{
    public static object HandleCommand(JObject @params)
    {
        string scriptableObjectType = @params["scriptableObjectType"]?.ToString();
        string folder = @params["folder"]?.ToString() ?? "Assets/Testing/";

        try
        {
            // Find the ScriptableObject type via reflection
            Type type = FindScriptableObjectType(scriptableObjectType);
            if (type == null)
            {
                return new { success = false, error = $"Type '{scriptableObjectType}' not found" };
            }

            // Create the asset
            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            string assetPath = $"{folder}{scriptableObjectType}.asset";
            
            // Ensure folder exists and save asset
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            return new
            {
                success = true,
                message = $"Created {scriptableObjectType} at {assetPath}",
                assetPath = assetPath,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new { success = false, error = $"Failed to create asset: {ex.Message}" };
        }
    }
}
```

### Type Discovery
```csharp
private static Type FindScriptableObjectType(string typeName)
{
    // Searches all loaded assemblies for the ScriptableObject type
    return AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .FirstOrDefault(type => 
            type.Name == typeName && 
            type.IsSubclassOf(typeof(ScriptableObject)));
}
```

### Python Side Integration
```python
@mcp.tool()
def print_hello_world(ctx: Context, random_string: str) -> Dict[str, Any]:
    try:
        connection = get_unity_connection()
        result = connection.send_command("print_hello_world", {})
        return {"success": True, "data": result}
    except Exception as e:
        return {"success": False, "error": str(e)}

@mcp.tool()
def create_scriptable_object(
    ctx: Context,
    scriptable_object_type: str,
    folder: str = "Assets/Testing/"
) -> Dict[str, Any]:
    try:
        params = {
            "scriptableObjectType": scriptable_object_type,
            "folder": folder
        }
        connection = get_unity_connection()
        result = connection.send_command("create_scriptable_object", params)
        return {"success": True, "data": result}
    except Exception as e:
        return {"success": False, "error": str(e)}
```

### System Integration
```csharp
// CommandRegistry.cs
{ "HandlePrintHelloWorld", PrintHelloWorld.HandleCommand },
{ "HandleCreateScriptableObject", CreateScriptableObject.HandleCommand },

// UnityMcpBridge.cs
"print_hello_world" => PrintHelloWorld.HandleCommand(paramsObject),
"create_scriptable_object" => CreateScriptableObject.HandleCommand(paramsObject),
```
