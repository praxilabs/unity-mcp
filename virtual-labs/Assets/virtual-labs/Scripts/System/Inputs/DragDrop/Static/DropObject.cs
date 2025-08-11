using UnityEngine;

namespace Praxilabs.Input
{
    public static class DropObject
    {
        private static DraggableObject _draggableTarget;

        public static void Drop(bool resetRotation, bool isDragging, GameObject target)
        {
            SetDraggableTarget(target);
            StopDragging(resetRotation, isDragging, target);
        }

        private static void SetDraggableTarget(GameObject target)
        {
            _draggableTarget = target.GetComponent<DraggableObject>();
        }

        private static void StopDragging(bool resetRotation, bool isDragging, GameObject target)
        {
            isDragging = false;

            _draggableTarget.ResetPosition();

            if (resetRotation)
                _draggableTarget.ResetRotation();
            target = null;
        }
    }
}