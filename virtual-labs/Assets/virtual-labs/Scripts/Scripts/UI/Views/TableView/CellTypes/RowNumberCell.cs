using System.Collections;
using System.Collections.Generic;
using Table.UI.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.Views.TableViewElements.CellTypes
{
    public class RowNumberCell : BaseTableCell
    {
        [field: SerializeField] private TMPro.TextMeshProUGUI _cellText;
        [SerializeField] private Image _deleteButton;
        public override float PreferredWidth { get { _cellText.CalculateLayoutInputHorizontal(); return _cellText.preferredWidth; } }

        public override float PreferredHeight { get { _cellText.CalculateLayoutInputVertical(); return _cellText.preferredHeight; } }
        [SerializeField] private Color _normalDeleteIconColor;
        [SerializeField] private Color _disabledDeleteIconColor;
        public void UpdateDeleteButton()
        {
            _deleteButton.color = Row.CanDelete ? _normalDeleteIconColor : _disabledDeleteIconColor;
        }
        public void UpdateData(string editedText)
        {
            TableField.SetValue(editedText);
        }
        protected override void UpdateVisual(string value)
        {
            _cellText.text = value;
        }

        public void DeleteRow()
        {
            Row.DeleteRow();
        }
        public void HighlightRow()
        {
            Row.HighlightRow();
        }
        public void UnHighlightRow()
        {
            Row.UnHighlightRow();
        }
    }
}