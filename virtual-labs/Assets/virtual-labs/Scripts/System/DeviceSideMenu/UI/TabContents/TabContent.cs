using System;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    [RequireComponent(typeof(TabPanelUI))]
    public abstract class TabContent : MonoBehaviour 
    {
        public event Action<TabContent,float> OnHeightChanged;
        public abstract TabType TabType {get; protected set;}
        protected float _previousHeight;

        [SerializeField] protected RectTransform _contentRect;

        protected virtual void Awake()
        {
            _previousHeight = _contentRect.rect.height;
        }

        private void Update() 
        {
            if(!gameObject.activeInHierarchy) return;
            
            if (Mathf.Abs(_previousHeight - _contentRect.rect.height) > Mathf.Epsilon)  // Check for change in height
            {
                RaiseOnHeightChanged(this, _contentRect.rect.height);
                _previousHeight = _contentRect.rect.height;
            }
        }
        
        public virtual float GetPageHeight()
        {
            return _contentRect.rect.height;
        }

        public virtual void UpdateData(string title, string body)
        {
            
        }
        public virtual void UpdateData(DeviceInfo deviceInfo, List<GameObject> obj)
        {

        }

        protected void RaiseOnHeightChanged(TabContent tabContent, float value)
        {
            OnHeightChanged?.Invoke(tabContent, value);
        }
    }
}