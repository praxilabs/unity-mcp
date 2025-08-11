using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SideMenu
{
    public class SideMenuTesterDeletional : MonoBehaviour
    {
        [SerializeField] private SideMenu _sideMenu;
        [Header("demo inputs")]
        [SerializeField] private TMPro.TMP_InputField _islandIdNameInputField;
        [SerializeField] private TMPro.TMP_InputField _buttonId;

        [ContextMenu("Add button")]
        public void RemoveButton()
        {
            _sideMenu.RemoveButton(_buttonId.text);
        }
        public void RemoveIsland()
        {
            _sideMenu.RemoveSideMenuIsland(_islandIdNameInputField.text);
        }
    }
}
