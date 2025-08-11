using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SideMenu
{
    public class SideMenuButtonExpandable : SideMenuButton
    {
        [SerializeField] protected List<SideMenuButton> _buttons;
        [Header("Expandable related")]
        [SerializeField] protected GameObject _purpleBackground;
        [SerializeField] protected Image _grayBackground;
        [SerializeField] private RectTransform _expandableButton;
        private RectTransform _sideMenuIsland;
        private VerticalLayoutGroup _sideMenuLayout;
        private VerticalLayoutGroup _expandableButtonLayout;
        public event System.Action OnResizeComplete;
        public event System.Action OnResizeBegin;
        public event System.Action<SideMenuButtonExpandable> OnExpand;
        public event System.Action<SideMenuButtonExpandable> OnCollapse;
        [SerializeField] private float _resizingDifferenceThreshold = 0.05f;
        [SerializeField] private float _resizeSpeed = 75f;
        private bool _collapsing;
        private bool _resizeComplete;
        public bool ResizeComplete => _resizeComplete;
        public bool Collapsed => _collapsing;
        [SerializeField] private RectTransform _expandableButtonScroll;
        [SerializeField] private RectTransform _expandableButtonScrollContent;
        [SerializeField] private VerticalLayoutGroup _expandableButtonScrollLayout;
        [SerializeField] private float _maxVerticalSize = 160;

        private void Awake()
        {
            _sideMenuIsland = transform.parent.GetComponent<RectTransform>();
            _sideMenuLayout = _sideMenuIsland.GetComponent<VerticalLayoutGroup>();
            _expandableButtonLayout = _expandableButton.GetComponent<VerticalLayoutGroup>();
            _collapsing = true;
            //Initialize();
            //Collapse();
        }
        private void Initialize()
        {
            _expandableButtonLayout.padding = new RectOffset(0, 0, 0, 0);
            _expandableButtonLayout.spacing = 0;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButton);
            _expandableButton.sizeDelta = new(_expandableButton.sizeDelta.x, _expandableButtonLayout.preferredHeight);
            _expandableButtonScrollContent.sizeDelta = new(_expandableButtonScrollContent.sizeDelta.x, _expandableButtonScrollLayout.preferredHeight);
            _expandableButtonScroll.sizeDelta = new(_expandableButtonScroll.sizeDelta.x, Mathf.MoveTowards(_expandableButtonScroll.sizeDelta.y, _expandableButtonScrollLayout.preferredHeight, _resizeSpeed));

            _grayBackground.enabled = false;
            _purpleBackground.SetActive(false);
        }
        /// <summary>
        /// Adds button <paramref name="sideMenuButton"/> to the expandable button.
        /// </summary>
        public void AddButton(SideMenuButton sideMenuButton)
        {
            if(sideMenuButton is SideMenuButtonExpandable)
            {
                Debug.LogError($"Cannot add expandable button {sideMenuButton} to expandable button {this}." +
                    $"\n" +
                    $"Expandable under expandable is not allowed !");
                return;
            }
            sideMenuButton.transform.SetParent(_expandableButtonScrollContent);
            sideMenuButton.gameObject.SetActive(!_collapsing);
            _buttons.Add(sideMenuButton);
            Debug.Log($"COllapsing = {_collapsing}");
            if(!_collapsing)
            {
                StopCoroutine(nameof(ResizeExpandableButton));
                StartCoroutine(nameof(ResizeExpandableButton));
            }
        }
        /// <summary>
        /// Removes button <paramref name="sideMenuButton"/> from the expandable button.
        /// </summary>
        public void RemoveButton(SideMenuButton sideMenuButton)
        {
            if (!_buttons.Exists(x => x == sideMenuButton))
            {
                Debug.Log($"Button {sideMenuButton} is not found in {this}");
                return;
            }


            _buttons.Remove(sideMenuButton);
            sideMenuButton.transform.SetParent(null);
            if (!_collapsing)
            {
                StopCoroutine(nameof(ResizeExpandableButton));
                StartCoroutine(nameof(ResizeExpandableButton));
            }
        }
        /// <summary>
        /// Removes  all buttons of id <paramref name="Id"/> from the expandable button.
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveButton(string Id)
        {
            List<SideMenuButton> sideMenuButton = GetButtonsByID(Id);
            if(sideMenuButton.Count>0)
            {
                while (sideMenuButton.Count > 0)
                {
                    SideMenuButton button = sideMenuButton[0];
                    sideMenuButton.Remove(button);
                    RemoveButton(button);
                }
            }
        }
        /// <summary>
        /// Has buttons of id '<paramref name="id"/>'
        /// </summary>
        /// <param name="id">Id to check for</param>
        /// <returns></returns>
        public bool HasID(string id)
        {
            return _buttons.Exists(x => x.Id == id);
        }
        /// <summary>
        /// Get existing buttons that has id '<paramref name="id"/>'
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<SideMenuButton> GetButtonsByID(string id)
        {
            return _buttons.FindAll(x => x.Id == id);
        }
        public bool RemoveIfLastButton(out SideMenuButton sideMenuButton)
        {
            if(_buttons.Count==1)
            {
                sideMenuButton = _buttons[0];
                RemoveButton(sideMenuButton);
                return true;
            }
            sideMenuButton = null;
            return false;
        }
        /// <summary>
        /// Removes all buttons from the expandable button
        /// </summary>
        public void Clear()
        {
            while (_buttons.Count > 0)
                RemoveButton(_buttons[0]);
            
        }
        /// <summary>
        /// Click, called on clicking the button.<br></br>
        /// Here it toggles expand/collapse if there is more than 1 button., or calls button click if theres only one button, and it calls self on button click event.
        /// </summary>
        public override void Click()
        {
            switch(_buttons.Count)
            {
                case 0:
                    Debug.Log($"Expandable side menu button has no children!");
                    return;
                case 1:
                    _buttons[0].Click();
                    break;
                case > 1:
                    Toggle();
                    break;
                default:
                    Debug.LogError($"Unhandled case with button count: <{_buttons.Count}>");
                    break;
            }
            OnResizeBegin?.Invoke();
            onButtonClick?.Invoke();
        }
        [ContextMenu("Expand")]
        public void Expand()
        {
            foreach (var item in _buttons)
            {
                item.gameObject.SetActive(true);
            }
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButton);
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButtonScroll);
            _grayBackground.enabled = true;
            _purpleBackground.SetActive(true);

            _expandableButtonLayout.padding = new RectOffset(12, 12, 12, 12);
            _expandableButtonLayout.spacing = 17;
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_sideMenu);
            StopCoroutine(nameof(ResizeExpandableButton));
            StartCoroutine(nameof(ResizeExpandableButton));
            OnExpand?.Invoke(this);
            _collapsing = false;
            _resizeComplete = false;
        }
        [ContextMenu("Collapse")]
        public void Collapse()
        {
            foreach (var item in _buttons)
            {
                item.gameObject.SetActive(false);
            }
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButton);
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButtonScroll);
            
            StopCoroutine(nameof(ResizeExpandableButton));
            StartCoroutine(nameof(ResizeExpandableButton));
            OnCollapse?.Invoke(this);
            _collapsing = true;
            _resizeComplete = false;
        }
        /// <summary>
        /// Collapses if expanded
        /// <br/>or<br/>
        /// Expands if collapsed
        /// </summary>
        public void Toggle()
        {
            if (_collapsing)
                Expand();
            else
                Collapse();
        }
        
        private IEnumerator ResizeExpandableButton()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButtonScroll);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButton);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_sideMenuIsland);
            OnResizeBegin?.Invoke();
            while (Mathf.Abs(_expandableButtonScroll.sizeDelta.y - _expandableButtonScrollLayout.preferredHeight) > _resizingDifferenceThreshold)
            {
                yield return null;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_sideMenuIsland);
                LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButton);
                _sideMenuIsland.sizeDelta = new(_sideMenuIsland.sizeDelta.x, _sideMenuLayout.preferredHeight);
                _expandableButton.sizeDelta = new(_expandableButton.sizeDelta.x, Mathf.MoveTowards(_expandableButton.sizeDelta.y, _expandableButtonLayout.preferredHeight, _resizeSpeed));
                _expandableButtonScrollContent.sizeDelta = new(_expandableButtonScrollContent.sizeDelta.x, _expandableButtonScrollLayout.preferredHeight);
                Debug.Log($"Current expandalbel button scroll sizeDelta: {_expandableButtonScroll.sizeDelta}.\n" +
                    $"Preferred height: {_expandableButtonScrollLayout.preferredHeight}");
                _expandableButtonScroll.sizeDelta = new Vector2(
                    _expandableButtonScroll.sizeDelta.x,
                    Mathf.Clamp(
                        Mathf.MoveTowards(_expandableButtonScroll.sizeDelta.y,
                        _expandableButtonScrollLayout.preferredHeight, _resizeSpeed),float.MinValue,_maxVerticalSize));

            }

            if (_collapsing) // collapsed case
            {
                _expandableButtonLayout.padding = new RectOffset(0, 0, 0, 0);
                _expandableButtonLayout.spacing = 0;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButton);
                _expandableButton.sizeDelta = new(_expandableButton.sizeDelta.x, _expandableButtonLayout.preferredHeight);
                _expandableButtonScrollContent.sizeDelta = new(_expandableButtonScrollContent.sizeDelta.x, _expandableButtonScrollLayout.preferredHeight);
                _expandableButtonScroll.sizeDelta = new(_expandableButtonScroll.sizeDelta.x, Mathf.MoveTowards(_expandableButtonScroll.sizeDelta.y, _expandableButtonScrollLayout.preferredHeight, _resizeSpeed));

                _grayBackground.enabled = false;
                _purpleBackground.SetActive(false);
            }
            else // expanded case
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_expandableButton);
                _expandableButton.sizeDelta = new(_expandableButton.sizeDelta.x, _expandableButtonLayout.preferredHeight);
                _expandableButtonScroll.sizeDelta = new(_expandableButtonScroll.sizeDelta.x, Mathf.MoveTowards(_expandableButtonScroll.sizeDelta.y, _expandableButtonScrollLayout.preferredHeight, _resizeSpeed));
                _expandableButtonScrollContent.sizeDelta = new(_expandableButtonScrollContent.sizeDelta.x, _expandableButtonScrollLayout.preferredHeight);
            }
            _resizeComplete = true;
            
            OnResizeComplete?.Invoke();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_sideMenuIsland);
            _sideMenuIsland.sizeDelta = new(_sideMenuIsland.sizeDelta.x, _sideMenuLayout.preferredHeight);
        }
    }
}
