using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;

public static class SetNodeAsFirstStep
{
    // Args:
    // - graphPath: string (required) → path to a StepsGraph asset
    // - nodeName: string (optional)  → exact node name in the graph (e.g., "ClickStep_-123")
    // - nodeId: int (optional)       → InstanceID of the node
    public static object HandleCommand(JObject args)
    {
        try
        {
            string graphPath = args?["graphPath"]?.ToString();
            string nodeName = args?["nodeName"]?.ToString();
            int? nodeId = args?["nodeId"]?.ToObject<int?>();

            if (string.IsNullOrEmpty(graphPath))
            {
                return new { success = false, error = "Missing required argument: graphPath" };
            }

            // Load as base type to avoid compile-time dependency on user assemblies
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new { success = false, error = $"Could not load NodeGraph at path: {graphPath}" };
            }

            if (graph.nodes == null || graph.nodes.Count == 0)
            {
                return new { success = false, error = "Graph contains no nodes to assign as first step" };
            }

            // Pick target node
            Node targetNode = null;
            if (!string.IsNullOrEmpty(nodeName))
            {
                targetNode = graph.nodes.FirstOrDefault(n => n != null && n.name == nodeName);
            }
            else if (nodeId.HasValue)
            {
                targetNode = graph.nodes.FirstOrDefault(n => n != null && n.GetInstanceID() == nodeId.Value);
            }
            else
            {
                // No selector provided, choose the first node of a class name ending with "Step" if possible, else first non-null
                targetNode = graph.nodes.FirstOrDefault(n => n != null && n.GetType().Name.EndsWith("Step", StringComparison.Ordinal))
                             ?? graph.nodes.FirstOrDefault(n => n != null);
            }

            if (targetNode == null)
            {
                return new { success = false, error = "Could not resolve a target node in the graph" };
            }

            // Use SerializedObject to set StepsGraph.firstStep without direct type dependency
            SerializedObject so = new SerializedObject(graph);
            SerializedProperty firstStepProp = so.FindProperty("firstStep");
            if (firstStepProp == null)
            {
                return new { success = false, error = "Graph does not have a 'firstStep' property (is it a StepsGraph?)" };
            }

            firstStepProp.objectReferenceValue = targetNode;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();

            return new
            {
                success = true,
                message = $"Assigned '{targetNode.name}' as first step",
                graphPath,
                assignedNodeName = targetNode.name,
                assignedNodeId = targetNode.GetInstanceID()
            };
        }
        catch (Exception ex)
        {
            return new { success = false, error = $"Failed to set first step: {ex.Message}" };
        }
    }

    // Args:
    // - graphPath: string (required) → path to a StepsGraph asset
    public static object HandleListGraphNodesCommand(JObject args)
    {
        try
        {
            string graphPath = args?["graphPath"]?.ToString();

            if (string.IsNullOrEmpty(graphPath))
            {
                return new { success = false, error = "Missing required argument: graphPath" };
            }

            // Load as base type to avoid compile-time dependency on user assemblies
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new { success = false, error = $"Could not load NodeGraph at path: {graphPath}" };
            }

            if (graph.nodes == null || graph.nodes.Count == 0)
            {
                return new 
                { 
                    success = true, 
                    message = "Graph contains no nodes",
                    data = new { nodes = new List<object>() }
                };
            }

            // Get first step node if it exists
            Node firstStepNode = null;
            try
            {
                SerializedObject so = new SerializedObject(graph);
                SerializedProperty firstStepProp = so.FindProperty("firstStep");
                if (firstStepProp != null)
                {
                    firstStepNode = firstStepProp.objectReferenceValue as Node;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Could not get first step node: {ex.Message}");
            }

            // Create list of node information
            var nodesList = new List<object>();
            foreach (var node in graph.nodes)
            {
                if (node == null) continue;

                nodesList.Add(new
                {
                    name = node.name,
                    type = node.GetType().Name,
                    fullType = node.GetType().FullName,
                    instanceId = node.GetInstanceID(),
                    position = new { x = node.position.x, y = node.position.y },
                    isFirstStep = (firstStepNode != null && node.GetInstanceID() == firstStepNode.GetInstanceID()),
                    hasInputPorts = node.Inputs?.Count() > 0,
                    hasOutputPorts = node.Outputs?.Count() > 0,
                    inputPortCount = node.Inputs?.Count() ?? 0,
                    outputPortCount = node.Outputs?.Count() ?? 0
                });
            }

            return new
            {
                success = true,
                message = $"Found {nodesList.Count} nodes in graph",
                data = new { nodes = nodesList },
                graphPath,
                totalNodes = nodesList.Count
            };
        }
        catch (Exception ex)
        {
            return new { success = false, error = $"Failed to list graph nodes: {ex.Message}" };
        }
    }
}