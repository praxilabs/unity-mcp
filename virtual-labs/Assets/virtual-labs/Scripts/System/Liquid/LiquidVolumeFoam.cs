using LiquidVolumeFX;
using UnityEngine;

public class LiquidVolumeFoam : MonoBehaviour
{
    private LiquidVolume _liquidVolume;
    private LiquidVolumeHelper _helper;
    private LiquidVolumeController _liquidController;

    private void Start()
    {
        _liquidController = GetComponent<LiquidVolumeController>();
        _liquidVolume = _liquidController.LiquidVolume;
        _helper = _liquidController.Helper;
    }

    /// <summary>
    /// Lerps foam color to a new one
    /// </summary>
    /// <param name="hexColor">New color in Hex Code</param>
    public void ChangeFoamColor(string hexColor, float time)
    {
        Color newColor = GameHelper.ConvertHexToColor(hexColor);
        StartCoroutine(_helper.LerpColor(newColor, time, (x) => _liquidVolume.foamColor = x, _liquidVolume.foamColor));
    }

    /// <summary>
    /// Adjusts relative height of the foam with respect to the flask height
    /// </summary>
    /// <param name="foamThickness">New thickness to lerp to</param>
    public void ChangeFoamThickness(float foamThickness, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(foamThickness, 0, 0.3f), time, (x) => _liquidVolume.foamThickness = x, _liquidVolume.foamThickness));
    }

    /// <summary>
    /// Lerps the resolution of the foam
    /// </summary>
    /// <param name="foamScale">New Scale to lerp to</param>
    public void ChangeFoamScale(float foamScale, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(foamScale, 0, 1f), time, (x) => _liquidVolume.foamScale = x, _liquidVolume.foamScale));
    }

    /// <summary>
    /// The higher the density the more opaque the foam looks 
    /// It acts as alpha for the foam
    /// </summary>
    /// <param name="foamDensity">New foam Density from 0 to 1</param>
    public void ChangeFoamDensity(float foamDensity, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(foamDensity, -1, 1), time, (x) => _liquidVolume.foamDensity = x, _liquidVolume.foamDensity));
    }

    /// <summary>
    /// controls the smoothness of the foam at the bottom, near the liquid
    /// </summary>
    /// <param name="foamWeight">New weight</param>
    public void ChangeFoamWeight(float foamWeight, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(foamWeight, 4, 100), time, (x) => _liquidVolume.foamWeight = x, _liquidVolume.foamWeight));
    }

    /// <summary>
    /// Adjust the multiplier to the turbulence factors of the liquid. Set this to zero to produce a static foam regardless of the liquid turbulence
    /// it affects also the liquid
    /// </summary>
    /// <param name="foamTurbulence">New foam turbulence</param>
    public void ChangeFoamTurbulence(float foamTurbulence, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(foamTurbulence, 0, 10), time, (x) => _liquidVolume.foamTurbulence = x, _liquidVolume.foamTurbulence));
    }

    /// <summary>
    /// Enable this option to allow the foam to be visible from bottom-up, through the liquid(if liquid alpha is low).
    /// </summary>
    public void ChangeFoamVisibleFromBottom(bool isVisible)
    {
        _liquidVolume.foamVisibleFromBottom = isVisible;
    }
}
