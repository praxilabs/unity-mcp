using LiquidVolumeFX;
using UnityEngine;

public class LiquidVolumeFlask : MonoBehaviour
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
    /// change the flask color to a new one
    /// </summary>
    public void ChangeFlaskColor(string hexColor)
    {
        Color newColor = GameHelper.ConvertHexToColor(hexColor);
        _liquidVolume.flaskMaterial.SetColor("_BaseColor", newColor);
    }

    /// <summary>
    /// Adjust the Smoothness
    /// </summary>
    public void ChangeFlaskSmoothness(float newSmoothness)
    {
        _liquidVolume.flaskMaterial.SetFloat("_Smoothness", newSmoothness);
    }

    /// <summary>
    /// Adjust the Metallic value
    /// </summary>
    public void ChangeFlaskMetallic(float newMetallic)
    {
        _liquidVolume.flaskMaterial.SetFloat("_Metallic", newMetallic);
    }

    /// <summary>
    /// When enabled, the visible background is blurred to simulate light refraction
    /// P.S. you can use it if the liquid is hot or contains smoke
    /// </summary>
    public void SetRefractionBlur(bool refract)
    {
        _liquidVolume.refractionBlur = refract;
    }

    /// <summary>
    /// Adjusts the inensity of the refraction the higher the value the stronger the refraction effect
    /// </summary>
    public void ChangeRefractionBlurIntensity(float newIntensity, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(newIntensity, 0, 1), time, (x) => _liquidVolume.blurIntensity = x,
            _liquidVolume.blurIntensity));
    }

    /// <summary>
    /// Flasks have walls, and this function controls the width of the walls
    /// It acts like a scale function for the liquid 
    /// </summary>
    public void ChangeFlaskThickness(float newThickness, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(newThickness, -1, 1), time, (x) => _liquidVolume.flaskThickness = x,
            _liquidVolume.flaskThickness));
    }
}
