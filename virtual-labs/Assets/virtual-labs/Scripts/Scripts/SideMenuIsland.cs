using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SideMenu.SideMenu;

namespace SideMenu
{
    public class SideMenuIsland : MonoBehaviour
    {
        [SerializeField] private RectTransform _sideMenuIsland;
        [field: SerializeField] public string NameID { get; set; } = "Main Side Menu";
        private VerticalLayoutGroup _sideMenuLayout;
        private List<SideMenuButton> _sideMenuButtons = new();
        [SerializeField] private List<SideMenuButton> _preExistingButtons;
        /// <summary>
        /// Default position for created buttons if no position is specified.
        /// </summary>
        [SerializeField] public SideMenu.SideMenuPosition DefaultSideMenuPosition { get; set; }

        public event System.Action OnResize;
        [Header("Prefabs")]
        [SerializeField] protected SideMenuButtonExpandable _sideMenuButtonExpandablePrefab;


        private SideMenuButtonExpandable _expandedButton;

        private void Awake()
        {
            _sideMenuLayout = _sideMenuIsland.GetComponent<VerticalLayoutGroup>();
            _sideMenuButtons.AddRange(_preExistingButtons);
        }
        private void Start()
        {
            Resize();
        }
        private IEnumerator MarkForResize()
        {
            RectOffset padding= new RectOffset();
            if (_sideMenuButtons.Count == 0)
            {
                _sideMenuLayout.spacing = 0;
                padding = new(0, 0, 0, 0);
            }
            else
            {
                SideMenuButtonExpandable first = _sideMenuButtons[^1] as SideMenuButtonExpandable;
                SideMenuButtonExpandable last = _sideMenuButtons[0] as SideMenuButtonExpandable;

                padding.left = padding.right = (12);
                padding.top = (first != null && (!first.Collapsed || first.Collapsed && !first.ResizeComplete) ) ? 0 : 12;
                padding.bottom = (last != null && (!last.Collapsed || last.Collapsed && !last.ResizeComplete) )? 0 : 12;

                _sideMenuLayout.spacing = _sideMenuButtons[0] != _sideMenuButtons[_sideMenuButtons.Count - 1] ? 17 : 0;

            }
            _sideMenuLayout.padding = padding;
                
            LayoutRebuilder.MarkLayoutForRebuild(_sideMenuIsland);
            yield return null;
            _sideMenuIsland.sizeDelta = new(_sideMenuIsland.sizeDelta.x, _sideMenuLayout.preferredHeight);
            OnResize?.Invoke();
        }
        public void OnExpand() { }
        public void OnCollapse() { }
        /// <summary>
        /// Resize vertical size depending on contents.
        /// </summary>
        public void Resize()
        {
            if (_sideMenuButtons.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                gameObject.SetActive(true);
            }
            StopCoroutine(nameof(MarkForResize));
            StartCoroutine(nameof(MarkForResize));
        }

