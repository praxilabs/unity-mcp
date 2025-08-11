using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Table.UI.TableSettings.Types
{
    public class TextOnlyTableSetting : TableSettingTypeBase
    {
        public override string StringValue { get => LabelText; set { LabelText = value; UpdateVisual(true); } }

        

        public override void Initialize(bool setToggleWithoutNotify)
        {
            base.Initialize(setToggleWithoutNotify);
        }
    }
}