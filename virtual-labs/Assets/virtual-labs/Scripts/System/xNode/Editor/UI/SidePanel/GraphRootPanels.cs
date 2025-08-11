using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using XNodeEditor;

namespace Praxilabs.xNode.Editor
{
    public static class GraphRootPanels
    {
        public static VisualElement root;
        public static VisualElement expandPanelsParent;
        public static VisualElement sidePanelBackground;
        public static VisualElement tabsBackground;
        public static ScrollView tabsScrollView;

        public static List<Button> graphTabs = new List<Button>();

        public static async Task CreateSidePanel(StepsGraph currentGraph)
        {
            await Task.Delay(500);

            PrepareUIElements();
            SetUIStyle();
            CreateUI(currentGraph);
        }

        /// <summary>
        /// Initialize vissual elements
        /// </summary>
        public static void PrepareUIElements()
        {
            NodeEditorWindow.current.uiRoot.Clear();
            Debug.Log("ui cleared");

            if (NodeEditorWindow.current.uiRoot == null)
                NodeEditorWindow.current.GetUIRoot();

            root = NodeEditorWindow.current.uiRoot;

            expandPanelsParent = new VisualElement();
            sidePanelBackground = new VisualElement();
            tabsBackground = new VisualElement();

            tabsScrollView = new ScrollView();
        }

        /// <summary>
        /// set visual elements styles (size, color, flex, margin, text...etc)
        /// </summary>
        private static void SetUIStyle()
        {
            SetRootFlexDirection();
            SetTabsScrollViewStyle();
            sidePanelBackground.style.display = DisplayStyle.None;
            UIElementsStyleHelper.SetVisualElementStyle(sidePanelBackground, 25, 10, "#35353540", FlexDirection.Column);
            sidePanelBackground.style.width = 300;
            sidePanelBackground.style.marginTop = 26;
            UIElementsStyleHelper.SetVisualElementStyle(tabsScrollView, 100, 100, "#66434340", FlexDirection.Column);
            UIElementsStyleHelper.SetPadding(tabsScrollView, 3, 0, 3, 0);
            UIElementsStyleHelper.SetVisualElementStyle(tabsBackground, 100, 100, "#66434340", FlexDirection.Row);
        }

        /// <summary>
        /// set root flex-direction
        /// </summary>
        private static void SetRootFlexDirection()
        {
            root.style.flexDirection = FlexDirection.Row;
            expandPanelsParent.style.flexDirection = FlexDirection.Column;
        }

        /// <summary>
        /// set scrollview style
        /// </summary>
        private static void SetTabsScrollViewStyle()
        {
            tabsScrollView.mode = ScrollViewMode.Horizontal;

            tabsScrollView.contentContainer.style.flexDirection = FlexDirection.Row;
            tabsScrollView.contentContainer.style.height = Length.Percent(95);

            tabsScrollView.horizontalScroller.style.width = Length.Percent(100);

            Color horizontalScrollerColor = new Color();
            ColorUtility.TryParseHtmlString("#3B3030", out horizontalScrollerColor);
            tabsScrollView.horizontalScroller.slider.style.color = horizontalScrollerColor;

            tabsScrollView.horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        }

        /// <summary>
        /// create UI by adding them to root
        /// </summary>
        private static void CreateUI(StepsGraph currentGraph)
        {
            ShortcutsPanel.CreateSidePanel();
            GraphUtilityButtonsPanel.CreateUI(currentGraph);

            root.Add(ShortcutsPanel.shortcutsPanel);

            expandPanelsParent.Add(sidePanelBackground);
            expandPanelsParent.Add(GraphUtilityButtonsPanel.utilityPanel);
            root.Add(expandPanelsParent);

            sidePanelBackground.Add(tabsBackground);
            tabsBackground.Add(tabsScrollView);
        }

        /// <summary>
        /// clear tabs and add new ones to refresh
        /// </summary>
        public static void RefreshGraphsTab()
        {
            if (tabsScrollView.contentContainer.childCount > 0)
                tabsScrollView.contentContainer.Clear();

            foreach (Button graphBTN in graphTabs)
            {
                tabsScrollView.contentContainer.Add(graphBTN);
                UIElementsStyleHelper.SetButtonStyle(graphBTN, 35, 80, LengthUnit.Percent, LengthUnit.Percent, "#3B3030", "#FFFFFF", "#272829");
            }
            UIElementsStyleHelper.SetButtonStyle(graphTabs[graphTabs.Count - 1], 35, 80, LengthUnit.Percent, LengthUnit.Percent, "#2E2E2E", "#FFFFFF", "#272829");
        }
    }
}