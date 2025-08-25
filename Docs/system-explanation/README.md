# Unity MCP Bridge System Extension Guide 🛠️

Comprehensive guide for extending and modifying the Unity MCP Bridge system. Learn how to create custom tools, register them, and deploy your changes.

## 📋 Overview

This guide covers the complete process of extending Unity MCP Bridge with your own functionality:
1. **Creating Custom Tools** in Unity MCP Bridge
2. **Registering Tools** in the Command Registry
3. **Implementing Python Side** handlers
4. **Deploying Changes** to GitHub
5. **Updating Unity Package** in projects

## 🎯 System Extension Architecture

```
┌─────────────────┐    Custom Tool     ┌───────────────────┐    Unity API    ┌──────────────┐
│   AI Assistant  │ ◄────────────────► │  Unity MCP Server │ ◄─────────────► │ Unity Editor │
│ (Cursor/Claude) │                    │     (Python)      │                 │              │
└─────────────────┘                    └───────────────────┘                 └──────────────┘
                                                 │
                                                 │ Same Repository
                                                 ▼
                                        ┌──────────────────┐
                                        │ Unity MCP Bridge │
                                        │ + Your Tools     │
                                        └──────────────────┘
```

## 🔧 Extension Components

### 1. Unity MCP Bridge (Unity Package)
**Location**: `UnityMcpBridge/Editor/Tools/`

**What You'll Modify**:
- **Tool Scripts** - Add new tool implementations following existing patterns
- **Command Registry** - Register your tools in CommandRegistry
- **UnityMcpBridge.cs** - Add command routing for your tool
- **Parameter Handling** - Use JObject for parameter processing

### 2. Unity MCP Server (Python)
**Location**: `UnityMcpServer/src/tools/`

**What You'll Add**:
- **Tool Handlers** - Python implementations following existing patterns
- **Parameter Validation** - Input/output validation
- **Error Handling** - Robust error management
- **MCP Registration** - Register with FastMCP decorator

### 3. Tool Registration System
**Location**: `UnityMcpBridge/Editor/Tools/CommandRegistry.cs`

**What You'll Configure**:
- **Handler Registration** - Register new tool handlers
- **Command Routing** - Connect command types to handlers
- **Parameter Processing** - Handle JObject parameters

## 🛠️ Step-by-Step Extension Process

### Step 1: Create Your Custom Tool in Unity

#### 1.1 Create Tool Script (Following Real Pattern)
```csharp
// UnityMcpBridge/Editor/Tools/MyCustomTool.cs
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UnityMcpBridge.Editor.Tools
{
    /// <summary>
    /// Custom tool for specific functionality
    /// </summary>
    public static class MyCustomTool
    {
        public static object HandleCommand(JObject @params)
        {
            try
            {
                // Extract parameters using the same pattern as existing tools
                string action = @params["action"]?.ToString().ToLower();
                string target = @params["target"]?.ToString();
                float value = @params["value"]?.ToObject<float>() ?? 0f;
                
                if (string.IsNullOrEmpty(action))
                {
                    return Response.Error("Action parameter is required.");
                }
                
                // Your custom logic here
                object result = action switch
                {
                    "create" => CreateCustomObject(target, value),
                    "modify" => ModifyCustomObject(target, value),
                    _ => throw new System.ArgumentException($"Unknown action: {action}")
                };
                
                return Response.Success(result);
            }
            catch (System.Exception ex)
            {
                return Response.Error($"Custom tool error: {ex.Message}");
            }
        }
        
        private static object CreateCustomObject(string name, float value)
        {
            // Your Unity-specific implementation
            var go = new GameObject(name ?? "CustomObject");
            
            // Add custom component
            var customComponent = go.AddComponent<MyCustomComponent>();
            customComponent.SetValue(value);
            
            return new
            {
                success = true,
                objectName = go.name,
                objectId = go.GetInstanceID(),
                message = $"Created custom object '{go.name}' with value {value}"
            };
        }
        
        private static object ModifyCustomObject(string target, float value)
        {
            // Find the target object
            var go = GameObject.Find(target);
            if (go == null)
            {
                throw new System.ArgumentException($"Object not found: {target}");
            }
            
            // Modify the object
            var customComponent = go.GetComponent<MyCustomComponent>();
            if (customComponent == null)
            {
                customComponent = go.AddComponent<MyCustomComponent>();
            }
            
            customComponent.SetValue(value);
            
            return new
            {
                success = true,
                modified = target,
                newValue = value,
                message = $"Modified '{target}' with value {value}"
            };
        }
    }
    
    // Custom component for your tool
    public class MyCustomComponent : MonoBehaviour
    {
        [SerializeField] private float customValue;
        
        public void SetValue(float value)
        {
            customValue = value;
            Debug.Log($"MyCustomComponent value set to: {value}");
        }
        
        public float GetValue() => customValue;
    }
}
```

