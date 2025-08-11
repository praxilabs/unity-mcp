using LiquidVolumeFX;
using UnityEngine;

public class LiquidVolumeLiquidSettings : MonoBehaviour
{
    private LiquidVolume _liquidVolume;
    private LiquidVolume _otherContainerLV;
    private LiquidVolumeHelper _helper;
    private LiquidVolumeController _liquidController;

    public float LiquidLevel
    {
        get { return _liquidController.liquidLevel; }
        set { _liquidController.liquidLevel = value; }
    }

    public float ContainerCapacity
    {
        get { return _liquidController.containerCapacity; }
        set { _liquidController.containerCapacity = value; }
    }

    private void Start()
    {
        _liquidController = GetComponent<LiquidVolumeController>();
        _liquidVolume = _liquidController.LiquidVolume;
        _helper = _liquidController.Helper;
    }

    /// <summary>
    /// Fills or takes X amount of ML liquid from the container. Also assign thefillerObject liquid color to this container
    /// </summary>
    /// <param name="valueToReach">How much ML of the container to fill</param>
    /// <param name="timeToReach">Time to fill the container</param>
    /// <param name="fillerObject">Other container GameObject name to take color from</param>
    public void FillLiquid(float valueToReach, float timeToReach, string fillerObject)
    {
        LiquidLevel = valueToReach / ContainerCapacity;
        FillLiquidHelper(valueToReach, timeToReach, fillerObject);
    }

    /// <summary>
    /// Fills or takes X amount of ML liquid from the container by with the informative to the liquid level we want. Also assign thefillerObject liquid color to this container
    /// </summary>
    /// <param name="wantedLiquidLevel">How much ML of the container to fill</param>
    /// <param name="timeToReach">Time to fill the container</param>
    /// <param name="fillerObject">Other container GameObject name to take color from</param>
    public void FillLiquidByLiquidLevel(float wantedLiquidLevel, float timeToReach, string fillerObject)
    {
        LiquidLevel = wantedLiquidLevel;
        FillLiquidHelper(wantedLiquidLevel, timeToReach, fillerObject);
    }

    private void FillLiquidHelper(float wantedLiquidLevel, float timeToReach, string fillerObject)
    {
        GameObject filler = GameObject.Find(fillerObject);

        if (filler == null)
        {
            Debug.Log("Color=#FFA725>Couldn't find the filler object gameobject, will not continue</Color>");
            return;
        }
        if (filler.GetComponent<LiquidVolumeController>() && filler.GetComponent<LiquidVolumeController>().LiquidVolume)
            _otherContainerLV = filler.GetComponent<LiquidVolumeController>().LiquidVolume;
        else
            Debug.Log("Color=#FFA725>Couldn't find the filler object LiquidVolume componenet, will not continue</Color>");

        ChangeLiquidColorBeforeFilling(filler);
        StartCoroutine(_helper.AdjustLiquidLevelWithTime(LiquidLevel, timeToReach));
    }

    /// <summary>
    /// Fills or takes X amount of ML liquid from the container.
    /// </summary>
    /// <param name="valueToReach">How much ML of the container to fill</param>
    public void TakeLiquid(float valueToReach, float timeToReach)
    {
        LiquidLevel = valueToReach / ContainerCapacity;
        StartCoroutine(_helper.AdjustLiquidLevelWithTime(LiquidLevel, timeToReach));
    }

    public virtual void TakeLiquidAndProcessImmediately(float valueToReach, float timeToReach)
    {
        LiquidLevel = valueToReach / ContainerCapacity;
        StartCoroutine(_helper.AdjustLiquidLevelWithTime(LiquidLevel, timeToReach));
    }

    /// <summary>
    /// Changes the container's liquid color so that it matches the filler container color
    /// </summary>
    /// <param name="fillerContainer">Filler container GameObject</param>
    private void ChangeLiquidColorBeforeFilling(GameObject fillerContainer)
    {
        if (_liquidVolume != null && _otherContainerLV != null)
        {
            if (_liquidVolume.detail == DETAIL.Simple || _liquidVolume.detail == DETAIL.SimpleNoFlask)
            {
                _liquidVolume.liquidColor1 = _otherContainerLV.liquidColor1;
            }

            if (_liquidVolume.detail == DETAIL.Default || _liquidVolume.detail == DETAIL.DefaultNoFlask)
            {
                _liquidVolume.liquidColor1 = _otherContainerLV.liquidColor1;
                _liquidVolume.liquidColor2 = _otherContainerLV.liquidColor2;
            }
        }
    }

    /// <summary>
    /// Lerp liquids colors incase of liquid detail being Default, DefaultNoFlask, BumpTexture and Reflections
    /// </summary>
    public void ChangeLiquidColor(string hexColor1, string hexColor2, float timeToReach)
    {
        float time2Reach = timeToReach;
        Color newColor1 = GameHelper.ConvertHexToColor(hexColor1);
        Color newColor2 = GameHelper.ConvertHexToColor(hexColor2);

        StartCoroutine(_helper.ChangeLiquidColorWithTime(newColor1, newColor2, time2Reach));
    }

