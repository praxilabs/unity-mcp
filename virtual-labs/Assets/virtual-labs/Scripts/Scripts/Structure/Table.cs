using System.Collections.Generic;
using Table.UI.Views.TableViewElements.CellTypes;
using Table.UI.Views.TableViewStructure;
using UnityEngine;

namespace Table.Structure
{
    [System.Serializable]
    public class Table
    {
        #region Properties
        [field: SerializeField]
        public int TableIndex { get; set; }
        //[Header("Excel sheet strings")]
        //[SerializeField, Tooltip("Table sheet name for excel sheet")] private string _tableNameTemplate;

        public string TableName => TableName.Replace("#", TableIndex.ToString());
        /// <summary>
        /// Table description for excel file.
        /// </summary>
        [field: SerializeField, Tooltip("Table description for excel file")] public string TableDescription { get; set; }
        /// <summary>
        /// Template for duplicating rows from, and setting Column headers from field names.
        /// </summary>
        [field: SerializeField, Tooltip("Template for duplicating rows from, and setting Column headers from field names.")]
        public TableRecord Template { get; set; }
        /// <summary>
        /// Prefab for Column title cells.
        /// </summary>
        [SerializeField] private BaseTableCell _titleCellPrefab;
        /// <summary>
        /// Records avalable in this table
        /// </summary>
        [field: SerializeField, HideInInspector] protected List<TableRecord> TableRecords { get; set; }
        public List<ColumnTitle> columnTitles = new List<ColumnTitle>();
        /// <summary>
        /// Can user delete rows?
        /// </summary>
        [field: SerializeField] public bool CanDelete { get; protected set; }
        /// <summary>
        /// Can user Add rows?
        /// </summary>
        [field: SerializeField] public bool CanAddRecord { get; protected set; }
        [SerializeField] protected int _minimumRecordsAmount = 0;
        [SerializeField] protected int _maximumRecordsAmount = int.MaxValue;
        /// <summary>
        /// Minimum records amound for auto creation on table creation, and if <see cref="CanDelete"/> enabled, deletion is disabled upon reaching this amount.
        /// </summary>
        public int MinimumRecordsAmount => _minimumRecordsAmount + 1;
        /// <summary>
        /// Maximim records amount, if <see cref="CanAddRecord"/> enabled, add record stops after number of rows reach this amount
        /// </summary>
        public int MaximumRecordsAmount => _maximumRecordsAmount + 1;
        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int NumberOfColumns => Template.TableFields.Length;
        #endregion

        public void SetTableFieldNames()
        {
            List<ColumnName> columnNames = TablesManager.Instance.tablesData[TableIndex].columnNames;

            for (int i = 0; i < Template.TableFields.Length; i++)
            {
                Template.TableFields[i].fieldID = columnNames[i].columnID;
                Template.TableFields[i].Name = columnNames[i].columnName;
            }
        }

        /// <summary>
        /// Deletes record of index <paramref name="recordIndex"/>
        /// </summary>
        /// <param name="recordIndex">Record index for deletion</param>
        public void Delete(int recordIndex)
        {
            TableRecords.RemoveAt(recordIndex);
        }

        /// <summary>
        /// Adds a new row <paramref name="row"/> as child of <paramref name="tableCellsParent"/><br/>
        /// As title if [optional] <paramref name="title"/> is True
        /// </summary>
        /// <param name="row"></param>
        /// <param name="tableCellsParent"></param>
        /// <param name="title">Cell is cloned from title prefabs if true</param>
        public void AddRow(Row row, RectTransform tableCellsParent, bool title = false)
        {
            TableRecord record = Template.Clone() as TableRecord;

            for (int i = 0; i < Template.TableFields.Length; i++)
            {
                BaseTableCell cell = BaseTableCell.Instantiate<BaseTableCell>
                    (title ? _titleCellPrefab : Template.TableFields[i].TableCell, tableCellsParent);
                if (title)
                    columnTitles.Add(cell as ColumnTitle);
                record.TableFields[i].TableCell = cell;
                cell.TableField = record.TableFields[i];

                cell.Initialize();
                row.AddCell(cell);
            }
            TableRecords.Add(record);
        }
        /// <summary>
        /// Number of records/rows in the table, including title row
        /// </summary>
        /// <returns></returns>
        public int RecordsCount() => TableRecords.Count;
        /// <summary>
        /// Gets record/row of index <paramref name="i"/>
        /// </summary>
        /// <param name="i">Row index</param>
        /// <returns></returns>
        public TableRecord GetRecord(int i) => TableRecords[i];
        /// <summary>
        /// Returns name of column of index <paramref name="i"/>
        /// </summary>
        /// <param name="i">Column index</param>
        /// <returns></returns>
        public string ColumnName(int i) => Template.TableFields[i].Name;
        public string ColumnID(int i) => Template.TableFields[i].fieldID;
        public Table()
        {
            TableRecords = new List<TableRecord>();
        }

    }

}