using System.Collections;
using UltimateClean;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SafetyLabTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform _tooltipUI;
    [SerializeField] private CleanButton _informationButton;

    [SerializeField] private Image _tooltipIcon;
    [SerializeField] private Sprite _tooltipIconSource;
    [SerializeField] private Vector2 _offset;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ToggleToolTipCoroutine(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(ToggleToolTipCoroutine(false));
    }

    private IEnumerator ToggleToolTipCoroutine(bool toggle)
    {
        yield return new WaitForSeconds(1);
        _tooltipUI.gameObject.SetActive(toggle);

        UpdateIcon();
        UpdatePosition();
    }

    private void UpdateIcon()
    {
        _tooltipIcon.sprite = _tooltipIconSource;
    }

    private void UpdatePosition()
    {
        //Get top center world position of the UI element
        Vector3[] corners = new Vector3[4];
        _informationButton.GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector3 topCenterWorld = (corners[1] + corners[2]) * 0.5f;

        Canvas tooltipCanvas = _tooltipUI.GetComponentInParent<Canvas>();
        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(null, topCenterWorld);

        //Convert to local UI position
        RectTransform parentRect = _tooltipUI.parent as RectTransform;
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPoint,
            null,
            out localPoint
        );

        _tooltipUI.anchoredPosition = localPoint + _offset;
    }
}
