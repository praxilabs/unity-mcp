using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table.UI.Views
{
    public class TableScrollArea : MonoBehaviour
    {
        private UnityEngine.UI.VerticalLayoutGroup _parentVerticalLayoutGroup;
        private RectTransform _parent;

        [SerializeField] private float _minHeight = 300;
        [SerializeField] private List<RectTransform> _siblings;
        private RectTransform _myTransform;
        private float _spacing;
        private float _totalSiblingsHeight;
        private Vector2 _mySize;

        private void Start()
        {
            Initialize();
        }
        public void Initialize()
        {

            _parent = transform.parent.GetComponent<RectTransform>();
            _parentVerticalLayoutGroup = transform.parent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
            _myTransform = GetComponent<RectTransform>();
            _siblings = new List<RectTransform>();

            for (int i = 0; i < _parent.childCount; i++)
            {
                if (_parent.GetChild(i) != transform)
                    _siblings.Add(_parent.GetChild(i).GetComponent<RectTransform>());
            }
        }
        private void LateUpdate()
        {
            AdjustSize();
        }


        [ContextMenu("AdjustSize")]
        public void AdjustSize()
        {
            _siblings.RemoveAll(x => x == null);
            _spacing = _parentVerticalLayoutGroup.spacing;
            _totalSiblingsHeight = 0;
            foreach (var item in _siblings)
            {
                if (item == null || item == _myTransform || !(item.gameObject.activeInHierarchy && item.gameObject.activeSelf))
                    continue;
                _totalSiblingsHeight += item.sizeDelta.y;
                _totalSiblingsHeight += _spacing;
            }
            _totalSiblingsHeight -= _spacing;
            _totalSiblingsHeight += _parentVerticalLayoutGroup.padding.vertical;
            _mySize = _myTransform.sizeDelta;
            _mySize.y = Mathf.Max(_minHeight, _parent.rect.size.y - _totalSiblingsHeight - 2 * _spacing);
            _myTransform.sizeDelta = _mySize;

        }
    }
}