using System.Collections;
using System.Collections.Generic;
using Table.UI.Views.Buttons;
using Table.Download;
using UnityEngine;

namespace Table.UI.Views
{
    public class TablesWindow : MonoBehaviour
    {
        #region types definitions
        /// <summary>
        /// Simple class grouping Table prefab, table button, and table index
        /// </summary>
        public class TableViewEntry
        {
            public TableView TableUI { set; get; }
            public TableSettings.TableSettingsView TableSettingsView { get; set; }
            public TableButton TableButton { set; get; }
            public int TableIndex { set; get; }
        }
        #endregion


        #region fields
        [SerializeField] private bool _initializeOnStart = true;
        public UnityEngine.Events.UnityEvent<bool> onTableWindowPinChanged;
        [field: Header("Table creation prefabs")]
        [field: SerializeField] public TableView TableViewPrefab { get; set; }
        [SerializeField] private TableButton _tableButtonPrefab;

        [Header("Table window title")]
        [SerializeField] private TMPro.TextMeshProUGUI _tableViewTitle;

        [Header("Table, table button containers for creation")]
        [SerializeField] private RectTransform _tableParent;
        [SerializeField] private RectTransform _tableButtonParent;

        [Header("Multi table section")]
        [Space(10)]
        [SerializeField] private bool _multiTable = true;
        [SerializeField] private GameObject _addNewTableButtonGameObject;
        [SerializeField] private UnityEngine.UI.Button _addNewTableButton;
        [SerializeField] private GameObject _multiTableSection;
        [SerializeField, Tooltip("Weather the table, table button numbering shall start from 0, 1 or else.")] private uint _startingTableIndex = 1;

        [Header("Add new row / record button")]
        [Space(10)]
        [SerializeField] private RectTransform _addRecordButtonRect;
        [SerializeField] private UnityEngine.UI.Button _addRecordButton;
        

        [Header("Download table stuff")]
        [SerializeField] private Texture2D _praxilabsLogoForExcel;
        private TableView _activeTable;
        private TableButton _activeTableButton;
        private Dictionary<int, TableViewEntry> _tableViewEntries = new Dictionary<int, TableViewEntry>();
        private Dictionary<TableView, int> _tableToIndex;
        private int _lastTableIndex = 0;
        private bool _isPinned;
        public List<TableView> TablesInView { get; protected set; }
        public int StageNumber { get; set; }
        public RectTransform TableButtonParent { get => _tableButtonParent; set => _tableButtonParent = value; }
        [Header("Table settings part")]
        [SerializeField] private bool _showTableSettings;
        [SerializeField] private GameObject _tableSettingPlaceholder;
        [field: SerializeField] public TableSettings.TableSettingsView TableSettingsPrefab { get; set; }
        [SerializeField] private TableSettings.TableSettingsView _activeTableSettings;
        [SerializeField] private GameObject _tableSettingSpacer;
        public bool ShowTableSettings
        {
            get
            {
                return _showTableSettings;
            }
            set
            {
                _showTableSettings = value;
                UpdateTableSettings();
            }
        }

        #endregion
        public void UpdateTableSettings()
        {
            _activeTableSettings?.gameObject.SetActive(_showTableSettings);
            _tableSettingSpacer.SetActive(_showTableSettings );

        }
        private void Start()
        {
            if (_initializeOnStart)
                Initialize();
        }

        public void UpdateUI(TablesData tableData)
        {
            //Use # as place holder for stage number
            _tableViewTitle.text = tableData.tableViewTitle.Replace("#", StageNumber.ToString());

            TableViewEntry selectedEntry = _tableViewEntries[1];
            if (selectedEntry == null) return;

            TableButton tableButton = selectedEntry.TableButton;
            tableButton.Initialize(tableData.newTrialHeader, tableData.newTrialSubHeader);

            TableView tableView = selectedEntry.TableUI;
            tableView.UpdateColumnTitle(tableData);
            tableView.AdjustColumnsPositions();

            _activeTableSettings.UpdateTypeLabels(tableData.tableSettingsTypeLabels);
        }

