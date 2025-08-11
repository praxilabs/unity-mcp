using UnityEngine;

namespace Praxilabs.CameraSystem
{
    public class CameraSettings : MonoBehaviour
    {
        [Header("Zoom settings")]
        public float zoomSpeed;
        public float maxForwardDistance;
        public float maxBackwardDistance;

        [Header("Rotation settings")]
        public float rotationSpeed;
        public float verticalAxisInitialValue;
        public float horizontalAxisInitialValue;
        public bool canRotateVertically;
        public bool canRotateHorizontally;

        [Header("Initial transform")]
        public Vector3 initialPosition;
        public Quaternion initialRotation;
        public float movementSpeed = 5;

        private void Start()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
        }
    }
}