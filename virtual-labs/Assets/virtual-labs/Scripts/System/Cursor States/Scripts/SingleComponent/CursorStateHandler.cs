using UnityEngine;
using UnityEngine.EventSystems;

namespace CursorStates
{
    [DisallowMultipleComponent]
    public class CursorStateHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public bool canHandleCursorState = true;
        [SerializeField] private CursorInteractionType _cursorInteractionType;
        [HideInInspector][SerializeField] private CursorsCollection _cursorData;

        private bool _isDragging;

        public CursorInteractionType CursorInteractionTypeObject
        {
            get => _cursorInteractionType;
            set => _cursorInteractionType = value;
        }

        public void OnPointerEnter(PointerEventData eventData) => SetCursorForInteraction();
        public void OnPointerExit(PointerEventData eventData) => ResetCursor();
        public void OnPointerDown(PointerEventData eventData) => HandlePointerDown();
        public void OnPointerUp(PointerEventData eventData) => HandlePointerUp();

        private void OnMouseOver()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            SetCursorForInteraction();
        }

        private void OnMouseExit()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            ResetCursor();
        }

        private void OnMouseDown()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            HandlePointerDown();
        }

        private void OnMouseUp()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            HandlePointerUp(); 
        }

        private void OnDisable() => ResetCursor();
        private void SetCursorForInteraction()
        {
            if (_isDragging || !canHandleCursorState) return;

            switch (CursorInteractionTypeObject)
            {
                case CursorInteractionType.Draggable:
                    Cursor.SetCursor(_cursorData[CursorData.CursorSpriteKey.OpenHand].Cursor, _cursorData[CursorData.CursorSpriteKey.OpenHand].Offset, CursorMode.Auto);
                    break;
                case CursorInteractionType.Clickable:
                    Cursor.SetCursor(_cursorData[CursorData.CursorSpriteKey.Clickable].Cursor, _cursorData[CursorData.CursorSpriteKey.Clickable].Offset, CursorMode.Auto);
                    break;
            }
        }

        private void ResetCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            _isDragging = false;
        }

        private void HandlePointerDown()
        {
            if (_isDragging || CursorInteractionTypeObject != CursorInteractionType.Draggable || !canHandleCursorState) return;

            Cursor.SetCursor(_cursorData[CursorData.CursorSpriteKey.ClosedHand].Cursor, _cursorData[CursorData.CursorSpriteKey.ClosedHand].Offset, CursorMode.Auto);
            _isDragging = true;
        }

        private void HandlePointerUp()
        {
            if (CursorInteractionTypeObject != CursorInteractionType.Draggable || !canHandleCursorState) return;

            Cursor.SetCursor(_cursorData[CursorData.CursorSpriteKey.OpenHand].Cursor, _cursorData[CursorData.CursorSpriteKey.OpenHand].Offset, CursorMode.Auto);
            _isDragging = false;
        }

        public enum CursorInteractionType
        {
            Clickable,
            Draggable
        }
    }
}