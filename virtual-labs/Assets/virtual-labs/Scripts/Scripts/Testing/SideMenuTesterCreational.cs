using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SideMenu
{
    public class SideMenuTesterCreational : MonoBehaviour
    {
        [SerializeField] private SideMenu _sideMenu;
        [Header("dummy prefabs")]
        [SerializeField] private SideMenuButton _stopwatchDummy;
        [SerializeField] private SideMenuButton _tableDummy;
        [SerializeField] private SideMenuButton _notificationDummy;

        [Header("demo inputs")]
        [SerializeField] private TMPro.TMP_Dropdown _sideButtonsDropdown;
        [SerializeField] private TMPro.TMP_Dropdown _newButtonDirectionDropdown;
        [SerializeField] private TMPro.TMP_Dropdown _newIslandDirectionDropdown;
        [SerializeField] private TMPro.TMP_InputField _islandIdNameInputField;
        [SerializeField] private TMPro.TMP_InputField _familyNameInputField;
        [SerializeField] private TMPro.TMP_InputField _buttonId;

        [ContextMenu("Add button")]
        public void AddButton()
        {
            SideMenuButton sideMenuButton = Instantiate(
                (_sideButtonsDropdown.value) switch
                {
                    0 => _stopwatchDummy,
                    1 => _tableDummy,
                    2 => _notificationDummy,
                    _ => throw new System.NotImplementedException()
                });
            sideMenuButton.Family = _familyNameInputField.text;
            sideMenuButton.Id = _buttonId.text;
            _sideMenu.AddButton(_islandIdNameInputField.text, sideMenuButton,
                DropdownToSideMenuPosition(_newButtonDirectionDropdown));
        }
        public void CreateIsland()
        {
            _sideMenu.CreateSideMenuIsland(_islandIdNameInputField.text,
                DropdownToSideMenuPosition(_newIslandDirectionDropdown));
        }

        private SideMenu.SideMenuPosition DropdownToSideMenuPosition(TMPro.TMP_Dropdown dropdown)
        {
            return (dropdown.value) switch
            {
                0 => SideMenu.SideMenuPosition.Top,
                1 => SideMenu.SideMenuPosition.Bottom,
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
