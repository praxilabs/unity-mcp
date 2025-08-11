using System.Collections;
using System.Collections.Generic;
using Table.UI.Views.TableViewElements.CellTypes;
using UnityEngine;

namespace Table.Structure
{
    [System.Serializable]
    public class TableField:System.ICloneable
    {
        public event System.Action<string> onValueChange;

        #region properties
        public string fieldID;
        /// <summary>
        /// Name of the field/column
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Can the user edit this cell?
        /// </summary>
        [field: SerializeField] public bool Editable { get; set; }
        /// <summary>
        /// Table cell UI attached to this field.
        /// </summary>
        [field: SerializeField] public BaseTableCell TableCell { get; set; }
        [field: SerializeField] public string Value { get; protected set; }
        #endregion
        /// <summary>
        /// Sets value of field using string <paramref name="value"/>
        /// </summary>
        /// <param name="value">Field value as string</param>
        public void SetValue(string value)
        {
            Value = value;
            onValueChange?.Invoke(Value);
        }
        /// <summary>
        /// Clone this object to another object.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            TableField tableFieldClone = new TableField();

            tableFieldClone.Editable = this.Editable;
            tableFieldClone.fieldID = this.fieldID;
            tableFieldClone.Name = Name;
            tableFieldClone.TableCell = TableCell;
            tableFieldClone.Value = Value;
            return tableFieldClone;
            
        }

    }
}