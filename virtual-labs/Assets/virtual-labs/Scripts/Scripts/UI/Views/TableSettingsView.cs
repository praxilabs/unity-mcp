using System.Collections.Generic;
using Table.UI.TableSettings.Types;
using UnityEngine;

namespace Table.UI.Views.TableSettings
{
    public class TableSettingsView : MonoBehaviour
    {
        #region fields
        [Header("References")]
        [SerializeField] private RectTransform _myRectTransform;

        [SerializeField, Tooltip("Items that shall be visible on collapse.")]
        private TableSettingTypeBase[] _tableSettingsNoCollapsable;

        [SerializeField, Tooltip("Items that shall be visible on expand, and hidden on collapse.")]
        private TableSettingTypeBase[] _tableSettingsCollapsable;

        [Space(20), Header("Transition related")]
        [SerializeField] private float _transitionSpeed;
        private Vector2 _expandedSize;
        private Vector2 _collapsedSize;
        private Vector2 _size;
        private Vector2 _expansionVector;
        private Vector2 _collapsingVector;
        private System.Collections.Generic.Dictionary<string, TableSettingTypeBase> _tableSettingsDictionary;
        [Space(20), Header("Alignments & visual structure")]
        [SerializeField, Tooltip("Top & bottom combined")] private float _verticalPadding;
        [SerializeField, Tooltip("Left & right combined")] private float _horizontalPadding;
        [SerializeField] private float _verticalSpacing;
        private bool _collapsing;
        public bool HasCollapsingSettings => _tableSettingsCollapsable.Length != 0;
        [Header("Expand / See more")]
        [SerializeField] private GameObject _seeMorePanel;
        [SerializeField] private GameObject _seeLessPanel;
        [SerializeField] private GameObject _SeeMoreLessParentObject;
        #endregion

        #region methods
        private void Start()
        {
            _expansionVector = (_expandedSize - _collapsedSize).normalized;
            _collapsingVector = (_collapsedSize - _expandedSize).normalized;
            #region initialize table settings elements
            for (int i = 0; i < _tableSettingsCollapsable.Length; i++)
            {
                _tableSettingsCollapsable[i].Initialize();
            }

            for (int i = 0; i < _tableSettingsNoCollapsable.Length; i++)
            {
                _tableSettingsNoCollapsable[i].Initialize();
            }
            #endregion
            InitializeTableSettingsDictionary();
            UpdateSizesAndPositions();
            _collapsedSize.y = 20;
            _myRectTransform.sizeDelta = _collapsedSize;

            _SeeMoreLessParentObject.SetActive(HasCollapsingSettings);

            Collapse();
        }

        public void UpdateTypeLabels(List<string> labels)
        {
            if (_tableSettingsCollapsable != null && _tableSettingsNoCollapsable.Length > 0)
            {
                for (int i = 0; i < _tableSettingsNoCollapsable.Length; i++)
                    _tableSettingsNoCollapsable[i].UpdateLabel(labels[i]);
            }

            if(_tableSettingsCollapsable == null || _tableSettingsCollapsable.Length == 0)
            {
            for (int i = 0; i < _tableSettingsCollapsable.Length; i++)
                _tableSettingsCollapsable[i].UpdateLabel(labels[i]);
            }
        }

        private void InitializeTableSettingsDictionary()
        {
            if (_tableSettingsDictionary is not null)
                return;
            _tableSettingsDictionary = new System.Collections.Generic.Dictionary<string, TableSettingTypeBase>();

            foreach (var tableSetting in _tableSettingsCollapsable)
            {
                if (!_tableSettingsDictionary.ContainsKey(tableSetting.Id))
                    _tableSettingsDictionary.Add(tableSetting.Id, tableSetting);
            }
            foreach (var tableSetting in _tableSettingsNoCollapsable)
            {
                if (!_tableSettingsDictionary.ContainsKey(tableSetting.Id))
                    _tableSettingsDictionary.Add(tableSetting.Id, tableSetting);
            }
        }
        public void Expand()
        {
            _collapsing = false;
        }

        public void Collapse()
        {
            _collapsing = true;
        }

        public TableSettingTypeBase GetSettings(string id)
        {
            if (_tableSettingsDictionary is null)
                InitializeTableSettingsDictionary();
            return _tableSettingsDictionary[id];
        }