        public void Initialize()
        {
            _multiTableSection.SetActive(_multiTable);
            _tableViewEntries = new Dictionary<int, TableViewEntry>();
            _tableToIndex = new Dictionary<TableView, int>();
            TablesInView = new List<TableView>();
            _lastTableIndex = (int)_startingTableIndex;
            UpdateTableSettings();
            NewTable();
        }

        public Dictionary<int, TableViewEntry> GetTableViewEntries() => _tableViewEntries;

        /// <summary>
        /// Adds empty record to the active table in the window.
        /// </summary>
        public void AddRecord()
        {
            InitializeTable();
            _activeTable.AddNewRow();
        }

        /// <summary>
        /// Adds a empty record, then populates named fields with passed values in <paramref name="data"/><br/>
        /// <b>For ease keys are not case sensitive. ie. "Name" = "name" = "NaMe"</b>
        /// </summary>
        /// <param name="data">Key value pair for the fields to be set. key is column/field name</param>
        public void AddRecord(Dictionary<string, string> data)
        {
            InitializeTable();
            _activeTable.AddNewRow(data);
        }

        public void InitializeTable()
        {
            if (_activeTable is null)
            {
                Initialize();
                _initializeOnStart = false;
            }
        }

        public void SetRowData(int row, Dictionary<string, string> data)
        {
            _activeTable.SetRowData(row, data);
        }

        public void SetLastRowData(Dictionary<string, string> data)
        {
            _activeTable.SetLastRowData(data);
        }
        public void SetRowDataWithKey(string key, string value, Dictionary<string, string> data)
        {
            _activeTable.SetRowDataWithKey(key, value, data);
        }
        public void SetSetting(string id, string value)
        {
            if (_activeTableSettings is null)
            {
                Initialize();
                _initializeOnStart = false;

            }
            _activeTableSettings.GetSettings(id).StringValue = value;
            _activeTableSettings.GetSettings(id).UpdateVisual(true);
        }
        public void SetNewTrialButtonInteractible(bool interactible)
        {
            _addNewTableButton.interactable = interactible;
        }
        [ContextMenu("EnableNewTrialButtonInteractible")]
        public void EnableNewTrialButtonInteractible() => SetNewTrialButtonInteractible(true);
        [ContextMenu("DisableNewTrialButtonInteractible")]
        public void DisableNewTrialButtonInteractible() => SetNewTrialButtonInteractible(false);
        /// <summary>
        /// Creates new table and adds rows till min amount of rows/records is met.<br/>
        /// New table gets next index from last created table, even if last created table is deleted.
        /// </summary>
        public void NewTable()
        {
            #region Create table button
            TableButton newTableButton = Instantiate(_tableButtonPrefab, TableButtonParent);
            newTableButton.TablesView = this;
            newTableButton.TableIndex = _lastTableIndex;
            #endregion
            //Send create table button to the right most, without this it would be before last created table button.
            _addNewTableButtonGameObject.transform.SetAsLastSibling();

            #region Create Table
            TableView newTableUI = Instantiate(TableViewPrefab, _tableParent);
            newTableUI.Table.TableIndex = StageNumber;
            newTableUI.Initialize();
            //newTableUI.Table.TableName = newTableUI.Table.TableName + lastTableIndex;
            for(int i=1; i < newTableUI.Table.MinimumRecordsAmount; i++)
            {
                newTableUI.AddNewRow();
            }
            #endregion

            #region Create table settings
            TableSettings.TableSettingsView tableSettingsView = Instantiate(TableSettingsPrefab, _tableSettingPlaceholder.transform.parent);
            tableSettingsView.transform.SetSiblingIndex(_tableSettingPlaceholder.transform.GetSiblingIndex());
            _tableSettingPlaceholder.gameObject.SetActive(false);
            #endregion


            TableViewEntry tableViewEntry = new TableViewEntry()
            {
                TableButton = newTableButton,
                TableUI = newTableUI,
                TableIndex = _lastTableIndex,
                TableSettingsView = tableSettingsView
            };

            _tableViewEntries.Add(_lastTableIndex, tableViewEntry);
            _tableToIndex.Add(newTableUI, _lastTableIndex);
            TablesInView.Add(newTableUI);
            Activate(_lastTableIndex);
            while (_tableViewEntries.ContainsKey(_lastTableIndex)) _lastTableIndex++;
            _addRecordButtonRect.transform.SetAsLastSibling();
        }

