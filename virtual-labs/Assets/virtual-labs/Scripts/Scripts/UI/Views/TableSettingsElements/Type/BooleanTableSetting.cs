using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.TableSettings.Types
{
    public class BooleanTableSetting : TableSettingTypeBase
    {
        [field: SerializeField] public bool Value { get; set; }
        //[SerializeField] string _stringValue;
        public override string StringValue
        {
            get => Value.ToString();
            set {
                Value = bool.TrueString.ToLower() == value.ToLower();
            }
        }

        [SerializeField] private Toggle _toggle;
        
        public override void UpdateVisual(bool setValueWithoutNotify)
        {
            LabelTextMesh.text = base.LabelText;
            if (setValueWithoutNotify)
                _toggle.SetIsOnWithoutNotify(Value);
            else
                _toggle.isOn = Value;
        }

    }
}