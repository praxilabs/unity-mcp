using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Table.UI.TableSettings.Types
{
    public abstract class TableSettingTypeBase : MonoBehaviour
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField, TextArea] public string LabelText { get; set; } = "Default label";
        [field: SerializeField] public TextMeshProUGUI LabelTextMesh { get; set; }
        [field: SerializeField] public RectTransform ItemRect { get; private set; }
        public abstract string StringValue { get; set; }
        public float PreferredHeight { get; protected set; }
        [ContextMenu("Initialize")]
        public void Initialize() => Initialize(false);
        [ContextMenu("InitializeSilent")]
        public void InitializeSilent() => Initialize(true);

        public virtual void Initialize(bool setValueWithoutNotify)
        {
            LabelTextMesh.text = LabelText;

            UpdateVisual(setValueWithoutNotify);
        }

        public void UpdateLabel(string labelText)
        {
            LabelText = labelText;
            LabelTextMesh.text = LabelText;
        }

        [ContextMenu("Update preferred height")]
        public virtual void UpdatePreferredHeight()
        {
            LabelTextMesh.CalculateLayoutInputVertical();
            PreferredHeight = LabelTextMesh.preferredHeight;
            ItemRect.sizeDelta = new Vector2(ItemRect.sizeDelta.x, PreferredHeight);
        }
        public virtual void UpdateVisual(bool setValueWithoutNotify) { }

    }
}