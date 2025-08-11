using UnityEngine;
using UnityEditor;
using XNode;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GraphNodeAdder", menuName = "Praxilabs/Tools/Graph Node Adder")]
public class GraphNodeAdder : ScriptableObject
{
    [Header("Graph Reference")]
    [SerializeField] private StepsGraph targetGraph;
    
    [Header("Node Settings")]
    [SerializeField] private Vector2 nodePosition = Vector2.zero;
    [SerializeField] private string stepId = "";
    
    [Header("Position Helpers")]
    [SerializeField] private bool useRandomPosition = false;
    [SerializeField] private Vector2 randomRangeMin = new Vector2(-300, -300);
    [SerializeField] private Vector2 randomRangeMax = new Vector2(300, 300);
    [SerializeField] private float gridSpacing = 150f;
    
    [Header("Click Step Settings")]
    [SerializeField] private string tooltip = "Click on the target object";
    
    /// <summary>
    /// Adds a ClickStep node to the target graph
    /// </summary>
    [ContextMenu("Add Click Step")]
    public void AddClickStep()
    {
        if (targetGraph == null)
        {
            Debug.LogError("Target graph is not assigned!");
            return;
        }
        
#if UNITY_EDITOR
        // Record undo for the graph
        Undo.RecordObject(targetGraph, "Add Click Step");
        
        // Create the ClickStep node
        ClickStep clickStep = targetGraph.AddNode<ClickStep>();
        
        if (clickStep != null)
        {
            // Set position
            clickStep.position = nodePosition;
            
            // Set step ID (generate unique if empty)
            if (string.IsNullOrEmpty(stepId))
            {
                clickStep.stepId = GenerateUniqueStepId();
            }
            else
            {
                clickStep.stepId = stepId;
            }
            
            // Set tooltip
            clickStep.tooltip = tooltip;
            
            // Register the created node for undo
            Undo.RegisterCreatedObjectUndo(clickStep, "Add Click Step");
            
            // Add to asset database if the graph is an asset
            string assetPath = AssetDatabase.GetAssetPath(targetGraph);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.AddObjectToAsset(clickStep, targetGraph);
                AssetDatabase.SaveAssets();
            }
            
            // Mark the graph as dirty
            EditorUtility.SetDirty(targetGraph);
            
            Debug.Log($"Successfully added ClickStep with ID: {clickStep.stepId} at position {nodePosition}");
        }
        else
        {
            Debug.LogError("Failed to create ClickStep node!");
        }
#else
        Debug.LogWarning("AddClickStep can only be used in the Unity Editor!");
#endif
    }
    
    /// <summary>
    /// Adds a ClickStep node with custom parameters
    /// </summary>
    /// <param name="position">Position of the node in the graph</param>
    /// <param name="customStepId">Custom step ID (optional)</param>
    /// <param name="customTooltip">Custom tooltip (optional)</param>
    /// <returns>The created ClickStep node</returns>
    public ClickStep AddClickStep(Vector2 position, string customStepId = "", string customTooltip = "")
    {
        if (targetGraph == null)
        {
            Debug.LogError("Target graph is not assigned!");
            return null;
        }
        
#if UNITY_EDITOR
        // Record undo for the graph
        Undo.RecordObject(targetGraph, "Add Click Step");
        
        // Create the ClickStep node
        ClickStep clickStep = targetGraph.AddNode<ClickStep>();
        
        if (clickStep != null)
        {
            // Set position
            clickStep.position = position;
            
            // Set step ID
            if (string.IsNullOrEmpty(customStepId))
            {
                clickStep.stepId = GenerateUniqueStepId();
            }
            else
            {
                clickStep.stepId = customStepId;
            }
            
            // Set tooltip
            clickStep.tooltip = string.IsNullOrEmpty(customTooltip) ? tooltip : customTooltip;
            
            // Register the created node for undo
            Undo.RegisterCreatedObjectUndo(clickStep, "Add Click Step");
            
            // Add to asset database if the graph is an asset
            string assetPath = AssetDatabase.GetAssetPath(targetGraph);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.AddObjectToAsset(clickStep, targetGraph);
                AssetDatabase.SaveAssets();
            }
            
            // Mark the graph as dirty
            EditorUtility.SetDirty(targetGraph);
            
            Debug.Log($"Successfully added ClickStep with ID: {clickStep.stepId} at position {position}");
        }
        
        return clickStep;