        public bool HasButtonOfFamily(string familyName) => _sideMenuButtons.Exists(x => x.Family == familyName);
        /// <summary>
        /// Adds button <paramref name="sideMenuButton"/> to menu
        /// Creates new expandable button and stack if same family exists.
        /// </summary>
        public void AddButton(SideMenuButton sideMenuButton) => AddButton(sideMenuButton, DefaultSideMenuPosition);
        /// <summary>
        /// Adds button <paramref name="sideMenuButton"/> to menu at position <paramref name="sideMenuPosition"/>.
        /// Creates new expandable button and stack if same family exists.
        /// </summary>
        public void AddButton(SideMenuButton sideMenuButton, SideMenu.SideMenuPosition sideMenuPosition)
        {
            SideMenuButton buttonOfMyFamily = _sideMenuButtons.Find(x => x.Family == sideMenuButton.Family);
            if (buttonOfMyFamily == null)
            {
                sideMenuButton.transform.SetParent(transform);
                _sideMenuButtons.Add(sideMenuButton);
                SetButtonPosition(sideMenuButton, sideMenuPosition);
            }
            else
            {
                SideMenuButtonExpandable buttonExpandable;
                if (buttonOfMyFamily is SideMenuButtonExpandable)
                {
                    buttonExpandable = buttonOfMyFamily as SideMenuButtonExpandable;
                }
                else
                {
                    int siblingIndex = buttonOfMyFamily.transform.GetSiblingIndex();
                    buttonExpandable = Instantiate(_sideMenuButtonExpandablePrefab, transform);
                    buttonExpandable.Family = buttonOfMyFamily.Family;
                    buttonExpandable.Icon = buttonOfMyFamily.Icon;
                    buttonExpandable.OnResizeBegin += Resize;
                    buttonExpandable.OnResizeComplete += Resize;
                    buttonExpandable.OnExpand += ButtonExpandable_OnExpand;
                    buttonExpandable.OnCollapse += ButtonExpandable_OnCollapse;
                    _sideMenuButtons.Remove(buttonOfMyFamily);
                    _sideMenuButtons.Add(buttonExpandable);
                    buttonExpandable.transform.SetSiblingIndex(siblingIndex);
                    buttonExpandable.AddButton(buttonOfMyFamily);
                    /// position button expandable
                    SetButtonPosition(buttonExpandable, sideMenuPosition);
                }
                buttonExpandable.AddButton(sideMenuButton);
                SetButtonPosition(sideMenuButton, sideMenuPosition);
            }
            Resize();
        }

        private void ButtonExpandable_OnCollapse(SideMenuButtonExpandable obj)
        {
            _expandedButton = null;
        }

