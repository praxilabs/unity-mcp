using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table.Structure
{
    public class TestTableJson : MonoBehaviour
    {
        [SerializeField]
        private string _tablePath = System.IO.Path.Combine(Application.dataPath, "TableData.json");

        [SerializeField]
        private Structure.Table _table;


        [ContextMenu("Table to Json")]
        public void TabeToJson()
        {
            string json = TableJsonConverter.Instance.ToJson(_table, true);
            //print($"TabeJson: \n{json}");
            System.IO.File.WriteAllText(_tablePath, json);
        }

        [ContextMenu("Json to Table")]
        public void JsonToTable()
        {
            string json = System.IO.File.ReadAllText(_tablePath);
            //print($"TabeJson: \n{json}");
            _table = TableJsonConverter.Instance.FromJson(json);
        }

    }
}
