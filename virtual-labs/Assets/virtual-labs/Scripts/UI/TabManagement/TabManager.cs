using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs
{
    public class TabManager : MonoBehaviour 
    {
        public Action<int> OnTabChanged;

        [Header("References"), Space]
        [SerializeField] private List<TabUI> _tabs;
        [SerializeField] private List<TabPanelUI> _tabPanels;
        [Header("Color Settings"), Space]
        [SerializeField] private Color _activeTabColor;
        [SerializeField] private Color _inactiveTabColor;
        [Header("Tabs Scroll Area"), Space]
        [SerializeField] private RectTransform _tabsArea;
        [SerializeField] private float _tabsAreaMin;
        [SerializeField] private float _tabsAreaMax;
        

        [SerializeField, Space] private bool _shouldSetupOnEnable;
        private Button _activeTabBtn;
        private TabPanelUI _activeTabPanel;
        private int _activeTabIndex;
        private bool _isInitializedOnEnable;

        public List<TabUI> Tabs => _tabs;
        public List<TabUI> TabPanels => _tabs;

        private void OnEnable() 
        {
            if(_isInitializedOnEnable)
            {
                UpdateDisplay(0);
            }
        }

        private void Start() 
        {
            if(_shouldSetupOnEnable & !_isInitializedOnEnable)
            {
                SetupInstantly();
            }
            else
            {
                InitializeTabs();
            }
        }

        private void SetupInstantly()
        {
            InitializeTabs();
            _isInitializedOnEnable = true;
            UpdateDisplay(0);
        }
        

        private void InitializeTabs()
        {
            // Initialize Tabs
            for (int i = 0; i < _tabs.Count; i++)
            {
                TabUI tab = _tabs[i];
                int index = i;
                tab.Btn.onClick.AddListener(() => OnTabSelected(index));
            }
        }

        public void UpdateDisplay(int index)
        {
            if(_activeTabPanel == _tabPanels[index]) return;

            if(_activeTabPanel != null)
                SetTabState(_activeTabBtn, _activeTabPanel, false);

            // Set active tab and page
            _activeTabBtn = _tabs[index].Btn;
            _activeTabPanel = _tabPanels[index];
            _activeTabIndex = index;
            SetTabState(_activeTabBtn, _activeTabPanel, true);
        }

        public void UnselectAll()
        {
            for(int i = 0; i < _tabs.Count; i++)
            {
                SetTabState(_tabs[i].Btn, _tabPanels[i], false);
            }
        }

        public void EnforceTabSelection(TabUI tabUI)
        {
            for(int i = 0; i < _tabs.Count; i++)
            {
                if(tabUI == _tabs[i])
                {
                    OnTabSelected(i);
                    UpdateTabsArea(i);
                    return;
                }
            }
            Debug.LogWarning($"Couldn't find the required TabUI: {tabUI}");
        }

        public void UpdateTabsArea()
        {
            UpdateTabsArea(_activeTabIndex);
        }

        private void OnTabSelected(int index)
        {
            UpdateDisplay(index);
            OnTabChanged?.Invoke(index);
        }

        private void SetTabState(Button tabBtn, TabPanelUI tabPanel, bool isActive)
        {
            UpdateTabColor(tabBtn, isActive);
            SetTabPanelVisibility(tabPanel, isActive);
        }

        private void UpdateTabColor(Button tabBtn, bool isActive)
        {
            tabBtn.GetComponent<Image>().color = isActive ? _activeTabColor : _inactiveTabColor;
        }

        private void SetTabPanelVisibility(TabPanelUI tabPanel, bool isActive)
        {
            tabPanel.gameObject.SetActive(isActive);
        }

        private void UpdateTabsArea(int index)
        {
            if(_tabsArea == null) return;

            int tabsViewPositionThreshold = (Tabs.Count / 2) - 1;

            if(index > tabsViewPositionThreshold)
            {
                _tabsArea.anchoredPosition = new Vector2(_tabsAreaMax, _tabsArea.anchoredPosition.y);
            }
            else
            {
                _tabsArea.anchoredPosition = new Vector2(_tabsAreaMin, _tabsArea.anchoredPosition.y);
            }
        }
    }
}