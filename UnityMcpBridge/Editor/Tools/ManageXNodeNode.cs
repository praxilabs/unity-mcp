using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using System.Collections.Generic;

namespace UnityMcpBridge.Editor.Tools {
    public static class ManageXNodeNode
    {
        public static object CreateNode(JObject args)
        {
            // Validate input arguments
            var validation = ValidateCreateNodeArgs(args);
            if (!validation.IsValid)
            {
                return ToolUtils.CreateErrorResponse(validation.ErrorMessage);
            }

            try
            {
                // Load and validate graph
                var graph = ToolUtils.LoadNodeGraph(validation.GraphPath);
                if (graph == null)
                {
                    return ToolUtils.CreateErrorResponse($"Could not load NodeGraph at path: {validation.GraphPath}");
                }

                // Find the StepNode type
                Type nodeType = ToolUtils.FindNodeType(validation.NodeTypeName);
                if (nodeType == null)
                {
                    return ToolUtils.CreateErrorResponse($"Could not find StepNode type: {validation.NodeTypeName}");
                }

                // Create the StepNode
                StepNode newNode = graph.AddNode(nodeType) as StepNode;
                if (newNode == null)
                {
                    return ToolUtils.CreateErrorResponse($"Failed to create StepNode of type: {validation.NodeTypeName}");
                }

                // Verify the node was added to the graph
                if (!graph.nodes.Contains(newNode))
                {
                    return ToolUtils.CreateErrorResponse($"StepNode was created but not added to graph nodes collection");
                }

                // Configure the StepNode
                ConfigureNewNode(newNode, validation.NodeTypeName, validation.PositionX, validation.PositionY, validation.Tooltip, validation.Description);
                // Save changes
                ToolUtils.SaveGraphChanges(graph);
                EditorUtility.SetDirty(newNode);
                EditorUtility.SetDirty(graph);

                return CreateNodeSuccessResponse(newNode, validation.GraphPath, validation.NodeTypeName, validation.PositionX, validation.PositionY, validation.Tooltip, validation.Description);
            }
            catch (Exception ex)
            {
                return ToolUtils.CreateErrorResponse($"Failed to create StepNode: {ex.Message}");
            }
        }

        private static (bool IsValid, string GraphPath, string NodeTypeName, float PositionX, float PositionY, string ErrorMessage, string Tooltip, string Description) ValidateCreateNodeArgs(JObject args)
        {
            string graphPath = args?["graphPath"]?.ToString();
            string nodeTypeName = args?["nodeTypeName"]?.ToString();
            float posX = args?["positionX"]?.ToObject<float>() ?? 0f;
            float posY = args?["positionY"]?.ToObject<float>() ?? 0f;
            string tooltip = args?["tooltip"]?.ToString();
            string description = args?["description"]?.ToString();

            if (string.IsNullOrEmpty(graphPath))
            {
                return (false, null, null, 0f, 0f, "Missing required argument: graphPath", null, null);
            }

            if (string.IsNullOrEmpty(nodeTypeName))
            {
                return (false, null, null, 0f, 0f, "Missing required argument: nodeTypeName", null, null);
            }

            return (true, graphPath, nodeTypeName, posX, posY, null, tooltip, description);
        }

