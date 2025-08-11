using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.Views.TableViewElements.CellTypes
{
    public class CheckBoxTableCell : BaseTableCell
    {
        [SerializeField] private Toggle _toggle;

        //For caching first size.
        private RectTransform _toggleUIRect;
        public override float PreferredWidth => _toggleUIRect ?
            _toggleUIRect.sizeDelta.x :
            (_toggleUIRect = _toggle.GetComponent<RectTransform>()).sizeDelta.x;
        public override float PreferredHeight => _toggleUIRect ?
            _toggleUIRect.sizeDelta.y :
            (_toggleUIRect = _toggle.GetComponent<RectTransform>()).sizeDelta.y;
        /// <summary>
        /// Updates table field value, depending on <paramref name="checkBoxValue"/>.
        /// </summary>
        public void UpdateData(bool checkBoxValue)
        {
            TableField.SetValue(checkBoxValue.ToString());
        }
        
        public override void Initialize()
        {

            _toggle.interactable = TableField.Editable;
            base.Initialize();
        }
        /// <summary>
        /// Sets the toggle to 'On' if the <paramref name="value"/> is equal "true", and off otherwise.<br/>
        /// *Not case sensitive*
        /// </summary>
        /// <param name="value">Checkbox value, "True" sets it on, anything else sets to off </param>
        protected override void UpdateVisual(string value)
        {
            _toggle.SetIsOnWithoutNotify(value.ToLower() == "true");
        }
    }
}