using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.Views.TableViewElements.CellTypes
{
    public class MultipleOptionTableCell : BaseTableCell
    {
        [field: SerializeField] private TMPro.TMP_Dropdown _cellOptionsDropdown;
        [field: SerializeField] public Vector2 PreferredSize = new Vector2(300, 50);
        private Dictionary<string, int> _optionsToIndex;


        [field: SerializeField] public List<string> Options { set; get; }
        public override float PreferredWidth => PreferredSize.x;
        public override float PreferredHeight => PreferredSize.y;


        public override void Initialize()
        {
            _cellOptionsDropdown.ClearOptions();
            _cellOptionsDropdown.AddOptions(Options);
            _optionsToIndex = new Dictionary<string, int>(Options.Count);
            for (int i = 0; i < Options.Count; i++)
            {

                _optionsToIndex.Add(Options[i], i);
            }
            _cellOptionsDropdown.interactable = TableField.Editable;
            base.Initialize();
        }
        public void UpdateData(int selectedOption)
        {
            TableField.SetValue(_cellOptionsDropdown.options[selectedOption].text.ToString());
        }
        protected override void UpdateVisual(string value)
        {
            int index = _cellOptionsDropdown.value;
            _optionsToIndex.TryGetValue(value, out index);
            _cellOptionsDropdown.SetValueWithoutNotify(index);
            OnCellUpdateInvoke();
        }
    }
}