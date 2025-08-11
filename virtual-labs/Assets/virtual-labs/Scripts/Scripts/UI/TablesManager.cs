using LocalizationSystem;
using Newtonsoft.Json;
using SideMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using Table.UI.Views;
using Table.UI.Views.Buttons;
using UnityEngine;

namespace Table
{
    public class TablesManager : Singleton<TablesManager>
    {
        [SerializeField] private TableWindowButton _tableWindowButtonPrefab;

        [Header("Parent transforms")]
        [SerializeField] private RectTransform _tableWindowParent;
        [SerializeField] private SideMenu.SideMenu _sideMenu;
        [SerializeField] private Table.UI.SideMenuButtons.RecordReadingsButton _recordButtonPrefab;

        [SerializeField] private Dictionary<int, TableWindowButton> _stagesButtons;

        private UI.SideMenuButtons.RecordReadingsButton _recordReadingsButton;
        public StageTableEntry[] stagesAvailable;
        public UI.SideMenuButtons.RecordReadingsButton RecordReadingsButton => _recordReadingsButton;

        private SideMenuIsland _recordButtonIsland;
        private TablesWindow _lastTablesWindowActive;
        private int _lastStageEnabled;
        private bool _recordReadingsButtonCreated = false;
        private bool _initialized = false;
        private bool _isTableEnabled = false;

        //Localization Variables
        private Action _OnLanguageChangeDelegate;
        private bool _dataExists;
        private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
        public Dictionary<int, TablesData> tablesData = new Dictionary<int, TablesData>();
        public TablesData currentTablesData;

        private void OnEnable()
        {
            _OnLanguageChangeDelegate = () =>
            {
                if (!_dataExists) return;
                LoadTablesJson();
            };
            LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
        }

        private void Start()
        {
            LoadJson();
        }

        [ContextMenu("Initialize")]
        public void Initialize(int stageNumber)
        {
            if (_initialized)
                return;
            _initialized = true;

            SetupStageButtons(stageNumber);
            //SetupRecordButton();
        }

        private void SetupStageButtons(int stageNumber)
        {
            ///Setup 
            bool showNumbers = stagesAvailable.Length > 1;
            _stagesButtons = new Dictionary<int, TableWindowButton>(stagesAvailable.Length);
            for (int i = 0; i < stagesAvailable.Length; i++)
            {
                TablesWindow tableWindowInstance = Instantiate(stagesAvailable[i].tableWindow, _tableWindowParent);
                tableWindowInstance.StageNumber = stagesAvailable[i].stage;
                tableWindowInstance.TableViewPrefab = stagesAvailable[i].tableViewPrefab;
                tableWindowInstance.ShowTableSettings = stagesAvailable[i].showTableSettings;
                tableWindowInstance.TableSettingsPrefab = stagesAvailable[i].tableSettingsViewPrefab;
                TableWindowButton tableViewButton = Instantiate(_tableWindowButtonPrefab);

                tableViewButton.StageIndex = stagesAvailable[i].stage;
                tableViewButton.TablesManager = this;
                tableViewButton.TableTransitions = tableWindowInstance.GetComponent<Table.UI.Views.TableTransitions>();
                tableViewButton.TablesView = tableWindowInstance;
                tableViewButton.ShowNumber = showNumbers;
                _stagesButtons.Add(stagesAvailable[i].stage, tableViewButton);
                tableViewButton.SetPinned(false);
                tableViewButton.SetCurrentStageIndex(-1);
                tableViewButton.Initialize();
                SideMenu.SideMenuButton tableViewSideMenuButton = tableViewButton.GetComponent<SideMenu.SideMenuButton>();
                tableViewSideMenuButton.Id = $"table {stagesAvailable[i].stage}";
                tableViewSideMenuButton.SetNotification(showNumbers);
                tableViewSideMenuButton.SetHighlight(false);
                tableViewSideMenuButton.SetNotificationText(stagesAvailable[i].stage.ToString());
                tableViewSideMenuButton.SetButtonActive(false);
                tableViewSideMenuButton.transform.parent = null;
                //_sideMenu.AddButton("main", tableViewButton.GetComponent<SideMenu.SideMenuButton>());
            }

            _lastStageEnabled = 0;
        }

