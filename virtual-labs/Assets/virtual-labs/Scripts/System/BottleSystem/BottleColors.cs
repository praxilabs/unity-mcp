using LiquidVolumeFX;
using UnityEngine;

public class BottleColors : MonoBehaviour
{
    [SerializeField] private MeshRenderer _capMat;
    [SerializeField] private MeshRenderer _bodyMat;

    [SerializeField] private Camera _textRenderCamera;
    [SerializeField] private LiquidVolume _liquid;

    public void SetColors(BottleInfo info)
    {
        _capMat.material.color = info.capColor;
        _bodyMat.material.color = info.bottleColor;

        _textRenderCamera.backgroundColor = info.labelBackgroundColor;
        _liquid.liquidColor1 = info.liquidColor;
    }
}
