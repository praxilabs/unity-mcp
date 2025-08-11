public class InterestPointBillboard : CameraBillboardBase
{
    protected override void Update()
    {
        if (IsVisible(_rectTransform))
            _rectTransform.LookAt(_mainCamera.transform);
    }
}
