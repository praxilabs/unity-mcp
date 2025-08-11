using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformativeMessagesDisplayDualButtons : InformativeMessagesDisplayBase
{
    [SerializeField] private TextMeshProUGUI _acceptButtonText;
    [SerializeField] private TextMeshProUGUI _ignoreButtonText;

    public override void UpdateButtonsText(InformativeMessagesDataScriptable infoSO)
    {
        _acceptButtonText.text = infoSO.AcceptButtonText;
        _ignoreButtonText.text = infoSO.IgnoreButtonText;
    }
}
