using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table.UI.Views.Buttons
{
    public class TableButton : MonoBehaviour
    {
        [field: Header("Headers with # index place holder")]
        [field: SerializeField] public string TableHeader { set; get; } = "Readings table #";
        [field: SerializeField] public string TableSubheader { get; set; }
        [SerializeField] private UnityEngine.UI.Button _myButton;
        public int TableIndex { get; set; }
        public TablesWindow TablesView { get; set; }

        [SerializeField] private TMPro.TextMeshProUGUI _tableHeaderTmp;
        [SerializeField] private TMPro.TextMeshProUGUI _tableSubheaderTmp;

        [SerializeField] private bool _active;

        public bool Active { get { return _active; } set { _active = value; _myButton.interactable = !_active; } }
        public void SetActive(bool active)
        {
            Active = active;
        }

        public void ActivateMyTable()
        {
            TablesView.Activate(TableIndex);
        }
        public void DeleteMyTable()
        {
            TablesView.DeleteTable(TableIndex);
        }
        public void DownloadMyTable()
        {
            TablesView.Download(TableIndex);
        }
        public void Terminate()
        {
            Destroy(this.gameObject);
        }

        public void Initialize(string tableHeader, string tableSubHeader)
        {
            _tableHeaderTmp.text = tableHeader.Replace("#", TableIndex.ToString());
            _tableSubheaderTmp.text = tableSubHeader.Replace("#", TableIndex.ToString());
        }
    }
}