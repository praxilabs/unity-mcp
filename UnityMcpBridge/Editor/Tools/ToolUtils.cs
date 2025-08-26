using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

public static class ToolUtils
{
    #region Graph Management

    /// <summary>
    /// Loads a NodeGraph from the given path
    /// </summary>
    public static NodeGraph LoadNodeGraph(string graphPath)
    {
        return AssetDatabase.LoadAssetAtPath<NodeGraph>(graphPath);
    }

    /// <summary>
    /// Validates that a graph path is provided and the graph can be loaded
    /// </summary>
    public static (bool IsValid, NodeGraph Graph, string ErrorMessage) ValidateGraphPath(string graphPath)
    {
        if (string.IsNullOrEmpty(graphPath))
        {
            return (false, null, "Missing required argument: graphPath");
        }

        var graph = LoadNodeGraph(graphPath);
        if (graph == null)
        {
            return (false, null, $"Could not load NodeGraph at path: {graphPath}");
        }

        return (true, graph, null);
    }

    /// <summary>
    /// Saves changes to a graph and refreshes the asset database
    /// </summary>
    public static void SaveGraphChanges(NodeGraph graph)
    {
        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #endregion

    #region Node Management

    /// <summary>
    /// Finds a node by identifier (name or ID)
    /// </summary>
    public static Node FindNodeByIdentifier(NodeGraph graph, string nodeIdentifier, string identifierType = "name")
    {
        if (identifierType.ToLower() == "id")
        {
            if (int.TryParse(nodeIdentifier, out int instanceId))
            {
                return graph.nodes.FirstOrDefault(n => n != null && n.GetInstanceID() == instanceId);
            }
            return null;
        }
        
        return graph.nodes.FirstOrDefault(n => n != null && n.name == nodeIdentifier);
    }

    /// <summary>
    /// Finds a node by name
    /// </summary>
    public static Node FindNodeByName(NodeGraph graph, string nodeName)
    {
        return graph.nodes.FirstOrDefault(n => n != null && n.name == nodeName);
    }

    /// <summary>
    /// Finds a node by instance ID
    /// </summary>
    public static Node FindNodeById(NodeGraph graph, int nodeId)
    {
        return graph.nodes.FirstOrDefault(n => n != null && n.GetInstanceID() == nodeId);
    }

    /// <summary>
    /// Gets the first step node from a StepsGraph
    /// </summary>
    public static Node GetFirstStepNode(NodeGraph graph)
    {
        try
        {
            SerializedObject so = new SerializedObject(graph);
            SerializedProperty firstStepProp = so.FindProperty("firstStep");
            if (firstStepProp != null)
            {
                return firstStepProp.objectReferenceValue as Node;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Could not get first step node: {ex.Message}");
        }
        return null;
    }

    #endregion

    #region Node Type Management

    /// <summary>
    /// Finds a node type by name using xNode's reflection system
    /// </summary>
    public static Type FindNodeType(string nodeTypeName)
    {
        Type[] nodeTypes = NodeEditorReflection.GetDerivedTypes(typeof(Node));
        
        return nodeTypes.FirstOrDefault(t => 
            t.Name == nodeTypeName || 
            string.Equals(t.Name, nodeTypeName, StringComparison.OrdinalIgnoreCase) ||
            t.FullName.EndsWith(nodeTypeName));
    }

    /// <summary>
    /// Gets all available node types
    /// </summary>
    public static object[] GetAvailableNodeTypes()
    {
        Type[] nodeTypes = NodeEditorReflection.GetDerivedTypes(typeof(Node));
        
        return nodeTypes
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
    }

    /// <summary>
    /// Gets the create node menu path for a node type
    /// </summary>
    public static string GetCreateNodeMenuPath(Type nodeType)
    {
        var attributes = nodeType.GetCustomAttributes(typeof(XNode.Node.CreateNodeMenuAttribute), false);
        if (attributes.Length > 0)
        {
            XNode.Node.CreateNodeMenuAttribute menuAttribute = attributes[0] as XNode.Node.CreateNodeMenuAttribute;
            return menuAttribute.menuName;
        }
        return null;
    }

    #endregion

    #region Node Information

    /// <summary>
    /// Creates node information object
    /// </summary>
    public static object CreateNodeInfo(Node node, Node firstStepNode = null)
    {
        return new
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
        };
    }

    /// <summary>
    /// Gets all nodes in a graph as information objects
    /// </summary>
    public static List<object> GetGraphNodesInfo(NodeGraph graph)
    {
        var firstStepNode = GetFirstStepNode(graph);
        var nodesList = new List<object>();
        
        foreach (var node in graph.nodes)
        {
            if (node == null) continue;
            nodesList.Add(CreateNodeInfo(node, firstStepNode));
        }
        
        return nodesList;
    }

    #endregion

    #region Response Creation

    /// <summary>
    /// Creates a standard error response
    /// </summary>
    public static object CreateErrorResponse(string errorMessage)
    {
        return new
        {
            success = false,
            error = errorMessage
        };
    }

    /// <summary>
    /// Creates a standard success response
    /// </summary>
    public static object CreateSuccessResponse(string message, object data = null)
    {
        var response = new
        {
            success = true,
            message = message,
            timestamp = System.DateTime.Now.ToString()
        };

        if (data != null)
        {
            // Use dynamic to add data property
            var dynamicResponse = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
            dynamicResponse["success"] = response.success;
            dynamicResponse["message"] = response.message;
            dynamicResponse["timestamp"] = response.timestamp;
            dynamicResponse["data"] = data;
            return dynamicResponse;
        }

        return response;
    }

    #endregion

    #region Argument Validation

    /// <summary>
    /// Validates that a required string argument is provided
    /// </summary>
    public static (bool IsValid, string Value, string ErrorMessage) ValidateRequiredString(JObject args, string key, string displayName = null)
    {
        string value = args?[key]?.ToString();
        if (string.IsNullOrEmpty(value))
        {
            return (false, null, $"Missing required argument: {displayName ?? key}");
        }
        return (true, value, null);
    }

    /// <summary>
    /// Validates that a required integer argument is provided
    /// </summary>
    public static (bool IsValid, int Value, string ErrorMessage) ValidateRequiredInt(JObject args, string key, string displayName = null)
    {
        var token = args?[key];
        if (token == null)
        {
            return (false, 0, $"Missing required argument: {displayName ?? key}");
        }

        if (int.TryParse(token.ToString(), out int value))
        {
            return (true, value, null);
        }

        return (false, 0, $"Invalid integer value for {displayName ?? key}");
    }

    /// <summary>
    /// Gets an optional float value with default
    /// </summary>
    public static float GetOptionalFloat(JObject args, string key, float defaultValue = 0f)
    {
        return args?[key]?.ToObject<float>() ?? defaultValue;
    }

    /// <summary>
    /// Gets an optional string value
    /// </summary>
    public static string GetOptionalString(JObject args, string key, string defaultValue = null)
    {
        return args?[key]?.ToString() ?? defaultValue;
    }

    #endregion
}