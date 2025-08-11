using TMPro;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class SafetyProceduresTabContent : TabContent
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _bodyText;

        public override TabType TabType {get; protected set;} = TabType.SafetyProcedures;

        public override void UpdateData(string title, string body)
        {
            string safetyProceduresTitle = title;
            if(safetyProceduresTitle == "") 
                safetyProceduresTitle = "Safety Procedures";
            _titleText.text = safetyProceduresTitle;
            _bodyText.text = body;
        }
    }
}