        [ContextMenu("Setup record button")]
        public void SetupRecordButton()
        {
            if (_recordReadingsButtonCreated)
            {
                Debug.Log("color=#E50046>Record button already created !</color>");
                return;
            }
            _recordReadingsButtonCreated = true;
            _recordButtonIsland = _sideMenu.CreateSideMenuIsland("tableRecordButton", SideMenu.SideMenu.SideMenuPosition.Top);
            _recordReadingsButton = Instantiate(_recordButtonPrefab);
            SideMenu.SideMenuButton recorderSideMenuButton = _recordReadingsButton.GetComponent<SideMenu.SideMenuButton>();
            recorderSideMenuButton.Family = "tableRecordButtons";
            recorderSideMenuButton.Id = "tableRecordButton";
            recorderSideMenuButton.SetHighlight(false);
            recorderSideMenuButton.SetNotification(false);
            _recordButtonIsland.AddButton(recorderSideMenuButton);

            _recordReadingsButton.recordLastRow.AddListener(SetLastRowData);
            _recordReadingsButton.tablesManager = this;
        }

        public void RemoveRecordButton()
        {
            _recordReadingsButtonCreated = false;
            _recordButtonIsland.RemoveButton("tableRecordButton");
            Destroy(_recordReadingsButton.gameObject);

            _sideMenu.RemoveSideMenuIsland("tableRecordButton");
        }

        public UI.SideMenuButtons.RecordReadingsButton GetRecordButton() => _recordReadingsButton;

        /// <summary>
        /// Sets last active table window
        /// </summary>
        /// <param name="tablesView"></param>
        public void SetLastActiveTableWindow(TablesWindow tablesView)
        {
            _lastTablesWindowActive = tablesView;
        }
        /// <summary>
        /// Adds empty record / row for last active table window.
        /// </summary>
        public void AddRecordForLastActive()
        {
            _lastTablesWindowActive.AddRecord();
        }
        /// <summary>
        /// Adds record with data by Key (field name) Value (field value) specified in <paramref name="data"/> dictionary
        /// </summary>
        /// <param name="data">Dictionary of Key (field name) Value (field value)</param>
        public void AddRecordForLastActive(Dictionary<string, string> data)
        {
            if (!_lastTablesWindowActive)
            {
                Debug.LogError($"last table active is null, try enabling a stage table before recording/adding a row.");
                return;
            }
            _lastTablesWindowActive.AddRecord(data);
        }

        public void SetRowData(int row, Dictionary<string, string> data)
        {
            _lastTablesWindowActive.SetRowData(row, data);
        }

        public void SetLastRowData(Dictionary<string, string> data)
        {
            _lastTablesWindowActive.SetLastRowData(data);
        }
        public void SetRecordByKeyForLastActive(string key, string value, Dictionary<string, string> data)
        {
            _lastTablesWindowActive.SetRowDataWithKey(key, value, data);
        }
        /// <summary>
        /// Enables next stage from stages array after last stage enabled with this function
        /// </summary>
        [ContextMenu("Enable new stage")]
        public void EnableNewStage()
        {
            if (_lastStageEnabled >= stagesAvailable.Length)
            {
                Debug.LogError($"All stages available already enabled !");
                return;
            }
            EnableStage(stagesAvailable[_lastStageEnabled].stage);
            //_stagesButtons[_stagesAvailable[_lastStageEnabled].Stage].SetActive(true);
            //_sideMenu.AddButton("main", _stagesButtons[_stagesAvailable[_lastStageEnabled].Stage].GetComponent<SideMenu.SideMenuButton>());

            _lastStageEnabled += 1;
        }
        /// <summary>
        /// Enables stage of number <paramref name="stageNumber"/> if exists. ignored otherwise.<br></br>
        /// If stage was passed it also creates a new table.
        /// </summary>
        /// <param name="stageNumber"></param>
        public void EnableStage(int stageNumber)
        {
            if (!_stagesButtons.ContainsKey(stageNumber))
                return;

            ///Add button
            _stagesButtons[stageNumber].gameObject.SetActive(true);
            SetLastActiveTableWindow(_stagesButtons[stageNumber].TablesView);
            if (!_sideMenu.HasId(_stagesButtons[stageNumber].GetComponent<SideMenu.SideMenuButton>().Id))
            {
                if (stageNumber > 1)
                {
                    for (int i = 1; i <= stageNumber; i++)
                        _sideMenu.AddButton("main", _stagesButtons[i].GetComponent<SideMenu.SideMenuButton>());
                }
                else
                    _sideMenu.AddButton("main", _stagesButtons[stageNumber].GetComponent<SideMenu.SideMenuButton>());
            }

            //Remove buttons that shall not be active
            foreach (var stage in _stagesButtons)
            {
                //if (stage.Value.StageIndex == stageNumber)
                //    continue;
                stage.Value.SetCurrentStageIndex(stageNumber);
                Debug.Log($"Current Stage: {stageNumber}, stage: {stage.Value.StageIndex}, Stage.Active: {stage.Value.Active}");
                if (!stage.Value.Active)
                {
                    _sideMenu.RemoveButton($"table {stage.Value.StageIndex}");
                    //_stagesButtons[stageNumber] = null;
                    stage.Value.transform.SetParent(null);
                    stage.Value.gameObject.SetActive(false);
                }
            }

            _lastTablesWindowActive.InitializeTable();

            if (_stagesButtons[stageNumber].StagePassed)
                _stagesButtons[stageNumber].TablesView.NewTable();

            _isTableEnabled = true;
            UpdateUI();
        }

