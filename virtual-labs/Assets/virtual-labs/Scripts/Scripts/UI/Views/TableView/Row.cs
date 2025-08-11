using System.Collections;
using System.Collections.Generic;
using Table.UI.Views;
using Table.UI.Views.TableViewElements.CellTypes;
using UnityEngine;
namespace Table.UI.Views.TableViewStructure
{
    [System.Serializable]
    public class Row
    {
        #region Properties
        [field: SerializeField] public float RowHeight { get; set; }
        [field: SerializeField] public float RowYPosition { get; set; }
        [field: SerializeField] public float TopBottomMargins { get; set; }
        [field: SerializeField] public float RowBackgroundMargin { get; set; }
        [field: SerializeField] public List<BaseTableCell> TableCells { get; set; }
        /// <summary>
        /// Computes can delete bool condition
        /// </summary>
        public bool CanDelete => (TableView.Table.CanDelete && TableView.Table.RecordsCount() > TableView.Table.MinimumRecordsAmount);

        private float MinX
        {
            get
            {
                float minX = float.MaxValue;
                for (int i = 0; i < TableCells.Count; i++)
                {
                    minX = Mathf.Min(TableCells[i].PositionX, minX);
                }
                return minX;
            }
        }

        private float MaxX
        {
            get
            {
                float maxX = float.MinValue;
                for (int i = 0; i < TableCells.Count; i++)
                {
                    maxX = Mathf.Max(TableCells[i].PositionX + TableCells[i].Width, maxX);
                }
                return maxX;
            }
        }

        private float RowWidth => Mathf.Abs(MaxX - MinX);
        public TableView TableView { get; set; }
        [field: SerializeField] public RectTransform RowBackground { get; set; }
        #endregion

        private RowNumberCell _rowNumberCell;
        private int _rowIndex;
        /// <summary>
        /// Sets row number for this row.<br/>
        /// Calling on Row without RowNumberCell shall be avoided. Will throw exception if RowNumberCell isn't found.<br/>
        /// Title row has no numberCell, so it's special case but still will throw exception.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <exception cref="System.NullReferenceException"> RowNumberCell is null</exception>
        public void UpdateRowNumber(int rowIndex)
        {
            if (_rowNumberCell == null)
                FindRowNumberCell();
            if (_rowNumberCell == null)
                throw new System.NullReferenceException($"Cannot find find RowNumberCell in row cells!");
            this._rowIndex = rowIndex;
            _rowNumberCell.RowNumber = rowIndex;
            _rowNumberCell.UpdateData(rowIndex.ToString());
        }

        private void FindRowNumberCell()
        {
            _rowNumberCell = (RowNumberCell)TableCells.Find(x => x is RowNumberCell);
        }
        /// <summary>
        /// Updates height of row to match heighest cell, then Updates all cells for matching this height.
        /// </summary>
        public void UpdateHeight()
        {
            float maxHeight = 0;// if no cells in row
            for (int i = 0; i < TableCells.Count; i++)
            {
                //Debug.Log($"table row: <{this.rowIndex}, cell[{i}] type: <{TableCells[i].GetType()}>, height: <{TableCells[i].PreferredHeight}>, cellName:<{TableCells[i]}>");
                if (maxHeight < TableCells[i].PreferredHeight)
                {
                    maxHeight = TableCells[i].PreferredHeight;
                }
            }
            RowHeight = maxHeight;
            UpdateCells();
        }
        /// <summary>
        /// Updates cells height, positionY, Row background. depending on Row height, and RowY position
        /// </summary>
        public void UpdateCells()
        {
            for (int i = 0; i < TableCells.Count; i++)
            {
                TableCells[i].Height = RowHeight + TopBottomMargins;
                TableCells[i].PositionY = RowYPosition;
            }
            UpdateRowBackground();
        }
        /// <summary>
        /// Adds cell <paramref name="baseTableCell"/> to row cells.
        /// </summary>
        /// <param name="baseTableCell"></param>
        public void AddCell(BaseTableCell baseTableCell)
        {
            if (TableCells == null)
                TableCells = new List<BaseTableCell>();
            TableCells.Add(baseTableCell);
            baseTableCell.Row = this;
            if (baseTableCell is RowNumberCell)
                _rowNumberCell = baseTableCell as RowNumberCell;
        }
        /// <summary>
        /// removes cell from row cells
        /// </summary>
        /// <param name="baseTableCell"></param>
        public void RemoveCell(BaseTableCell baseTableCell)
        {
            if (TableCells == null)
                return;
            TableCells.Remove(baseTableCell);
        }
        /// <summary>
        /// deletes this row.
        /// </summary>
        public void DeleteRow()
        {
            if (!CanDelete)
                return;
            TableView.RemoveRow(_rowIndex);
            GameObject.Destroy(RowBackground.gameObject);
            UpdateDeleteButton();
        }

        public void UpdateDeleteButton()
        {
            _rowNumberCell?.UpdateDeleteButton();
        }
        [ContextMenu("Update row background")]
        public void UpdateRowBackground()
        {
            if (TableCells.Count <= 0)
            {
                Debug.Log($"Table has no cells yet ! !");
                return;
            }
            RowBackground.anchorMin = Vector2.up;
            RowBackground.anchorMax = Vector2.up;
            RowBackground.pivot = Vector2.up;
            RowBackground.localPosition = new Vector3(MinX , RowYPosition + RowBackgroundMargin / 2);
            RowBackground.sizeDelta = new Vector2(RowWidth+RowBackgroundMargin, RowHeight + TopBottomMargins + RowBackgroundMargin);
        }
        public void HighlightRow()
        {
            RowBackground.gameObject.SetActive(true);
            UpdateRowBackground();
        }
        public void UnHighlightRow()
        {
            RowBackground.gameObject.SetActive(false);
        }
        /// <summary>
        /// Sets row data specified with keys, to specified values.<br/>
        /// <b>Keys are not case sensititve.</b>
        /// </summary>
        /// <param name="data">Key(field name), value(field value) dictionary</param>
        public void SetRowData(Dictionary<string, string> data)
        {
            foreach (var item in data)
            {
                TableCells.Find(x => x.TableField.fieldID.ToLower() == item.Key.ToLower()).TableField.SetValue(item.Value);
            }
        }
        /// <summary>
        /// Returns weather that row has a field with name/key <paramref name="key"/> and that field is of value <paramref name="value"/>
        /// <br/>
        /// Usage is you pass field name and field value to check for.
        /// Example: HasKeyValue("Name","Mazen") would return true if there is a field with name 'Name' and it has value 'Mazen'
        /// </summary>
        /// <param name="key">field key / name. Like Employee name, age, position</param>
        /// <param name="value">field value for that of key requested. Like 'Mazen' for name, 27 for age, Engineer for position</param>
        /// <returns></returns>
        public bool HasKeyValue(string key, string value)
        {
            return TableCells.Exists(x => x.TableField.fieldID == key && x.TableField.Value == value);
        }
    }
}