#else
        Debug.LogWarning("AddClickStep can only be used in the Unity Editor!");
        return null;
#endif
    }
    
    /// <summary>
    /// Generates a unique step ID for the target graph
    /// </summary>
    /// <returns>Unique step ID</returns>
    private string GenerateUniqueStepId()
    {
        if (targetGraph == null) return "s1";
        
        int maxId = 0;
        foreach (var node in targetGraph.nodes)
        {
            if (node is StepNode stepNode)
            {
                string id = stepNode.stepId;
                if (id.StartsWith("s") && int.TryParse(id.Substring(1), out int numericId))
                {
                    maxId = Mathf.Max(maxId, numericId);
                }
            }
        }
        
        return "s" + (maxId + 1);
    }
    
    /// <summary>
    /// Sets the target graph reference
    /// </summary>
    /// <param name="graph">The StepsGraph to target</param>
    public void SetTargetGraph(StepsGraph graph)
    {
        targetGraph = graph;
    }
    
    /// <summary>
    /// Gets the current target graph
    /// </summary>
    /// <returns>The current target graph</returns>
    public StepsGraph GetTargetGraph()
    {
        return targetGraph;
    }
    
    /// <summary>
    /// Gets the current node position
    /// </summary>
    /// <returns>The current node position</returns>
    public Vector2 GetNodePosition()
    {
        return nodePosition;
    }
    
    /// <summary>
    /// Sets the default position for new nodes
    /// </summary>
    /// <param name="position">Default position</param>
    public void SetDefaultPosition(Vector2 position)
    {
        nodePosition = position;
    }
    
    /// <summary>
    /// Sets the default step ID for new nodes
    /// </summary>
    /// <param name="id">Default step ID</param>
    public void SetDefaultStepId(string id)
    {
        stepId = id;
    }
    
    /// <summary>
    /// Sets the default tooltip for new nodes
    /// </summary>
    /// <param name="tooltipText">Default tooltip</param>
    public void SetDefaultTooltip(string tooltipText)
    {
        tooltip = tooltipText;
    }
    
    /// <summary>
    /// Gets the next position based on current settings
    /// </summary>
    /// <returns>Position for the next node</returns>
    public Vector2 GetNextPosition()
    {
        if (useRandomPosition)
        {
            return GetRandomPosition();
        }
        return nodePosition;
    }
    
    /// <summary>
    /// Gets a random position within the specified range
    /// </summary>
    /// <returns>Random position</returns>
    public Vector2 GetRandomPosition()
    {
        float x = Random.Range(randomRangeMin.x, randomRangeMax.x);
        float y = Random.Range(randomRangeMin.y, randomRangeMax.y);
        return new Vector2(x, y);
    }
    
    /// <summary>
    /// Gets a grid position based on the current node count
    /// </summary>
    /// <param name="index">Grid index</param>
    /// <returns>Grid position</returns>
    public Vector2 GetGridPosition(int index)
    {
        int columns = 5; // Number of columns in the grid
        int row = index / columns;
        int col = index % columns;
        
        return new Vector2(
            nodePosition.x + (col * gridSpacing),
            nodePosition.y + (row * gridSpacing)
        );
    }
    
    /// <summary>
    /// Adds multiple ClickStep nodes in a grid pattern
    /// </summary>
    /// <param name="count">Number of nodes to add</param>
    /// <param name="baseTooltip">Base tooltip text</param>
    public void AddClickStepsInGrid(int count, string baseTooltip = "")
    {
        if (targetGraph == null)
        {
            Debug.LogError("Target graph is not assigned!");
            return;
        }
        
        string tooltipToUse = string.IsNullOrEmpty(baseTooltip) ? tooltip : baseTooltip;
        
        for (int i = 0; i < count; i++)
        {
            Vector2 gridPos = GetGridPosition(i);
            string nodeTooltip = $"{tooltipToUse} {i + 1}";
            AddClickStep(gridPos, "", nodeTooltip);
        }
    }
    
    /// <summary>
    /// Adds a ClickStep node using the next calculated position
    /// </summary>
    /// <returns>The created ClickStep node</returns>
    public ClickStep AddClickStepAtNextPosition()
    {
        return AddClickStep(GetNextPosition());
    }
    
    /// <summary>
    /// Adds a UIClickStep node to the target graph
    /// </summary>
    /// <param name="position">Position of the node in the graph</param>
    /// <param name="customStepId">Custom step ID (optional)</param>
    /// <param name="customTooltip">Custom tooltip (optional)</param>
    /// <returns>The created UIClickStep node</returns>
    public UIClickStep AddUIClickStep(Vector2 position, string customStepId = "", string customTooltip = "")
    {
        if (targetGraph == null)
        {
            Debug.LogError("Target graph is not assigned!");
            return null;
        }
        
#if UNITY_EDITOR
        // Record undo for the graph
        Undo.RecordObject(targetGraph, "Add UI Click Step");
        
        // Create the UIClickStep node
        UIClickStep uiClickStep = targetGraph.AddNode<UIClickStep>();
        
        if (uiClickStep != null)
        {
            // Set position
            uiClickStep.position = position;
            
            // Set step ID
            if (string.IsNullOrEmpty(customStepId))
            {
                uiClickStep.stepId = GenerateUniqueStepId();
            }
            else
            {
                uiClickStep.stepId = customStepId;
            }
            
            // Set tooltip
            uiClickStep.tooltip = string.IsNullOrEmpty(customTooltip) ? "Click on UI button" : customTooltip;
            
            // Register the created node for undo
            Undo.RegisterCreatedObjectUndo(uiClickStep, "Add UI Click Step");
            
            // Add to asset database if the graph is an asset
            string assetPath = AssetDatabase.GetAssetPath(targetGraph);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.AddObjectToAsset(uiClickStep, targetGraph);
                AssetDatabase.SaveAssets();
            }
            
            // Mark the graph as dirty
            EditorUtility.SetDirty(targetGraph);
            
            Debug.Log($"Successfully added UIClickStep with ID: {uiClickStep.stepId} at position {position}");
        }
        
        return uiClickStep;
