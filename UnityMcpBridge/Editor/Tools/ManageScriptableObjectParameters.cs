using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityMcpBridge.Editor.Helpers;

namespace UnityMcpBridge.Editor.Tools
{
    /// <summary>
    /// Handles ScriptableObject parameter management for graphs and other ScriptableObjects.
    /// Provides functionality to list, get, and set parameters on any ScriptableObject asset.
    /// </summary>
    public static class ManageScriptableObjectParameters
    {
        /// <summary>
        /// Main entry point for handling ScriptableObject parameter commands
        /// </summary>
        public static object HandleCommand(JObject paramsObject)
        {
            try
            {
                var action = paramsObject["action"]?.ToString();
                if (string.IsNullOrEmpty(action))
                {
                    return Response.Error("Action parameter is required");
                }

                var scriptableObjectManager = new ScriptableObjectParameterManager();
                
                return action.ToLower() switch
                {
                    "get" => scriptableObjectManager.GetParameter(paramsObject),
                    "set" => scriptableObjectManager.SetParameter(paramsObject),
                    "list" => scriptableObjectManager.ListParameters(paramsObject),
                    _ => Response.Error($"Unknown action: {action}. Supported actions: get, set, list")
                };
            }
            catch (Exception ex)
            {
                return Response.Error($"Error in ManageScriptableObjectParameters: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Manages ScriptableObject parameter operations
    /// </summary>
    public class ScriptableObjectParameterManager
    {
        private readonly ScriptableObjectLoader _loader;
        private readonly ParameterReflectionHelper _reflectionHelper;
        private readonly ValueConverter _valueConverter;

        public ScriptableObjectParameterManager()
        {
            _loader = new ScriptableObjectLoader();
            _reflectionHelper = new ParameterReflectionHelper();
            _valueConverter = new ValueConverter();
        }

        /// <summary>
        /// Gets a specific parameter value from a ScriptableObject
        /// </summary>
        public object GetParameter(JObject paramsObject)
        {
            var (assetPath, parameterName) = ExtractRequiredParameters(paramsObject);
            if (assetPath == null || parameterName == null)
            {
                return Response.Error("Asset path and parameter name are required");
            }

            try
            {
                var scriptableObject = _loader.LoadScriptableObject(assetPath);
                if (scriptableObject == null)
                {
                    return Response.Error($"Could not load ScriptableObject at path: {assetPath}");
                }

                var parameterInfo = _reflectionHelper.GetParameterInfo(scriptableObject, parameterName);
                if (parameterInfo == null)
                {
                    return Response.Error($"Parameter '{parameterName}' not found on ScriptableObject");
                }

                var value = parameterInfo.GetValue(scriptableObject);
                var serializableValue = _valueConverter.GetSerializableValue(value);

                return Response.Success($"Successfully retrieved parameter '{parameterName}'", new
                {
                    parameterName = parameterName,
                    parameterValue = serializableValue,
                    parameterType = parameterInfo.Type.FullName,
                    isProperty = parameterInfo.IsProperty,
                    isField = parameterInfo.IsField
                });
            }
            catch (Exception ex)
            {
                return Response.Error($"Error getting parameter '{parameterName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Sets a parameter value on a ScriptableObject
        /// </summary>
        public object SetParameter(JObject paramsObject)
        {
            var (assetPath, parameterName) = ExtractRequiredParameters(paramsObject);
            var parameterValue = paramsObject["parameter_value"];

            if (assetPath == null || parameterName == null)
            {
                return Response.Error("Asset path and parameter name are required");
            }

            try
            {
                var scriptableObject = _loader.LoadScriptableObject(assetPath);
                if (scriptableObject == null)
                {
                    return Response.Error($"Could not load ScriptableObject at path: {assetPath}");
                }

                var parameterInfo = _reflectionHelper.GetParameterInfo(scriptableObject, parameterName);
                if (parameterInfo == null)
                {
                    return Response.Error($"Parameter '{parameterName}' not found on ScriptableObject");
                }

                if (!parameterInfo.CanWrite)
                {
                    return Response.Error($"Parameter '{parameterName}' is not writable");
                }

                var convertedValue = _valueConverter.ConvertValue(parameterValue, parameterInfo.Type);
                parameterInfo.SetValue(scriptableObject, convertedValue);

                // Mark the asset as dirty so Unity saves the changes
                EditorUtility.SetDirty(scriptableObject);
                AssetDatabase.SaveAssets();

                var serializableValue = _valueConverter.GetSerializableValue(convertedValue);

                return Response.Success($"Successfully set parameter '{parameterName}'", new
                {
                    parameterName = parameterName,
                    parameterValue = serializableValue,
                    parameterType = parameterInfo.Type.FullName,
                    isProperty = parameterInfo.IsProperty,
                    isField = parameterInfo.IsField
                });
            }
            catch (Exception ex)
            {
                return Response.Error($"Error setting parameter '{parameterName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Lists all parameters from a ScriptableObject
        /// </summary>
        public object ListParameters(JObject paramsObject)
        {
            var assetPath = paramsObject["asset_path"]?.ToString();
            if (string.IsNullOrEmpty(assetPath))
            {
                return Response.Error("Asset path is required");
            }

            try
            {
                var scriptableObject = _loader.LoadScriptableObject(assetPath);
                if (scriptableObject == null)
                {
                    return Response.Error($"Could not load ScriptableObject at path: {assetPath}");
                }

                var parameters = _reflectionHelper.GetAllParameters(scriptableObject);

                return Response.Success($"Successfully listed parameters for ScriptableObject", new
                {
                    assetPath = assetPath,
                    scriptableObjectType = scriptableObject.GetType().FullName,
                    parameters = parameters,
                    count = parameters.Count
                });
            }
            catch (Exception ex)
            {
                return Response.Error($"Error listing parameters: {ex.Message}");
            }
        }

        private (string assetPath, string parameterName) ExtractRequiredParameters(JObject paramsObject)
        {
            var assetPath = paramsObject["asset_path"]?.ToString();
            var parameterName = paramsObject["parameter_name"]?.ToString();
            return (assetPath, parameterName);
        }
    }

    /// <summary>
    /// Handles loading ScriptableObject assets
    /// </summary>
    public class ScriptableObjectLoader
    {
        /// <summary>
        /// Loads a ScriptableObject from the given asset path
        /// </summary>
        public ScriptableObject LoadScriptableObject(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
        }
    }

    /// <summary>
    /// Handles reflection operations for ScriptableObject parameters
    /// </summary>
    public class ParameterReflectionHelper
    {
        private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// Gets parameter information for a specific parameter name
        /// </summary>
        public ParameterInfo GetParameterInfo(ScriptableObject scriptableObject, string parameterName)
        {
            var type = scriptableObject.GetType();

            // Try property first
            var property = type.GetProperty(parameterName, DefaultBindingFlags);
            if (property != null)
            {
                return new ParameterInfo
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    IsProperty = true,
                    IsField = false,
                    CanWrite = property.CanWrite,
                    GetValue = (obj) => property.GetValue(obj),
                    SetValue = (obj, value) => property.SetValue(obj, value)
                };
            }

            // Try field
            var field = type.GetField(parameterName, DefaultBindingFlags);
            if (field != null)
            {
                return new ParameterInfo
                {
                    Name = field.Name,
                    Type = field.FieldType,
                    IsProperty = false,
                    IsField = true,
                    CanWrite = true, // Fields are always writable
                    GetValue = (obj) => field.GetValue(obj),
                    SetValue = (obj, value) => field.SetValue(obj, value)
                };
            }

            return null;
        }

        /// <summary>
        /// Gets all parameters from a ScriptableObject
        /// </summary>
        public List<object> GetAllParameters(ScriptableObject scriptableObject)
        {
            var parameters = new List<object>();
            var type = scriptableObject.GetType();

            // Get all properties
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    parameters.Add(CreateParameterInfo(scriptableObject, property, true));
                }
            }

            // Get all fields
            var fields = type.GetFields(DefaultBindingFlags);
            foreach (var field in fields)
            {
                // Skip backing fields for properties
                if (field.Name.Contains("<") && field.Name.Contains(">"))
                    continue;

                parameters.Add(CreateParameterInfo(scriptableObject, field, false));
            }

            return parameters;
        }

        private object CreateParameterInfo(ScriptableObject scriptableObject, PropertyInfo property, bool isProperty)
        {
            try
            {
                var value = property.GetValue(scriptableObject);
                var serializableValue = new ValueConverter().GetSerializableValue(value);

                return new
                {
                    parameterName = property.Name,
                    parameterValue = serializableValue,
                    parameterType = property.PropertyType.FullName,
                    isProperty = isProperty,
                    isField = !isProperty,
                    canWrite = property.CanWrite,
                    isPublic = property.GetGetMethod()?.IsPublic ?? false
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    parameterName = property.Name,
                    parameterValue = $"Error reading: {ex.Message}",
                    parameterType = property.PropertyType.FullName,
                    isProperty = isProperty,
                    isField = !isProperty,
                    canWrite = property.CanWrite,
                    error = ex.Message
                };
            }
        }

        private object CreateParameterInfo(ScriptableObject scriptableObject, FieldInfo field, bool isProperty)
        {
            try
            {
                var value = field.GetValue(scriptableObject);
                var serializableValue = new ValueConverter().GetSerializableValue(value);

                return new
                {
                    parameterName = field.Name,
                    parameterValue = serializableValue,
                    parameterType = field.FieldType.FullName,
                    isProperty = isProperty,
                    isField = !isProperty,
                    isPublic = field.IsPublic
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    parameterName = field.Name,
                    parameterValue = $"Error reading: {ex.Message}",
                    parameterType = field.FieldType.FullName,
                    isProperty = isProperty,
                    isField = !isProperty,
                    isPublic = field.IsPublic,
                    error = ex.Message
                };
            }
        }
    }

    /// <summary>
    /// Represents parameter information for reflection operations
    /// </summary>
    public class ParameterInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsProperty { get; set; }
        public bool IsField { get; set; }
        public bool CanWrite { get; set; }
        public Func<object, object> GetValue { get; set; }
        public Action<object, object> SetValue { get; set; }
    }

    /// <summary>
    /// Handles value conversion and serialization
    /// </summary>
    public class ValueConverter
    {
        /// <summary>
        /// Converts a value to the target type
        /// </summary>
        public object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return null;

            // Handle List<ScriptableObject> references
            if (IsScriptableObjectList(targetType))
            {
                return ConvertScriptableObjectList(value, targetType);
            }

            // Handle List<ExperimentStage> conversion
            if (IsExperimentStageList(targetType))
            {
                return ConvertExperimentStageList(value, targetType);
            }

            // Handle JToken conversion
            if (value is JToken jToken)
            {
                return jToken.ToObject(targetType);
            }

            // Handle basic type conversions
            if (targetType.IsAssignableFrom(value.GetType()))
                return value;

            // Handle string to enum conversion
            if (targetType.IsEnum && value is string enumString)
            {
                return Enum.Parse(targetType, enumString);
            }

            // Handle string to bool conversion
            if (targetType == typeof(bool) && value is string boolString)
            {
                return bool.Parse(boolString.ToLower());
            }

            // Handle numeric conversions
            if (IsNumericType(targetType))
            {
                return Convert.ChangeType(value, targetType);
            }

            // Handle Unity Object references
            if (typeof(UnityEngine.Object).IsAssignableFrom(targetType) && value is string assetPath)
            {
                return AssetDatabase.LoadAssetAtPath(assetPath, targetType);
            }

            // For complex types, try JSON deserialization
            try
            {
                if (value is string jsonString)
                {
                    return JsonConvert.DeserializeObject(jsonString, targetType);
                }
            }
            catch
            {
                // If JSON deserialization fails, throw the original exception
            }

            throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to {targetType}");
        }

        /// <summary>
        /// Gets a serializable representation of a value
        /// </summary>
        public object GetSerializableValue(object value, HashSet<object> visited = null)
        {
            if (value == null)
                return null;

            // Initialize visited set on first call to prevent circular references
            if (visited == null)
                visited = new HashSet<object>();

            // Check for circular reference
            if (visited.Contains(value))
                return $"[Circular Reference: {value.GetType().Name}]";

            visited.Add(value);

            // Handle Unity types
            if (value is UnityEngine.Color color)
            {
                return new { r = color.r, g = color.g, b = color.b, a = color.a };
            }
            else if (value is UnityEngine.Vector3 vector3)
            {
                return new { x = vector3.x, y = vector3.y, z = vector3.z };
            }
            else if (value is UnityEngine.Vector2 vector2)
            {
                return new { x = vector2.x, y = vector2.y };
            }
            else if (value is UnityEngine.Object unityObject)
            {
                return unityObject != null ? AssetDatabase.GetAssetPath(unityObject) : null;
            }
            else if (value is System.Collections.IList list)
            {
                return SerializeList(list, visited);
            }
            else if (IsComplexObject(value))
            {
                return $"[{value.GetType().Name}]";
            }

            return value;
        }

        private bool IsScriptableObjectList(Type targetType)
        {
            return targetType.IsGenericType && 
                   targetType.GetGenericTypeDefinition() == typeof(List<>) && 
                   typeof(UnityEngine.Object).IsAssignableFrom(targetType.GetGenericArguments()[0]);
        }

        private bool IsExperimentStageList(Type targetType)
        {
            return targetType.IsGenericType && 
                   targetType.GetGenericTypeDefinition() == typeof(List<>) && 
                   targetType.GetGenericArguments()[0].Name == "ExperimentStage";
        }

        private bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(float) || type == typeof(double);
        }

        private bool IsComplexObject(object value)
        {
            return value.GetType().IsClass && 
                   value.GetType() != typeof(string) && 
                   !value.GetType().IsPrimitive;
        }

        private object ConvertScriptableObjectList(object value, Type targetType)
        {
            if (value is JArray jArray)
            {
                var listType = targetType.GetGenericArguments()[0];
                var list = (System.Collections.IList)Activator.CreateInstance(targetType);
                
                foreach (var item in jArray)
                {
                    UnityEngine.Object obj = null;
                    
                    if (item.Type == JTokenType.String)
                    {
                        var assetPathStr = item.ToString();
                        obj = AssetDatabase.LoadAssetAtPath(assetPathStr, listType);
                    }
                    else if (item.Type == JTokenType.Integer)
                    {
                        var instanceId = item.ToObject<int>();
                        obj = EditorUtility.InstanceIDToObject(instanceId);
                    }
                    
                    if (obj != null && listType.IsAssignableFrom(obj.GetType()))
                        list.Add(obj);
                }
                
                return list;
            }
            return null;
        }

        private object ConvertExperimentStageList(object value, Type targetType)
        {
            if (value is JArray jArray)
            {
                var list = (System.Collections.IList)Activator.CreateInstance(targetType);
                var experimentStageType = targetType.GetGenericArguments()[0];
                
                foreach (var item in jArray)
                {
                    if (item.Type == JTokenType.Object)
                    {
                        var stageObj = Activator.CreateInstance(experimentStageType);
                        var stageObjType = stageObj.GetType();
                        
                        // Set experimentGraph field
                        SetExperimentStageField(stageObj, stageObjType, "experimentGraph", item["experimentGraph"]);
                        
                        // Set experimentTools field
                        SetExperimentStageField(stageObj, stageObjType, "experimentTools", item["experimentTools"]);
                        
                        list.Add(stageObj);
                    }
                }
                return list;
            }
            return null;
        }

        private void SetExperimentStageField(object stageObj, Type stageObjType, string fieldName, JToken fieldValue)
        {
            var field = stageObjType.GetField(fieldName);
            if (field != null && fieldValue != null)
            {
                var path = fieldValue.ToString();
                if (!string.IsNullOrEmpty(path))
                {
                    var asset = fieldName == "experimentGraph" 
                        ? AssetDatabase.LoadAssetAtPath<ScriptableObject>(path)
                        : AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                    field.SetValue(stageObj, asset);
                }
            }
        }

        private object SerializeList(System.Collections.IList list, HashSet<object> visited)
        {
            var serializedList = new List<object>();
            foreach (var item in list)
            {
                if (item != null && item.GetType().Name == "ExperimentStage")
                {
                    serializedList.Add(SerializeExperimentStage(item));
                }
                else
                {
                    serializedList.Add(GetSerializableValue(item, visited));
                }
            }
            return serializedList;
        }

        private object SerializeExperimentStage(object item)
        {
            var stageType = item.GetType();
            var graphField = stageType.GetField("experimentGraph");
            var toolsField = stageType.GetField("experimentTools");
            
            var graphValue = graphField?.GetValue(item);
            var toolsValue = toolsField?.GetValue(item);
            
            return new
            {
                experimentGraph = GetAssetPathOrTypeName(graphValue, "StepsGraph"),
                experimentTools = GetAssetPathOrTypeName(toolsValue, "GameObject")
            };
        }

        private string GetAssetPathOrTypeName(object obj, string typeName)
        {
            if (obj is UnityEngine.Object unityObj)
            {
                var path = AssetDatabase.GetAssetPath(unityObj);
                return string.IsNullOrEmpty(path) ? $"[{typeName}]" : path;
            }
            return null;
        }
    }
}
