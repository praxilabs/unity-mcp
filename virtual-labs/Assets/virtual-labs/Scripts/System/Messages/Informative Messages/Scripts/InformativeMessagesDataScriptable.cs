using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Informative Message Data", menuName = "Messages/Informative Message")]
public class InformativeMessagesDataScriptable : ScriptableObject
{
    [SerializeField] private Sprite _headerMessageImage;
    [SerializeField] private string _headerMessageTitleText;
    [SerializeField] private string _headerMessageBodyText;

    [SerializeField] private string _moreInfoText;
    [SerializeField] private Sprite _moreInfoTitleImage;
    [SerializeField] private GameObject _moreInfoBodyVideo;

    [SerializeField] private string _acceptButtonText;
    [SerializeField] private string _ignoreButtonText;
    [SerializeField] private string _sliderText;


    public Sprite HeaderMessageImage { get => _headerMessageImage; set => _headerMessageImage = value; }
    public string HeaderMessageTitleText { get => _headerMessageTitleText; set => _headerMessageTitleText = value; }
    public string HeaderMessageBodyText { get => _headerMessageBodyText; set => _headerMessageBodyText = value; }
    public string MoreInfoText { get => _moreInfoText; set => _moreInfoText = value; }
    public Sprite MoreInfoImage { get => _moreInfoTitleImage; set => _moreInfoTitleImage = value; }
    public GameObject MoreInfoBodyVideo { get => _moreInfoBodyVideo; set => _moreInfoBodyVideo = value; }
    public string AcceptButtonText { get => _acceptButtonText; set => _acceptButtonText = value; }
    public string IgnoreButtonText { get => _ignoreButtonText; set => _ignoreButtonText = value; }
    public string SliderText { get => _sliderText; set => _sliderText = value; }
}
