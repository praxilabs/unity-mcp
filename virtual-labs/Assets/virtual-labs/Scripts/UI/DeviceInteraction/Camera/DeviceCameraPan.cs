using Cinemachine;
using Praxilabs.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DeviceCameraPan : MonoBehaviour
{
    public bool canPan = true;

    [SerializeField] private float _panSpeed = 1f;
    [SerializeField] private float _panLimit = 0.15f;

    private DeviceSideController _device;
    private CinemachineVirtualCamera _activeCamera;
    private Vector3 _originalLocalPosition;

    private void Start()
    {
        _device = GetComponentInParent<DeviceSideController>();
        SetDefaultValues();
    }

    // private void Update()
    // {
    //     if (canPan && MouseInputManager.leftPressAction.IsPressed() && !GetComponent<DeviceCameraZoom>().isZooming)
    //         PanCamera();
    // }

    private void PanCamera()
    {
        if (!CanPan())
            return;
        ValidateActiveCamera();

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        Vector3 move = new Vector3(mouseDelta.x, mouseDelta.y, 0) * _panSpeed * Time.deltaTime;

        // Use the active camera's local right and up directions to pan
        Vector3 right = _activeCamera.transform.right;
        Vector3 up = _activeCamera.transform.up;

        // Calculate the new local position based on the camera's local axes
        Vector3 newLocalPosition = _activeCamera.transform.localPosition + (right * move.x) + (up * move.y);

        // Clamp the new local position to stay within the pan limits
        float distanceX = Vector3.Dot(newLocalPosition - _originalLocalPosition, right);  // Distance along X (right) axis
        float distanceY = Vector3.Dot(newLocalPosition - _originalLocalPosition, up);     // Distance along Y (up) axis

        // Clamp the movement along the local right and up directions
        newLocalPosition = _originalLocalPosition + (right * Mathf.Clamp(distanceX, -_panLimit, _panLimit))
                                              + (up * Mathf.Clamp(distanceY, -_panLimit, _panLimit));

        _activeCamera.transform.localPosition = newLocalPosition;
    }

    private void ValidateActiveCamera()
    {
        if (_activeCamera.Priority <= 0)
        {
            _activeCamera.transform.localPosition = _originalLocalPosition;
            SetDefaultValues();
        }
    }

    public void SetDefaultValues()
    {
        _activeCamera = _device.activeCamera;
        _originalLocalPosition = _activeCamera.transform.localPosition;
    }

    private bool CanPan()
    {
        if (IsPointerOverUI())
            return false;
        return true;
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}