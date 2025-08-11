using UnityEngine;
using UnityEngine.UI;
using Praxilabs;

namespace Praxilabs.DeviceSideMenu
{
    public class TabSideMenuUI : TabUI 
    {
        [field: SerializeField] public TabType Type {get; set;}
    }
}