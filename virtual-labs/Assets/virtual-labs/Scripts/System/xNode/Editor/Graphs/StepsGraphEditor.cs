using Praxilabs.xNode.Editor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using XNode;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(StepsGraph))]
public class StepsGraphEditor : NodeGraphEditor
{
    protected StepsGraph stepsGraph { get { return _stepGraph != null ? _stepGraph : _stepGraph = target as StepsGraph; } }
    private StepsGraph _stepGraph;

    private GUIStyle _graphNameStyle;
    private GUIStyle GraphNameStyle => 
        _graphNameStyle ??= StyleHelperxNode.Style(null, 30f, "#00000000", true, "#FFFFFF");

    [SerializeField] private GUIStyle _editGraphButtonStyle;
    private GUIStyle EditGraphButtonStyle =>
        _editGraphButtonStyle ??= StyleHelperxNode.Style(null, 35f, "#ECB176", false, "#000000");

    protected event Action OnSelectNode;
    protected Action _graphButtonClickDelegate;
    protected Action _clearGraphPanelsDelegate;

    protected Vector2 _previousGridPosition;
    protected float _previousZoom;
    protected bool _isPortsHidden;

    protected bool displayAll = false;
    protected bool _displayButtons = false;

    public override void OnCreate()
    {
        base.OnCreate();
        OnSelectNode += GetSelectedNode;

        if (string.IsNullOrEmpty(stepsGraph.graphID))
        {
            stepsGraph.graphID = xNodeUtility.BuildID();
            UnityEditor.EditorUtility.SetDirty(stepsGraph);
        }
    }

    public override async void OnOpen()
    {
        base.OnOpen();
        NavigateToFirstStep();

        if (_clearGraphPanelsDelegate == null)
            _clearGraphPanelsDelegate = () => GraphRootPanels.root.Clear();


        if (!stepsGraph.isOpenedFromNode)
        {
            //ButtonStyle.ClearCache();
            await GraphRootPanels.CreateSidePanel(stepsGraph);

            AddGraphTab(stepsGraph);

            NodeEditorWindow.current.OnCloseWindow += _clearGraphPanelsDelegate;
        }
        else
        {
            stepsGraph.isOpenedFromNode = false;
        }
    }

    public override void OnGUI()
    {
        GUILayout.Label(stepsGraph.name, GraphNameStyle);

        using (new EditorGUI.ChangeCheckScope())
        {
            base.OnGUI();

            ResetGraph.ResetGraphView();
            SaveGraphUpdates();
            HidePortsOnPortSelect.HidePortsOnSelect(stepsGraph, NodeEditorWindow.current.hoveredPort);

            if (Selection.activeObject is StepNode)
                OnSelectNode?.Invoke();
        }
    }

    private void SaveGraphUpdates()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.S && (e.control || e.command))
        {
            NodeEditorWindow.current.Save();
            e.Use(); // Prevent further processing of this event
        }
    }

    private void NavigateToFirstStepButton()
    {
        if (GUILayout.Button("Edit graph", EditGraphButtonStyle))
            NavigateToFirstStep();
    }

    private void NavigateToFirstStep()
    {
        StepsGraph stepsGraph = (StepsGraph)target;
        NodeEditorWindow nodeEditorWindow = NodeEditorWindow.Open(serializedObject.targetObject as XNode.NodeGraph);

        if (stepsGraph.firstStep == null)
        {
            if (stepsGraph.nodes.Count > 0)
                nodeEditorWindow.Home((Node)stepsGraph.nodes[0]);
        }
        else
            nodeEditorWindow.Home((Node)stepsGraph.firstStep);
    }

    private void AddGraphTab(StepsGraph graph)
    {
        if (GraphRootPanels.graphTabs.Count > 0)
            GraphRootPanels.graphTabs.Clear();

        Button mainGraphBTN = new Button();
        mainGraphBTN.text = "Main";

        GraphRootPanels.graphTabs.Add(mainGraphBTN);
        graph.runningGraphs.Clear();

        AddGraphButtonEvent(mainGraphBTN, graph);

        GraphRootPanels.RefreshGraphsTab();
    }

    private void AddGraphButtonEvent(Button mainGraphBTN, StepsGraph graph)
    {
        if (_graphButtonClickDelegate == null)
        {
            _graphButtonClickDelegate = () =>
            {
                graph.runningGraphs.Clear();
                OpenGraph(graph);

                mainGraphBTN.clicked -= _graphButtonClickDelegate;
            };
        }

        mainGraphBTN.clicked += _graphButtonClickDelegate;
    }

    private void OpenGraph(NodeGraph graph)
    {
        NodeEditorWindow.Open(graph);
    }

    private void GetSelectedNode()
    {
        HidePortsOnNodeSelect.activeStep = Selection.activeObject as Node;
    }

    public override void OnWindowFocusLost()
    {
        base.OnWindowFocusLost();
        ResetGraph.StorePreviousGridPosition();
    }

    /// <summary>
    /// override context menu to add extra menu items
    /// </summary>
    public override void AddContextMenuItems(GenericMenu menu, Type compatibleType = null, XNode.NodePort.IO direction = XNode.NodePort.IO.Input)
    {
        Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);

        base.AddContextMenuItems(menu, compatibleType, direction);
        AddSearchEditor(menu);

        foreach (var data in stepsGraph.globalVariables)
        {
            menu.AddItem(new GUIContent($"Add Node/{data.name}"), false, () => CreateCustomNode(data, pos));
        }
    }

    /// <summary>
    /// add node search editor to context menu item
    /// </summary>
    private void AddSearchEditor(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Search"), false, () => NodeSearchEditor.DisplaySearchWindow());
        NodeSearchEditor.TargetGraph = target;
    }

    private void CreateCustomNode(GlobalVariables data, Vector2 pos)
    {
        VariableNode node = CreateNode(typeof(VariableNode), pos) as VariableNode;

        node.Init(data); // Initialize the node with the custom data
    }
}