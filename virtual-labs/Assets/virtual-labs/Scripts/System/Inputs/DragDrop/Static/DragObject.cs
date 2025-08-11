using Praxilabs.CameraSystem;
using UnityEngine;

namespace Praxilabs.Input
{
    public static class DragObject
    {
        private static DraggableObject _draggedObject;
      
        private static Vector3 _newPosition;
        private static Vector3 _screenPoint;
        private static Vector3 _offset;

        public static bool isDragging;

        public static void Drag(bool canMoveInZAxis, GameObject target)
        {
            SetPositionInWorld();
            RestrictYAxisDragging(target);
            SetRotation();

            if (!canMoveInZAxis)
                SetNewPosition(new Vector3(0, 0, -1f), target);
            else
                SetNewPosition(new Vector3(0, -1f, 0), target);
        }

        public static void SetDragSettings(GameObject target)
        {
                isDragging = true;
                _screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);
                _offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(MouseInputManager.screenPosition.ReadValue<Vector2>().x,
                                                                                                 MouseInputManager.screenPosition.ReadValue<Vector2>().y,
                                                                                                 _screenPoint.z));
        }

        private static void SetRotation()
        {
            _draggedObject.SetDraggingRotation();
        }

        private static void SetPositionInWorld()
        {
            var curScreenSpace = new Vector3(MouseInputManager.screenPosition.ReadValue<Vector2>().x, 
                                             MouseInputManager.screenPosition.ReadValue<Vector2>().y, 
                                             _screenPoint.z);

            //convert the screen mouse position to world point and adjust with offset
            _newPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + _offset;
        }

        private static void RestrictYAxisDragging(GameObject target)
        {
            _draggedObject = target.GetComponent<DraggableObject>();

            if (_draggedObject != null)
            {
                if (_newPosition.y < _draggedObject.minDraggingY)
                    _newPosition = new Vector3(_newPosition.x, _draggedObject.minDraggingY, _newPosition.z);
            }

        }

        private static void SetNewPosition(Vector3 planeNormal, GameObject target)
        {
            target.transform.position = ChangeMovementPlane(planeNormal, target.transform.position, _newPosition);
        }

        private static Vector3 ChangeMovementPlane(Vector3 planeNormal, Vector3 oldPosition, Vector3 newPosition)
        {
            planeNormal = _draggedObject.directionPlane.TransformDirection(planeNormal);
            planeNormal.Normalize();

            var distance = -Vector3.Dot(planeNormal.normalized, (newPosition - oldPosition));
            return newPosition + planeNormal * distance;
        }
    }
}