using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Table.UI.SideMenuButtons
{
    public class RecordReadingsButton : MonoBehaviour
    {
        public UnityEvent<Dictionary<string, string>> recordLastRow;
        public UnityEvent<int, Dictionary<string, string>> recordRow;

        public TablesManager tablesManager;

        [field: SerializeField] public List<CellData> CellsData { get; set; }

        private UnityAction<Dictionary<string, string>> _recordLastRowDelegate;
        private UnityAction<int, Dictionary<string, string>> _recordRowDelegate;

        private bool _addToLastRow;

        private void OnDestroy()
        {
            RemoveRecordEvent();
        }

        public void AddRecordEvent(bool addToLastRow)
        {
            _addToLastRow = addToLastRow;

            RemoveRecordEvent();

            _recordLastRowDelegate = tablesManager.SetLastRowData;
            _recordRowDelegate = tablesManager.SetRowData;

            if (_addToLastRow)
                recordLastRow.AddListener(tablesManager.SetLastRowData);
            else
                recordRow.AddListener(tablesManager.SetRowData);
        }

        public void RemoveRecordEvent()
        {
            if (_recordLastRowDelegate != null)
                recordLastRow.RemoveListener(_recordLastRowDelegate);
            if (_recordRowDelegate != null)
                recordRow.RemoveListener(_recordRowDelegate);
        }

        public void SendRecord(int row = 0)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var cellData in CellsData)
                keyValuePairs.Add(cellData.FieldName, cellData.Value);

            if (_addToLastRow)
                recordLastRow?.Invoke(keyValuePairs);
            else
                recordRow?.Invoke(row, keyValuePairs);
        }
    }

    [System.Serializable]
    public struct CellData
    {
        [field: SerializeField] public string FieldName { get; set; }
        [field: SerializeField] public string Value { get; set; }
    }
}
