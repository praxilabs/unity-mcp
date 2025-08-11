using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.DeviceSideMenu
{
    [RequireComponent(typeof(ScrollRect))]
    public class PageScrollViewUI : MonoBehaviour 
    {
        private ScrollRect _scrollRect;
        private TabContent _tabContent;
        private float _scrollRectHeight;

        private void Awake() 
        {
            _scrollRect = GetComponent<ScrollRect>();
            _tabContent = GetComponentInParent<TabContent>();
            _scrollRectHeight = GetComponent<RectTransform>().rect.height;
        }

        private void Start()
        {
            _tabContent.OnHeightChanged += (_,contentRectHeight) => UpdateScrollRectMovementType(contentRectHeight);
        }

        private void UpdateScrollRectMovementType(float contentRectHeight)
        {
            _scrollRect.movementType = (contentRectHeight > _scrollRectHeight) ? ScrollRect.MovementType.Elastic : ScrollRect.MovementType.Clamped;
        }
    }
}