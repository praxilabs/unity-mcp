using System.Collections;
using System.Collections.Generic;
using Table.UI.Views.TableViewElements.CellTypes;
using UnityEngine;
namespace Table.UI.Views.TableViewStructure
{
    [System.Serializable]
    public class Column
    {
        [field: SerializeField] public string ColumnName { get; }
        [field: SerializeField] public float ColumnWidth { get; set; }
        [field: SerializeField] public float ColumnXPosition { get; set; }
        [field: SerializeField] public float LeftRightMargins { get; set; }
        [field: SerializeField] public List<BaseTableCell> TableCells { get; set; }

        public void UpdateWidthAndCells() => UpdateWidth(true);
        public void UpdateWidth(bool updateCells)
        {
            float maxWidth = 0;// if no cells in column
            for (int i = 0; i < TableCells.Count; i++)
            {
                if (maxWidth < TableCells[i].PreferredWidth)
                    maxWidth = TableCells[i].PreferredWidth;
            }
            ColumnWidth = maxWidth;
            if (updateCells)
                UpdateCells();
        }
        public void UpdateCells()
        {
            for (int i = 0; i < TableCells.Count; i++)
            {
                TableCells[i].Width = ColumnWidth + LeftRightMargins;
                TableCells[i].PositionX = ColumnXPosition;
            }
        }
        public Column(Structure.TableField field)
        {
            this.ColumnName = field.Name;
            this.TableCells = new List<BaseTableCell>();
        }
        public static explicit operator Structure.TableField(Column column)
        {
            return new Structure.TableField() { Name = column.ColumnName, Editable = false, TableCell = null };
        }
    }
}