        private static void ConfigureNewNode(StepNode newNode, string nodeTypeName, float posX, float posY, string tooltip, string description)
        {
            try
            {
                // Set position first
                newNode.position = new Vector2(posX, posY);
                
                newNode.metadata.tooltip = tooltip ?? "";
                newNode.metadata.description = description ?? "";

                newNode.name = $"{nodeTypeName}_{newNode.GetInstanceID()}";

                // Add the StepNode as a sub-asset to the graph
                if (!AssetDatabase.Contains(newNode))
                {
                    AssetDatabase.AddObjectToAsset(newNode, newNode.graph);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to configure node: {ex.Message}");
                throw;
            }
        }

        private static object CreateNodeSuccessResponse(StepNode newNode, string graphPath, string nodeTypeName, float posX, float posY, string tooltip, string description)
        {
            return new
            {
                success = true,
                message = $"StepNode '{nodeTypeName}' created in graph",
                graphPath = graphPath,
                nodeId = newNode.GetInstanceID(),
                nodeName = newNode.name,
                nodeType = nodeTypeName,
                position = new { x = posX, y = posY },
                tooltip = tooltip,
                description = description,
                timestamp = System.DateTime.Now.ToString()
            };
        }

        public static object DeleteNode(JObject args)
        {
            // Validate input arguments
            var validation = ValidateDeleteNodeArgs(args);
            if (!validation.IsValid)
            {
                return ToolUtils.CreateErrorResponse(validation.ErrorMessage);
            }

            try
            {
                // Load and validate graph
                var graph = ToolUtils.LoadNodeGraph(validation.GraphPath);
                if (graph == null)
                {
                    return ToolUtils.CreateErrorResponse($"Could not load NodeGraph at path: {validation.GraphPath}");
                }

                // Find the StepNode
                var node = ToolUtils.FindNodeByIdentifier(graph, validation.NodeIdentifier, validation.IdentifierType);
                if (node == null)
                {
                    return ToolUtils.CreateErrorResponse($"Could not find StepNode with {validation.IdentifierType}: {validation.NodeIdentifier}");
                }

                // Delete the StepNode
                var deletionResult = DeleteSingleNode(graph, node, validation.NodeIdentifier);
                if (!deletionResult.Success)
                {
                    return ToolUtils.CreateErrorResponse($"Failed to delete StepNode: {deletionResult.ErrorInfo}");
                }

                // Save changes
                ToolUtils.SaveGraphChanges(graph);

                return CreateDeleteNodeSuccessResponse(deletionResult.NodeInfo, validation.GraphPath, validation.IdentifierType, validation.NodeIdentifier);
            }
            catch (Exception ex)
            {
                return ToolUtils.CreateErrorResponse($"Failed to delete StepNode: {ex.Message}");
            }
        }

        private static (bool IsValid, string GraphPath, string NodeIdentifier, string IdentifierType, string ErrorMessage) ValidateDeleteNodeArgs(JObject args)
        {
            string graphPath = args?["graphPath"]?.ToString();
            string nodeIdentifier = args?["nodeIdentifier"]?.ToString();
            string identifierType = args?["identifierType"]?.ToString() ?? "name";

            if (string.IsNullOrEmpty(graphPath))
            {
                return (false, null, null, null, "Missing required argument: graphPath");
            }

            if (string.IsNullOrEmpty(nodeIdentifier))
            {
                return (false, null, null, null, "Missing required argument: nodeIdentifier");
            }

            return (true, graphPath, nodeIdentifier, identifierType, null);
        }

        private static object CreateDeleteNodeSuccessResponse(object nodeInfo, string graphPath, string identifierType, string nodeIdentifier)
        {
            return new {
                success = true,
                message = "StepNode deleted successfully",
                graphPath = graphPath,
                deletedNode = nodeInfo,
                timestamp = System.DateTime.Now.ToString()
            };
        }

        public static object DeleteMultipleNodes(JObject args)
        {
            // Validate input arguments
            var validation = ValidateDeleteMultipleArgs(args);
            if (!validation.IsValid)
            {
                return ToolUtils.CreateErrorResponse(validation.ErrorMessage);
            }

            try
            {
                // Load and validate graph
                var graph = ToolUtils.LoadNodeGraph(validation.GraphPath);
                if (graph == null)
                {
                    return ToolUtils.CreateErrorResponse($"Could not load NodeGraph at path: {validation.GraphPath}");
                }

                // Process node deletions
                var result = ProcessNodeDeletions(graph, validation.NodeIdentifiers, validation.IdentifierType);

                // Save changes if any nodes were deleted
                if (result.DeletedCount > 0)
                {
                    ToolUtils.SaveGraphChanges(graph);
                }

                return CreateSuccessResponse(result, validation.GraphPath);
            }
            catch (Exception ex)
            {
                return ToolUtils.CreateErrorResponse($"Failed to delete multiple nodes: {ex.Message}");
            }
        }

        private static (bool IsValid, string GraphPath, string[] NodeIdentifiers, string IdentifierType, string ErrorMessage) ValidateDeleteMultipleArgs(JObject args)
        {
            string graphPath = args?["graphPath"]?.ToString();
            JArray nodeIdentifiersArray = args?["nodeIdentifiers"] as JArray;
            string identifierType = args?["identifierType"]?.ToString() ?? "name";

            if (string.IsNullOrEmpty(graphPath))
            {
                return (false, null, null, null, "Missing required argument: graphPath");
            }

            if (nodeIdentifiersArray == null || nodeIdentifiersArray.Count == 0)
            {
                return (false, null, null, null, "Missing or empty required argument: nodeIdentifiers");
            }

            var nodeIdentifiers = nodeIdentifiersArray.Select(token => token.ToString()).ToArray();
            return (true, graphPath, nodeIdentifiers, identifierType, null);
        }



        private static (int DeletedCount, int FailedCount, List<object> DeletedNodes, List<object> FailedNodes) ProcessNodeDeletions(NodeGraph graph, string[] nodeIdentifiers, string identifierType)
        {
            var deletedNodes = new List<object>();
            var failedNodes = new List<object>();

            foreach (string nodeIdentifier in nodeIdentifiers)
            {
                var node = ToolUtils.FindNodeByIdentifier(graph, nodeIdentifier, identifierType);
                
                if (node != null)
                {
                    var deletionResult = DeleteSingleNode(graph, node, nodeIdentifier);
                    if (deletionResult.Success)
                    {
                        deletedNodes.Add(deletionResult.NodeInfo);
                    }
                    else
                    {
                        failedNodes.Add(deletionResult.ErrorInfo);
                    }
                }
                else
                {
                    failedNodes.Add(new {
                        identifier = nodeIdentifier,
                        error = $"StepNode not found with {identifierType}: {nodeIdentifier}"
                    });
                }
            }

            return (deletedNodes.Count, failedNodes.Count, deletedNodes, failedNodes);
        }



        private static (bool Success, object NodeInfo, object ErrorInfo) DeleteSingleNode(NodeGraph graph, Node node, string nodeIdentifier)
        {
            try
            {
                string nodeName = node.name;
                int nodeId = node.GetInstanceID();
                
                graph.RemoveNode(node);
                
                // Remove the StepNode as a sub-asset from the AssetDatabase
                if (AssetDatabase.Contains(node))
                {
                    AssetDatabase.RemoveObjectFromAsset(node);
                }
                
                var nodeInfo = new {
                    name = nodeName,
                    id = nodeId,
                    identifier = nodeIdentifier
                };
                
                return (true, nodeInfo, null);
            }
            catch (Exception ex)
            {
                var errorInfo = new {
                    identifier = nodeIdentifier,
                    error = ex.Message
                };
                
                return (false, null, errorInfo);
            }
        }



        private static object CreateSuccessResponse((int DeletedCount, int FailedCount, List<object> DeletedNodes, List<object> FailedNodes) result, string graphPath)
        {
            return new {
                success = true,
                message = $"Deleted {result.DeletedCount} StepNode(s), {result.FailedCount} failed",
                graphPath = graphPath,
                deletedNodes = result.DeletedNodes.ToArray(),
                failedNodes = result.FailedNodes.ToArray(),
                successCount = result.DeletedCount,
                failureCount = result.FailedCount,
                timestamp = System.DateTime.Now.ToString()
            };
        }

        public static object SetNodePosition(JObject args)
        {
            // Validate input arguments
            var validation = ValidateSetNodePositionArgs(args);
            if (!validation.IsValid)
            {
                return ToolUtils.CreateErrorResponse(validation.ErrorMessage);
            }

            try
            {
                // Load and validate graph
                var graph = ToolUtils.LoadNodeGraph(validation.GraphPath);
                if (graph == null)
                {
                    return ToolUtils.CreateErrorResponse($"Could not load NodeGraph at path: {validation.GraphPath}");
                }

                // Find the StepNode
                var node = ToolUtils.FindNodeByIdentifier(graph, validation.NodeId, "id");
                if (node == null)
                {
                    return ToolUtils.CreateErrorResponse($"Could not find StepNode with ID: {validation.NodeId}");
                }

                // Set position
                SetNodePosition(node, validation.PositionX, validation.PositionY);

                // Save changes
                ToolUtils.SaveGraphChanges(graph);
                EditorUtility.SetDirty(node);

                return CreateSetNodePositionSuccessResponse(validation.GraphPath, validation.NodeId, validation.PositionX, validation.PositionY);
            }
            catch (Exception ex)
            {
                return ToolUtils.CreateErrorResponse($"Failed to set StepNode position: {ex.Message}");
            }
        }

        private static (bool IsValid, string GraphPath, string NodeId, float PositionX, float PositionY, string ErrorMessage) ValidateSetNodePositionArgs(JObject args)
        {
            string graphPath = args?["graphPath"]?.ToString();
            string nodeId = args?["nodeId"]?.ToString();
            float posX = args?["positionX"]?.ToObject<float>() ?? 0f;
            float posY = args?["positionY"]?.ToObject<float>() ?? 0f;

            if (string.IsNullOrEmpty(graphPath))
            {
                return (false, null, null, 0f, 0f, "Missing required argument: graphPath");
            }

            if (string.IsNullOrEmpty(nodeId))
            {
                return (false, null, null, 0f, 0f, "Missing required argument: nodeId");
            }

            return (true, graphPath, nodeId, posX, posY, null);
        }

        private static void SetNodePosition(Node node, float posX, float posY)
        {
            node.position = new Vector2(posX, posY);
        }

        private static object CreateSetNodePositionSuccessResponse(string graphPath, string nodeId, float posX, float posY)
        {
            return new {
                success = true,
                message = "StepNode position set",
                graphPath = graphPath,
                nodeId = nodeId,
                position = new { x = posX, y = posY },
                timestamp = System.DateTime.Now.ToString()
            };
        }
        public static object HandleListAvailableNodeTypes(JObject args)
        {
            try
            {
                var nodeTypes = ToolUtils.GetAvailableNodeTypes();
                return CreateListNodeTypesSuccessResponse(nodeTypes);
            }
            catch (Exception ex)
            {
                return ToolUtils.CreateErrorResponse($"Failed to list StepNode types: {ex.Message}");
            }
        }



        private static object CreateListNodeTypesSuccessResponse(object[] nodeTypes)
        {
            return new
            {
                success = true,
                message = "Retrieved available StepNode types",
                nodeTypes = nodeTypes,
                count = nodeTypes.Length,
                timestamp = System.DateTime.Now.ToString()
            };
        }

        // Args:
        // - graphPath: string (required) → path to a StepsGraph asset
        // - nodeName: string (optional)  → exact node name in the graph (e.g., "ClickStep_-123")
        // - nodeId: int (optional)       → InstanceID of the node
        public static object SetNodeAsFirstStep(JObject args)
        {
            try
            {
                // Validate graph path
                string graphPath = args?["graphPath"]?.ToString();
                var graphValidation = ToolUtils.ValidateGraphPath(graphPath);
                if (!graphValidation.IsValid)
                {
                    return ToolUtils.CreateErrorResponse(graphValidation.ErrorMessage);
                }

                var graph = graphValidation.Graph;

                if (graph.nodes == null || graph.nodes.Count == 0)
                {
                    return ToolUtils.CreateErrorResponse("Graph contains no StepNodes to assign as first step");
                }

                // Find target StepNode
                var targetNode = FindTargetNode(graph, args);
                if (targetNode == null)
                {
                    return ToolUtils.CreateErrorResponse("Could not resolve a target StepNode in the graph");
                }

                // Set first step StepNode
                if (!SetFirstStepNode(graph, targetNode))
                {
                    return ToolUtils.CreateErrorResponse("Graph does not have a 'firstStep' property (is it a StepsGraph?)");
                }

                // Save changes
                ToolUtils.SaveGraphChanges(graph);

                return CreateSetFirstStepSuccessResponse(targetNode, graphPath);
            }
            catch (Exception ex)
            {
                return ToolUtils.CreateErrorResponse($"Failed to set first step: {ex.Message}");
            }
        }
        public static bool SetFirstStepNode(NodeGraph graph, Node targetNode)
        {
            try
            {
                SerializedObject so = new SerializedObject(graph);
                SerializedProperty firstStepProp = so.FindProperty("firstStep");
                if (firstStepProp == null)
                {
                    return false;
                }

                firstStepProp.objectReferenceValue = targetNode;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(graph);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to set first step StepNode: {ex.Message}");
                return false;
            }
        }


        // Args:
        // - graphPath: string (required) → path to a StepsGraph asset
        public static object ListGraphNodes(JObject args)
        {
            try
            {
                // Validate graph path
                string graphPath = args?["graphPath"]?.ToString();
                var graphValidation = ToolUtils.ValidateGraphPath(graphPath);
                if (!graphValidation.IsValid)
                {
                    return ToolUtils.CreateErrorResponse(graphValidation.ErrorMessage);
                }

                var graph = graphValidation.Graph;

                if (graph.nodes == null || graph.nodes.Count == 0)
                {
                    return new
                    {
                        success = true,
                        message = "Graph contains no StepNodes",
                        data = new { nodes = new List<object>() },
                        timestamp = System.DateTime.Now.ToString()
                    };
                }

                // Get StepNodes information
                var nodesList = ToolUtils.GetGraphNodesInfo(graph);

                return new
                {
                    success = true,
                    message = $"Found {nodesList.Count} StepNodes in graph",
                    data = new { 
                        nodes = nodesList,
                        graphPath = graphPath,
                        totalStepNodes = nodesList.Count
                    },
                    timestamp = System.DateTime.Now.ToString()
                };
            }
            catch (Exception ex)
            {
                return ToolUtils.CreateErrorResponse($"Failed to list graph StepNodes: {ex.Message}");
            }
        }



        private static object CreateSetFirstStepSuccessResponse(Node targetNode, string graphPath)
        {
            return new
            {
                success = true,
                message = $"Assigned '{targetNode.name}' as first step",
                graphPath = graphPath,
                assignedNodeName = targetNode.name,
                assignedNodeId = targetNode.GetInstanceID(),
                timestamp = System.DateTime.Now.ToString()
            };
        }

        private static Node FindTargetNode(NodeGraph graph, JObject args)
        {
            string nodeName = args?["nodeName"]?.ToString();
            int? nodeId = args?["nodeId"]?.ToObject<int?>();

            // Try to find by name first
            if (!string.IsNullOrEmpty(nodeName))
            {
                return graph.nodes.FirstOrDefault(n => n != null && n.name == nodeName);
            }

            // Try to find by ID
            if (nodeId.HasValue)
            {
                return graph.nodes.FirstOrDefault(n => n != null && n.GetInstanceID() == nodeId.Value);
            }

            // No selector provided, choose the first node of a class name ending with "Step" if possible, else first non-null
            return graph.nodes.FirstOrDefault(n => n != null && n.GetType().Name.EndsWith("Step", StringComparison.Ordinal))
                ?? graph.nodes.FirstOrDefault(n => n != null);
        }

    }
}