        private void ButtonExpandable_OnExpand(SideMenuButtonExpandable obj)
        {
            _expandedButton?.Collapse();
            _expandedButton = obj;
        }
        /// <summary>
        /// Removes buttons of id <paramref name="id"/>
        /// </summary>
        public void RemoveButton(string id)
        {
            List<SideMenuButton> sideMenuButton = _sideMenuButtons.FindAll(x => x.Id == id);

            RemoveButtons(sideMenuButton);
            ///Expandable buttons
            RemoveFromExpandableButton(id);
            Resize();
        }
        /// <summary>
        /// Do buttons of <paramref name="id"/> exist
        /// </summary>
        public bool HasID(string id)
        {
            return _sideMenuButtons.Exists(x => x.Id == id || (x is SideMenuButtonExpandable) && (x as SideMenuButtonExpandable).HasID(id));
        }
        /// <summary>
        /// Gets all buttons in island, or in a child expandable button that has id <paramref name="id"/>.
        /// </summary>
        public List<SideMenuButton> GetButtonsByID(string id)
        {
            List<SideMenuButton> buttonsWithId=new List<SideMenuButton>();
            buttonsWithId.AddRange(_sideMenuButtons.FindAll(x => x.Id == id));
            List<SideMenuButtonExpandable> sideMenuButtonExpandables = new List<SideMenuButtonExpandable>();
            _sideMenuButtons.FindAll(x => x is SideMenuButtonExpandable && (x as SideMenuButtonExpandable).HasID(id))
                .ForEach(x => sideMenuButtonExpandables.Add((SideMenuButtonExpandable)x));

            foreach (SideMenuButtonExpandable sideMenuButtonIterator in sideMenuButtonExpandables)
            {
                buttonsWithId.Add(sideMenuButtonIterator);
            }
            return buttonsWithId;
        }
        private void RemoveButtons(List<SideMenuButton> sideMenuButton)
        {
            if (sideMenuButton.Count > 0)
            {
                while (sideMenuButton.Count > 0)
                {
                    SideMenuButton button = sideMenuButton[0];
                    if (button is SideMenuButtonExpandable)
                        (button as SideMenuButtonExpandable).Clear();
                    sideMenuButton.Remove(button);
                    RemoveButton(button);
                }
            }
        }
        /// <summary>
        /// Removes button <paramref name="sideMenuButton"/>
        /// </summary>
        public void RemoveButton(SideMenuButton sideMenuButton)
        {
            if (_sideMenuButtons.Exists(x => x == sideMenuButton))
            {
                if (sideMenuButton is SideMenuButtonExpandable)
                {
                    RemoveButtonExpandable(sideMenuButton as SideMenuButtonExpandable);
                    return;
                }
                else
                {
                    _sideMenuButtons.Remove(sideMenuButton);
                    sideMenuButton.transform.SetParent(null);
                }
            }
            else
            {
                if (_sideMenuButtons.Exists(x => x.Family == sideMenuButton.Family))
                {
                    SideMenuButtonExpandable buttonExpandable = _sideMenuButtons.Find(x => x.Family == sideMenuButton.Family) as SideMenuButtonExpandable;
                    buttonExpandable.RemoveButton(sideMenuButton);
                }
            }

            Resize();
        }
        private void RemoveFromExpandableButton(string id)
        {
            List<SideMenuButtonExpandable> sideMenuButtonExpandables=new List<SideMenuButtonExpandable>();
            _sideMenuButtons.FindAll(x => x is SideMenuButtonExpandable && (x as SideMenuButtonExpandable).HasID(id))
                .ForEach(x => sideMenuButtonExpandables.Add((SideMenuButtonExpandable)x));

            foreach (SideMenuButtonExpandable sideMenuButtonIterator in sideMenuButtonExpandables)
            {
                sideMenuButtonIterator.RemoveButton(id);
                HandleExpandableButtonWithOneButton(sideMenuButtonIterator);
                
            }
        }
        private void HandleExpandableButtonWithOneButton(SideMenuButtonExpandable sideMenuButtonExpandable)
        {
            SideMenuButton sideMenuButton;
            if(sideMenuButtonExpandable.RemoveIfLastButton(out sideMenuButton))
            {
                sideMenuButtonExpandable.Family = "ToBeRemoved";
                AddButton(sideMenuButton);
                sideMenuButton.transform.SetSiblingIndex(sideMenuButtonExpandable.transform.GetSiblingIndex());
                sideMenuButton.gameObject.SetActive(true);
                RemoveButtonExpandable(sideMenuButtonExpandable);
            }
        }
        private void SetButtonPosition(SideMenuButton sideMenuButton, SideMenu.SideMenuPosition sideMenuPosition)
        {
            switch (sideMenuPosition)
            {
                case SideMenu.SideMenuPosition.Top:
                    sideMenuButton.transform.SetAsFirstSibling();
                    break;
                case SideMenu.SideMenuPosition.Bottom:
                    sideMenuButton.transform.SetAsLastSibling();
                    if (!_sideMenuButtons.Exists(x => x == sideMenuButton))
                        break;
                    SideMenuButton temp = _sideMenuButtons[_sideMenuButtons.Count - 1];
                    for (int i = _sideMenuButtons.Count - 1; i > 0; i--)
                    {
                        _sideMenuButtons[i] = _sideMenuButtons[i - 1];
                    }
                    _sideMenuButtons[0] = temp;
                    break;
                default:
                    Debug.LogError($"Error, un expected SideMenuPosition value: <{(int)sideMenuPosition}>");
                    break;
            }
        }
        
        /// <summary>
        /// Removes Expandable button <paramref name="sideMenuButtonExpandable"/>
        /// </summary>
        public void RemoveButtonExpandable(SideMenuButtonExpandable sideMenuButtonExpandable)
        {
            if (_sideMenuButtons.Exists(x => (x as SideMenuButtonExpandable) == sideMenuButtonExpandable))
            {

                sideMenuButtonExpandable.OnResizeBegin -= Resize;
                sideMenuButtonExpandable.OnResizeComplete -= Resize;
                sideMenuButtonExpandable.OnExpand -= ButtonExpandable_OnExpand;
                sideMenuButtonExpandable.OnCollapse -= ButtonExpandable_OnCollapse;

                _sideMenuButtons.Remove(sideMenuButtonExpandable);
                sideMenuButtonExpandable.Clear();
                if (_expandedButton == sideMenuButtonExpandable)
                    _expandedButton = null;
                sideMenuButtonExpandable.transform.SetParent(null);
            }
        }
        /// <summary>
        /// Clears the island of any buttons.
        /// </summary>
        public void Clear()
        {
            while (_sideMenuButtons.Count > 0)
                RemoveButton(_sideMenuButtons[0]);
        }
    }
}
