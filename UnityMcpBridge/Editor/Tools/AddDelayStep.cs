using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;
using UnityMcpBridge.Editor.Helpers;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityMcpBridge.Editor.Tools
{
    [CreateAssetMenu(fileName = "AddDelayStep", menuName = "Praxilabs/Tools/Add Delay Step")]
    public class AddDelayStep : ScriptableObject
    {
        [Header("Graph Reference")]
        [SerializeField] private ScriptableObject targetGraph; // Using ScriptableObject as base type

        [Header("Node Settings")]
        [SerializeField] private Vector2 nodePosition = Vector2.zero;
        [SerializeField] private string stepId = "";

        [Header("Delay Step Settings")]
        [SerializeField] private float delayTime = 1.0f;
        [SerializeField] private string tooltip = "Wait for specified seconds";

        // Cached types for reflection
        private static Type nodeGraphType;
        private static Type nodeType;
        private static Type stepsGraphType;
        private static Type delayStepType;
        private static Type stepNodeType;

        // Regular expressions for natural language parsing
        private static readonly Regex GraphPattern = new Regex(@"(?:in|graph|asset:?\s*)?([\w-]+\.asset)", RegexOptions.IgnoreCase);
        private static readonly Regex DelayTimePattern = new Regex(@"(?:wait|delay|for:?\s*)?(\d+(?:\.\d+)?)\s*(?:seconds?|s)?", RegexOptions.IgnoreCase);

        /// <summary>
        /// Initialize type references using reflection
        /// </summary>
        private static void InitializeTypes()
        {
            if (nodeGraphType == null || nodeType == null || stepsGraphType == null || delayStepType == null || stepNodeType == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var types = assembly.GetTypes();

                        if (nodeGraphType == null)
                            nodeGraphType = Array.Find(types, t => t.Name == "NodeGraph");

                        if (nodeType == null)
                            nodeType = Array.Find(types, t => t.Name == "Node");

                        if (stepsGraphType == null)
                            stepsGraphType = Array.Find(types, t => t.Name == "StepsGraph");

                        if (delayStepType == null)
                            delayStepType = Array.Find(types, t => t.Name == "DelayStep");

                        if (stepNodeType == null)
                            stepNodeType = Array.Find(types, t => t.Name == "StepNode");

                        if (nodeGraphType != null && nodeType != null && stepsGraphType != null &&
                            delayStepType != null && stepNodeType != null)
                            break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the target graph is a StepsGraph
        /// </summary>
        private bool IsStepsGraph(ScriptableObject graph)
        {
            InitializeTypes();
            return graph != null && stepsGraphType != null && stepsGraphType.IsInstanceOfType(graph);
        }

        /// <summary>
        /// Checks if the target graph is a NodeGraph (XNode)
        /// </summary>
        private bool IsNodeGraph(ScriptableObject graph)
        {
            InitializeTypes();
            return graph != null && nodeGraphType != null && nodeGraphType.IsInstanceOfType(graph);
        }

        /// <summary>
        /// Normalizes a file path to Unity's asset path format
        /// </summary>
        private static string NormalizeUnityPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            // Convert backslashes to forward slashes
            path = path.Replace('\\', '/');

            // If it's a full system path, extract the Assets-relative portion
            int assetsIndex = path.IndexOf("Assets/", StringComparison.OrdinalIgnoreCase);
            if (assetsIndex >= 0)
            {
                path = path.Substring(assetsIndex);
            }

            // Ensure it starts with Assets/
            if (!path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                path = "Assets/" + path.TrimStart('/');
            }

            return path;
        }

        /// <summary>
        /// Adds a DelayStep node to the target graph (Context Menu)
        /// </summary>
        [ContextMenu("Add Delay Step")]
        public void AddDelayStepContextMenu()
        {
            var result = AddDelayStepToGraph(targetGraph, delayTime, nodePosition, stepId, tooltip);
            if (result != null)
            {
                Debug.Log($"Successfully added DelayStep: {GetStepId(result)}");
            }
        }

        /// <summary>
        /// Adds a DelayStep node to the specified graph
        /// </summary>
        /// <param name="graph">Target graph (should be StepsGraph)</param>
        /// <param name="delayTimeValue">Delay time in seconds</param>
        /// <param name="position">Position in the graph</param>
        /// <param name="customStepId">Custom step ID (optional)</param>
        /// <param name="customTooltip">Custom tooltip (optional)</param>
        /// <returns>The created DelayStep node</returns>
        public ScriptableObject AddDelayStepToGraph(ScriptableObject graph, float delayTimeValue, Vector2 position, string customStepId = "", string customTooltip = "")
        {
            if (graph == null)
            {
                Debug.LogError("Target graph is not assigned!");
                return null;
            }

            if (!IsStepsGraph(graph))
            {
                Debug.LogError("Target graph must be a StepsGraph!");
                return null;
            }

            if (delayTimeValue < 0)
            {
                Debug.LogError("Delay time must be non-negative!");
                return null;
            }

            InitializeTypes();
            if (delayStepType == null)
            {
                Debug.LogError("DelayStep type not found! Make sure the assembly is loaded.");
                return null;
            }

            if (nodeGraphType == null)
            {
                Debug.LogError("NodeGraph type not found! Make sure XNode is installed.");
                return null;
            }

#if UNITY_EDITOR
            // Record undo for the graph
            Undo.RecordObject(graph, "Add Delay Step");

            // Create the DelayStep node using reflection
            var addNodeMethod = nodeGraphType.GetMethod("AddNode", new Type[0]);
            if (addNodeMethod == null)
            {
                Debug.LogError("AddNode method not found on NodeGraph!");
                return null;
            }

            var genericAddNodeMethod = addNodeMethod.MakeGenericMethod(delayStepType);
            var delayStep = genericAddNodeMethod.Invoke(graph, null) as ScriptableObject;

            if (delayStep != null)
            {
                // Set position using reflection
                SetPosition(delayStep, position);

                // Set step ID (generate unique if empty)
                string finalStepId = string.IsNullOrEmpty(customStepId) ? GenerateUniqueStepId(graph) : customStepId;
                SetStepId(delayStep, finalStepId);

                // Set tooltip
                string finalTooltip = string.IsNullOrEmpty(customTooltip) ? $"Wait for {delayTimeValue} seconds" : customTooltip;
                SetTooltip(delayStep, finalTooltip);

                // Set delay time using reflection
                SetDelayTime(delayStep, delayTimeValue);

                // Register the created node for undo
                Undo.RegisterCreatedObjectUndo(delayStep, "Add Delay Step");

                // Add to asset database if the graph is an asset
                string assetPath = AssetDatabase.GetAssetPath(graph);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    AssetDatabase.AddObjectToAsset(delayStep, graph);
                    AssetDatabase.SaveAssets();
                }

                // Mark the graph as dirty
                EditorUtility.SetDirty(graph);

                Debug.Log($"Successfully added DelayStep with ID: {finalStepId} and delay time: {delayTimeValue}s at position {position}");
                return delayStep;
            }
            else
            {
                Debug.LogError("Failed to create DelayStep node!");
                return null;
            }
#else
            Debug.LogWarning("AddDelayStep can only be used in the Unity Editor!");
            return null;
#endif
        }

        /// <summary>
        /// Sets the position on a node using reflection
        /// </summary>
        private void SetPosition(ScriptableObject node, Vector2 position)
        {
            try
            {
                var positionField = node.GetType().GetField("position", BindingFlags.Public | BindingFlags.Instance);
                if (positionField != null)
                {
                    positionField.SetValue(node, position);
                }
                else
                {
                    Debug.LogWarning("Could not find position field on node");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error setting position: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the step ID on a node using reflection
        /// </summary>
        private void SetStepId(ScriptableObject node, string id)
        {
            try
            {
                var stepIdField = node.GetType().GetField("stepId", BindingFlags.Public | BindingFlags.Instance);
                if (stepIdField != null)
                {
                    stepIdField.SetValue(node, id);
                }
                else
                {
                    Debug.LogWarning("Could not find stepId field on node");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error setting step ID: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the step ID from a node using reflection
        /// </summary>
        private string GetStepId(ScriptableObject node)
        {
            try
            {
                var stepIdField = node.GetType().GetField("stepId", BindingFlags.Public | BindingFlags.Instance);
                if (stepIdField != null)
                {
                    return stepIdField.GetValue(node) as string;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting step ID: {ex.Message}");
            }
            return "unknown";
        }

        /// <summary>
        /// Sets the tooltip on a node using reflection
        /// </summary>
        private void SetTooltip(ScriptableObject node, string tooltipText)
        {
            try
            {
                var tooltipField = node.GetType().GetField("tooltip", BindingFlags.Public | BindingFlags.Instance);
                if (tooltipField != null)
                {
                    tooltipField.SetValue(node, tooltipText);
                }
                else
                {
                    Debug.LogWarning("Could not find tooltip field on node");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error setting tooltip: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the delay time on the DelayStep using reflection
        /// </summary>
        private void SetDelayTime(ScriptableObject delayStep, float timeValue)
        {
            try
            {
                // Try to set delay time using reflection (looking for private timeToWait field)
                var timeToWaitField = delayStep.GetType().GetField("timeToWait", 
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (timeToWaitField != null)
                {
                    timeToWaitField.SetValue(delayStep, timeValue);
                    Debug.Log($"Set delay time to: {timeValue} seconds");
                }
                else
                {
                    Debug.LogWarning("Could not find timeToWait field on DelayStep");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error setting delay time: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the nodes list from a graph using reflection
        /// </summary>
        private object GetNodesFromGraph(ScriptableObject graph)
        {
            try
            {
                var nodesField = graph.GetType().GetField("nodes", BindingFlags.Public | BindingFlags.Instance);
                if (nodesField != null)
                {
                    return nodesField.GetValue(graph);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting nodes from graph: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Generates a unique step ID for the target graph
        /// </summary>
        private string GenerateUniqueStepId(ScriptableObject graph)
        {
            if (graph == null) return "s1";

            InitializeTypes();
            if (stepNodeType == null) return "s1";

            try
            {
                var nodesList = GetNodesFromGraph(graph);
                if (nodesList == null) return "s1";

                // Get the nodes using reflection (it should be some kind of collection)
                var enumerableType = nodesList.GetType();
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(enumerableType))
                {
                    int maxId = 0;
                    foreach (var node in (System.Collections.IEnumerable)nodesList)
                    {
                        if (stepNodeType.IsInstanceOfType(node))
                        {
                            string id = GetStepId(node as ScriptableObject);
                            if (id.StartsWith("s") && int.TryParse(id.Substring(1), out int numericId))
                            {
                                maxId = Mathf.Max(maxId, numericId);
                            }
                        }
                    }
                    return "s" + (maxId + 1);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error generating unique step ID: {ex.Message}");
            }

            return "s1";
        }

        /// <summary>
        /// Sets the target graph reference
        /// </summary>
        public void SetTargetGraph(ScriptableObject graph)
        {
            targetGraph = graph;
        }

        /// <summary>
        /// Sets the delay time
        /// </summary>
        public void SetDelayTime(float time)
        {
            delayTime = time;
        }

        /// <summary>
        /// Sets the default position for new nodes
        /// </summary>
        public void SetNodePosition(Vector2 position)
        {
            nodePosition = position;
        }

        // Static method to maintain compatibility with MCP system
        public static object HandleCommand(JObject parameters)
        {
            try
            {
                Debug.Log($"AddDelayStep received parameters: {parameters?.ToString()}");

                string prompt = parameters["prompt"]?.ToString();
                string targetGraphPath = parameters["targetGraphPath"]?.ToString();
                string stepIdParam = parameters["stepId"]?.ToString();
                float delayTimeParam = 1.0f;

                // Parse delayTime parameter
                if (parameters["delayTime"] != null)
                {
                    if (float.TryParse(parameters["delayTime"].ToString(), out float parsedDelay))
                    {
                        delayTimeParam = parsedDelay;
                    }
                }

                Debug.Log($"Parsed parameters - targetGraphPath: {targetGraphPath}, delayTime: {delayTimeParam}, prompt: {prompt}");

                // Try to parse from natural language if prompt is provided
                if (!string.IsNullOrEmpty(prompt))
                {
                    var graphMatch = GraphPattern.Match(prompt);
                    var delayTimeMatch = DelayTimePattern.Match(prompt);

                    if (graphMatch.Success && string.IsNullOrEmpty(targetGraphPath))
                    {
                        targetGraphPath = $"Assets/_MCP/AddStepsExample/{graphMatch.Groups[1].Value}";
                        Debug.Log($"Parsed targetGraphPath from prompt: {targetGraphPath}");
                    }

                    if (delayTimeMatch.Success && parameters["delayTime"] == null)
                    {
                        if (float.TryParse(delayTimeMatch.Groups[1].Value, out float parsedTime))
                        {
                            delayTimeParam = parsedTime;
                            Debug.Log($"Parsed delayTime from prompt: {delayTimeParam}");
                        }
                    }
                }

                // Validate required parameters
                if (string.IsNullOrEmpty(targetGraphPath))
                {
                    return Response.Error("targetGraphPath parameter is required.");
                }

                if (delayTimeParam < 0)
                {
                    return Response.Error("delayTime must be non-negative.");
                }

                // Normalize the path to Unity format and check if asset exists
                targetGraphPath = NormalizeUnityPath(targetGraphPath);

                // Load the StepsGraph asset - need to create temporary instance first to access non-static method
                var tempInstance = CreateInstance<AddDelayStep>();
                var loadedGraph = AssetDatabase.LoadAssetAtPath<ScriptableObject>(targetGraphPath);
                if (loadedGraph == null || !tempInstance.IsStepsGraph(loadedGraph))
                {
                    DestroyImmediate(tempInstance);
                    return Response.Error($"StepsGraph asset not found at path: {targetGraphPath}. Make sure the path is relative to Assets/ and uses forward slashes.");
                }

                // Set up the temporary instance
                tempInstance.SetTargetGraph(loadedGraph);

                // Add the delay step using the new method
                var delayStep = tempInstance.AddDelayStepToGraph(loadedGraph, delayTimeParam, Vector2.zero, stepIdParam, "");

                if (delayStep != null)
                {
                    // Get the step ID before cleaning up the temp instance
                    string finalStepId = GetStepIdStatic(delayStep);

                    // Clean up temporary instance
                    DestroyImmediate(tempInstance);

                    // Create success response data
                    var successData = new Dictionary<string, object>
                    {
                        { "stepId", finalStepId },
                        { "delayTime", delayTimeParam },
                        { "graphPath", targetGraphPath },
                        { "message", "Delay step added successfully" }
                    };

                    // Convert dictionary to JSON string
                    string successMessage = Newtonsoft.Json.JsonConvert.SerializeObject(successData);
                    return Response.Success(successMessage);
                }
                else
                {
                    DestroyImmediate(tempInstance);
                    return Response.Error("Failed to create DelayStep node");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in AddDelayStep.HandleCommand: {ex.Message}\n{ex.StackTrace}");
                return Response.Error($"Exception occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the step ID from a node using reflection (static version)
        /// </summary>
        private static string GetStepIdStatic(ScriptableObject node)
        {
            try
            {
                var stepIdField = node.GetType().GetField("stepId", BindingFlags.Public | BindingFlags.Instance);
                if (stepIdField != null)
                {
                    return stepIdField.GetValue(node) as string;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting step ID: {ex.Message}");
            }
            return "unknown";
        }
    }
}