#### 1.2 Register Tool in CommandRegistry
```csharp
// UnityMcpBridge/Editor/Tools/CommandRegistry.cs
public static class CommandRegistry
{
    private static readonly Dictionary<string, Func<JObject, object>> _handlers = new()
    {
        // Existing handlers...
        { "HandleManageScript", ManageScript.HandleCommand },
        { "HandleManageScene", ManageScene.HandleCommand },
        { "HandleManageEditor", ManageEditor.HandleCommand },
        { "HandleManageGameObject", ManageGameObject.HandleCommand },
        { "HandleManageAsset", ManageAsset.HandleCommand },
        { "HandleReadConsole", ReadConsole.HandleCommand },
        { "HandleExecuteMenuItem", ExecuteMenuItem.HandleCommand },
        { "PrintHelloWorld", PrintHelloWorld.HandleCommand },
        { "CreateScriptableObject", CreateScriptableObject.HandleCommand },
        { "SetNodeAsFirstStep", SetNodeAsFirstStep.HandleCommand },
        { "HandleListRegistryParents", ManageRegistryData.HandleListParents },
        { "HandleListRegistryAll", ManageRegistryData.HandleListAll },
        { "HandleListRegistryChildren", ManageRegistryData.HandleListChildren },
        { "HandleGetChildComponents", ManageRegistryData.HandleGetChildComponents },
        { "HandleGetComponentMethods", ManageRegistryData.HandleGetComponentMethods },
        { "HandleCreateXNodeNode", ManageXNodeNode.HandleCommand },
        { "HandleDeleteXNodeNode", ManageXNodeNode.DeleteNode },
        { "HandleSetXNodeNodePosition", ManageXNodeNode.SetNodePosition },
        
        // Add your custom tool handler
        { "HandleMyCustomTool", MyCustomTool.HandleCommand },
    };

    public static Func<JObject, object> GetHandler(string commandName)
    {
        return _handlers.TryGetValue(commandName, out var handler) ? handler : null;
    }
}
```

#### 1.3 Add Command Routing in UnityMcpBridge.cs
```csharp
// UnityMcpBridge/Editor/UnityMcpBridge.cs
private static string ExecuteCommand(Command command)
{
    try
    {
        // ... existing code ...
        
        // Route command based on the tool structure
        object result = command.type switch
        {
            // Existing commands...
            "manage_script" => ManageScript.HandleCommand(paramsObject),
            "manage_scene" => ManageScene.HandleCommand(paramsObject),
            "manage_editor" => ManageEditor.HandleCommand(paramsObject),
            "manage_gameobject" => ManageGameObject.HandleCommand(paramsObject),
            "manage_asset" => ManageAsset.HandleCommand(paramsObject),
            "read_console" => ReadConsole.HandleCommand(paramsObject),
            "execute_menu_item" => ExecuteMenuItem.HandleCommand(paramsObject),
            "print_hello_world" => PrintHelloWorld.HandleCommand(paramsObject),
            "create_scriptable_object" => CreateScriptableObject.HandleCommand(paramsObject),
            "create_xnode_node" => ManageXNodeNode.HandleCommand(paramsObject),
            "delete_xnode_node" => ManageXNodeNode.DeleteNode(paramsObject),
            "delete_multiple_nodes" => ManageXNodeNode.DeleteMultipleNodes(paramsObject),
            "set_node_position" => ManageXNodeNode.SetNodePosition(paramsObject),
            "list_available_node_types" => ManageXNodeNode.HandleListAvailableNodeTypes(paramsObject),
            "make_connection_between_nodes" => MakeAConnectionBetweenNodes.HandleCommand(paramsObject),
            "set_node_as_first_step" => SetNodeAsFirstStep.HandleCommand(paramsObject),
            "list_graph_nodes" => SetNodeAsFirstStep.HandleListGraphNodesCommand(paramsObject),
            "manage_node_parameters" => ManageNodeParameters.HandleCommand(paramsObject),
            "manage_scriptable_object_parameters" => ManageScriptableObjectParameters.HandleCommand(paramsObject),
            "list_registry_parents" => ManageRegistryData.HandleListParents(paramsObject),
            "list_registry_all" => ManageRegistryData.HandleListAll(paramsObject),
            "list_registry_children" => ManageRegistryData.HandleListChildren(paramsObject),
            "get_child_components" => ManageRegistryData.HandleGetChildComponents(paramsObject),
            "get_component_methods" => ManageRegistryData.HandleGetComponentMethods(paramsObject),
            "register_scene_objects" => ManageRegistryData.HandleRegisterSceneObjects(paramsObject),
            
            // Add your custom tool command
            "my_custom_tool" => MyCustomTool.HandleCommand(paramsObject),
            
            _ => throw new ArgumentException($"Unknown or unsupported command type: {command.type}")
        };

        // Standard success response format
        var response = new { status = "success", result };
        return JsonConvert.SerializeObject(response);
    }
    catch (Exception ex)
    {
        // ... existing error handling ...
    }
}
```

