using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using System.Collections.Generic;

namespace UnityMcpBridge.Editor.Tools
{
    public static class ManageXNodeNode
{
    public static object HandleCommand(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        string nodeTypeName = args?["nodeTypeName"]?.ToString();
        float posX = args?["positionX"]?.ToObject<float>() ?? 0f;
        float posY = args?["positionY"]?.ToObject<float>() ?? 0f;

        if (string.IsNullOrEmpty(graphPath) || string.IsNullOrEmpty(nodeTypeName))
        {
            return new
            {
                success = false,
                error = "Missing required arguments: graphPath and nodeTypeName"
            };
        }

        try
        {
            // Load the graph asset
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Find the node type
            Type nodeType = FindNodeType(nodeTypeName);
            if (nodeType == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not find node type: {nodeTypeName}"
                };
            }

            // Create the node
            Node newNode = graph.AddNode(nodeType);
            if (newNode == null)
            {
                return new
                {
                    success = false,
                    error = $"Failed to create node of type: {nodeTypeName}"
                };
            }

            // Set position
            newNode.position = new Vector2(posX, posY);

            // Give the node a proper name
            newNode.name = $"{nodeTypeName}_{newNode.GetInstanceID()}";

            // Add the node as a sub-asset to the graph (this is the critical fix!)
            if (!AssetDatabase.Contains(newNode))
            {
                AssetDatabase.AddObjectToAsset(newNode, graph);
            }

            // Mark assets as dirty and save
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(newNode);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new
            {
                success = true,
                message = $"Node '{nodeTypeName}' created in graph",
                graphPath = graphPath,
                nodeId = newNode.GetInstanceID(),
                nodeName = newNode.name,
                nodeType = nodeTypeName,
                position = new { x = posX, y = posY },
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to create node: {ex.Message}"
            };
        }
    }

