using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace MCPForUnity.Editor.Tools
{
    /// <summary>
    /// Generic step handler that can create different types of steps based on input parameters.
    /// Acts as a router to specific step creation handlers.
    /// </summary>
    public static class AddStep
    {
        // --- Step Type Registry ---
        private static readonly Dictionary<string, StepTypeHandler> StepHandlers = new Dictionary<string, StepTypeHandler>
        {
            { "delay", new StepTypeHandler("DelayStep", CreateDelayStep, new[] { "delay", "wait", "pause", "sleep" }) },
            { "click", new StepTypeHandler("ClickStep", CreateClickStep, new[] { "click", "tap", "press", "button" }) },
            // Add more step types here as needed
        };

        // --- Pattern Recognition ---
        private static readonly Regex GraphPattern = new Regex(@"(?:in|graph|asset:?\s*)?([\w-]+\.asset)", RegexOptions.IgnoreCase);
        private static readonly Regex DelayPattern = new Regex(@"(?:wait|delay|for:?\s*)?(\d+(?:\.\d+)?)\s*(?:seconds?|s)?", RegexOptions.IgnoreCase);
        private static readonly Regex ClickPattern = new Regex(@"(?:click|tap|press)\s+(?:on\s+)?([\w-]+)(?:/([^/\s]+))?", RegexOptions.IgnoreCase);
        private static readonly Regex StepTypePattern = new Regex(@"(?:add|create)\s+(?:a\s+)?(\w+)\s*(?:step)?", RegexOptions.IgnoreCase);

        // --- Cached Types ---
        private static Type nodeGraphType;
        private static Type stepsGraphType;
        private static Dictionary<string, Type> stepTypes = new Dictionary<string, Type>();

        // --- Main Handler ---
        public static object HandleCommand(JObject @params)
        {
            try
            {
                Debug.Log($"AddStep received parameters: {@params?.ToString()}");

                // Extract parameters
                string prompt = @params["prompt"]?.ToString();
                string stepType = @params["stepType"]?.ToString()?.ToLower();
                string targetGraphPath = @params["targetGraphPath"]?.ToString() ?? @params["path"]?.ToString();

                Debug.Log($"Initial parameters - stepType: {stepType}, targetGraphPath: {targetGraphPath}, prompt: {prompt}");

                // Determine step type from prompt if not explicitly provided
                if (string.IsNullOrEmpty(stepType) && !string.IsNullOrEmpty(prompt))
                {
                    stepType = DetermineStepTypeFromPrompt(prompt);
                    Debug.Log($"Determined stepType from prompt: {stepType}");
                }

                // Parse graph path from prompt if not provided
                if (string.IsNullOrEmpty(targetGraphPath) && !string.IsNullOrEmpty(prompt))
                {
                    var graphMatch = GraphPattern.Match(prompt);
                    if (graphMatch.Success)
                    {
                        targetGraphPath = $"Assets/_MCP/AddStepsExample/{graphMatch.Groups[1].Value}";
                        Debug.Log($"Parsed targetGraphPath from prompt: {targetGraphPath}");
                    }
                }

                // Validate required parameters
                if (string.IsNullOrEmpty(stepType))
                {
                    return CreateErrorResponse($"Could not determine step type. Please specify 'stepType' parameter or use keywords like: {string.Join(", ", GetAllStepKeywords())}");
                }

                if (string.IsNullOrEmpty(targetGraphPath))
                {
                    return CreateErrorResponse("targetGraphPath or path parameter is required.");
                }

                // Find the appropriate handler
                if (!StepHandlers.TryGetValue(stepType, out var handler))
                {
                    var availableTypes = string.Join(", ", StepHandlers.Keys);
                    return CreateErrorResponse($"Unknown step type: '{stepType}'. Available types: {availableTypes}");
                }

                // Normalize and validate the graph path
                targetGraphPath = SanitizeAssetPath(targetGraphPath);
                var loadedGraph = AssetDatabase.LoadAssetAtPath<ScriptableObject>(targetGraphPath);
                
                if (loadedGraph == null || !IsStepsGraph(loadedGraph))
                {
                    return CreateErrorResponse($"StepsGraph asset not found at path: {targetGraphPath}. Make sure the path is relative to Assets/ and uses forward slashes.");
                }

                // Call the specific handler
                var result = handler.CreateStep(loadedGraph, @params, prompt);
                
                if (result != null)
                {
                    return CreateSuccessResponse(
                        $"{handler.TypeName} added successfully to '{targetGraphPath}'.",
                        result
                    );
                }
                else
                {
                    return CreateErrorResponse($"Failed to create {handler.TypeName}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in AddStep.HandleCommand: {ex.Message}\n{ex.StackTrace}");
                return CreateErrorResponse($"Exception occurred: {ex.Message}");
            }
        }

        // --- Response Helper Methods ---
        
        private static object CreateErrorResponse(string message)
        {
            return new Dictionary<string, object>
            {
                { "success", false },
                { "error", message }
            };
        }

        private static object CreateSuccessResponse(string message, object data = null)
        {
            var response = new Dictionary<string, object>
            {
                { "success", true },
                { "message", message }
            };
            
            if (data != null)
            {
                response["data"] = data;
            }
            
            return response;
        }

        // --- Step Type Determination ---
        private static string DetermineStepTypeFromPrompt(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
                return null;

            string lowerPrompt = prompt.ToLower();

            // Check each handler's keywords
            foreach (var kvp in StepHandlers)
            {
                if (kvp.Value.Keywords.Any(keyword => lowerPrompt.Contains(keyword)))
                {
                    return kvp.Key;
                }
            }

            // Fallback: try to extract from "add [type] step" pattern
            var stepTypeMatch = StepTypePattern.Match(prompt);
            if (stepTypeMatch.Success)
            {
                string extractedType = stepTypeMatch.Groups[1].Value.ToLower();
                if (StepHandlers.ContainsKey(extractedType))
                {
                    return extractedType;
                }
            }

            return null;
        }

        private static string[] GetAllStepKeywords()
        {
            return StepHandlers.SelectMany(kvp => kvp.Value.Keywords).Distinct().ToArray();
        }

        // --- Step Creation Handlers ---
        private static object CreateDelayStep(ScriptableObject graph, JObject @params, string prompt)
        {
            float delayTime = @params["delayTime"]?.ToObject<float?>() ?? 1.0f;
            string stepId = @params["stepId"]?.ToString();
            string tooltip = @params["tooltip"]?.ToString();
            Vector2 position = ParseVector2(@params["position"]) ?? Vector2.zero;

            // Parse delay time from prompt if not provided
            if (@params["delayTime"] == null && !string.IsNullOrEmpty(prompt))
            {
                var delayMatch = DelayPattern.Match(prompt);
                if (delayMatch.Success && float.TryParse(delayMatch.Groups[1].Value, out float parsedDelay))
                {
                    delayTime = parsedDelay;
                    Debug.Log($"Parsed delayTime from prompt: {delayTime}");
                }
            }

            if (delayTime < 0)
            {
                throw new ArgumentException("delayTime must be non-negative");
            }

            var delayStep = CreateStepNode(graph, "DelayStep", stepId);
            if (delayStep != null)
            {
                SetNodeProperty(delayStep, "position", position);
                SetNodeProperty(delayStep, "tooltip", tooltip ?? $"Wait for {delayTime} seconds");
                SetNodeProperty(delayStep, "timeToWait", delayTime, BindingFlags.NonPublic | BindingFlags.Instance);

                return new Dictionary<string, object>
                {
                    { "stepId", GetNodeProperty(delayStep, "stepId") ?? "unknown" },
                    { "stepType", "delay" },
                    { "delayTime", delayTime },
                    { "position", new { x = position.x, y = position.y } },
                    { "message", "Delay step created successfully" }
                };
            }
            return null;
        }

        private static object CreateClickStep(ScriptableObject graph, JObject @params, string prompt)
        {
            string targetName = @params["targetName"]?.ToString();
            string stepId = @params["stepId"]?.ToString();
            Vector2 position = ParseVector2(@params["position"]) ?? Vector2.zero;

            // Parse target from prompt if not provided
            if (string.IsNullOrEmpty(targetName) && !string.IsNullOrEmpty(prompt))
            {
                var clickMatch = ClickPattern.Match(prompt);
                if (clickMatch.Success)
                {
                    string prefabName = clickMatch.Groups[1].Value;
                    string childName = clickMatch.Groups[2].Success ? clickMatch.Groups[2].Value : prefabName;
                    targetName = $"{prefabName}/{childName}";
                    Debug.Log($"Parsed targetName from prompt: {targetName}");
                }
            }

            if (string.IsNullOrEmpty(targetName))
            {
                throw new ArgumentException("targetName parameter is required for click steps");
            }

            string[] targetParts = targetName.Split('/');
            if (targetParts.Length != 2)
            {
                throw new ArgumentException($"targetName must be in format 'prefabName/childName', got: {targetName}");
            }

            var clickStep = CreateStepNode(graph, "ClickStep", stepId);
            if (clickStep != null)
            {
                SetNodeProperty(clickStep, "position", position);

                // Create registry item for the target
                var registryItemType = FindType("RegistryItem");
                if (registryItemType != null)
                {
                    var registryItem = Activator.CreateInstance(registryItemType);
                    SetObjectProperty(registryItem, "prefabName", targetParts[0]);
                    SetObjectProperty(registryItem, "childName", targetParts[1]);
                    SetNodeProperty(clickStep, "_targetName", registryItem);
                }

                return new Dictionary<string, object>
                {
                    { "stepId", GetNodeProperty(clickStep, "stepId") ?? "unknown" },
                    { "stepType", "click" },
                    { "targetName", targetName },
                    { "position", new { x = position.x, y = position.y } },
                    { "message", "Click step created successfully" }
                };
            }
            return null;
        }

        // --- Helper Methods ---
        private static string SanitizeAssetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            path = path.Replace('\\', '/');
            if (!path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                return "Assets/" + path.TrimStart('/');
            }
            return path;
        }

        private static bool IsStepsGraph(ScriptableObject graph)
        {
            InitializeTypes();
            return graph != null && stepsGraphType != null && stepsGraphType.IsInstanceOfType(graph);
        }

        private static void InitializeTypes()
        {
            if (stepsGraphType == null || nodeGraphType == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var types = assembly.GetTypes();

                        if (nodeGraphType == null)
                            nodeGraphType = Array.Find(types, t => t.Name == "NodeGraph");

                        if (stepsGraphType == null)
                            stepsGraphType = Array.Find(types, t => t.Name == "StepsGraph");

                        if (nodeGraphType != null && stepsGraphType != null)
                            break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        private static Type FindType(string typeName)
        {
            if (stepTypes.TryGetValue(typeName, out var cachedType))
                return cachedType;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetType(typeName, false, true) ??
                              Array.Find(assembly.GetTypes(), t => t.Name == typeName);
                    
                    if (type != null)
                    {
                        stepTypes[typeName] = type;
                        return type;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return null;
        }

        private static ScriptableObject CreateStepNode(ScriptableObject graph, string stepTypeName, string stepId = null)
        {
            InitializeTypes();
            
            var stepType = FindType(stepTypeName);
            if (stepType == null)
            {
                Debug.LogError($"{stepTypeName} type not found!");
                return null;
            }

            if (nodeGraphType == null)
            {
                Debug.LogError("NodeGraph type not found!");
                return null;
            }

#if UNITY_EDITOR
            try
            {
                Undo.RecordObject(graph, $"Add {stepTypeName}");

                var addNodeMethod = nodeGraphType.GetMethod("AddNode", new Type[0]);
                if (addNodeMethod == null)
                {
                    Debug.LogError("AddNode method not found on NodeGraph!");
                    return null;
                }

                var genericAddNodeMethod = addNodeMethod.MakeGenericMethod(stepType);
                var stepNode = genericAddNodeMethod.Invoke(graph, null) as ScriptableObject;

                if (stepNode != null)
                {
                    // Set step ID
                    string finalStepId = string.IsNullOrEmpty(stepId) ? GenerateUniqueStepId(graph) : stepId;
                    SetNodeProperty(stepNode, "stepId", finalStepId);

                    // Register for undo and mark dirty
                    Undo.RegisterCreatedObjectUndo(stepNode, $"Add {stepTypeName}");
                    
                    string assetPath = AssetDatabase.GetAssetPath(graph);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        AssetDatabase.AddObjectToAsset(stepNode, graph);
                        AssetDatabase.SaveAssets();
                    }
                    
                    EditorUtility.SetDirty(graph);
                    return stepNode;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating {stepTypeName}: {ex.Message}");
            }
#endif
            return null;
        }

        private static bool SetNodeProperty(object target, string propertyName, object value, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            try
            {
                var field = target.GetType().GetField(propertyName, flags);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return true;
                }
                
                var property = target.GetType().GetProperty(propertyName, flags);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(target, value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to set property '{propertyName}': {ex.Message}");
            }
            return false;
        }

        private static object GetNodeProperty(object target, string propertyName, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            try
            {
                var field = target.GetType().GetField(propertyName, flags);
                if (field != null)
                {
                    return field.GetValue(target);
                }
                
                var property = target.GetType().GetProperty(propertyName, flags);
                if (property != null && property.CanRead)
                {
                    return property.GetValue(target);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to get property '{propertyName}': {ex.Message}");
            }
            return null;
        }

        private static bool SetObjectProperty(object target, string propertyName, object value)
        {
            return SetNodeProperty(target, propertyName, value);
        }

        private static Vector2? ParseVector2(JToken token)
        {
            if (token == null)
                return null;

            try
            {
                if (token is JArray arr && arr.Count >= 2)
                {
                    return new Vector2(arr[0].ToObject<float>(), arr[1].ToObject<float>());
                }
                else if (token is JObject obj)
                {
                    float x = obj["x"]?.ToObject<float>() ?? 0f;
                    float y = obj["y"]?.ToObject<float>() ?? 0f;
                    return new Vector2(x, y);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error parsing Vector2: {ex.Message}");
            }
            return null;
        }

        private static string GenerateUniqueStepId(ScriptableObject graph)
        {
            try
            {
                var nodesField = graph.GetType().GetField("nodes", BindingFlags.Public | BindingFlags.Instance);
                if (nodesField?.GetValue(graph) is System.Collections.IEnumerable nodes)
                {
                    int maxId = 0;
                    foreach (var node in nodes)
                    {
                        if (node is ScriptableObject so)
                        {
                            string id = GetNodeProperty(so, "stepId") as string;
                            if (!string.IsNullOrEmpty(id) && id.StartsWith("s") && int.TryParse(id.Substring(1), out int numericId))
                            {
                                maxId = Math.Max(maxId, numericId);
                            }
                        }
                    }
                    return "s" + (maxId + 1);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error generating unique step ID: {ex.Message}");
            }
            return "s1";
        }

        // --- Helper Classes ---
        private class StepTypeHandler
        {
            public string TypeName { get; }
            public Func<ScriptableObject, JObject, string, object> CreateStep { get; }
            public string[] Keywords { get; }

            public StepTypeHandler(string typeName, Func<ScriptableObject, JObject, string, object> createStep, string[] keywords)
            {
                TypeName = typeName;
                CreateStep = createStep;
                Keywords = keywords;
            }
        }
    }
}