    /// <summary>
    /// Lerp liquids colors by timer
    /// </summary>
    public void ChangeLiquidColorByTimer(string hexColor1, string hexColor2)
    {
        Color newColor1 = GameHelper.ConvertHexToColor(hexColor1);
        Color newColor2 = GameHelper.ConvertHexToColor(hexColor2);

        StartCoroutine(_helper.ChangeLiquidColorWithTimer(newColor1, newColor2));
    }

    /// <summary>
    /// Change Emission color
    /// </summary>
    /// <param name="emissionHexColor">Color In Hex Code</param>
    /// <param name="time">Lerp time</param>
    public void ChangeEmissionColor(string emissionHexColor, float time)
    {
        Color newColor = GameHelper.ConvertHexToColor(emissionHexColor);
        StartCoroutine(_helper.LerpColor(newColor, time, (x) => _liquidVolume.emissionColor = x, _liquidVolume.emissionColor));
    }

    /// <summary>
    /// Lerp liquid color incase of liquid detail being Simple
    /// </summary>
    /// <param name="hexColor">New color in Hex Code</param>
    /// <param name="time">Lerp time</param>
    public void ChangeSimpleLiquidColor(string hexColor, float time)
    {
        Color newColor = GameHelper.ConvertHexToColor(hexColor);
        StartCoroutine(_helper.LerpColor(newColor, time, (x) => _liquidVolume.liquidColor1 = x, _liquidVolume.liquidColor1));
    }

    /// <summary>
    /// Changes Scale applied to the texture of the liquid.
    /// </summary>
    public void ChangeLiquidScale(float liquidNumber, float newScale, float time)
    {
        if (liquidNumber == 1)
            StartCoroutine(_helper.LerpFloat(Mathf.Clamp(newScale, 0.1f, 4.85f), time, (x) => _liquidVolume.liquidScale1 = x, _liquidVolume.liquidScale1));
        else if (liquidNumber == 2)
            StartCoroutine(_helper.LerpFloat(Mathf.Clamp(newScale, 2, 4.85f), time, (x) => _liquidVolume.liquidScale2 = x, _liquidVolume.liquidScale2));
    }

    /// <summary>
    /// Adjusts how turpid and murky the liquid looks
    /// </summary>
    /// <param name="murkiness">New murkiness value from 0 to 1</param>
    public void ChangeMurkiness(float murkiness, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(murkiness, 0, 1), time, (x) => _liquidVolume.murkiness = x, _liquidVolume.murkiness));
    }

    /// <summary>
    /// Adjust the Low-amplitude turbulence applied to the liquid's surface
    /// </summary>
    /// <param name="turbulence1">New turbulence1 value from 0 to 1</param>
    public void ChangeTurbulence1(float turbulence1, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(turbulence1, 0, 1), time, (x) => _liquidVolume.turbulence1 = x, _liquidVolume.turbulence1));
    }

    /// <summary>
    /// Adjust the High-amplitude turbulence applied to the liquid's surface
    /// </summary>
    /// <param name="turbulence2">New turbulence2 value from 0 to 1</param>
    public void ChangeTurbulence2(float turbulence2, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(turbulence2, 0, 1), time, (x) => _liquidVolume.turbulence2 = x, _liquidVolume.turbulence2));
    }

    /// <summary>
    /// Adjust the Speed of the turbulence animation.
    /// </summary>
    /// <param name="turbulenceSpeed">New Speed From 0 to 10</param>
    public void ChangeTurbulenceSpeed(float turbulenceSpeed, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(turbulenceSpeed, 0, 10), time, (x) => _liquidVolume.speed = x, _liquidVolume.speed));
    }

    /// <summary>
    /// Adjusts the Frequency of the turbulence
    /// P.S. Increase to produce shorter waves
    /// </summary>
    /// <param name="frequency">New Frequency</param>
    public void ChangeFrequency(float frequency, float time)
    {
        StartCoroutine(_helper.LerpFloat(frequency, time, (x) => _liquidVolume.frecuency = x, _liquidVolume.frecuency));
    }

    /// <summary>
    /// Adjust how dark the bottom of the liquid is
    /// </summary>
    /// <param name="deepObscurance">New deep obscurance</param>
    public void ChangeDeepObscurance(float deepObscurance, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Abs(deepObscurance), time, (x) => _liquidVolume.deepObscurance = x, _liquidVolume.deepObscurance));
    }

    /// <summary>
    /// Adjusts brightness of the sparkling / glitter particles
    /// </summary>
    /// <param name="intensity">New intensity from 0 to 5</param>
    public void ChangeSparklingIntensity(float intensity, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(intensity, 0, 5), time, (x) => _liquidVolume.sparklingIntensity = x, _liquidVolume.sparklingIntensity));
    }

    /// <summary>
    /// Adjusts Amount of sparkling / glitter particle
    /// </summary>
    /// <param name="amount">New particles amount from 0 to 1</param>
    public void ChangeSparklingAmount(float amount, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(amount, 0, 1), time, (x) => _liquidVolume.sparklingAmount = x, _liquidVolume.sparklingAmount));
    }
}
