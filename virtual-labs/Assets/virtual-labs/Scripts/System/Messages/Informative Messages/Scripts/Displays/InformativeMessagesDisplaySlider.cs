using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformativeMessagesDisplaySlider : InformativeMessagesDisplayBase
{
    [SerializeField] private TextMeshProUGUI _sliderText;

    public override void UpdateButtonsText(InformativeMessagesDataScriptable infoSO)
    {
        _sliderText.text = infoSO.SliderText;
    }
}
