using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadExcelMiddlePromptMessageManager : MonoBehaviour
{
    [SerializeField] private Button _downloadExcelButton;
    [SerializeField] private string _messageBody = "The Excel sheet is safe to open.";

    private void Start()
    {
        _downloadExcelButton.onClick.AddListener(ShowPromptMessage);
    }
    private void ShowPromptMessage()
    {
        MiddleMessageData middleMessageData = new MiddleMessageData
        {
            MessageTitle = "Download",
            MessageHeader = "Download Confirmation",
            Message = _messageBody,
            ActionOneText = "Confirm",
        };
        MiddleMessage middleMessage = MiddleMessagesManager.Instance.OpenMiddleMessage(MessageType.Informative, MiddleMessageType.OneAction, middleMessageData);
    }
    
}