### Step 2: Implement Python Side Handler

#### 2.1 Create Python Tool Handler (Following Real Pattern)
```python
# UnityMcpServer/src/tools/my_custom_tool.py
from mcp.server.fastmcp import FastMCP, Context
from typing import Dict, Any
from unity_connection import get_unity_connection

def register_my_custom_tool_tools(mcp: FastMCP):
    """Register MyCustomTool with the MCP server."""

    @mcp.tool()
    def my_custom_tool(
        ctx: Context,
        action: str,
        target: str = None,
        value: float = 0.0
    ) -> Dict[str, Any]:
        """Custom tool for specific functionality.

        Args:
            action: Action to perform ('create' or 'modify')
            target: Target object name (required for 'modify')
            value: Numeric value to set

        Returns:
            Dictionary with results ('success', 'message', 'data').
        """
        try:
            # Validate parameters
            if action not in ["create", "modify"]:
                return {
                    "success": False,
                    "error": f"Invalid action: {action}. Must be 'create' or 'modify'"
                }
            
            if action == "modify" and not target:
                return {
                    "success": False,
                    "error": "Target is required for 'modify' action"
                }
            
            # Prepare parameters for Unity
            params = {
                "action": action,
                "value": value
            }
            
            if target:
                params["target"] = target
            
            # Send command to Unity
            connection = get_unity_connection()
            result = connection.send_command("my_custom_tool", params)
            
            return {
                "success": True,
                "message": "Custom tool executed successfully",
                "data": result
            }
            
        except Exception as e:
            return {
                "success": False,
                "error": f"Failed to execute custom tool: {str(e)}"
            }
```

#### 2.2 Register Tool in Server
```python
# UnityMcpServer/src/server.py
from .tools.my_custom_tool import register_my_custom_tool_tools

class UnityMcpServer:
    def __init__(self):
        # ... existing initialization ...
    
    def register_tools(self):
        """Register all available tools"""
        # Register existing tools
        register_manage_script_tools(self.mcp)
        register_manage_scene_tools(self.mcp)
        register_manage_editor_tools(self.mcp)
        register_manage_gameobject_tools(self.mcp)
        register_manage_asset_tools(self.mcp)
        register_read_console_tools(self.mcp)
        register_execute_menu_item_tools(self.mcp)
        register_print_hello_world_tools(self.mcp)
        register_create_scriptable_object_tools(self.mcp)
        register_manage_xnode_node_tools(self.mcp)
        register_delete_xnode_node_tools(self.mcp)
        register_set_node_first_step_tools(self.mcp)
        register_manage_node_parameters_tools(self.mcp)
        register_manage_scriptable_object_parameters_tools(self.mcp)
        register_manage_registry_data_tools(self.mcp)
        register_create_connection_between_nodes_tools(self.mcp)
        register_manage_shader_tools(self.mcp)
        
        # Register your custom tool
        register_my_custom_tool_tools(self.mcp)
```

### Step 3: Test Your Tool

#### 3.1 Test in Unity Editor
```csharp
// Test your tool directly in Unity
var testParams = new JObject
{
    ["action"] = "create",
    ["value"] = 42.0f
};

var result = MyCustomTool.HandleCommand(testParams);
Debug.Log($"Tool result: {JsonConvert.SerializeObject(result, Formatting.Indented)}");
```

#### 3.2 Test via MCP
```python
# Test via MCP client
result = await mcp_client.call_tool("my_custom_tool", {
    "action": "create",
    "value": 42.0
})
print(f"Tool result: {result}")

# Test modify action
result = await mcp_client.call_tool("my_custom_tool", {
    "action": "modify",
    "target": "CustomObject",
    "value": 100.0
})
print(f"Tool result: {result}")
```

### Step 4: Deploy Your Changes

#### 4.1 Commit All Changes to Repository
```bash
# Since everything is in the same repository, commit all changes together
git add UnityMcpBridge/Editor/Tools/MyCustomTool.cs
git add UnityMcpBridge/Editor/Tools/CommandRegistry.cs
git add UnityMcpBridge/Editor/UnityMcpBridge.cs
git add UnityMcpServer/src/tools/my_custom_tool.py
git add UnityMcpServer/src/server.py

git commit -m "Add custom tool: my_custom_tool"
git push origin main
```

### Step 5: Update Projects Using Your Package

#### 5.1 Update Package Reference
```json
// In Unity project's manifest.json
{
  "dependencies": {
    "com.praxilabs.unity-mcp": "https://github.com/praxilabs/unity-mcp.git?path=/UnityMcpBridge#main"
  }
}
```

