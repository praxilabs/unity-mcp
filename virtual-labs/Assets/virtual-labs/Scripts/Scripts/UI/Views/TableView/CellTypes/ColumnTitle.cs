using System.Collections;
using System.Collections.Generic;
using Table.UI.Views.TableViewStructure;
using UnityEngine;

namespace Table.UI.Views.TableViewElements.CellTypes
{
    public class ColumnTitle : BaseTableCell
    {
        [SerializeField] private TMPro.TextMeshProUGUI _titleText;
        private Vector2 _sizePadding;

        public override float PreferredWidth { get { _titleText.CalculateLayoutInputHorizontal(); return _titleText.preferredWidth; } }

        public override float PreferredHeight { get { _titleText.CalculateLayoutInputVertical(); return _titleText.preferredHeight; } }

        public void UpdateTitle(string title)
        {
            _titleText.text = title;
        }

        public void SetColumn(Column column)
        {
            UpdateTitle(column.ColumnName);
            _sizeDelta = _rectTransform.sizeDelta;
            _sizeDelta.x = column.ColumnWidth;
            _sizeDelta.y = _titleText.preferredHeight + _sizePadding.y;
            _rectTransform.sizeDelta = _sizeDelta;
        }

        protected override void UpdateVisual(string value)
        {
            _titleText.text = value;
        }

        public override void Initialize()
        {
            UpdateVisual(TableField.Name);
        }
    }
}