        public void RemoveTrials()
        {
            for (int i = 1; i <= _lastTableIndex; i++)
                DeleteTable(i);
            _initializeOnStart = true;
        }

        /// <summary>
        /// Activates table of index <paramref name="tableIndex"/>
        /// </summary>
        public void Activate(int tableIndex)
        {
            TableViewEntry selectedEntry = (_tableViewEntries == null || !_tableViewEntries.ContainsKey(tableIndex)) ? null : _tableViewEntries[tableIndex];
            _activeTableButton?.SetActive(false);
            _activeTable?.gameObject.SetActive(false);
            _activeTableSettings?.gameObject.SetActive(false);
            if (_activeTable != null)
                _activeTable.OnRowsChange -= UpdateAddRecordButtonInteractable;
            _activeTable = selectedEntry == null ? null : selectedEntry.TableUI;
            _activeTableButton = selectedEntry == null ? null : selectedEntry.TableButton;
            _activeTableSettings = selectedEntry == null ? null : selectedEntry.TableSettingsView;
            _activeTableButton?.SetActive(true);
            _activeTable?.gameObject.SetActive(true);
            _activeTableSettings?.gameObject.SetActive(true);
            if (_activeTable != null)
                _activeTable.OnRowsChange += UpdateAddRecordButtonInteractable;
            UpdateAddRecordButtonInteractable();
        }
        /// <summary>
        /// Enables Add record button if
        /// <list type="bullet">
        /// <item>There is active table</item>
        /// <item>Active table allows adding rows</item>
        /// <item>active table max row amount is not reached yet</item>
        /// </list>
        /// <br/>
        /// Add row/record is disabled otherwise
        /// </summary>
        private void UpdateAddRecordButtonInteractable()
        {
            _addRecordButton.interactable = _activeTable != null &&
                _activeTable.Table.CanAddRecord &&
                (_activeTable.NumberOfRows < _activeTable.Table.MaximumRecordsAmount);
        }
        /// <summary>
        /// Deletes table whose index is <paramref name="index"/>
        /// </summary>
        public void DeleteTable(int index)
        {
            TableViewEntry removedEntry;
            if(!_tableViewEntries.Remove(index, out removedEntry))
                return;
            _tableToIndex.Remove(removedEntry.TableUI);
            removedEntry.TableUI.Terminate();
            removedEntry.TableButton.Terminate();
            int previousTableIndex = removedEntry.TableIndex;
            int nextTableIndex = removedEntry.TableIndex;

            for (int i = removedEntry.TableIndex; i < _lastTableIndex; i++)
            {
                if(_tableViewEntries.ContainsKey(i))
                {
                    nextTableIndex = i;
                    break;
                }
            }
            if (nextTableIndex != removedEntry.TableIndex)
                Activate(nextTableIndex);
            else
            {

                while (!_tableViewEntries.ContainsKey(previousTableIndex) && previousTableIndex != 0)
                {
                    previousTableIndex--;
                }
                Activate(previousTableIndex);
            }
           
        }
        public void TogglePinned()
        {
            _isPinned = !_isPinned;
            onTableWindowPinChanged?.Invoke(_isPinned);
        }
        
        public bool IsPinned => _isPinned;

        /// <summary>
        /// Deletes table <paramref name="tableUI"/>
        /// </summary>
        public void DeleteTable(TableView tableUI)
        {
            DeleteTable(_tableToIndex[tableUI]);
        }
        /// <summary>
        /// Downloads table of index <paramref name="tableIndex"/>
        /// </summary>
        public void Download(int tableIndex)
        {
            TableDownloader.Download(
                _praxilabsLogoForExcel,
                _tableViewEntries[tableIndex].TableUI,
                _tableViewEntries[tableIndex].TableUI.Table.TableName
                );
        }
        /// <summary>
        /// Downloads all tables in the window.
        /// </summary>
        public void DownloadAllTables()
        {
            TableWindowDownloader.Download(_praxilabsLogoForExcel, this, _tableViewTitle.text);
        }
    }
}