        public void ResetTable(int tableButtonKey)
        {
            if (_lastTablesWindowActive == null)
                return;

            for (int i = 1; i <= _stagesButtons.Count; i++)
            {
                string tableSideMenuID = _stagesButtons[i].GetComponent<SideMenu.SideMenuButton>().Id;

                if (_sideMenu.GetIslandById("main").HasID(tableSideMenuID))
                    _sideMenu.GetIslandById("main").RemoveButton(tableSideMenuID);
            }

            _lastTablesWindowActive = null;
            _isTableEnabled = false;
        }

        public void RemoveTrials()
        {
            if (_lastTablesWindowActive == null)
                return;

            _lastTablesWindowActive.RemoveTrials();
        }

        public void SetStagePassed(int stageNumber) => SetStagePassed(stageNumber, true);
        public void UnSetStagePassed(int stageNumber) => SetStagePassed(stageNumber, false);
        public void SetStagePassed(int stageNumber, bool passed)
        {
            _stagesButtons[stageNumber].StagePassed = passed;
        }

        public void AddNewTrial()
        {
            _lastTablesWindowActive.NewTable();
        }
        public void NewTrialButtonInteractible(bool interactible)
        {
            _lastTablesWindowActive.SetNewTrialButtonInteractible(interactible);
        }

        public void SetSetting(string id, string value)
        {
            _lastTablesWindowActive.SetSetting(id, value);
        }

        public TablesWindow GetTableWindow()
        {
            return _lastTablesWindowActive;
        }

        public void ToggleTable(bool toggle)
        {
            if (_lastTablesWindowActive == null ||
               _lastTablesWindowActive.IsPinned)
                return;

            TableTransitions tableTransitions = _lastTablesWindowActive.GetComponent<TableTransitions>();

            if (toggle)
                tableTransitions.ExpandFull();
            else
                tableTransitions.Collapse();
        }

        #region Localization
        private void LoadJson()
        {
            _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.Tables.ToString());

            if (!_dataExists) return;

            _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.Tables.ToString()];
            LoadTablesJson();
        }

        public void LoadTablesJson()
        {
            if (!_dataExists) return;

            string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
            tablesData = JsonConvert.DeserializeObject<Dictionary<int, TablesData>>(currentJson);

            currentTablesData = tablesData[ExperimentManager.Instance.stageIndex + 1];

            if (_isTableEnabled)
                UpdateUI();
        }

        private void UpdateUI()
        {
            _lastTablesWindowActive.UpdateUI(currentTablesData);
        }
        #endregion

        /// <summary>
        /// Simple structure combining stage number, table window prefab and table view prefab for each stage.
        /// </summary>
        [System.Serializable]
        public struct StageTableEntry
        {
            public int stage;
            public bool showTableSettings;
            public TablesWindow tableWindow;
            public Table.UI.Views.TableSettings.TableSettingsView tableSettingsViewPrefab;
            public Table.UI.Views.TableView tableViewPrefab;
        }
    }

    [Serializable]
    public class TablesData
    {
        public string tableViewTitle;
        public string newTrialHeader;
        public string newTrialSubHeader;
        public List<ColumnName> columnNames = new List<ColumnName>();
        public List<string> tableSettingsTypeLabels = new List<string>();
    }

    [Serializable]
    public class ColumnName
    {
        public string columnID;
        public string columnName;
    }
}