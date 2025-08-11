using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using XNodeEditor;

namespace Praxilabs.xNode.Editor
{
    [CustomNodeEditor(typeof(SubGraphStep))]
    public class SubGraphStepEditor : StepNodeEditor
    {
        private Action _clickOnGraphButtonDelegate;

        private GUIStyle _openGraphButtonStyle;
        private GUIStyle OpenGraphButtonStyle =>
                    _openGraphButtonStyle ??= StyleHelperxNode.Style(200f, 25f, "#5A72A0", false, "#FFFFFF");

        public override void OnBodyGUI(XNode.Node currentNode)
        {
            if (currentNode.isGroup) return;

            using (var so = new EditorGUI.ChangeCheckScope())
            {
                serializedObject.Update();

                OpenSubGraphButton();

                if (so.changed)
                    serializedObject.ApplyModifiedProperties();
            }

            base.OnBodyGUI(currentNode);
        }

        private void OpenSubGraphButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Open Graph", OpenGraphButtonStyle))
                OpenSubGraph();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
        }

        /// <summary>
        /// Open subgraph and add it to UI graph tabs list, 
        /// and clear the opened subGraphs list on the graph
        /// </summary>
        private void OpenSubGraph()
        {
            SubGraphStep subGraphStep = target as SubGraphStep;
            if (subGraphStep.subGraph == null) return;

            StepsGraph parentGraph = subGraphStep.graph as StepsGraph;

            if (parentGraph?.runningGraphs == null || !parentGraph.runningGraphs.Contains(subGraphStep.subGraph))
            {
                parentGraph.runningGraphs.Add(subGraphStep.subGraph);
                AddGraphTabUI(subGraphStep.subGraph);
            }

            subGraphStep.subGraph?.runningGraphs.Clear();
            subGraphStep.subGraph.isOpenedFromNode = true;
            NodeEditorWindow.Open(subGraphStep.subGraph);
        }

        /// <summary>
        /// Check if there is already a tab for the graph, if not add it
        /// </summary>
        private void AddGraphTabUI(StepsGraph graph)
        {
            Button newGraphTab = new Button { text = "Sub" };
            GraphRootPanels.graphTabs.Add(newGraphTab);

            AddGraphTabEvent(newGraphTab, graph);
            GraphRootPanels.RefreshGraphsTab();
        }

        /// <summary>
        /// opens graph if you click on the tab, and remove any tab after it
        /// </summary>
        private void AddGraphTabEvent(Button graphTab, StepsGraph graph)
        {
            _clickOnGraphButtonDelegate = () =>
            {
                if (GraphRootPanels.graphTabs.Contains(graphTab))
                {
                    int buttonIndex = GraphRootPanels.graphTabs.IndexOf(graphTab);
                    GraphRootPanels.graphTabs.RemoveRange(buttonIndex + 1, GraphRootPanels.graphTabs.Count - buttonIndex - 1);
                    GraphRootPanels.RefreshGraphsTab();

                    graph.runningGraphs.Clear();
                }

                graph.isOpenedFromNode = true;

                NodeEditorWindow.Open(graph);

                graphTab.clicked -= _clickOnGraphButtonDelegate;
            };

            graphTab.clicked += _clickOnGraphButtonDelegate;
        }
    }
}