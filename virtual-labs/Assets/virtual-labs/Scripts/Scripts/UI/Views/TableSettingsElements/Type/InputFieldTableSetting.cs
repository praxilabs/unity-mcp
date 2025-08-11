using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.TableSettings.Types
{
    public class InputFieldTableSetting : TableSettingTypeBase
    {
        [field: SerializeField] public string Value { get; set; }
        public override string StringValue
        {
            get => Value;
            set
            {
                Value = value;
            }
        }
        [SerializeField] private TMPro.TMP_InputField _inputField;

        public override void Initialize(bool setToggleWithoutNotify)
        {
            base.Initialize(setToggleWithoutNotify);
        }

        public override void UpdateVisual(bool setValueWithoutNotify)
        {
            if (setValueWithoutNotify)
                _inputField.SetTextWithoutNotify(Value);
            else
                _inputField.text = Value;
        }
        public override void UpdatePreferredHeight()
        {
            LabelTextMesh.CalculateLayoutInputVertical();
            PreferredHeight = LabelTextMesh.preferredHeight;
            PreferredHeight = Mathf.Max(PreferredHeight, _inputField.preferredHeight);
            ItemRect.sizeDelta = new Vector2(ItemRect.sizeDelta.x, PreferredHeight);
        }
    }
}