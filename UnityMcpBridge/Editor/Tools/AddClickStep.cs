using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;
using UnityMcpBridge.Editor.Helpers;
using System.Text.RegularExpressions;

namespace UnityMcpBridge.Editor.Tools
{
    public static class AddClickStep
    {
        // Regular expressions for natural language parsing
        private static readonly Regex GraphPattern = new Regex(@"(?:in|graph|asset:?\s*)?([\w-]+\.asset)", RegexOptions.IgnoreCase);
        private static readonly Regex TargetPattern = new Regex(@"(?:in|on|at|button:?\s*)?([\w-]+)/([\w-]+)", RegexOptions.IgnoreCase);

        public static object HandleCommand(JObject @params)
        {
            string prompt = @params["prompt"]?.ToString();
            string targetGraphPath = @params["targetGraphPath"]?.ToString();
            string targetName = @params["targetName"]?.ToString();
            string stepId = @params["stepId"]?.ToString();

            // Try to parse from natural language if prompt is provided
            if (!string.IsNullOrEmpty(prompt))
            {
                var graphMatch = GraphPattern.Match(prompt);
                var targetMatch = TargetPattern.Match(prompt);

                if (graphMatch.Success && string.IsNullOrEmpty(targetGraphPath))
                {
                    targetGraphPath = $"Assets/_MCP/Graphs/{graphMatch.Groups[1].Value}";
                }

                if (targetMatch.Success && string.IsNullOrEmpty(targetName))
                {
                    targetName = $"{targetMatch.Groups[1].Value}/{targetMatch.Groups[2].Value}";
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

            // Load the target graph asset
            var targetGraph = AssetDatabase.LoadAssetAtPath<StepsGraph>(targetGraphPath);
            if (targetGraph == null)
            {
                return Response.Error($"Could not find StepsGraph at path: {targetGraphPath}");
            }

            // Create the click step
            var clickStep = ScriptableObject.CreateInstance<ClickStep>();
            clickStep.stepId = stepId ?? System.Guid.NewGuid().ToString();
            
            // Set up the registry item for the target
            var registryItem = new RegistryItem
            {
                prefabName = targetName.Split('/')[0], // Assuming format: "prefabName/childName"
                childName = targetName.Split('/')[1]
            };
            clickStep._targetName = registryItem;

            // Add the step to the graph
            targetGraph.nodes.Add(clickStep);
            EditorUtility.SetDirty(targetGraph);
            AssetDatabase.SaveAssets();

            return Response.Success(new
            {
                stepId = clickStep.stepId,
                message = "Click step added successfully"
            });
        }
    }
}