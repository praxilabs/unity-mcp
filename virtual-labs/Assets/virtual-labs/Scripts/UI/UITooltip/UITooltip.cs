using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PraxiLabs.Tooltip
{
    public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private int _textID;
        [SerializeField] private float _delayTime = 0;
        private bool _isHovered = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(DelayShowToolTip());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(DelayHideToolTip());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(DelayHideToolTip());
        }

        private IEnumerator DelayShowToolTip()
        {
            yield return new WaitForSeconds(_delayTime);
            UITooltipManager.Instance.ShowTooltip(_textID, this.GetComponent<RectTransform>());
            Debug.Log("entered");
            _isHovered = true;
        }

        private IEnumerator DelayHideToolTip()
        { 
            yield return new WaitUntil(() => _isHovered);
            UITooltipManager.Instance.HideTooltip();
            _isHovered = false;
            Debug.Log("exited");
        }
    }
}