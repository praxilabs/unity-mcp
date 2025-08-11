using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DescriptionTabContent : TabContent
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _bodyText;

        public override TabType TabType {get; protected set;} = TabType.Description;

        public override void UpdateData(string title, string body)
        {
            string descriptionTitle = title;
            if(descriptionTitle == "") 
                descriptionTitle = "Description";
            _titleText.text = descriptionTitle;
            _bodyText.text = body;
        }
    }
}