#else
        Debug.LogWarning("AddUIClickStep can only be used in the Unity Editor!");
        return null;
#endif
    }
    
    /// <summary>
    /// Adds a DelayStep node to the target graph
    /// </summary>
    /// <param name="position">Position of the node in the graph</param>
    /// <param name="delayTime">Delay time in seconds</param>
    /// <param name="customStepId">Custom step ID (optional)</param>
    /// <param name="customTooltip">Custom tooltip (optional)</param>
    /// <returns>The created DelayStep node</returns>
    public DelayStep AddDelayStep(Vector2 position, float delayTime = 1.0f, string customStepId = "", string customTooltip = "")
    {
        if (targetGraph == null)
        {
            Debug.LogError("Target graph is not assigned!");
            return null;
        }
        
#if UNITY_EDITOR
        // Record undo for the graph
        Undo.RecordObject(targetGraph, "Add Delay Step");
        
        // Create the DelayStep node
        DelayStep delayStep = targetGraph.AddNode<DelayStep>();
        
        if (delayStep != null)
        {
            // Set position
            delayStep.position = position;
            
            // Set step ID
            if (string.IsNullOrEmpty(customStepId))
            {
                delayStep.stepId = GenerateUniqueStepId();
            }
            else
            {
                delayStep.stepId = customStepId;
            }
            
            // Set tooltip
            delayStep.tooltip = string.IsNullOrEmpty(customTooltip) ? $"Wait for {delayTime} seconds" : customTooltip;
            
            // Set delay time using reflection since the field is private
            var timeToWaitField = typeof(DelayStep).GetField("timeToWait", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (timeToWaitField != null)
            {
                timeToWaitField.SetValue(delayStep, delayTime);
            }
            
            // Register the created node for undo
            Undo.RegisterCreatedObjectUndo(delayStep, "Add Delay Step");
            
            // Add to asset database if the graph is an asset
            string assetPath = AssetDatabase.GetAssetPath(targetGraph);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.AddObjectToAsset(delayStep, targetGraph);
                AssetDatabase.SaveAssets();
            }
            
            // Mark the graph as dirty
            EditorUtility.SetDirty(targetGraph);
            
            Debug.Log($"Successfully added DelayStep with ID: {delayStep.stepId} and delay time: {delayTime}s at position {position}");
        }
        
        return delayStep;
#else
        Debug.LogWarning("AddDelayStep can only be used in the Unity Editor!");
        return null;
#endif
    }
}