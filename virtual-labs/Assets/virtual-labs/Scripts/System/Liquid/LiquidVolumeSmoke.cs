using LiquidVolumeFX;
using UnityEngine;

public class LiquidVolumeSmoke : MonoBehaviour
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
    /// Enables or disables smoke
    /// </summary>
    public void ChangeSmokeVisibility(bool isVisible)
    {
        _liquidVolume.smokeEnabled = isVisible;
    }

    /// <summary>
    /// Lerps smoke color to a new one
    /// </summary>
    public void ChangeSmokeColor(string hexColor, float time)
    {
        Color newColor = GameHelper.ConvertHexToColor(hexColor);
        StartCoroutine(_helper.LerpColor(newColor, time, (x) => _liquidVolume.smokeColor = x, _liquidVolume.smokeColor));
    }

    /// <summary>
    /// Lerps the density of the smoke (it's resolution) 
    /// </summary>
    /// <param name="smokeScale">New smoke scale from 0 to 1</param>
    public void ChangeSmokeScale(float smokeScale, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(smokeScale, 0, 1), time, (x) => _liquidVolume.smokeScale = x,
            _liquidVolume.smokeScale));
    }

    /// <summary>
    /// Adjusts the speed of the smoke going up
    /// P.S. if a -ve value is passed to this function the smoke will move downward instead of up
    /// </summary>
    /// <param name="smokeSpeed">new Smoke Speed from -100 to 100</param>
    public void ChangeSmokeSpeed(float smokeSpeed, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(smokeSpeed, -100, 100), time, (x) => _liquidVolume.smokeSpeed = x,
            _liquidVolume.smokeSpeed));
    }

    /// <summary>
    /// Makes the smoke darker at the bottom.
    /// </summary>
    /// <param name="smokeBaseObscurance">new base obscurance to lerp to</param>
    public void ChangeSmokeBaseObscurance(float smokeBaseObscurance, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(smokeBaseObscurance, 0, 10), time, (x) => _liquidVolume.smokeBaseObscurance = x,
            _liquidVolume.smokeBaseObscurance));
    }

    /// <summary>
    /// Changes the smoke height from the top
    /// </summary>
    /// <param name="smokeHeightReduction">New height From 0 to 10</param>
    public void ChangeSmokeHeightReduction(float smokeHeightReduction, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(smokeHeightReduction, 0, 10), time, (x) => _liquidVolume.smokeHeightAtten = x,
            _liquidVolume.smokeHeightAtten));
    }
}
