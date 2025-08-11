using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ScrollViewWithLock : ScrollRect
{
    [field: SerializeField] public bool DragLocksToOneDirection { get; set; } = true;

    private Scrollbar.Direction _scrolDirection = Scrollbar.Direction.LeftToRight;
    private Vector2 _pressPosition;

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (!DragLocksToOneDirection)
        {
            base.OnInitializePotentialDrag(eventData);
            return;
        }

        base.OnInitializePotentialDrag(eventData);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!DragLocksToOneDirection)
        {
            base.OnBeginDrag(eventData);
            return;
        }

        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            _scrolDirection = Scrollbar.Direction.LeftToRight;
        }

        if (Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y))
        {
            _scrolDirection = Scrollbar.Direction.TopToBottom;
        }

        _pressPosition = eventData.position;
        base.OnBeginDrag(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (!DragLocksToOneDirection)
        {
            base.OnDrag(eventData);
            return;
        }


        switch (_scrolDirection)
        {
            case Scrollbar.Direction.LeftToRight:
            case Scrollbar.Direction.RightToLeft:
                _pressPosition.x = eventData.position.x;
                break;
            case Scrollbar.Direction.BottomToTop:
            case Scrollbar.Direction.TopToBottom:
                _pressPosition.y = eventData.position.y;
                break;
            default:
                break;
        }

        eventData.position = _pressPosition;
        base.OnDrag(eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!DragLocksToOneDirection)
        {
            base.OnEndDrag(eventData);
            return;
        }

        _scrolDirection = Scrollbar.Direction.TopToBottom;
        base.OnEndDrag(eventData);
    }
}