    public static object DeleteNode(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        string nodeIdentifier = args?["nodeIdentifier"]?.ToString();
        string identifierType = args?["identifierType"]?.ToString() ?? "name";

        if (string.IsNullOrEmpty(graphPath) || string.IsNullOrEmpty(nodeIdentifier))
        {
            return new
            {
                success = false,
                error = "Missing required arguments: graphPath and nodeIdentifier"
            };
        }

        try
        {
            // Load the graph asset
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Find the node based on identifier type
            Node node = null;
            if (identifierType.ToLower() == "id")
            {
                // Find by instance ID
                if (int.TryParse(nodeIdentifier, out int instanceId))
                {
                    node = graph.nodes.FirstOrDefault(n => n.GetInstanceID() == instanceId);
                }
            }
            else
            {
                // Find by name (default)
                node = graph.nodes.FirstOrDefault(n => n.name == nodeIdentifier);
            }

            if (node == null)
            {
                return new {
                    success = false,
                    error = $"Could not find node with {identifierType}: {nodeIdentifier}"
                };
            }

            string deletedNodeName = node.name;
            int deletedNodeId = node.GetInstanceID();

            // Remove the node from the graph
            graph.RemoveNode(node);
            if (AssetDatabase.Contains(node))
            {
                AssetDatabase.RemoveObjectFromAsset(node);
            }

            // Mark assets as dirty and save
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new {
                success = true,
                message = "Node deleted successfully",
                graphPath = graphPath,
                deletedNode = new {
                    name = deletedNodeName,
                    id = deletedNodeId,
                    identifierType = identifierType,
                    identifier = nodeIdentifier
                },
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new {
                success = false,
                error = $"Failed to delete node: {ex.Message}"
            };
        }
    }

    public static object DeleteMultipleNodes(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        JArray nodeIdentifiersArray = args?["nodeIdentifiers"] as JArray;
        string identifierType = args?["identifierType"]?.ToString() ?? "name";

        if (string.IsNullOrEmpty(graphPath) || nodeIdentifiersArray == null || nodeIdentifiersArray.Count == 0)
        {
            return new
            {
                success = false,
                error = "Missing required arguments: graphPath and nodeIdentifiers"
            };
        }

        try
        {
            // Load the graph asset
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            var deletedNodes = new List<object>();
            var failedNodes = new List<object>();

            foreach (JToken identifierToken in nodeIdentifiersArray)
            {
                string nodeIdentifier = identifierToken.ToString();
                
                // Find the node based on identifier type
                Node node = null;
                if (identifierType.ToLower() == "id")
                {
                    if (int.TryParse(nodeIdentifier, out int instanceId))
                    {
                        node = graph.nodes.FirstOrDefault(n => n.GetInstanceID() == instanceId);
                    }
                }
                else
                {
                    node = graph.nodes.FirstOrDefault(n => n.name == nodeIdentifier);
                }

                if (node != null)
                {
                    try
                    {
                        string nodeName = node.name;
                        int nodeId = node.GetInstanceID();
                        
                        graph.RemoveNode(node);
                        
                        // Remove the asset object if it exists
                        if (AssetDatabase.Contains(node))
                        {
                            AssetDatabase.RemoveObjectFromAsset(node);
                        }
                        
                        deletedNodes.Add(new {
                            name = nodeName,
                            id = nodeId,
                            identifier = nodeIdentifier
                        });
                    }
                    catch (Exception ex)
                    {
                        failedNodes.Add(new {
                            identifier = nodeIdentifier,
                            error = ex.Message
                        });
                    }
                }
                else
                {
                    failedNodes.Add(new {
                        identifier = nodeIdentifier,
                        error = $"Node not found with {identifierType}: {nodeIdentifier}"
                    });
                }
            }

            // Mark assets as dirty and save
            if (deletedNodes.Count > 0)
            {
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return new {
                success = true,
                message = $"Deleted {deletedNodes.Count} node(s), {failedNodes.Count} failed",
                graphPath = graphPath,
                deletedNodes = deletedNodes.ToArray(),
                failedNodes = failedNodes.ToArray(),
                successCount = deletedNodes.Count,
                failureCount = failedNodes.Count,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new {
                success = false,
                error = $"Failed to delete multiple nodes: {ex.Message}"
            };
        }
    }

    public static object SetNodePosition(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        string nodeId = args?["nodeId"]?.ToString();
        float posX = args?["positionX"]?.ToObject<float>() ?? 0f;
        float posY = args?["positionY"]?.ToObject<float>() ?? 0f;

        if (string.IsNullOrEmpty(graphPath) || string.IsNullOrEmpty(nodeId))
        {
            return new
            {
                success = false,
                error = "Missing required arguments: graphPath and nodeId"
            };
        }

        try
        {
            // Load the graph asset
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Find the node
            Node node = graph.nodes.FirstOrDefault(n => n.GetInstanceID().ToString() == nodeId);
            if (node == null)
            {
                return new {
                    success = false,
                    error = $"Could not find node with ID: {nodeId}"
                };
            }

            // Set position
            node.position = new Vector2(posX, posY);

            // Mark assets as dirty and save
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(node);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new {
                success = true,
                message = "Node position set",
                graphPath = graphPath,
                nodeId = nodeId,
                position = new { x = posX, y = posY },
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new {
                success = false,
                error = $"Failed to set node position: {ex.Message}"
            };
        }
    }
    public static object HandleListAvailableNodeTypes(JObject args)
    {
        try
        {
            // Get all node types using xNode's reflection system
            Type[] nodeTypes = NodeEditorReflection.GetDerivedTypes(typeof(Node));
            
            var availableNodes = nodeTypes
                .Where(type => !type.IsAbstract)
                .Select(type => new
                {
                    typeName = type.Name,
                    fullName = type.FullName,
                    assembly = type.Assembly.GetName().Name,
                    menuPath = GetCreateNodeMenuPath(type),
                    hasCreateMenu = type.GetCustomAttributes(typeof(XNode.Node.CreateNodeMenuAttribute), false).Length > 0
                })
                .OrderBy(node => node.typeName)
                .ToArray();

            return new
            {
                success = true,
                message = "Retrieved available node types",
                nodeTypes = availableNodes,
                count = availableNodes.Length,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to list node types: {ex.Message}"
            };
        }
    }

    private static Type FindNodeType(string nodeTypeName)
    {
        // Get all node types using xNode's reflection system
        Type[] nodeTypes = NodeEditorReflection.GetDerivedTypes(typeof(Node));
        
        // First try exact match
        Type nodeType = nodeTypes.FirstOrDefault(t => t.Name == nodeTypeName);
        
        // If not found, try case-insensitive match
        if (nodeType == null)
        {
            nodeType = nodeTypes.FirstOrDefault(t => 
                string.Equals(t.Name, nodeTypeName, StringComparison.OrdinalIgnoreCase));
        }
        
        // If still not found, try matching by full name
        if (nodeType == null)
        {
            nodeType = nodeTypes.FirstOrDefault(t => t.FullName.EndsWith(nodeTypeName));
        }

        return nodeType;
    }

    private static string GetCreateNodeMenuPath(Type nodeType)
    {
        var attributes = nodeType.GetCustomAttributes(typeof(XNode.Node.CreateNodeMenuAttribute), false);
        if (attributes.Length > 0)
        {
            XNode.Node.CreateNodeMenuAttribute menuAttribute = attributes[0] as XNode.Node.CreateNodeMenuAttribute;
            return menuAttribute.menuName;
        }
        return null;
    }
    }
}using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using System.Collections.Generic;

namespace UnityMcpBridge.Editor.Tools
{
    public static class ManageXNodeNode
{
    public static object HandleCommand(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        string nodeTypeName = args?["nodeTypeName"]?.ToString();
        float posX = args?["positionX"]?.ToObject<float>() ?? 0f;
        float posY = args?["positionY"]?.ToObject<float>() ?? 0f;

        if (string.IsNullOrEmpty(graphPath) || string.IsNullOrEmpty(nodeTypeName))
        {
            return new
            {
                success = false,
                error = "Missing required arguments: graphPath and nodeTypeName"
            };
        }

        try
        {
            // Load the graph asset
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Find the node type
            Type nodeType = FindNodeType(nodeTypeName);
            if (nodeType == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not find node type: {nodeTypeName}"
                };
            }

            // Create the node
            Node newNode = graph.AddNode(nodeType);
            if (newNode == null)
            {
                return new
                {
                    success = false,
                    error = $"Failed to create node of type: {nodeTypeName}"
                };
            }

            // Set position
            newNode.position = new Vector2(posX, posY);

            // Give the node a proper name
            newNode.name = $"{nodeTypeName}_{newNode.GetInstanceID()}";

            // Add the node as a sub-asset to the graph (this is the critical fix!)
            if (!AssetDatabase.Contains(newNode))
            {
                AssetDatabase.AddObjectToAsset(newNode, graph);
            }

            // Mark assets as dirty and save
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(newNode);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new
            {
                success = true,
                message = $"Node '{nodeTypeName}' created in graph",
                graphPath = graphPath,
                nodeId = newNode.GetInstanceID(),
                nodeName = newNode.name,
                nodeType = nodeTypeName,
                position = new { x = posX, y = posY },
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to create node: {ex.Message}"
            };
        }
    }

    public static object DeleteNode(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        string nodeIdentifier = args?["nodeIdentifier"]?.ToString();
        string identifierType = args?["identifierType"]?.ToString() ?? "name";

        if (string.IsNullOrEmpty(graphPath) || string.IsNullOrEmpty(nodeIdentifier))
        {
            return new
            {
                success = false,
                error = "Missing required arguments: graphPath and nodeIdentifier"
            };
        }

        try
        {
            // Load the graph asset
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Find the node based on identifier type
            Node node = null;
            if (identifierType.ToLower() == "id")
            {
                // Find by instance ID
                if (int.TryParse(nodeIdentifier, out int instanceId))
                {
                    node = graph.nodes.FirstOrDefault(n => n.GetInstanceID() == instanceId);
                }
            }
            else
            {
                // Find by name (default)
                node = graph.nodes.FirstOrDefault(n => n.name == nodeIdentifier);
            }

            if (node == null)
            {
                return new {
                    success = false,
                    error = $"Could not find node with {identifierType}: {nodeIdentifier}"
                };
            }

            string deletedNodeName = node.name;
            int deletedNodeId = node.GetInstanceID();

            // Remove the node from the graph
            graph.RemoveNode(node);
            if (AssetDatabase.Contains(node))
            {
                AssetDatabase.RemoveObjectFromAsset(node);
            }

            // Mark assets as dirty and save
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new {
                success = true,
                message = "Node deleted successfully",
                graphPath = graphPath,
                deletedNode = new {
                    name = deletedNodeName,
                    id = deletedNodeId,
                    identifierType = identifierType,
                    identifier = nodeIdentifier
                },
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new {
                success = false,
                error = $"Failed to delete node: {ex.Message}"
            };
        }
    }

    public static object DeleteMultipleNodes(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        JArray nodeIdentifiersArray = args?["nodeIdentifiers"] as JArray;
        string identifierType = args?["identifierType"]?.ToString() ?? "name";

        if (string.IsNullOrEmpty(graphPath) || nodeIdentifiersArray == null || nodeIdentifiersArray.Count == 0)
        {
            return new
            {
                success = false,
                error = "Missing required arguments: graphPath and nodeIdentifiers"
            };
        }

        try
        {
            // Load the graph asset
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            var deletedNodes = new List<object>();
            var failedNodes = new List<object>();

            foreach (JToken identifierToken in nodeIdentifiersArray)
            {
                string nodeIdentifier = identifierToken.ToString();
                
                // Find the node based on identifier type
                Node node = null;
                if (identifierType.ToLower() == "id")
                {
                    if (int.TryParse(nodeIdentifier, out int instanceId))
                    {
                        node = graph.nodes.FirstOrDefault(n => n.GetInstanceID() == instanceId);
                    }
                }
                else
                {
                    node = graph.nodes.FirstOrDefault(n => n.name == nodeIdentifier);
                }

                if (node != null)
                {
                    try
                    {
                        string nodeName = node.name;
                        int nodeId = node.GetInstanceID();
                        
                        graph.RemoveNode(node);
                        
                        // Remove the asset object if it exists
                        if (AssetDatabase.Contains(node))
                        {
                            AssetDatabase.RemoveObjectFromAsset(node);
                        }
                        
                        deletedNodes.Add(new {
                            name = nodeName,
                            id = nodeId,
                            identifier = nodeIdentifier
                        });
                    }
                    catch (Exception ex)
                    {
                        failedNodes.Add(new {
                            identifier = nodeIdentifier,
                            error = ex.Message
                        });
                    }
                }
                else
                {
                    failedNodes.Add(new {
                        identifier = nodeIdentifier,
                        error = $"Node not found with {identifierType}: {nodeIdentifier}"
                    });
                }
            }

            // Mark assets as dirty and save
            if (deletedNodes.Count > 0)
            {
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return new {
                success = true,
                message = $"Deleted {deletedNodes.Count} node(s), {failedNodes.Count} failed",
                graphPath = graphPath,
                deletedNodes = deletedNodes.ToArray(),
                failedNodes = failedNodes.ToArray(),
                successCount = deletedNodes.Count,
                failureCount = failedNodes.Count,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new {
                success = false,
                error = $"Failed to delete multiple nodes: {ex.Message}"
            };
        }
    }

    public static object SetNodePosition(JObject args)
    {
        string graphPath = args?["graphPath"]?.ToString();
        string nodeId = args?["nodeId"]?.ToString();
        float posX = args?["positionX"]?.ToObject<float>() ?? 0f;
        float posY = args?["positionY"]?.ToObject<float>() ?? 0f;

        if (string.IsNullOrEmpty(graphPath) || string.IsNullOrEmpty(nodeId))
        {
            return new
            {
                success = false,
                error = "Missing required arguments: graphPath and nodeId"
            };
        }

        try
        {
            // Load the graph asset
            NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
            if (graph == null)
            {
                return new
                {
                    success = false,
                    error = $"Could not load NodeGraph at path: {graphPath}"
                };
            }

            // Find the node
            Node node = graph.nodes.FirstOrDefault(n => n.GetInstanceID().ToString() == nodeId);
            if (node == null)
            {
                return new {
                    success = false,
                    error = $"Could not find node with ID: {nodeId}"
                };
            }

            // Set position
            node.position = new Vector2(posX, posY);

            // Mark assets as dirty and save
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(node);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new {
                success = true,
                message = "Node position set",
                graphPath = graphPath,
                nodeId = nodeId,
                position = new { x = posX, y = posY },
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new {
                success = false,
                error = $"Failed to set node position: {ex.Message}"
            };
        }
    }
    public static object HandleListAvailableNodeTypes(JObject args)
    {
        try
        {
            // Get all node types using xNode's reflection system
            Type[] nodeTypes = NodeEditorReflection.GetDerivedTypes(typeof(Node));
            
            var availableNodes = nodeTypes
                .Where(type => !type.IsAbstract)
                .Select(type => new
                {
                    typeName = type.Name,
                    fullName = type.FullName,
                    assembly = type.Assembly.GetName().Name,
                    menuPath = GetCreateNodeMenuPath(type),
                    hasCreateMenu = type.GetCustomAttributes(typeof(XNode.Node.CreateNodeMenuAttribute), false).Length > 0
                })
                .OrderBy(node => node.typeName)
                .ToArray();

            return new
            {
                success = true,
                message = "Retrieved available node types",
                nodeTypes = availableNodes,
                count = availableNodes.Length,
                timestamp = System.DateTime.Now.ToString()
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                error = $"Failed to list node types: {ex.Message}"
            };
        }
    }

    private static Type FindNodeType(string nodeTypeName)
    {
        // Get all node types using xNode's reflection system
        Type[] nodeTypes = NodeEditorReflection.GetDerivedTypes(typeof(Node));
        
        // First try exact match
        Type nodeType = nodeTypes.FirstOrDefault(t => t.Name == nodeTypeName);
        
        // If not found, try case-insensitive match
        if (nodeType == null)
        {
            nodeType = nodeTypes.FirstOrDefault(t => 
                string.Equals(t.Name, nodeTypeName, StringComparison.OrdinalIgnoreCase));
        }
        
        // If still not found, try matching by full name
        if (nodeType == null)
        {
            nodeType = nodeTypes.FirstOrDefault(t => t.FullName.EndsWith(nodeTypeName));
        }

        return nodeType;
    }

    private static string GetCreateNodeMenuPath(Type nodeType)
    {
        var attributes = nodeType.GetCustomAttributes(typeof(XNode.Node.CreateNodeMenuAttribute), false);
        if (attributes.Length > 0)
        {
            XNode.Node.CreateNodeMenuAttribute menuAttribute = attributes[0] as XNode.Node.CreateNodeMenuAttribute;
            return menuAttribute.menuName;
        }
        return null;
    }
    }
}