using UnityEngine;

public class SideCameraBillBoard : CameraBillboardBase
{
    protected override void Update()
    {
        if (IsVisible(_rectTransform))
        {
            Vector3 cameraForward = _mainCamera.transform.rotation * Vector3.forward;
            Vector3 cameraUp = _mainCamera.transform.rotation * Vector3.up;
            _rectTransform.LookAt(_rectTransform.position + cameraForward, cameraUp);
        }
    }
}
