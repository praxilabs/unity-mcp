using UnityEngine;

public class CameraBillboardBase : MonoBehaviour
{
    protected Camera _mainCamera;
    protected RectTransform _rectTransform;

    protected virtual void Start()
    {
        _mainCamera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Update() {}

    // Check if the UI element is within the camera's view frustum
    protected bool IsVisible(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        foreach (Vector3 corner in corners)
        {
            if (IsWithinRange(corner))
                return true;
        }
        return false;
    }

    protected bool IsWithinRange(Vector3 corner)
    {
        Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(corner);

        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }
}