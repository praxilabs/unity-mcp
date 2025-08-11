using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Table.Structure
{
    [System.Serializable]
    public class TableRecord : System.ICloneable
    {
        /// <summary>
        /// Fields in table record
        /// </summary>
        [field: SerializeField] public TableField[] TableFields { get; set; }

        public object Clone()
        {
            TableRecord tableRecordClone = new TableRecord ();
            tableRecordClone.TableFields = new TableField[this.TableFields.Length];
            for (int i = 0; i < tableRecordClone.TableFields.Length; i++)
            {
                tableRecordClone[i] = this.TableFields[i].Clone() as TableField;
            }
            return tableRecordClone;
        }
        /// <summary>
        /// Returns table field of index <paramref name="i"/> from the table record
        /// </summary>
        /// <param name="i">Index of table field to be returned</param>
        /// <returns></returns>
        public TableField this[int i] { get => TableFields[i]; set => TableFields[i] = value; }
    }
}