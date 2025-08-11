using UnityEngine;

namespace Praxilabs.Input
{
    public static class DragDropTarget
    {
        public static GameObject draggableTarget;

        public static void CheckTarget(GameObject target)
        {
            SetTarget(target);
        }

        public static void SetTarget(GameObject target)
        {
            draggableTarget = target;
        }

        public static bool ValidateTarget(GameObject target)
        {
            if (target != null)
                return true;
            else
                return false;
        }

        public static bool CheckTargetTag(string targetTag)
        {
            if (CompareTargetTag(targetTag))
                return true;
            else
            {
                SetTarget(null);
                return false;
            }
        }

        private static bool CompareTargetTag(string targetTag)
        {
            return draggableTarget.CompareTag(targetTag);
        }
    }
}