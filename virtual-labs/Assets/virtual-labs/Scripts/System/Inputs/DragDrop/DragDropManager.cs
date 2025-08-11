using Praxilabs.CameraSystem;
using UnityEngine;

namespace Praxilabs.Input
{
    public class DragDropManager : Singleton<DragDropManager>
    {
        public string targetTag;
        public bool canMoveInZAxis;
        private RaycastHit _hitInfo;

        private void Update()
        {
            if (ToolPressed() /*&& !IsPointerOverGameObject()*/)
            {
                CheckTarget();
                SetDragSettings();
            }

            if (!ValidateTarget())
                return;

            if (IsDragging()/* && !IsPointerOverGameObject()*/)
                Drag();
            if (ToolReleased())
                Drop(true);
        }

        private bool ToolPressed()
        {
            return MouseInputManager.leftPressAction.WasPressedThisFrame();
        }

        private bool IsPointerOverGameObject()
        {
            return CameraManager.Instance.eventSystem.IsPointerOverGameObject();
        }

        private void CheckTarget()
        {
            DragDropTarget.CheckTarget(GameHelper.GetClickedObject(out _hitInfo));
        }

        private void SetDragSettings()
        {
            if (CheckTargetTag())
                DragObject.SetDragSettings(DragDropTarget.draggableTarget);
        }

        private bool CheckTargetTag()
        {
            if (!ValidateTarget())
                return false;

            return DragDropTarget.CheckTargetTag(targetTag);
        }

        private bool ValidateTarget()
        {
            return DragDropTarget.ValidateTarget(DragDropTarget.draggableTarget);
        }

        private bool IsDragging()
        {
            return DragObject.isDragging;
        }

        private void Drag()
        {
            DragObject.Drag(canMoveInZAxis, DragDropTarget.draggableTarget);
        }

        private bool ToolReleased()
        {
            return MouseInputManager.leftPressAction.WasReleasedThisFrame();
        }

        public void Drop(bool resetRotation)
        {
            DropObject.Drop(resetRotation, DragObject.isDragging, DragDropTarget.draggableTarget);
            DragDropTarget.SetTarget(null);
        }
    }
}