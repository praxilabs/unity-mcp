using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformativeMessagesDispalySingleButton : InformativeMessagesDisplayBase
{
    [SerializeField] private TextMeshProUGUI _acceptButtonText;
    public override void UpdateButtonsText(InformativeMessagesDataScriptable infoSO)
    {
        _acceptButtonText.text = infoSO.AcceptButtonText;
    }
}
