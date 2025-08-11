using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.TableSettings.Types
{
    public class MultipleChoiceTableSetting : TableSettingTypeBase
    {
        [field: SerializeField] public string Value { get; set; }
        
        public override string StringValue
        {
            get => Value;
            set
            {
                if (Options.Exists(x => x == value))
                {
                    Value = value;
                }
                else
                    Debug.LogError($"MultiOption settings: option <{value}> doesn't exist in {Options}");

            }
        }
        [SerializeField] private TMPro.TMP_Dropdown _choicesDropdown;
        [field: SerializeField] public List<string> Options { get; set; }

        private Dictionary<string, int> _optionToIndex;

        public override void Initialize(bool setToggleWithoutNotify)
        {
            _optionToIndex = new Dictionary<string, int>();
            for (int i = 0; i < Options.Count; i++)
            {
                _optionToIndex.Add(Options[i], i);
            }
            if (!_optionToIndex.ContainsKey(Value))
                Value = Options[0];
            _choicesDropdown.ClearOptions();
            _choicesDropdown.AddOptions(Options);
            base.Initialize(setToggleWithoutNotify);
        }

        public override void UpdateVisual(bool setValueWithoutNotify)
        {
            if (setValueWithoutNotify)
                _choicesDropdown.SetValueWithoutNotify(_optionToIndex[Value]);
            else
                _choicesDropdown.value = _optionToIndex[Value];
        }
        public void SelectOption(int optionIndex)
        {
            Value = Options[optionIndex];
        }

        public override void UpdatePreferredHeight()
        {
            LabelTextMesh.CalculateLayoutInputVertical();
            PreferredHeight = LabelTextMesh.preferredHeight + 16;
            PreferredHeight = Mathf.Max(PreferredHeight, _choicesDropdown.itemText.preferredHeight + 16);
            ItemRect.sizeDelta = new Vector2(ItemRect.sizeDelta.x, PreferredHeight);
        }
    }
}