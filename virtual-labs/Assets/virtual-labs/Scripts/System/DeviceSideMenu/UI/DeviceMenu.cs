using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    [RequireComponent(typeof(TabManager))]
    public class DeviceMenu : MonoBehaviour
    {
        public event Action<bool> OnTryingVisibilityChange;
        public event Action<bool> OnVisibilityChanged;
        public event Action UnregisterEventsDelegate;

        [field: SerializeField, Header("References"), Space] public List<TabContent> TabContents { get; private set; }
        [SerializeField] private TextMeshProUGUI _deviceText;
        [SerializeField] private RectTransform _panelRectTransform;
        [SerializeField] private IconRotator _pinIconRotator;
        [SerializeField] private TabManager _tabManager;

        #region Visualizing Dynamic Components - No Logic
        [Header("Visualizing Dynamic Components - Easily Access Them"), Space]
        [HideInInspector, SerializeField] private List<GameObject> _readingsComponents;
        [HideInInspector, SerializeField] private List<GameObject> _controlsComponents;
        #endregion

        private List<TabSideMenuUI> _tabs = new List<TabSideMenuUI>();
        private List<TabSideMenuUI> _allowedTabs = new List<TabSideMenuUI>();
        private TabContent _activeTabContent;
        private GameObject _panel;
        private float _defaultPanelHeight;
        private float _minExpandableHeight;
        private float _maxExpandableHeight = 515f;
        private bool _isPinned;
        private Vector3 _latestPanelPosition;
        private Vector3 _externalButtonPosition;
        private bool _isExternalButtonOpened;
        [SerializeField] private TabType _desiredTypes;

        private void Awake()
        {
            InitializePanelSettings();
            InitializeTabContentsListeners();

            InitializeTabs();

            if (UnregisterEventsDelegate == null)
                UnregisterEventsDelegate = UnregisterEvents;

            ExperimentManager.OnStageEnd += UnregisterEvents;
        }

        private void Start()
        {
            Setup();
            _tabManager.OnTabChanged += HandleTabChanged;
            DeviceMenuWrapper.Instance.RegisterDeviceMenu(this);
        }

        private void InitializePanelSettings()
        {
            _panel = _panelRectTransform.gameObject;
            _defaultPanelHeight = _panelRectTransform.rect.height;
            _minExpandableHeight = _defaultPanelHeight / 2;
        }

        private void InitializeTabContentsListeners()
        {
            for(int i = 0; i < TabContents.Count; i++)
            {
                TabContents[i].OnHeightChanged += HandlePageHeightChanged;
            }
        }

        private void InitializeTabs()
        {
            for(int i = 0; i < _tabManager.Tabs.Count; i++)
            {
                TabSideMenuUI tabSideMenuUI = _tabManager.Tabs[i] as TabSideMenuUI;
                _tabs.Add(tabSideMenuUI);
            }
        }

        private void Setup()
        {
            SetDeviceSideMenuVisibility(true);

            // Show the desired tabs and disable the others
            foreach(TabType tabType in Enum.GetValues(typeof(TabType)))
            {
                TabSideMenuUI tab = GetTabUI(tabType);
                bool isVisible = tabType != TabType.None && (_desiredTypes & tabType) == tabType;
                TabVisibility(tab, isVisible);
                
                if(isVisible)
                    _allowedTabs.Add(tab);
            }

            // Unselect all
            _tabManager.UnselectAll();

            // Set default tab content
            TabSideMenuUI tabUI = _allowedTabs[0];
            int initialTabIndex = _tabs.IndexOf(tabUI);
            _tabManager.UpdateDisplay(initialTabIndex);

            SetDeviceSideMenuVisibility(false);
        }

        public void InitializeMenu(string deviceName, TabType desiredTypes)
        {
            _desiredTypes = desiredTypes;
            UpdateDeviceName(deviceName);
        }

        public void UpdateDeviceName(string deviceName)
        {
            _deviceText.text = deviceName;
        }

        #region Visualizing Dynamic Components - No Logic
        public void StoreReadingsComponents(List<GameObject> readingsComponents)
        {
            _readingsComponents = new List<GameObject>(readingsComponents);
        }
        public void StoreControlsComponents(List<GameObject> controlsComponents)
        {
            _controlsComponents = new List<GameObject>(controlsComponents);
        }

        public List<ReadingsComponentUI> GetReadingsComponents()=> _readingsComponents.Select(x => x.GetComponent<ReadingsComponentUI>()).ToList();
        public List<ControlsComponentUI> GetControlsComponents()=> _controlsComponents.Select(x => x.GetComponent<ControlsComponentUI>()).ToList();
        #endregion

        public TabContent GetTabContent(TabType tabType)
        {
            for(int i = 0; i < TabContents.Count; i++)
            {
                if((tabType & TabContents[i].TabType) == TabContents[i].TabType)
                {
                    return TabContents[i];
                }
            }
            Debug.LogWarning($"{tabType} <- the required tab type is not found, returning the first tab content");
            return TabContents[0];
        }

        public void SetDeviceSideMenuVisibility(bool isVisible, TabType? tabType = null)
        {
            OnTryingVisibilityChange?.Invoke(isVisible);
            if(!_isPinned)
            {
                if(tabType != null)
                {
                    TabType selectedTabType = (TabType)tabType;
                    EnforceTabSelection(selectedTabType);
                }
                    
                TogglePanel(isVisible);
            }
        }

        public void SetDeviceMenuPosition(Vector2 position)
        {
            _panelRectTransform.anchoredPosition = position;
        }

        public void SetPin(bool isPinned)
        {
            _isPinned = isPinned;
            _pinIconRotator.RotateIcon(isPinned);
        }

        public void ForceDeviceMenuVisibility(bool isVisible)
        {
            SetPin(isVisible);
            SetDeviceSideMenuVisibility(isVisible);
        }

        public void EnforceTabSelection(TabType tabType)
        {
            TabSideMenuUI tab = GetTabUI(tabType);
            if(IsTabAllowed(tab))
                _tabManager.EnforceTabSelection(tab);
        }

        public void SetExternalButtonPosition(Vector3 externalButtonPosition)
        {
            _externalButtonPosition = externalButtonPosition;
        }

        public void SetExternalButtonState(bool isOpened)
        {
            _isExternalButtonOpened = isOpened;
        }

        public void ExpandWindow()
        {
            ExpandWindowAnimation();
        }
        
        // Collapse button invokes this function
        public void CollapseWindow()
        {
            SetPin(false);
            if(_isExternalButtonOpened)
                CollapseWindowAnimation();
            else
                CloseWindowInstantly();
        }

        private void ExpandWindowAnimation()
        {
            OpenWindowInstantly();
            Sequence sequence = DOTween.Sequence();

            sequence.Append(_panel.transform.DOMove(_latestPanelPosition, 0.3f))
                .Join(_panel.transform.DOScale(Vector3.one, 0.3f))
                .SetEase(Ease.InCirc)
                .OnComplete(() =>
                {
                    _tabManager.UpdateTabsArea();
                    sequence.Kill();
                });
        }

        private void CollapseWindowAnimation()
        {
            _latestPanelPosition = _panel.transform.position;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_panel.transform.DOMove(_externalButtonPosition, 0.3f))
                .Join(_panel.transform.DOScale(Vector3.zero, 0.3f))
                .SetEase(Ease.InCirc)
                .OnComplete(() =>
                {
                    CloseWindowInstantly();
                    sequence.Kill();
                });
        }

        private void TogglePanel(bool isVisible)
        {
            if(isVisible)
                OpenWindowAnimation();
            else
            {
                if(IsDeviceMenuVisible() && _latestPanelPosition != Vector3.zero)
                    _latestPanelPosition = _panel.transform.position;
                CloseWindowInstantly();
            }
        }

        private void OpenWindowAnimation()
        {
            if(_panel == null)
            {
                OpenWindowInstantly();
                return;
            }

            _panel.transform.localScale = Vector3.zero;

            OpenWindowInstantly();

            if(_latestPanelPosition != Vector3.zero)
            {
                _panel.transform.position = _latestPanelPosition;
            }

            _panel.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack);
        }

        private void OpenWindowInstantly()
        {
            if(_panel == null)
                _panel = _panelRectTransform.gameObject;   
            _panel.SetActive(true);
            OnVisibilityChanged?.Invoke(true);
        }

        private void CloseWindowInstantly()
        {
            if(_panel == null)
                _panel = _panelRectTransform.gameObject;   
            _panel.SetActive(false);
            OnVisibilityChanged?.Invoke(false);
        }

        private bool IsDeviceMenuVisible()
        {
            return _panel.activeInHierarchy;
        }

        private void HandleTabChanged(int index)
        {
            _activeTabContent = TabContents[index];

            // Adjust panel height based on the active tab page's height
            float pageHeight = _activeTabContent.GetPageHeight();
            AdjustPanelHeight(pageHeight, _activeTabContent);
        }

        private void HandlePageHeightChanged(TabContent tabContent, float pageHeight)
        {
            if(tabContent != _activeTabContent) return;

            AdjustPanelHeight(pageHeight, tabContent); // might add tabContent parameter to adjustPanelHeight to add scrollview to the page ////////////
        }

        private void AdjustPanelHeight(float pageHeight, TabContent tabContent)
        {
            if(_minExpandableHeight > pageHeight) // Page height doesn't exceeds the minimum panel height's threshold
            {
                // If panel height hasn't changed do nothing
                if(_panelRectTransform.rect.height == _defaultPanelHeight) return;

                // set the panel height to the default value
                _panelRectTransform.sizeDelta = new Vector2(_panelRectTransform.sizeDelta.x, _defaultPanelHeight);
            }
            else if(_maxExpandableHeight < pageHeight) // Page height exceeds the maximum panel height's threshold
            {
                // set the panel height to the maximum value
                float extensionValue = _minExpandableHeight + _maxExpandableHeight;
                _panelRectTransform.sizeDelta = new Vector2(_panelRectTransform.sizeDelta.x, extensionValue);
            }
            else // Page height exceeds the minimum panel height's threshold
            {
                // set the panel height to the desired value
                float extensionValue = _defaultPanelHeight + (pageHeight - _minExpandableHeight);
                _panelRectTransform.sizeDelta = new Vector2(_panelRectTransform.sizeDelta.x, extensionValue);
            }
        }

        private void TabVisibility(TabUI tab, bool isVisible)
        {
            tab.gameObject.SetActive(isVisible);
        }

        private bool IsTabAllowed(TabSideMenuUI tab)
        {
            return _allowedTabs.Contains(tab);
        }

        private TabSideMenuUI GetTabUI(TabType tabType)
        {
            foreach(TabSideMenuUI tab in _tabs)
            {
                if((tab.Type & tabType) == tabType)
                {
                    return tab;
                }
            }
            Debug.LogWarning($"{tabType} <- the required tab is not found, returning the first tab");
            return _tabs[0];
        }

        private void UnregisterEvents()
        {
            _tabManager.OnTabChanged -= HandleTabChanged;

            for(int i = 0; i < TabContents.Count; i++)
            {
                TabContents[i].OnHeightChanged -= HandlePageHeightChanged;
            }

            DeviceMenuWrapper.Instance.UnregisterDeviceMenu(this);
        }
    }
}

