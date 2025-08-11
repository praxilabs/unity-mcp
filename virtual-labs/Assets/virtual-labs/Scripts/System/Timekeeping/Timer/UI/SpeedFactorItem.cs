using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Praxilabs.Timekeeping.Timer
{
    public class SpeedFactorItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [field: SerializeField] public Button Button {get; private set;}
        [SerializeField] private TextMeshProUGUI _speedFactorTxt;
        [SerializeField] private Image _btnImage;
        [Header("Colors")]
        [SerializeField] private Color _highlightedColor;
        [SerializeField] private Color _pressedColor;
        [SerializeField] private Color _selectedColor;
        private Color _defaultColor;
        private bool _isSelected;

        private void Awake()
        {
            _defaultColor = _btnImage.color;
        }

        public void SetText(float newSpeedFactor)
        {
            _speedFactorTxt.text = $"{newSpeedFactor}x";
        }

        public void SetItemVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void Deselect()
        {
            _isSelected = false;
            _btnImage.color = _defaultColor;
        }

        #region ButtonStateColorManagement
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_isSelected) return;
            _btnImage.color = _highlightedColor;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _btnImage.color = _pressedColor;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            _btnImage.color = _isSelected ? _selectedColor : _defaultColor;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _isSelected = true;
            _btnImage.color = _selectedColor;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if(_isSelected) return;
            _btnImage.color = _defaultColor;
        }
        #endregion
    }
}