        private void Update()
        {
            _size = _myRectTransform.sizeDelta;
            UpdateSizesAndPositions();
            if (_collapsing)
            {
                CollapseUpdate();
            }
            else
            {
                ExpandUpdate();
            }
            _myRectTransform.sizeDelta = _size;

            SetExpandArrowRotation(_collapsing);
        }

        private void SetExpandArrowRotation(bool collapse)
        {
            if (collapse)
            {
                _seeMorePanel.SetActive(true);
                _seeLessPanel.SetActive(false);
            }
            else
            {
                _seeMorePanel.SetActive(false);
                _seeLessPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Update procedure if state is not collapsing, ie expanding
        /// </summary>
        private void ExpandUpdate()
        {

            if (_expandedSize.y > (_size + _expansionVector * _transitionSpeed * Time.deltaTime).y)
                _size += _expansionVector * _transitionSpeed * Time.deltaTime;
            else
                _size = _expandedSize;

        }

        /// <summary>
        /// Update procedure if state is collapsing
        /// </summary>
        private void CollapseUpdate()
        {

            if (_collapsedSize.y < (_size + _collapsingVector * _transitionSpeed * Time.deltaTime).y)
                _size += _collapsingVector * _transitionSpeed * Time.deltaTime;
            else
                _size = _collapsedSize;
        }
        /// <summary>
        /// Calls <see cref="CalculateSizes"/>, <seealso cref="UpdatePositions"/>  together.
        /// </summary>
        ///
        [ContextMenu("Update sizes, positions")]
        private void UpdateSizesAndPositions()
        {
            CalculateSizes();
            UpdatePositions();
        }
        /// <summary>
        /// Calculates settings expanded and collapsed sizes, going summing elements sizes and adding spacing and padding.
        /// </summary>
        private void CalculateSizes()
        {
            _expandedSize.x = _myRectTransform.sizeDelta.x;
            _collapsedSize.x = _myRectTransform.sizeDelta.x;
            _expandedSize.y = _verticalPadding;
            _collapsedSize.y = _verticalPadding;

            for (int i = 0; i < _tableSettingsNoCollapsable.Length; i++)
            {
                _tableSettingsNoCollapsable[i].UpdatePreferredHeight();
                _expandedSize.y += _tableSettingsNoCollapsable[i].PreferredHeight;
                _collapsedSize.y += _tableSettingsNoCollapsable[i].PreferredHeight;
            }

            for (int i = 0; i < _tableSettingsCollapsable.Length; i++)
            {
                _tableSettingsCollapsable[i].UpdatePreferredHeight();
                _expandedSize.y += _tableSettingsCollapsable[i].PreferredHeight;
            }

            _collapsedSize.y += _verticalSpacing * (_tableSettingsNoCollapsable.Length - 1);
            _expandedSize.y += _verticalSpacing * (_tableSettingsNoCollapsable.Length - 1) + _verticalSpacing * (_tableSettingsCollapsable.Length);

            _expansionVector = (_expandedSize - _collapsedSize).normalized;
            _collapsingVector = (_collapsedSize - _expandedSize).normalized;
        }
        ///<summary>
        ///Updates positions for settings elements
        ///</summary>
        private void UpdatePositions()
        {
            Vector2 position = _tableSettingsNoCollapsable[0].ItemRect.localPosition;
            position.y = 0;
            for (int i = 0; i < _tableSettingsNoCollapsable.Length; i++)
            {
                _tableSettingsNoCollapsable[i].ItemRect.anchoredPosition = position;
                position.y -= _tableSettingsNoCollapsable[i].PreferredHeight;
                position.y -= _verticalSpacing;
            }
            for (int i = 0; i < _tableSettingsCollapsable.Length; i++)
            {
                _tableSettingsCollapsable[i].ItemRect.anchoredPosition = position;
                position.y -= _tableSettingsCollapsable[i].PreferredHeight;
                position.y -= _verticalSpacing;
            }
        }
        /// <summary>
        /// Inverts current state if collapsing to expanding and vice versa.
        /// </summary>
        public void ToggleCollapse()
        {
            if (_collapsing)
                Expand();
            else
                Collapse();
        }
        #endregion
    }
}