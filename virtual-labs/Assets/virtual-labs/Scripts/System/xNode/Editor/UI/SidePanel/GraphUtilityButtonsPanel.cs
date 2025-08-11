using UnityEngine.UIElements;
using XNodeEditor;

namespace Praxilabs.xNode.Editor
{
    public static class GraphUtilityButtonsPanel
    {
        public static VisualElement utilityPanel;
     
        private static Button _dislayNodesContentBTN;
        private static Button _verifyConnectionsBTN;
        private static Button _displayPortsBTN;

        private static bool _isNodeContentDisplayed = false;
        private static bool _isPortsDisplayed = false;

        /// <summary>
        /// set shortcut buttons UI style, and add them to shortcuts panel
        /// </summary>
        /// <param name="stepsGraph"></param>
        public static void CreateUI(StepsGraph stepsGraph)
        {
            InitializeUI();
            CreateShortCutPanel();
            SetButtonsStyle();
            AddToShortcutsPanel();
            SetButtonEvents(stepsGraph);
        }

        /// <summary>
        /// Initialize vissual elements
        /// </summary>
        public static void InitializeUI()
        {
            if (NodeEditorWindow.current.uiRoot == null)
                NodeEditorWindow.current.GetUIRoot();
            GraphRootPanels.root = NodeEditorWindow.current.uiRoot;

            utilityPanel = new VisualElement();
            
            _dislayNodesContentBTN = new Button();
            _verifyConnectionsBTN = new Button();
            _displayPortsBTN = new Button();
        }

        /// <summary>
        /// create UI by adding them to root
        /// </summary>
        private static void CreateShortCutPanel()
        {
            utilityPanel.style.display = DisplayStyle.None;
        }

        private static void SetButtonsStyle()
        {
            _dislayNodesContentBTN.text = "Show";
            UIElementsStyleHelper.SetButtonStyle(_dislayNodesContentBTN, 115, 30, LengthUnit.Pixel, LengthUnit.Pixel, "#FFF8E3", "#000000", "#FFF8E3");
            _dislayNodesContentBTN.style.marginTop = 280;
           
            _verifyConnectionsBTN.text = "Verify Connections";
            UIElementsStyleHelper.SetButtonStyle(_verifyConnectionsBTN, 120, 30, LengthUnit.Pixel, LengthUnit.Pixel, "#FFF8E3", "#000000", "#FFF8E3");
            _verifyConnectionsBTN.style.marginTop = 280;
           
            _displayPortsBTN.text = "Hide Ports";
            UIElementsStyleHelper.SetButtonStyle(_displayPortsBTN, 115, 30, LengthUnit.Pixel, LengthUnit.Pixel, "#705C53", "#FFFFFF", "#705C53");
            _displayPortsBTN.style.marginTop = 280;
        }

        private static void AddToShortcutsPanel()
        {
            utilityPanel.Add(_dislayNodesContentBTN);
            utilityPanel.Add(_verifyConnectionsBTN);
            utilityPanel.Add(_displayPortsBTN);
            utilityPanel.style.flexDirection = FlexDirection.Row;
        }

        private static void SetButtonEvents(StepsGraph stepsGraph)
        {
            DisplayNodeContent(stepsGraph);
            VerifyPortsConnections(stepsGraph);
            DisplayPorts(stepsGraph);
        }

        private static void DisplayNodeContent(StepsGraph stepsGraph)
        {
            _dislayNodesContentBTN.clicked += () =>
            {
                if (_isNodeContentDisplayed)
                    _dislayNodesContentBTN.text = "Hide";
                else
                    _dislayNodesContentBTN.text = "Show";

                _isNodeContentDisplayed = !_isNodeContentDisplayed;
                foreach (var node in stepsGraph.nodes)
                {
                    node.nodeCollapsed = _isNodeContentDisplayed;
                }
            };
        }

        private static void VerifyPortsConnections(StepsGraph stepsGraph)
        {
            _verifyConnectionsBTN.clicked += () =>
            {
                stepsGraph.VerifyConnections();
            };
        }

        private static void DisplayPorts(StepsGraph stepsGraph)
        {
            _displayPortsBTN.clicked += () =>
            {
                if (_isNodeContentDisplayed)
                    _displayPortsBTN.text = "Hide Ports";
                else
                    _displayPortsBTN.text = "Show Ports";

                _isPortsDisplayed = !_isPortsDisplayed;
                HidePortsOnNodeSelect.HidePortsOnSelect(stepsGraph, _isPortsDisplayed);
            };
        }
    }
}