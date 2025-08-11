using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.Timekeeping.Timer
{
    public class SpeedFactorsUI : MonoBehaviour 
    {
        private const float FoldedMenuWidth = 62f;
        private const float UnfoldedMenuWidth = 330.12f;
        private const float ToggleMenuAnimationSpeed = 0.3f;
        [Header("Elements")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _menuContainer;
        [SerializeField] private RectTransform _menuScrollView;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private List<SpeedFactorItem> _speedFactorItems;
        [Header("Prefab")]
        [SerializeField] private SpeedFactorItem _itemPrefab;
        private List<float> _speedFactors;
        private bool _isMenuVisible;
        private SpeedFactorItem _selectedSpeedFactorItem;
        private TimerHandler _timerHandler;

        public event Action OnSpeedFactorsMenuClosed;

        public void Setup(List<float> speedFactors)
        {
            Reset();
            UpdateSpeedFactors(speedFactors);
        }

        public void SetSpeedFactorsMenuVisibility(bool isVisible)
        {
            _isMenuVisible = isVisible;
            HandleToggleSpeedFactorMenu();
        }

        private void Awake()
        {
            _timerHandler = FindObjectOfType<TimerHandler>();
            _closeButton.onClick.AddListener(() => SetSpeedFactorsMenuVisibility(false));
            InitializeSpeedFactorBtns();
        }

        private void InitializeSpeedFactorBtns()
        {
            for(int i = 0; i < _speedFactorItems.Count; i++)
            {
                int index = i;
                _speedFactorItems[index].Button.onClick.AddListener(() => HandleSpeedFactorButtonClicked(index, _speedFactorItems[index]));
            }
        }

        private void HandleSpeedFactorButtonClicked(int speedFactorIndex, SpeedFactorItem speedFactorItem)
        {
            _timerHandler.RequestSpeedChange(speedFactorIndex);
            _selectedSpeedFactorItem = speedFactorItem;
            DeselectOtherSpeedFactorBtns(speedFactorItem);
            SetSpeedFactorsMenuVisibility(false);
        }

        private void UpdateSpeedFactors(List<float> speedFactors)
        {
            _speedFactors = speedFactors;
            if(_speedFactors.Count < _speedFactorItems.Count)
            {
                // Hide extra buttons
                for(int i = _speedFactorItems.Count - 1; i >= _speedFactors.Count - 1; i--)
                {
                    _speedFactorItems[i].SetItemVisibility(false);
                }
            }
            else if(_speedFactors.Count > _speedFactorItems.Count)
            {
                // Create missing buttons
                int requiredSpeedFactorBtns = _speedFactors.Count - _speedFactorItems.Count;
                for(int i = 0; i < requiredSpeedFactorBtns; i++)
                {
                    SpeedFactorItem speedFactorBtnUIInstance = Instantiate(_itemPrefab, _contentTransform);
                    int index = _speedFactorItems.Count;
                    speedFactorBtnUIInstance.Button.onClick.AddListener(() => HandleSpeedFactorButtonClicked(index, speedFactorBtnUIInstance));
                    _speedFactorItems.Add(speedFactorBtnUIInstance);
                }
            }

            for(int i = 0; i < speedFactors.Count; i++)
            {
                _speedFactorItems[i].SetText(speedFactors[i]);
                _speedFactorItems[i].SetItemVisibility(true);
            }
        }

        private void HandleToggleSpeedFactorMenu()
        {
            if(_isMenuVisible)
            {
                _menuContainer.SetActive(_isMenuVisible);
                ToggleMenuAnimation(FoldedMenuWidth, UnfoldedMenuWidth);
            }
            else
            {
                _menuContainer.SetActive(_isMenuVisible);
                OnSpeedFactorsMenuClosed?.Invoke();
            }
        }

        private void ToggleMenuAnimation(float initialValue, float targetValue)
        {
            _menuScrollView.sizeDelta = new Vector2(initialValue, _menuScrollView.sizeDelta.y);
            _menuScrollView.DOSizeDelta(new Vector2(targetValue, _menuScrollView.sizeDelta.y), ToggleMenuAnimationSpeed).SetEase(Ease.InQuad);
        }

        private void DeselectOtherSpeedFactorBtns(SpeedFactorItem selectedSpeedFactorItem)
        {
            foreach(SpeedFactorItem speedFactorItem in _speedFactorItems)
            {
                if(speedFactorItem == selectedSpeedFactorItem) continue;
                speedFactorItem.Deselect();
            }
        }
        private void Reset()
        {
            if(_selectedSpeedFactorItem == null) return;
            _selectedSpeedFactorItem.Deselect();
            _selectedSpeedFactorItem = null;
        }

        private void OnDestroy() 
        {
            _closeButton.onClick.RemoveAllListeners();
            for(int i = 0; i < _speedFactorItems.Count; i++)
            {
                _speedFactorItems[i].Button.onClick.RemoveAllListeners();
            }
        }
    }
}