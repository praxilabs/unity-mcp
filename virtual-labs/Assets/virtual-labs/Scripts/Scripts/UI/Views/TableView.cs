using System.Collections;
using System.Collections.Generic;
using Table.Structure;
using Table.UI.Views.TableViewElements.CellTypes;
using Table.UI.Views.TableViewStructure;
using UnityEngine;

namespace Table.UI.Views
{
    public class TableView : MonoBehaviour
    {
        private Dictionary<string, Column> _columns;
        private Column[] _columnsArray;
        [SerializeField] private RectTransform _rowBackground;
        private List<Row> _rows;
        [SerializeField] private RectTransform _tablecellsParent;


        [field: SerializeField] public Structure.Table Table { get; set; }
        [Header("Positions, padding")]
        [SerializeField] private float _startPositionX;
        [SerializeField] private float _startPositionY;
        [SerializeField] private float _horizontalSpacing;
        [SerializeField] private float _verticalSpacing;
        [SerializeField] private float _verticalCellMargin;
        [SerializeField] private float _horizontalCellMargin;
        [SerializeField] private float _rowBackgroundMargin = 20;
        public int NumberOfRows => _rows == null ? 0 : _rows.Count;

        private bool _active;
        public bool Active { set { _active = value; gameObject.SetActive(_active); } get => _active; }

        private bool _initialized = false;

        public event System.Action OnRowsChange;
        public void SetActive(bool active)
        {
            Active = active;
        }
        private void Start()
        {
            Initialize();
        }

        public void UpdateTableFieldNames()
        {
            Table.SetTableFieldNames();
        }

        public void UpdateColumnTitle(TablesData tableData)
        {
            int counter = 0;
            List<ColumnName> columnNames = tableData.columnNames;

            foreach (var tableField in Table.Template.TableFields)
            {
                Table.columnTitles[counter].UpdateTitle(columnNames[counter].columnName);
                counter++;
            }
        }
        /// <summary>
        /// Deconstruct tableview and destroy
        /// </summary>
        public void Terminate()
        {
            for(int i = _rows.Count-1; i >= 0; i--)
            {
                RemoveRow(i);
            }
            Destroy(this.gameObject);
        }

        private void AddTitleRow() => AddNewRow(true);
        [ContextMenu("AddNewRow")]
        public void AddNewRow() => AddNewRow(false);

        private void AddNewRow(bool isTitle)
        {
            Row row = new Row();
            row.TableView = this;
            row.RowBackground = Instantiate(_rowBackground, _tablecellsParent);
            Table.AddRow(row, _tablecellsParent, isTitle);
            foreach (var cell in row.TableCells)
            {
                _columns[cell.TableField.fieldID].TableCells.Add(cell);
                _columns[cell.TableField.fieldID].UpdateWidthAndCells();
                cell.OnCellUpdate += AdjustColumnsPositions;
                cell.OnCellUpdate += AdjustRowsPositions;

            }

            _rows.Add(row);
            _rows.ForEach(x => x.UpdateDeleteButton());
            AdjustColumnsPositions();
            AdjustIndices();
            AdjustRowsPositions();
            OnRowsChange?.Invoke();
        }
        [ContextMenu("Initialize")]
        public void Initialize()
        {
            UpdateTableFieldNames();
            if (_initialized) return;
            _initialized = true;

            _columns = new Dictionary<string, Column>(Table.NumberOfColumns);
            _rows = new List<Row>();
            _columnsArray = new Column[Table.NumberOfColumns];
            for (int i = 0; i < Table.NumberOfColumns; i++)
            {
                Column column = new Column(Table.Template.TableFields[i]);
                _columns.Add(Table.ColumnID(i), column);
                _columnsArray[i] = column;
            }
            AddTitleRow();
        }
        [ContextMenu("Adjust columns Positions")]
        public void AdjustColumnsPositions()
        {
            float nextPosition = _startPositionX;
            for (int i = 0; i < _columnsArray.Length; i++)
            {

                _columnsArray[i].LeftRightMargins = _horizontalCellMargin;
                _columnsArray[i].UpdateWidthAndCells();
                _columnsArray[i].ColumnXPosition = nextPosition;
                nextPosition += _columnsArray[i].ColumnWidth + _columnsArray[i].LeftRightMargins + _horizontalSpacing;
                _columnsArray[i].UpdateCells();
            }
            Vector2 rectSize = _tablecellsParent.sizeDelta;
            rectSize.x = nextPosition;
            _tablecellsParent.sizeDelta = rectSize;
        }
        [ContextMenu("Adjust rows Positions")]
        public void AdjustRowsPositions()
        {
            float nextPosition = _startPositionY;
            for (int i = 0; i < _rows.Count; i++)
            {
                _rows[i].TopBottomMargins = _verticalCellMargin;
                _rows[i].RowBackgroundMargin = _rowBackgroundMargin;
                _rows[i].UpdateHeight();
                _rows[i].RowYPosition = nextPosition;
                nextPosition -= _rows[i].RowHeight + _rows[i].TopBottomMargins +_rowBackgroundMargin+ _verticalSpacing;
                _rows[i].UpdateCells();
                _rows[i].UpdateRowBackground();
            }
            Vector2 rectSize = _tablecellsParent.sizeDelta;
            rectSize.y = Mathf.Abs(nextPosition);
            _tablecellsParent.sizeDelta = rectSize;
        }
        

