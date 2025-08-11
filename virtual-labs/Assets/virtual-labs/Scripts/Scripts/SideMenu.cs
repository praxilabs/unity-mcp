using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SideMenu
{
    public class SideMenu : MonoBehaviour
    {
        [SerializeField] protected SideMenuIsland _sideMenuIslandPrefab;
        /// <summary>
        /// Tells the default island creation position (top/bottom) that will be used if no position mentioned.
        /// </summary>
        [SerializeField] public SideMenuPosition DefaultSideMenuCreationPosition { get; set; } = SideMenuPosition.Top;
        [SerializeField] private List<SideMenuIsland> _islandsAvailable;
        [SerializeField] private RectTransform _sideMenuRectTransform;
        [SerializeField] private VerticalLayoutGroup _sideMenuVerticalLayout;

        /// <summary>
        /// Adds button <paramref name="sideMenuButton"/> to island of name <paramref name="islandNameId"/>
        /// </summary>
        /// <param name="islandNameId">Island name/id</param>
        /// <param name="sideMenuButton">Side button to be added</param>
        public void AddButton(string islandNameId, SideMenuButton sideMenuButton) => AddButton(islandNameId, sideMenuButton, DefaultSideMenuCreationPosition);

        /// <summary>
        /// Adds button <paramref name="sideMenuButton"/> to side menu island of name/id <paramref name="islandNameId"/>
        /// at position <paramref name="sideMenuPosition"/>
        /// </summary>
        /// <param name="islandNameId">Island name/id</param>
        /// <param name="sideMenuButton">Side menu button to be added</param>
        /// <param name="sideMenuPosition">Position where the button should be added</param>
        public void AddButton(string islandNameId,SideMenuButton sideMenuButton, SideMenuPosition sideMenuPosition)
        {
            SideMenuIsland islandOfId = _islandsAvailable.Find(x => x.NameID == islandNameId);
            if (islandOfId == null)
            { 
                Debug.LogError($"No island of name \'{islandNameId}\' found, create island before adding button to it..");
                return;
                //if we want to auto create island
                //islandOfId = CreateSideMenuIsland(islandNameId);
            }
            AddButton(islandOfId, sideMenuButton, sideMenuPosition);
        }
        /// <summary>
        /// Adds button <paramref name="sideMenuButton"/> to side menu island <paramref name="sideMenuIsland"/>
        /// </summary>
        /// <param name="sideMenuIsland">Side island to contain the button</param>
        /// <param name="sideMenuButton">Side menu button to be added</param>
        public void AddButton(SideMenuIsland sideMenuIsland, SideMenuButton sideMenuButton) =>
            AddButton(sideMenuIsland, sideMenuButton, DefaultSideMenuCreationPosition);
        /// <summary>
        /// Adds button <paramref name="sideMenuButton"/> to side menu island <paramref name="sideMenuIsland"/> at position <paramref name="sideMenuPosition"/>
        /// </summary>
        /// <param name="sideMenuIsland">Side island to contain the button</param>
        /// <param name="sideMenuButton">Side menu button to be added</param>
        /// <param name="sideMenuPosition">Position where new button will be placed</param>
        public void AddButton(SideMenuIsland sideMenuIsland, SideMenuButton sideMenuButton, SideMenuPosition sideMenuPosition)
        {
            sideMenuIsland.AddButton(sideMenuButton, sideMenuPosition);
            sideMenuIsland.Resize();
        }
        /// <summary>
        /// Removes all button(s) of id <paramref name="buttonId"/>
        /// </summary>
        /// <param name="buttonId">Button id to be removed</param>
        public void RemoveButton(string buttonId)
        {
            _islandsAvailable.ForEach(x => x.RemoveButton(buttonId));
        }
        /// <summary>
        /// Removes button <paramref name="sideMenuButton"/>
        /// </summary>
        /// <param name="sideMenuButton"></param>
        public void RemoveButton(SideMenuButton sideMenuButton)
        {
            foreach (var island in _islandsAvailable)
            {
                island.RemoveButton(sideMenuButton);
            }
        }
        /// <summary>
        /// Returns any buttons in children islands, expandable buttons with id mentioned.
        /// </summary>
        /// <returns>list of sidebuttons of requested id found in all islands</returns>
        public List<SideMenuButton> GetButtonsOfId(string buttonId)
        {
            List<SideMenuButton> sideMenuButtons=new List<SideMenuButton>();
            _islandsAvailable.ForEach(x => sideMenuButtons.AddRange(x.GetButtonsByID(buttonId)));
            return sideMenuButtons;
        }
        /// <summary>
        /// Tells whether or not there is a button with the id mentioned in the children islands or expandable buttons.
        /// </summary>
        /// <returns>True if any island has button with id</returns>
        public bool HasId(string id)
        {
            bool hasId = false;
            foreach(var island in _islandsAvailable)
            {
                hasId &= island.HasID(id);
                if (hasId)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Creates side menu island of **Unique** name id.
        /// If island with same id already exists, then no creation happens, and the existing one is returned
        /// </summary>
        /// <param name="nameId">New islands name/id </param>
        /// <returns>Newly created island or the island of this id if already exists</returns>
        public SideMenuIsland CreateSideMenuIsland(string nameId) => CreateSideMenuIsland(nameId, DefaultSideMenuCreationPosition);
        /// <summary>
        /// Creates side menu island of **Unique** name id, at the position mentioned.
        /// If island with same id already exists, then no creation happens, and the existing one is returned
        /// </summary>
        /// <param name="nameId"></param>
        /// <param name="sideMenuPosition"></param>
        /// <returns></returns>
        public SideMenuIsland CreateSideMenuIsland(string nameId, SideMenuPosition sideMenuPosition)
        {
            if (_islandsAvailable.Exists(x => x.NameID == nameId))
            {
                Debug.LogError($"SideMenuIsland of name \'{nameId}\' already exist. Cannot create island with same name. Returning existing island.");

                return _islandsAvailable.Find(x => x.NameID == nameId);
            }
            SideMenuIsland sideMenuInstance = Instantiate(_sideMenuIslandPrefab, transform);
            sideMenuInstance.NameID = nameId;
            switch (sideMenuPosition)
            {
                case SideMenuPosition.Bottom:
                    sideMenuInstance.transform.SetAsLastSibling();
                    break;
                case SideMenuPosition.Top:
                    sideMenuInstance.transform.SetAsFirstSibling();
                    break;
            }
            sideMenuInstance.OnResize += Resize;
            sideMenuInstance.Resize();
            _islandsAvailable.Add(sideMenuInstance);
            return sideMenuInstance;
        }
        /// <summary>
        /// Finds island by id
        /// </summary>
        public SideMenuIsland GetIslandById(string nameId)
        {
            return _islandsAvailable.Find(x => x.NameID == nameId);
        }
        /// <summary>
        /// Removes island of name id <paramref name="nameId"/>
        /// </summary>
        /// <param name="nameId">Id/name of island to be removed</param>
        public void RemoveSideMenuIsland(string nameId)
        {
            if (!_islandsAvailable.Exists(x => x.NameID == nameId))
            {
                Debug.LogWarning($"No side menu island with name \'{nameId}\' exists for removal.");
                return;
            }
            SideMenuIsland sideMenuInstance = _islandsAvailable.Find(x => x.NameID == nameId);
            sideMenuInstance.OnResize -= Resize;
            sideMenuInstance.Clear();
            _islandsAvailable.Remove(sideMenuInstance);
            sideMenuInstance.transform.SetParent(null);
        }

        private IEnumerator MarkForResize()
        {
            LayoutRebuilder.MarkLayoutForRebuild(_sideMenuRectTransform);
            yield return null;
            _sideMenuRectTransform.sizeDelta = new(_sideMenuRectTransform.sizeDelta.x, _sideMenuVerticalLayout.preferredHeight);
        }
        /// <summary>
        /// Resize the Size menu depending on contents
        /// </summary>
        public void Resize()
        {
            StopCoroutine(nameof(MarkForResize));
            StartCoroutine(nameof(MarkForResize));
        }
        /// <summary>
        /// Holds the position where new side menu object should be placed on creation
        /// </summary>
        public enum SideMenuPosition
        {
            Top,
            Bottom
        }
    }
}
