
using UnityEngine;




public class InformativeMessagesManager : MonoBehaviour
{
    private InformativeMessagesDataScriptable _infoMessageSO;
    private InformativeMessagesDisplayBase _infoMessageDisplayObject;
    [SerializeField] private GameObject _mainContainer;
    [SerializeField] private GameObject _bodyContainer;

    private RectTransform _moreInfoBodyVideoRect, _moreInfoBodyImageRect;

    public void Open(InformativeMessagesDataScriptable _infoMessageScriptable)
    {
        _infoMessageSO = _infoMessageScriptable;
        _infoMessageDisplayObject = GetComponent<InformativeMessagesDisplayBase>();
        InitializeComponents();
    }

    private void InitializeComponents()
    {

        Vector2 mainContainersizeDelta = _mainContainer.GetComponent<RectTransform>().sizeDelta;
        Vector2 bodyContainersizeDelta = _bodyContainer.GetComponent<RectTransform>().sizeDelta;
        _moreInfoBodyVideoRect = _infoMessageDisplayObject.MoreInfoBodyVideo.gameObject.GetComponent<RectTransform>();
        _moreInfoBodyImageRect = _infoMessageDisplayObject.MoreInfoImage.gameObject.GetComponent<RectTransform>();

        if (_infoMessageSO != null)
        {

            _infoMessageDisplayObject.InitializeUI(_infoMessageSO);
            _infoMessageDisplayObject.UpdateButtonsText(_infoMessageSO);


            UpdateMoreInfoImage(ref mainContainersizeDelta, ref bodyContainersizeDelta);
            UpdateMoreInfoVideo(ref mainContainersizeDelta, ref bodyContainersizeDelta);

            _mainContainer.GetComponent<RectTransform>().sizeDelta = mainContainersizeDelta;
            _bodyContainer.GetComponent<RectTransform>().sizeDelta = bodyContainersizeDelta;

            
        }

    }
    private void UpdateMoreInfoImage(ref Vector2 mainContainersizeDelta, ref Vector2 bodyContainersizeDelta)
    {

        if (_infoMessageSO.MoreInfoImage != null)
        {
            _infoMessageDisplayObject.MoreInfoImage.gameObject.SetActive(true);
            _infoMessageDisplayObject.MoreInfoImage.sprite = _infoMessageSO.MoreInfoImage;
        }
        else
        {
            _infoMessageDisplayObject.MoreInfoImage.gameObject.SetActive(false);
            mainContainersizeDelta.y -= _moreInfoBodyImageRect.sizeDelta.y;
            bodyContainersizeDelta.y -= _moreInfoBodyImageRect.sizeDelta.y;
        }


    }
    private void UpdateMoreInfoVideo(ref Vector2 mainContainersizeDelta, ref Vector2 bodyContainersizeDelta)
    {


        if (_infoMessageSO.MoreInfoBodyVideo != null)
        {
            _infoMessageDisplayObject.MoreInfoBodyVideo.gameObject.SetActive(true);
            GameObject tmp = Instantiate(_infoMessageSO.MoreInfoBodyVideo, _infoMessageDisplayObject.MoreInfoBodyVideo.gameObject.transform);
        }
        else
        {
            _infoMessageDisplayObject.MoreInfoBodyVideo.gameObject.SetActive(false);
            mainContainersizeDelta.y -= _moreInfoBodyVideoRect.sizeDelta.y;
            bodyContainersizeDelta.y -= _moreInfoBodyVideoRect.sizeDelta.y;
        }


    }

}


