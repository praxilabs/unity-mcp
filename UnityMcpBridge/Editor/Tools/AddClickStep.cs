using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;
using UnityMcpBridge.Editor.Helpers;
using System.Text.RegularExpressions;
using System;

namespace UnityMcpBridge.Editor.Tools
{
    public static class AddClickStep
    {
        // Regular expressions for natural language parsing
        private static readonly Regex GraphPattern = new Regex(@"(?:in|graph|asset:?\s*)?([\w-]+\.asset)", RegexOptions.IgnoreCase);
        private static readonly Regex TargetPattern = new Regex(@"(?:in|on|at|button:?\s*)?([\w-]+)/([\w-]+)", RegexOptions.IgnoreCase);

        public static object HandleCommand(JObject @params)
        {
            try
            {
                // Add debug logging to see what parameters we're receiving
                Debug.Log($"AddClickStep received parameters: {@params?.ToString()}");

                string prompt = @params["prompt"]?.ToString();
                string targetGraphPath = @params["targetGraphPath"]?.ToString();
                string targetName = @params["targetName"]?.ToString();
                string stepId = @params["stepId"]?.ToString();

                Debug.Log($"Parsed parameters - targetGraphPath: {targetGraphPath}, targetName: {targetName}, prompt: {prompt}");

                // Try to parse from natural language if prompt is provided
                if (!string.IsNullOrEmpty(prompt))
                {
                    var graphMatch = GraphPattern.Match(prompt);
                    var targetMatch = TargetPattern.Match(prompt);

                    if (graphMatch.Success && string.IsNullOrEmpty(targetGraphPath))
                    {
                        targetGraphPath = $"Assets/_MCP/Graphs/{graphMatch.Groups[1].Value}";
                        Debug.Log($"Parsed targetGraphPath from prompt: {targetGraphPath}");
                    }

                    if (targetMatch.Success && string.IsNullOrEmpty(targetName))
                    {
                        targetName = $"{targetMatch.Groups[1].Value}/{targetMatch.Groups[2].Value}";
                        Debug.Log($"Parsed targetName from prompt: {targetName}");
                    }
                }

                // Validate required parameters
                if (string.IsNullOrEmpty(targetGraphPath))
                {
                    return Response.Error("targetGraphPath parameter is required.");
                }
                if (string.IsNullOrEmpty(targetName))
                {
                    return Response.Error("targetName parameter is required.");
                }

                // Check if the file exists before trying to load it
                if (!System.IO.File.Exists(targetGraphPath))
                {
                    return Response.Error($"File does not exist at path: {targetGraphPath}");
                }

                // Load the target graph asset
                var targetGraph = AssetDatabase.LoadAssetAtPath<StepsGraph>(targetGraphPath);
                if (targetGraph == null)
                {
                    return Response.Error($"Could not load StepsGraph from path: {targetGraphPath}. Make sure it's a valid StepsGraph asset.");
                }

                // Validate targetName format
                string[] targetParts = targetName.Split('/');
                if (targetParts.Length != 2)
                {
                    return Response.Error($"targetName must be in format 'prefabName/childName', got: {targetName}");
                }

                // Create the click step
                var clickStep = ScriptableObject.CreateInstance<ClickStep>();
                if (clickStep == null)
                {
                    return Response.Error("Failed to create ClickStep instance. Make sure ClickStep class exists and is properly defined.");
                }

                clickStep.stepId = stepId ?? System.Guid.NewGuid().ToString();
                
                // Set up the registry item for the target
                var registryItem = new RegistryItem
                {
                    prefabName = targetParts[0],
                    childName = targetParts[1]
                };
                clickStep._targetName = registryItem;

                Debug.Log($"Created ClickStep with ID: {clickStep.stepId}, targeting: {targetParts[0]}/{targetParts[1]}");

                // Initialize nodes list if it's null
                if (targetGraph.nodes == null)
                {
                    Debug.Log("Initializing nodes list for targetGraph");
                    targetGraph.nodes = new System.Collections.Generic.List<StepsGraphNode>();
                }

                // Add the step to the graph
                targetGraph.nodes.Add(clickStep);
                EditorUtility.SetDirty(targetGraph);
                
                // Force save and refresh
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Successfully added ClickStep to graph. Total nodes: {targetGraph.nodes.Count}");

                return Response.Success(new
                {
                    stepId = clickStep.stepId,
                    targetName = targetName,
                    graphPath = targetGraphPath,
                    nodesCount = targetGraph.nodes.Count,
                    message = "Click step added successfully"
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in AddClickStep.HandleCommand: {ex.Message}\n{ex.StackTrace}");
                return Response.Error($"Exception occurred: {ex.Message}");
            }
        }
    }
}