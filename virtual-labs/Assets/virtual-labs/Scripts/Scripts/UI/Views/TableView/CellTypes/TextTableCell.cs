using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.Views.TableViewElements.CellTypes
{
    public class TextTableCell : BaseTableCell
    {
        [field: SerializeField] private TMPro.TMP_InputField _cellInputField;
        [SerializeField] private List<Structure.Validators.TableFieldValidator> _validators;
        public override float PreferredWidth { get { _cellInputField.CalculateLayoutInputHorizontal(); return _cellInputField.preferredWidth; } }

        public override float PreferredHeight
        {
            get
            {
                _cellInputField.CalculateLayoutInputHorizontal();
                _cellInputField.CalculateLayoutInputVertical();
                return _cellInputField.preferredHeight;
            }
        }
        private void Start()
        {
            _cellInputField.interactable = TableField.Editable;
            
        }
        public void UpdateData(string editedText)
        {
            bool valid = true;

            foreach (Structure.Validators.TableFieldValidator validator in _validators)
            {
                valid &= validator.Validate(editedText);
                if (!valid)
                    break;
            }
            if (valid)
                TableField.SetValue(editedText);

        }
        protected override void UpdateVisual(string value)
        {
            _cellInputField.SetTextWithoutNotify( value);

            if(Row==null)
            {
                return;
            }
            base.OnCellUpdateInvoke();
        }

    }
}