using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiddleMessage : MonoBehaviour
{
    [field: SerializeField, Header("Buttons")] public Button CloseButton {get; set;}
    [field: SerializeField] public Button ActionOneButton {get; set;}
    [field: SerializeField] public Button ActionTwoButton {get; set;}

    [Header("Message")]
    [SerializeField] private TextMeshProUGUI _messageTitle;
    [SerializeField] private TextMeshProUGUI _messageTitleDescription;
    [SerializeField] private TextMeshProUGUI _messageHeader;
    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private TextMeshProUGUI _actionOneTxt;
    [SerializeField] private TextMeshProUGUI _actionTwoTxt;

    [Header("Other"), Space]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private UILayoutRefresher _layoutRefresher;
    [SerializeField] private DynamicScrollView _dynamicScrollView;

    public void Setup(MiddleMessageData middleMessageData)
    {
        _canvasGroup.alpha = 0;

        SetUpText(middleMessageData);
        RegisterButtons();
        LayoutRefresh();
    }

    public void SetUpText(MiddleMessageData middleMessageData)
    {
        _messageTitle.text = middleMessageData.MessageTitle;
        _messageHeader.text = middleMessageData.MessageHeader;
        _message.text = middleMessageData.Message;
        _actionOneTxt.text = middleMessageData.ActionOneText;
        ShowTitleDescription(middleMessageData.TitleDescription);

        if (_actionTwoTxt != null)
        {
            _actionTwoTxt.text = middleMessageData.ActionTwoText;
        }
    }
    
    private void RegisterButtons()
    {
        CloseButton.onClick.AddListener(WipeWindow);
        ActionOneButton.onClick.AddListener(WipeWindow);
        ActionTwoButton?.onClick.AddListener(WipeWindow);
    }

    private void ShowTitleDescription(string titleDescription)
    {
        if(string.IsNullOrEmpty(titleDescription)) return;

        _messageTitleDescription.text = titleDescription;
        _messageTitleDescription.gameObject.SetActive(true);
    }

    private void LayoutRefresh()
    {
        StartCoroutine(LayoutRefreshCoroutine());
    }

    private IEnumerator LayoutRefreshCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _dynamicScrollView.ContentUpdated();
        _layoutRefresher.Refresh();
        yield return new WaitForEndOfFrame();
        _canvasGroup.alpha = 1;
    }

    private void WipeWindow()
    {
        MiddleMessagesManager.Instance.ToggleBackground();
        Destroy(gameObject);
    }
}
