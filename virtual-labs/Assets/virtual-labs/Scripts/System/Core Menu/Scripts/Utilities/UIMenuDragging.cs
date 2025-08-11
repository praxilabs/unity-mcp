using UnityEngine;
using UnityEngine.EventSystems;
using CursorStates;
using static CursorStates.CursorStateHandler;
[RequireComponent(typeof(CursorStateHandler))]
public class UIMenuDragging : MonoBehaviour,IBeginDragHandler, IDragHandler , IEndDragHandler
{
    [SerializeField] private RectTransform rectTransformToDrag;
    [SerializeField] private RectTransform dragHandle;
    [SerializeField] private RectTransform dragBoxOffScreen;
    private Vector2 lastMousePosition;
    private bool isDragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsPointerOverRectTransform(dragHandle, eventData))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransformToDrag,
                eventData.position,
                eventData.pressEventCamera,
                out lastMousePosition
            );

            isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 currentMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransformToDrag,
            eventData.position,
            eventData.pressEventCamera,
            out currentMousePosition
        );

        Vector2 delta = currentMousePosition - lastMousePosition;
        rectTransformToDrag.anchoredPosition += delta;

        ClampToScreenBounds();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("Ended Drag");
        isDragging = false;
    }

    private bool IsPointerOverRectTransform(RectTransform rectTransform, PointerEventData eventData)
    {
        
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, eventData.enterEventCamera);
    }

    private void ClampToScreenBounds()
    {
        Vector3[] corners = new Vector3[4];
        rectTransformToDrag.GetWorldCorners(corners);

        float margin = 5f; // small offset from screen edge

        // Calculate how far off screen the panel is
        float offsetX = 0f;
        float offsetY = 0f;

        foreach (Vector3 corner in corners)
        {
            if (corner.x < margin)
                offsetX = Mathf.Max(offsetX, margin - corner.x);
            else if (corner.x > Screen.width - margin)
                offsetX = Mathf.Min(offsetX, (Screen.width - margin) - corner.x);

            if (corner.y < margin)
                offsetY = Mathf.Max(offsetY, margin - corner.y);
            else if (corner.y > Screen.height - margin)
                offsetY = Mathf.Min(offsetY, (Screen.height - margin) - corner.y);
        }

        Vector3 worldOffset = new Vector3(offsetX, offsetY, 0f);
        Vector2 localOffset = rectTransformToDrag.InverseTransformVector(worldOffset);

        rectTransformToDrag.anchoredPosition += localOffset;
    }



    private void OnValidate()
    {
        CursorStateHandler cursorStateHandler = gameObject.GetComponent<CursorStateHandler>();
        if (cursorStateHandler is null)
            cursorStateHandler = gameObject.AddComponent<CursorStateHandler>();
        cursorStateHandler.CursorInteractionTypeObject = CursorInteractionType.Draggable;
    }
}