        [ContextMenu("UpdateRowsBackground")]
        private void UpdateRowsBackground()
        {
            _rows.ForEach(x => x.UpdateRowBackground());
        }
        /// <summary>
        /// Deletes row of index <paramref name="index"/>
        /// </summary>
        public void RemoveRow(int index)
        {
            Row row = _rows[index];
            TableRecord tableRecordClone = Table.GetRecord(index);
            for (int i = 0; i < tableRecordClone.TableFields.Length; i++)
            {
                BaseTableCell cell = tableRecordClone.TableFields[i].TableCell;
                cell.OnCellUpdate -= AdjustColumnsPositions;
                cell.OnCellUpdate -= AdjustRowsPositions;
                cell.Terminate();
                _columns[cell.TableField.fieldID].TableCells.Remove(cell);
                _columns[cell.TableField.fieldID].UpdateWidthAndCells();
                row.RemoveCell(cell);
                Destroy(cell.gameObject);
            }
            Table.Delete(index);
            _rows.RemoveAt(index);
            _rows.ForEach(x => x.UpdateDeleteButton());
            AdjustRowsPositions();
            AdjustColumnsPositions();
            AdjustIndices();
            OnRowsChange?.Invoke();
        }
        /// <summary>
        /// Takes row number, and key value for cell data to set.
        /// If dictionary item name is same as table field name, it's value is set to dictionary item.value<br/>
        /// Not case sensitive.
        /// </summary>
        /// <param name="row">Row number</param>
        /// <param name="data">Dictionary where <b>Key</b> is field/column name, <b>value</b> is field value to be set.</param>
        public void SetRowData(int row, Dictionary<string,string> data)
        {
            _rows[row].SetRowData(data);
        }

        public Row FindRowWithKey(string key, string value)
        {
            return _rows.Find(x => x.HasKeyValue(key, value));
        }

        public List<Row> FindAllRowsWithKey(string key, string value)
        {
            return _rows.FindAll(x => x.HasKeyValue(key, value));
        }
        /// <summary>
        /// Finds first row with key/value mentioned, and sets it's data if found.
        /// </summary>
        /// <param name="key">field key/name to search for</param>
        /// <param name="value">field value</param>
        /// <param name="data">data</param>
        public void SetRowDataWithKey(string key, string value, Dictionary<string,string> data)
        {
            var row = _rows.Find(x => x.HasKeyValue(key, value));

            if (row is not null)
                row.SetRowData(data);
        }
        /// <summary>
        /// Takes Key value for cell name, data to set.
        /// if dictionary item name is same as table cell name, it's value is set to dictionary item.value
        /// <br/>
        /// Not case sensitive.
        /// </summary>
        /// <param name="data">Dictionary where <b>Key</b>key is cell/column name, value is value to be set.</param>
        public void AddNewRow(Dictionary<string,string> data)
        {
            AddNewRow();
            SetLastRowData(data);
        }
        public void SetLastRowData(Dictionary<string,string> data)
        {
            SetRowData(_rows.Count - 1, data);
        }
        /// <summary>
        /// Every row gets it's number / index.
        /// </summary>
        private void AdjustIndices()
        {
            ///start from 1 since title has no number, just '#'
            for (int i = 1; i < _rows.Count; i++)
            {
                _rows[i].UpdateRowNumber(i);
            }
        }
    }
}