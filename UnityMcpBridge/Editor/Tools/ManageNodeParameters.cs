using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;
using XNode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public static class ManageNodeParameters {

    public static object HandleCommand(JObject paramsObject)
    {
        try
        {
            var action = paramsObject["action"]?.ToString();
            if (string.IsNullOrEmpty(action))
            {
                return new { success = false, message = "Action parameter is required" };
            }

            switch (action.ToLower())
            {
                case "set":
                    return SetNodeParameter(paramsObject);
                case "get":
                    return GetNodeParameter(paramsObject);
                case "list":
                    return ListNodeParameters(paramsObject);
                default:
                    return new { success = false, message = $"Unknown action: {action}. Supported actions: set, get, list" };
            }
        }
        catch (Exception ex)
        {
            return new { success = false, message = $"Error in ManageNodeParameters: {ex.Message}" };
        }
    }

    private static object SetNodeParameter(JObject paramsObject)
    {
        var graphPath = paramsObject["graph_path"]?.ToString();
        var nodeName = paramsObject["node_name"]?.ToString();
        var parameterName = paramsObject["parameter_name"]?.ToString();
        var parameterValue = paramsObject["parameter_value"];

        if (string.IsNullOrEmpty(graphPath))
        {
            return new { success = false, message = "Graph path is required" };
        }

        if (string.IsNullOrEmpty(nodeName))
        {
            return new { success = false, message = "Node name is required" };
        }

        if (string.IsNullOrEmpty(parameterName))
        {
            return new { success = false, message = "Parameter name is required" };
        }

        // Load the graph
        var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
        if (graph == null)
        {
            return new { success = false, message = $"Could not load graph at path: {graphPath}" };
        }

        // Find the node
        Node targetNode = null;
        foreach (var node in graph.nodes)
        {
            if (node.name == nodeName)
            {
                targetNode = node;
                break;
            }
        }

        if (targetNode == null)
        {
            return new { success = false, message = $"Node '{nodeName}' not found in graph" };
        }

        // Find the parameter field
        var field = FindSerializedField(targetNode, parameterName);
        if (field == null)
        {
            return new { success = false, message = $"Parameter '{parameterName}' not found on node type '{targetNode.GetType().Name}'" };
        }

        // Set the parameter value
        try
        {
            var convertedValue = ConvertValue(parameterValue, field.FieldType);
            field.SetValue(targetNode, convertedValue);
            
            // Generic handling for RegistryDropdown fields
            HandleRegistryDropdownFieldUpdate(targetNode, field, convertedValue);
            
            // Mark the asset as dirty so Unity saves the changes
            EditorUtility.SetDirty(graph);
            
            return new
            {
                success = true,
                message = $"Successfully set {parameterName} to {parameterValue} on node {nodeName}",
                data = new { nodeName = nodeName, parameterName = parameterName, value = parameterValue }
            };
        }
        catch (Exception ex)
        {
            return new { success = false, message = $"Failed to set parameter: {ex.Message}" };
        }
    }

    private static object GetNodeParameter(JObject paramsObject)
    {
        var graphPath = paramsObject["graph_path"]?.ToString();
        var nodeName = paramsObject["node_name"]?.ToString();
        var parameterName = paramsObject["parameter_name"]?.ToString();

        if (string.IsNullOrEmpty(graphPath))
        {
            return new { success = false, message = "Graph path is required" };
        }

        if (string.IsNullOrEmpty(nodeName))
        {
            return new { success = false, message = "Node name is required" };
        }

        if (string.IsNullOrEmpty(parameterName))
        {
            return new { success = false, message = "Parameter name is required" };
        }

        // Load the graph
        var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
        if (graph == null)
        {
            return new { success = false, message = $"Could not load graph at path: {graphPath}" };
        }

        // Find the node
        Node targetNode = null;
        foreach (var node in graph.nodes)
        {
            if (node.name == nodeName)
            {
                targetNode = node;
                break;
            }
        }

        if (targetNode == null)
        {
            return new { success = false, message = $"Node '{nodeName}' not found in graph" };
        }

        // Find the parameter field
        var field = FindSerializedField(targetNode, parameterName);
        if (field == null)
        {
            return new { success = false, message = $"Parameter '{parameterName}' not found on node type '{targetNode.GetType().Name}'" };
        }

        // Get the parameter value
        try
        {
            var value = field.GetValue(targetNode);
            return new
            {
                success = true,
                message = $"Successfully retrieved {parameterName} from node {nodeName}",
                data = new { nodeName = nodeName, parameterName = parameterName, value = value }
            };
        }
        catch (Exception ex)
        {
            return new { success = false, message = $"Failed to get parameter: {ex.Message}" };
        }
    }

    private static object ListNodeParameters(JObject paramsObject)
    {
        var graphPath = paramsObject["graph_path"]?.ToString();
        var nodeName = paramsObject["node_name"]?.ToString();

        if (string.IsNullOrEmpty(graphPath))
        {
            return new { success = false, message = "Graph path is required" };
        }

        if (string.IsNullOrEmpty(nodeName))
        {
            return new { success = false, message = "Node name is required" };
        }

        // Load the graph
        var graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
        if (graph == null)
        {
            return new { success = false, message = $"Could not load graph at path: {graphPath}" };
        }

        // Find the node
        Node targetNode = null;
        foreach (var node in graph.nodes)
        {
            if (node.name == nodeName)
            {
                targetNode = node;
                break;
            }
        }

        if (targetNode == null)
        {
            return new { success = false, message = $"Node '{nodeName}' not found in graph" };
        }

        // Get all serialized fields
        var fields = GetSerializedFields(targetNode);
        var parameters = new List<object>();

        foreach (var field in fields)
        {
            try
            {
                var value = field.GetValue(targetNode);
                var serializedValue = SerializeValueSafely(value);
                
                parameters.Add(new
                {
                    name = field.Name,
                    type = field.FieldType.Name,
                    value = serializedValue,
                    isComplex = IsComplexType(field.FieldType)
                });
            }
            catch (Exception ex)
            {
                // Add parameter info even if we can't read the value
                parameters.Add(new
                {
                    name = field.Name,
                    type = field.FieldType.Name,
                    value = $"<Error reading value: {ex.Message}>",
                    isComplex = false
                });
            }
        }

        // Use a custom serializer to avoid circular references
        try
        {
            var result = new
        {
            success = true,
            message = $"Found {parameters.Count} parameters on node {nodeName}",
            data = new { nodeName = nodeName, parameters = parameters }
        };
            
            // Test serialization to catch circular references early
            var testJson = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
            
            return result;
        }
        catch (Exception ex)
        {
            // If serialization fails, return a simplified result
            return new
            {
                success = true,
                message = $"Found {parameters.Count} parameters on node {nodeName} (serialization simplified due to circular references)",
                data = new { nodeName = nodeName, parametersCount = parameters.Count }
            };
        }
    }

    private static object SerializeValueSafely(object value)
    {
        try
    {
        if (value == null) return null;
        
        var type = value.GetType();
        
        // Handle primitive types
        if (type.IsPrimitive || type == typeof(string))
        {
            return value;
        }
        
        // Handle Unity basic types
        if (type == typeof(Vector3))
        {
            var v = (Vector3)value;
            return new { x = v.x, y = v.y, z = v.z };
        }
        
        if (type == typeof(Color))
        {
            var c = (Color)value;
            return new float[] { c.r, c.g, c.b, c.a };
        }
        
        if (type == typeof(Color32))
        {
            var c = (Color32)value;
            return new byte[] { c.r, c.g, c.b, c.a };
        }
        
            // Handle Unity Object references - be very careful to avoid circular references
        if (value is UnityEngine.Object unityObj)
        {
                // Skip certain Unity types that are known to cause circular references
                if (unityObj is XNode.Node || unityObj is NodePort || unityObj is XNode.NodeGraph)
                {
                    return $"<UnityObject: {unityObj.GetType().Name}>";
                }
                
                // Only return basic info, avoid any properties that might cause circular references
            return new { name = unityObj.name, type = unityObj.GetType().Name };
        }
        
            // Handle RegistryItem specifically
                if (type.Name == "RegistryItem")
                {
                    var prefabNameField = type.GetField("prefabName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var childNameField = type.GetField("childName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    
                if (prefabNameField != null && childNameField != null)
                {
                    var prefabName = prefabNameField.GetValue(value)?.ToString() ?? "";
                    var childName = childNameField.GetValue(value)?.ToString() ?? "";
                    return new { prefabName = prefabName, childName = childName };
                }
            }
            
            // Handle simple serializable types
            if (type.GetCustomAttribute<System.SerializableAttribute>() != null && !IsComplexType(type))
            {
                return SerializeSerializableType(value, type);
            }
            
            // For safety, just return the type name for any other types to prevent crashes
            return $"<Type: {type.Name}>";
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error serializing value: {ex.Message}");
            return "<Serialization Error>";
        }
    }

    private static object SerializeComplexObject(object value, Type type)
    {
                var properties = new Dictionary<string, object>();
        
        // Get all public fields
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                    {
                        try
                        {
                var fieldValue = field.GetValue(value);
                if (fieldValue != null)
                {
                    properties[field.Name] = SerializeValueSafely(fieldValue);
                }
                        }
                        catch
                        {
                            // Skip problematic fields
                        }
        }
        
        // Get all public properties
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            try
            {
                if (prop.CanRead && prop.GetIndexParameters().Length == 0)
                {
                    var propValue = prop.GetValue(value);
                    if (propValue != null)
                    {
                        properties[prop.Name] = SerializeValueSafely(propValue);
                    }
                }
            }
            catch
            {
                // Skip problematic properties
                    }
                }
                
                return properties.Count > 0 ? properties : $"<Complex type: {type.Name}>";
    }

    private static object SerializeSerializableType(object value, Type type)
    {
        var properties = new Dictionary<string, object>();
        
        // Get all fields (public and private with SerializeField)
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            try
            {
                var fieldValue = field.GetValue(value);
                if (fieldValue != null)
                {
                    // Use SerializeValueSafely to handle Unity objects properly
                    properties[field.Name] = SerializeValueSafely(fieldValue);
                }
            }
            catch
            {
                // Skip problematic fields
            }
        }
        
        return properties.Count > 0 ? properties : $"<Serializable type: {type.Name}>";
    }

    private static bool IsComplexType(Type type)
    {
        // If it's a primitive or basic Unity type, it's not complex
        if (type.IsPrimitive || 
            type == typeof(string) || 
            type == typeof(Vector3) || 
            type == typeof(Color) ||
            type == typeof(Color32) ||
            typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            return false;
        }
        
        // Check if it's a simple serializable type
        if (type.GetCustomAttribute<System.SerializableAttribute>() != null)
        {
            // If it only has simple fields (primitives, strings, or other simple serializable types), 
            // treat it as simple, not complex
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            bool hasOnlySimpleFields = true;
            
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                if (!fieldType.IsPrimitive && 
                    fieldType != typeof(string) && 
                    fieldType != typeof(Vector3) && 
                    fieldType != typeof(Color) &&
                    fieldType != typeof(Color32) &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(fieldType) &&
                    fieldType.GetCustomAttribute<System.SerializableAttribute>() == null)
                {
                    hasOnlySimpleFields = false;
                    break;
                }
            }
            
            return !hasOnlySimpleFields;
        }
        
        return true;
    }

    private static FieldInfo FindSerializedField(Node node, string fieldName)
    {
        var currentType = node.GetType();
        
        // Look through inheritance hierarchy for exact match first
        while (currentType != null && currentType != typeof(object))
        {
            var field = currentType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        if (field != null && IsSerializedField(field))
        {
            return field;
            }
            currentType = currentType.BaseType;
        }

        // Look for case-insensitive match through inheritance hierarchy
        currentType = node.GetType();
        while (currentType != null && currentType != typeof(object))
        {
            var fields = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var f in fields)
        {
            if (IsSerializedField(f) && string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return f;
            }
            }
            currentType = currentType.BaseType;
        }

        return null;
    }

    /// <summary>
    /// Generic handling for RegistryDropdown field updates.
    /// This method handles any node type that uses RegistryDropdown attributes with RegistryItem fields.
    /// </summary>
    private static void HandleRegistryDropdownFieldUpdate(Node targetNode, FieldInfo field, object convertedValue)
    {
        // Check if this field has a RegistryDropdown attribute
        var hasRegistryDropdown = field.GetCustomAttributes()
            .Any(attr => attr.GetType().Name.Contains("RegistryDropdown"));
        
        if (!hasRegistryDropdown)
            return;

        // Check if the field type is RegistryItem
        if (field.FieldType.Name != "RegistryItem")
            return;

        // Handle different node types that may need special validation updates
        var nodeType = targetNode.GetType();
        
        // FunctionCallBase and its derived classes (FunctionCallStep, etc.)
        if (IsNodeTypeOrDerived(nodeType, "FunctionCallBase"))
        {
            HandleFunctionCallBaseUpdate(targetNode, field, convertedValue);
        }
        
        // Add more specific node type handlers here as needed
        // Example: UIClickStep, AttachStep, etc. might need their own validation logic
        // You can add handlers for other node types like:
        // if (IsNodeTypeOrDerived(nodeType, "UIClickStep")) { HandleUIClickStepUpdate(targetNode, field, convertedValue); }
        // if (IsNodeTypeOrDerived(nodeType, "AttachStep")) { HandleAttachStepUpdate(targetNode, field, convertedValue); }
    }

    /// <summary>
    /// Handles updates for FunctionCallBase and derived classes
    /// </summary>
    private static void HandleFunctionCallBaseUpdate(Node targetNode, FieldInfo field, object convertedValue)
    {
        // Update the previousCalledObjectName to trigger validation in the editor
        var previousField = targetNode.GetType().GetField("previousCalledObjectName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (previousField != null)
        {
            // Get the prefabName property value from the updated RegistryItem
            var prefabNameProperty = targetNode.GetType().GetProperty("prefabName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prefabNameProperty != null)
            {
                var prefabName = prefabNameProperty.GetValue(targetNode);
                previousField.SetValue(targetNode, prefabName);
            }
        }
    }

    /// <summary>
    /// Checks if a type is of a specific name or derives from a type with that name
    /// </summary>
    private static bool IsNodeTypeOrDerived(Type type, string baseTypeName)
    {
        var currentType = type;
        while (currentType != null && currentType != typeof(object))
        {
            if (currentType.Name == baseTypeName)
                return true;
            currentType = currentType.BaseType;
        }
        return false;
    }

    private static List<FieldInfo> GetSerializedFields(Node node)
    {
        var type = node.GetType();
        var serializedFields = new List<FieldInfo>();

        // Get fields from the current type and all base types
        var currentType = type;
        while (currentType != null && currentType != typeof(object))
        {
            var fields = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var field in fields)
        {
            if (IsSerializedField(field))
            {
                serializedFields.Add(field);
            }
        }
            
            currentType = currentType.BaseType;
        }



        return serializedFields;
    }

    private static bool IsSerializedField(FieldInfo field)
    {


        // Check for [SerializeField] attribute (this includes private fields with SerializeField)
        if (field.GetCustomAttribute<SerializeField>() != null)
        {
            return true;
        }

        // Check for public fields (Unity serializes these automatically, including HideInInspector fields)
        if (field.IsPublic)
        {
            return true;
        }

        // Check for fields marked with [System.Serializable]
        if (field.GetCustomAttribute<System.SerializableAttribute>() != null)
        {
            return true;
        }

        // Include fields with any custom attributes that might indicate serialization
        var customAttributes = field.GetCustomAttributes();
        foreach (var attr in customAttributes)
        {
            var attrTypeName = attr.GetType().Name;
            // Generic check for attributes that might indicate serialization
            if (attrTypeName.Contains("Attribute") && !attrTypeName.Contains("Hide"))
            {
                return true;
            }
        }



        return false;
    }

    private static object ConvertValue(object value, Type targetType)
    {
        if (value == null)
        {
            return null;
        }

        // If types match, return as is
        if (targetType.IsAssignableFrom(value.GetType()))
        {
            return value;
        }

        // Handle common conversions
        if (targetType == typeof(float))
        {
            return Convert.ToSingle(value);
        }
        else if (targetType == typeof(int))
        {
            return Convert.ToInt32(value);
        }
        else if (targetType == typeof(bool))
        {
            return Convert.ToBoolean(value);
        }
        else if (targetType == typeof(string))
        {
            return value.ToString();
        }
        else if (targetType == typeof(Vector3))
        {
            if (value is JArray array && array.Count >= 3)
            {
                return new Vector3(
                    Convert.ToSingle(array[0]),
                    Convert.ToSingle(array[1]),
                    Convert.ToSingle(array[2])
                );
            }
        }
        else if (targetType == typeof(Color))
        {
            if (value is JArray array && array.Count >= 4)
            {
                return new Color(
                    Convert.ToSingle(array[0]),
                    Convert.ToSingle(array[1]),
                    Convert.ToSingle(array[2]),
                    Convert.ToSingle(array[3])
                );
            }
        }

        // For Unity Object references, try to find by name
        if (typeof(UnityEngine.Object).IsAssignableFrom(targetType))
        {
            return HandleUnityObjectConversion(value, targetType);
        }

        // Handle complex objects by trying JSON deserialization
        if (value is JObject || value is JArray)
        {
            try
            {
                return JsonConvert.DeserializeObject(value.ToString(), targetType);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to deserialize JSON to {targetType.Name}: {ex.Message}");
            }
        }

        // Handle simple serializable types
        if (targetType.GetCustomAttribute<System.SerializableAttribute>() != null && !IsComplexType(targetType))
        {
            return HandleSerializableTypeConversion(value, targetType);
        }
        
        // Try to create instance and populate fields for complex types
        if (IsComplexType(targetType))
        {
            return HandleComplexObjectConversion(value, targetType);
        }

        throw new InvalidCastException($"Cannot convert {value} to type {targetType.Name}");
    }

    private static object HandleComplexObjectConversion(object value, Type targetType)
    {
        try
        {
            // Create a new instance of the complex type
            var instance = Activator.CreateInstance(targetType);
            
            if (value is JObject jsonObj)
            {
                // Populate fields from JSON object
                var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                foreach (var field in fields)
                {
                    if (jsonObj[field.Name] != null)
                    {
                        try
                        {
                            var fieldValue = ConvertValue(jsonObj[field.Name], field.FieldType);
                            field.SetValue(instance, fieldValue);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Failed to set field {field.Name} on {targetType.Name}: {ex.Message}");
                        }
                    }
                }
            }
            else if (value is string stringValue)
            {
                // Try to parse string as JSON
                try
                {
                    var parsedJsonObj = JObject.Parse(stringValue);
                    return HandleComplexObjectConversion(parsedJsonObj, targetType);
                }
                catch
                {
                    // If not JSON, try to set it as a simple field if the type has only one field
                    var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fields.Length == 1 && fields[0].FieldType == typeof(string))
                    {
                        fields[0].SetValue(instance, stringValue);
                    }
                }
            }
            
            return instance;
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert {value} to {targetType.Name}: {ex.Message}");
        }
    }

    private static object HandleSerializableTypeConversion(object value, Type targetType)
    {
        try
        {
            // Create a new instance of the simple serializable type
            var instance = Activator.CreateInstance(targetType);
            
            if (value is JObject jsonObj)
            {
                // Populate fields from JSON object
                var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                foreach (var field in fields)
                {
                    if (jsonObj[field.Name] != null)
                    {
                        try
                        {
                            var fieldValue = ConvertValue(jsonObj[field.Name], field.FieldType);
                            field.SetValue(instance, fieldValue);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Failed to set field {field.Name} on {targetType.Name}: {ex.Message}");
                        }
                    }
                }
            }
            else if (value is string stringValue)
            {
                // Try to parse string as JSON
                try
                {
                    var parsedJsonObj = JObject.Parse(stringValue);
                    return HandleSerializableTypeConversion(parsedJsonObj, targetType);
                }
                catch
                {
                    // If not JSON, try to set it as a simple field if the type has only one field
                    var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fields.Length == 1 && fields[0].FieldType == typeof(string))
                    {
                        fields[0].SetValue(instance, stringValue);
                    }
                }
            }
            
            return instance;
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert {value} to {targetType.Name}: {ex.Message}");
        }
    }

    private static object HandleUnityObjectConversion(object value, Type targetType)
    {
        if (value is string objectName)
        {
            // Try to find GameObject first
            if (targetType == typeof(GameObject) || targetType.IsSubclassOf(typeof(GameObject)))
            {
                var foundObject = GameObject.Find(objectName);
                if (foundObject != null)
                {
                    return foundObject;
                }
            }
            
            // Try to find Component
            if (targetType.IsSubclassOf(typeof(Component)))
            {
                var foundGameObject = GameObject.Find(objectName);
                if (foundGameObject != null)
                {
                    var component = foundGameObject.GetComponent(targetType);
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            
            // Try to find asset by name
            var assets = AssetDatabase.FindAssets($"t:{targetType.Name} {objectName}");
            if (assets.Length > 0)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, targetType);
                if (asset != null)
                {
                    return asset;
                }
            }
        }
        
        return null;
    }
}