using Praxilabs.xNode;
using Praxilabs.xNode.Editor;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using XNodeEditor;

public static class ShortcutsPanel
{
    public static VisualElement shortcutsPanel;

    private static Button _tabsBTN;
    private static Button _homeBTN;
    private static Button _saveBTN;
    private static Button _undoBTN;
    private static Button _redoBTN;
    private static Button _utilityShortcutsBTN;
    private static Button _settingsBTN;

    private static bool _isSidePanelDisplayed = false;
    private static bool _isShortcutsDisplayed = false;
    private static bool _buttonsStyled = false;
    private static bool _uiCreated = false;

    private static Action _tabsButtonDelegate;
    private static Action _utilityShortcutsDelegate;
    private static Action _homeDelegate;
    private static Action _saveDelegate;
    private static Action _undoDelegate;
    private static Action _redoDelegate;
    private static Action _settingsDelegate;

    public static void CreateSidePanel()
    {
        PrepareUIElements();
        SetUIStyle();
        CreateUI();
        AddEvents();
    }

    public static void PrepareUIElements()
    {
        if (NodeEditorWindow.current.uiRoot == null)
            NodeEditorWindow.current.GetUIRoot();

        GraphRootPanels.root = NodeEditorWindow.current.uiRoot;

        if (shortcutsPanel != null) return;

        shortcutsPanel = new VisualElement();

        _tabsBTN ??= new Button();
        _homeBTN ??= new Button();
        _saveBTN ??= new Button();
        _undoBTN ??= new Button();
        _redoBTN ??= new Button();
        _settingsBTN ??= new Button();
        _utilityShortcutsBTN ??= new Button();
    }

    private static void SetUIStyle()
    {
        UIElementsStyleHelper.SetVisualElementStyle(shortcutsPanel, 0, 0, "#66434380", FlexDirection.Column);

        shortcutsPanel.style.width = 50;
        shortcutsPanel.style.height = Length.Percent(100);

        shortcutsPanel.style.alignItems = Align.Center;
        SetButtonsStyle();
    }

    private static void SetButtonsStyle()
    {
        if (_buttonsStyled) return;
        _buttonsStyled = true;

        _tabsBTN.text = "Tabs";
        UIElementsStyleHelper.SetButtonStyle(_tabsBTN, 0, 0, LengthUnit.Percent, LengthUnit.Percent, "#353535", "#FFFFFF", "#FFFFFF");
        _tabsBTN.style.width = 40;
        _tabsBTN.style.height = 40;
        UIElementsStyleHelper.SetMargin(_tabsBTN, 60, 0, 0, 0);

        UIElementsStyleHelper.SetButtonStyle(_homeBTN, 0, 0, LengthUnit.Percent, LengthUnit.Percent, "#353535", "#FFFFFF", "#FFFFFF");
        UIElementsStyleHelper.SetMargin(_homeBTN, 20, 0, 0, 0);
        UIElementsStyleHelper.AddButtonIcon("home", _homeBTN);
        _homeBTN.style.width = 40;
        _homeBTN.style.height = 40;

        UIElementsStyleHelper.SetButtonStyle(_saveBTN, 0, 0, LengthUnit.Percent, LengthUnit.Percent, "#353535", "#FFFFFF", "#FFFFFF");
        UIElementsStyleHelper.SetMargin(_saveBTN, 20, 0, 0, 0);
        UIElementsStyleHelper.AddButtonIcon("save", _saveBTN);
        _saveBTN.style.width = 40;
        _saveBTN.style.height = 40;

        UIElementsStyleHelper.SetButtonStyle(_undoBTN, 0, 0, LengthUnit.Percent, LengthUnit.Percent, "#353535", "#FFFFFF", "#FFFFFF");
        UIElementsStyleHelper.SetMargin(_undoBTN, 20, 0, 0, 0);
        UIElementsStyleHelper.AddButtonIcon("undo", _undoBTN);
        _undoBTN.style.width = 40;
        _undoBTN.style.height = 40;

        UIElementsStyleHelper.SetButtonStyle(_redoBTN, 0, 0, LengthUnit.Percent, LengthUnit.Percent, "#353535", "#FFFFFF", "#FFFFFF");
        UIElementsStyleHelper.SetMargin(_redoBTN, 20, 0, 0, 0);
        UIElementsStyleHelper.AddButtonIcon("redo", _redoBTN);
        _redoBTN.style.width = 40;
        _redoBTN.style.height = 40;

        UIElementsStyleHelper.SetButtonStyle(_utilityShortcutsBTN, 0, 0, LengthUnit.Percent, LengthUnit.Percent, "#353535", "#FFFFFF", "#FFFFFF");
        UIElementsStyleHelper.SetMargin(_utilityShortcutsBTN, 20, 0, 0, 0);
        UIElementsStyleHelper.AddButtonIcon("forward_arrow", _utilityShortcutsBTN);
        _utilityShortcutsBTN.style.width = 40;
        _utilityShortcutsBTN.style.height = 40;

        UIElementsStyleHelper.SetButtonStyle(_settingsBTN, 0, 0, LengthUnit.Percent, LengthUnit.Percent, "#35353500", "#FFFFFF", "#FFFFFF00");
        UIElementsStyleHelper.AddButtonIcon("settings", _settingsBTN);
        _settingsBTN.style.position = Position.Absolute; // Absolute positioning
        _settingsBTN.style.bottom = Length.Percent(3);
        _settingsBTN.style.width = 40;
        _settingsBTN.style.height = 30;
    }