#### 5.2 Update Python Server
```bash
# In your Unity project, pull the updated server
cd UnityMcpServer
git pull origin main
```

## 🔍 Advanced Extension Patterns

### Custom Data Types with Serialization
```csharp
// UnityMcpBridge/Runtime/Serialization/MyCustomTypeConverter.cs
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnityMcpBridge.Runtime.Serialization
{
    public class MyCustomTypeConverter : JsonConverter<MyCustomType>
    {
        public override MyCustomType ReadJson(JsonReader reader, Type objectType, MyCustomType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            return new MyCustomType
            {
                Property1 = jo["property1"]?.ToObject<string>(),
                Property2 = jo["property2"]?.ToObject<float>()
            };
        }

        public override void WriteJson(JsonWriter writer, MyCustomType value, JsonSerializer serializer)
        {
            JObject jo = new JObject
            {
                ["property1"] = value.Property1,
                ["property2"] = value.Property2
            };
            jo.WriteTo(writer);
        }
    }
}
```

### Tool Dependencies (Using Other Tools)
```csharp
// Tools that depend on other tools
public static class DependentTool
{
    public static object HandleCommand(JObject @params)
    {
        try
        {
            // Use other tools by calling their handlers directly
            var gameObjectParams = new JObject
            {
                ["action"] = "create",
                ["name"] = "DependentObject",
                ["primitiveType"] = "Cube"
            };
            
            var gameObjectResult = ManageGameObject.HandleCommand(gameObjectParams);
            
            // Process the result
            var result = new
            {
                success = true,
                dependentObject = gameObjectResult,
                message = "Created dependent object using ManageGameObject"
            };
            
            return Response.Success(result);
        }
        catch (Exception ex)
        {
            return Response.Error($"Dependent tool error: {ex.Message}");
        }
    }
}
```


## 📁 File Structure for Extensions

```
unity-mcp/                           # Single Repository
├── UnityMcpBridge/                  # Unity Package
│   ├── Editor/
│   │   ├── Tools/
│   │   │   ├── MyCustomTool.cs              # Your custom tool
│   │   │   ├── CommandRegistry.cs           # Updated registry
│   │   │   └── ComplexTool.cs               # Advanced tool example
│   │   ├── UnityMcpBridge.cs                # Updated command routing
│   │   └── Runtime/
│   │       └── Serialization/
│   │           └── MyCustomTypeConverter.cs  # Custom data types
├── UnityMcpServer/                  # Python Server
│   └── src/
│       ├── tools/
│       │   ├── my_custom_tool.py            # Python handler
│       │   └── complex_tool.py              # Advanced Python handler
│       └── server.py                        # Updated tool registration
└── docs/
    └── system-explanation/
        └── README.md                         # This guide
```

## 🎯 Best Practices

### Tool Design
- **Follow Existing Patterns**: Use the same structure as `PrintHelloWorld` and `ManageGameObject`
- **Parameter Validation**: Always validate inputs using the same pattern as existing tools
- **Error Handling**: Use `Response.Error()` and `Response.Success()` consistently
- **Documentation**: Add XML documentation comments to your tools

### Code Organization
- **Namespace Consistency**: Use `UnityMcpBridge.Editor.Tools`
- **File Naming**: Follow existing conventions (PascalCase for C#, snake_case for Python)
- **Handler Pattern**: Use static `HandleCommand(JObject @params)` method
- **Testing**: Test tools in isolation before integration

### Deployment
- **Single Repository**: All changes go to the same repository
- **Version Control**: Use meaningful commit messages
- **Incremental Updates**: Deploy changes incrementally
- **Backward Compatibility**: Maintain compatibility when possible
- **Documentation**: Update documentation with your changes

## 🆘 Troubleshooting Extensions

### Common Issues
- **Tool Not Found**: Check registration in CommandRegistry and UnityMcpBridge.cs
- **Parameter Errors**: Verify parameter extraction matches existing patterns
- **Serialization Issues**: Use JObject for parameter handling
- **Python Connection**: Verify Unity-Python communication

### Debugging Tips
- **Unity Console**: Check for Unity-side errors in Console window
- **Python Logs**: Monitor Python server output for errors
- **MCP Protocol**: Verify MCP message format matches existing tools
- **Tool Isolation**: Test tools independently before integration

---

**Next Steps**: 
1. **Start Simple**: Create a basic tool following the `PrintHelloWorld` pattern
2. **Test Thoroughly**: Test in isolation before integration
3. **Document**: Update documentation with your changes
4. **Share**: Contribute your tools to the community

**Examples**: See existing tools like `PrintHelloWorld.cs` and `ManageGameObject.cs` for complete working examples.
