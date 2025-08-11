using DG.Tweening;
using UnityEngine;

namespace Praxilabs.Input
{
    public class DraggableObject : MonoBehaviour
    {
        public float minDraggingY;
        public Transform directionPlane;
        [HideInInspector] public bool resetPositionOnDrop;

        [SerializeField] private bool _canSetDraggingRotation;
        [SerializeField] private Vector3 _draggingRotation;
        private Vector3 _initialPosition;
        private Vector3 _initialRotation;

        void Start()
        {
            resetPositionOnDrop = true;
            SetInitialTransform();
        }

        public void SetInitialTransform()
        {
            _initialPosition = this.gameObject.transform.position;
            _initialRotation = this.gameObject.transform.rotation.eulerAngles;
        }

        public void SetDraggingRotation()
        {
            if (!_canSetDraggingRotation) return;

            transform.DORotate(_draggingRotation, 0.5f, RotateMode.Fast);
        }

        public void ResetPosition()
        {
            if (resetPositionOnDrop)
                transform.DOMove(_initialPosition, 0.5f, false);
        }

        public void ResetRotation()
        {
            transform.DORotate(_initialRotation, 0.5f, RotateMode.Fast);
        }
    }
}