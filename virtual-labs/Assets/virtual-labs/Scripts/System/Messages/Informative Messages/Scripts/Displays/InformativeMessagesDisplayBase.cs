
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class InformativeMessagesDisplayBase : MonoBehaviour,IUpdateTextBehavior
{
    [Header("Header Message")]
    [SerializeField] private Image _headerMessageImage;
    [SerializeField] private TextMeshProUGUI _headerMessageTitleText;
    [SerializeField] private TextMeshProUGUI _headerMessageBodyText;
    [Header("More Info")]
    [SerializeField] private TextMeshProUGUI _moreInfoText;
    [SerializeField] private Image _moreInfoTitleImage;
    [SerializeField] private GameObject _moreInfoBodyVideo;

    public Image HeaderMessageImage => _headerMessageImage;
    public TextMeshProUGUI HeaderMessageTitleText => _headerMessageTitleText;
    public TextMeshProUGUI HeaderMessageBodyText => _headerMessageBodyText;
    public TextMeshProUGUI MoreInfoText => _moreInfoText;
    public Image MoreInfoImage => _moreInfoTitleImage;
    public GameObject MoreInfoBodyVideo => _moreInfoBodyVideo;

    public abstract void UpdateButtonsText(InformativeMessagesDataScriptable _infoMessageSO);

    public void InitializeUI(InformativeMessagesDataScriptable _infoMessageSO)
    {

        /* if(_infoMessageSO.HeaderMessageImage != null)
         {
            _infoMessageDisplayObject.HeaderMessageImage.sprite = _infoMessageSO.HeaderMessageImage;
             //_infoMessageDisplayObject.HeaderMessageImage.gameObject.SetActive(false);
         }*/

        HeaderMessageTitleText.text = _infoMessageSO.HeaderMessageTitleText;
        HeaderMessageBodyText.text = _infoMessageSO.HeaderMessageBodyText;
        MoreInfoText.text = _infoMessageSO.MoreInfoText;
    }
}
