using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphNodeAdder))]
public class GraphNodeAdderEditor : Editor
{
    private SerializedProperty targetGraphProp;
    private SerializedProperty nodePositionProp;
    private SerializedProperty stepIdProp;
    private SerializedProperty tooltipProp;
    private SerializedProperty useRandomPositionProp;
    private SerializedProperty randomRangeMinProp;
    private SerializedProperty randomRangeMaxProp;
    private SerializedProperty gridSpacingProp;
    
    private void OnEnable()
    {
        targetGraphProp = serializedObject.FindProperty("targetGraph");
        nodePositionProp = serializedObject.FindProperty("nodePosition");
        stepIdProp = serializedObject.FindProperty("stepId");
        tooltipProp = serializedObject.FindProperty("tooltip");
        useRandomPositionProp = serializedObject.FindProperty("useRandomPosition");
        randomRangeMinProp = serializedObject.FindProperty("randomRangeMin");
        randomRangeMaxProp = serializedObject.FindProperty("randomRangeMax");
        gridSpacingProp = serializedObject.FindProperty("gridSpacing");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GraphNodeAdder adder = (GraphNodeAdder)target;
        
        // Graph Reference Section
        EditorGUILayout.LabelField("Graph Reference", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(targetGraphProp);
        
        EditorGUILayout.Space();
        
        // Node Position Section
        EditorGUILayout.LabelField("Node Position Settings", EditorStyles.boldLabel);
        
        // Current position field
        EditorGUILayout.PropertyField(nodePositionProp, new GUIContent("Node Position"));
        
        // Position preset buttons
        EditorGUILayout.LabelField("Position Presets:", EditorStyles.miniLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Origin (0,0)"))
        {
            nodePositionProp.vector2Value = Vector2.zero;
        }
        if (GUILayout.Button("Center (100,100)"))
        {
            nodePositionProp.vector2Value = new Vector2(100, 100);
        }
        if (GUILayout.Button("Right (200,100)"))
        {
            nodePositionProp.vector2Value = new Vector2(200, 100);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Left (-200,100)"))
        {
            nodePositionProp.vector2Value = new Vector2(-200, 100);
        }
        if (GUILayout.Button("Top (100,200)"))
        {
            nodePositionProp.vector2Value = new Vector2(100, 200);
        }
        if (GUILayout.Button("Bottom (100,-200)"))
        {
            nodePositionProp.vector2Value = new Vector2(100, -200);
        }
        EditorGUILayout.EndHorizontal();
        
        // Random position button
        if (GUILayout.Button("Random Position"))
        {
            float x = Random.Range(-300f, 300f);
            float y = Random.Range(-300f, 300f);
            nodePositionProp.vector2Value = new Vector2(x, y);
        }
        
        EditorGUILayout.Space();
        
        // Position Helpers Section
        EditorGUILayout.LabelField("Position Helpers", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(useRandomPositionProp, new GUIContent("Use Random Position"));
        
        if (useRandomPositionProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(randomRangeMinProp, new GUIContent("Random Range Min"));
            EditorGUILayout.PropertyField(randomRangeMaxProp, new GUIContent("Random Range Max"));
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.PropertyField(gridSpacingProp, new GUIContent("Grid Spacing"));
        
        EditorGUILayout.Space();
        
        // Step Settings Section
        EditorGUILayout.LabelField("Step Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(stepIdProp, new GUIContent("Step ID (leave empty for auto-generation)"));
        
        EditorGUILayout.Space();
        
        // Click Step Settings Section
        EditorGUILayout.LabelField("Click Step Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(tooltipProp);
        
        EditorGUILayout.Space();
        
        // Action Buttons Section
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
        
        GUI.enabled = adder.GetTargetGraph() != null;
        
        if (GUILayout.Button("Add Click Step at Current Position", GUILayout.Height(30)))
        {
            adder.AddClickStep();
        }
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add UI Click Step", GUILayout.Height(25)))
        {
            adder.AddUIClickStep(adder.GetNodePosition());
        }
        if (GUILayout.Button("Add Delay Step (1s)", GUILayout.Height(25)))
        {
            adder.AddDelayStep(adder.GetNodePosition(), 1.0f);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add at Origin (0,0)"))
        {
            adder.AddClickStep(Vector2.zero);
        }
        if (GUILayout.Button("Add at Center (100,100)"))
        {
            adder.AddClickStep(new Vector2(100, 100));
        }
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Add at Next Position (Smart)"))
        {
            adder.AddClickStepAtNextPosition();
        }
        
        EditorGUILayout.Space();
        
        // Grid Addition Section
        EditorGUILayout.LabelField("Batch Operations", EditorStyles.miniLabel);
        
        EditorGUILayout.BeginHorizontal();
         if (GUILayout.Button("Add 3x3 Grid"))
         {
             adder.AddClickStepsInGrid(9, "Grid Step");
         }
         if (GUILayout.Button("Add 2x5 Grid"))
         {
             adder.AddClickStepsInGrid(10, "Grid Step");
         }
         EditorGUILayout.EndHorizontal();
         
         EditorGUILayout.BeginHorizontal();
         if (GUILayout.Button("Add 3 UI Steps"))
         {
             for (int i = 0; i < 3; i++)
             {
                 Vector2 basePos = adder.GetNodePosition();
                 Vector2 pos = new Vector2(basePos.x + (i * 150), basePos.y - 100);
                 adder.AddUIClickStep(pos, "", $"UI Step {i + 1}");
             }
         }
         if (GUILayout.Button("Add 3 Delays"))
         {
             for (int i = 0; i < 3; i++)
             {
                 Vector2 basePos = adder.GetNodePosition();
                 Vector2 pos = new Vector2(basePos.x + (i * 150), basePos.y + 100);
                 float delayTime = (i + 1) * 0.5f; // 0.5s, 1.0s, 1.5s
                 adder.AddDelayStep(pos, delayTime, "", $"Delay {delayTime}s");
             }
         }
         EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add 5 Random"))
        {
            for (int i = 0; i < 5; i++)
            {
                adder.AddClickStep(adder.GetRandomPosition(), "", $"Random Step {i + 1}");
            }
        }
        if (GUILayout.Button("Add 3 Sequential"))
         {
             for (int i = 0; i < 3; i++)
             {
                 Vector2 basePos = adder.GetNodePosition();
                 Vector2 pos = new Vector2(basePos.x + (i * 150), basePos.y);
                 adder.AddClickStep(pos, "", $"Sequential Step {i + 1}");
             }
         }
        EditorGUILayout.EndHorizontal();
        
        GUI.enabled = true;
        
        EditorGUILayout.Space();
        
        // Graph Information Section
        if (adder.GetTargetGraph() != null)
        {
            EditorGUILayout.LabelField("Graph Information", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Graph Name: {adder.GetTargetGraph().name}");
            EditorGUILayout.LabelField($"Total Nodes: {adder.GetTargetGraph().nodes.Count}");
            
            int stepNodeCount = 0;
            foreach (var node in adder.GetTargetGraph().nodes)
            {
                if (node is StepNode) stepNodeCount++;
            }
            EditorGUILayout.LabelField($"Step Nodes: {stepNodeCount}");
        }
        else
        {
            // EditorGUILayout.HelpBox("Assign a target graph to enable node addition.", MessageType.Info);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}