    private static void CreateUI()
    {
        if (_uiCreated) return;
        _uiCreated = true;

        shortcutsPanel.Add(_tabsBTN);
        shortcutsPanel.Add(_homeBTN);
        shortcutsPanel.Add(_saveBTN);
        shortcutsPanel.Add(_undoBTN);
        shortcutsPanel.Add(_redoBTN);
        shortcutsPanel.Add(_utilityShortcutsBTN);
        shortcutsPanel.Add(_settingsBTN);
    }

    private static void AddEvents()
    {
        RemoveAllEvents(); // first, clean previous
        FillDelegates();

        _tabsBTN.clicked += _tabsButtonDelegate;
        _utilityShortcutsBTN.clicked += _utilityShortcutsDelegate;
        _homeBTN.clicked += _homeDelegate;
        _saveBTN.clicked += _saveDelegate;
        _undoBTN.clicked += _undoDelegate;
        _redoBTN.clicked += _redoDelegate;
        _settingsBTN.clicked += _settingsDelegate;
    }

    private static void RemoveAllEvents()
    {
        _tabsBTN.clicked -= _tabsButtonDelegate;
        _utilityShortcutsBTN.clicked -= _utilityShortcutsDelegate;
        _homeBTN.clicked -= _homeDelegate;
        _saveBTN.clicked -= _saveDelegate;
        _undoBTN.clicked -= _undoDelegate;
        _redoBTN.clicked -= _redoDelegate;
        _settingsBTN.clicked -= _settingsDelegate;
    }

    private static void FillDelegates()
    {
        _tabsButtonDelegate ??= () =>
        {
            _isSidePanelDisplayed = !_isSidePanelDisplayed;

            if (!_isSidePanelDisplayed)
            {
                GraphRootPanels.sidePanelBackground.style.display = DisplayStyle.None;
            }
            else
            {
                if (GraphUtilityButtonsPanel.utilityPanel.style.display == DisplayStyle.Flex)
                {
                    _isShortcutsDisplayed = false;
                    GraphUtilityButtonsPanel.utilityPanel.style.display = DisplayStyle.None;
                    UIElementsStyleHelper.AddButtonIcon("forward_arrow", _utilityShortcutsBTN);
                }

                GraphRootPanels.sidePanelBackground.style.display = DisplayStyle.Flex;
            }
        };

        _utilityShortcutsDelegate ??= () =>
        {
            _isShortcutsDisplayed = !_isShortcutsDisplayed;

            if (!_isShortcutsDisplayed)
            {
                GraphUtilityButtonsPanel.utilityPanel.style.display = DisplayStyle.None;
                UIElementsStyleHelper.AddButtonIcon("forward_arrow", _utilityShortcutsBTN);
            }
            else
            {
                if (GraphRootPanels.sidePanelBackground.style.display == DisplayStyle.Flex)
                {
                    GraphRootPanels.sidePanelBackground.style.display = DisplayStyle.None;
                    _isSidePanelDisplayed = false;
                }

                GraphUtilityButtonsPanel.utilityPanel.style.display = DisplayStyle.Flex;
                UIElementsStyleHelper.AddButtonIcon("back_arrow", _utilityShortcutsBTN);
            }
        };

        _homeDelegate ??= () =>
        {
            StepsGraph currentGraph = NodeEditorWindow.current.graph as StepsGraph;
            NodeEditorWindow.current.Home(currentGraph.firstStep);
        };

        _saveDelegate ??= () => NodeEditorWindow.current.Save();
        _undoDelegate ??= () => Undo.PerformUndo();
        _redoDelegate ??= () => Undo.PerformRedo();
        _settingsDelegate ??=() => NodeEditorReflection.OpenPreferences();
    }
}