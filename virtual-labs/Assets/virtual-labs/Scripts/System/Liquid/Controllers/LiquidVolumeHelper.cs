using LiquidVolumeFX;
using Praxilabs.Timekeeping.Timer;
using System.Collections;
using UnityEngine;

public class LiquidVolumeHelper : MonoBehaviour
{
    private LiquidVolume _liquidVolume;
    private TimerHandler _timerHandler;

    private void Start()
    {
        _liquidVolume = GetComponent<LiquidVolumeController>().LiquidVolume;
        _timerHandler = ExperimentItemsContainer.Instance.Resolve<TimerHandler>();
    }

    /// <summary>
    /// Returns the liquid surface Y position
    /// </summary>
    public float GetLiquidSurfaceYPosition()
    {
        return _liquidVolume.liquidSurfaceYPosition;
    }

    /// <summary>
    /// Returns the spill point of the liquid
    /// </summary>
    public Vector3 GetSpillPoint()
    {
        bool spillExist = _liquidVolume.GetSpillPoint(out Vector3 spillPoint);

        return spillExist ? spillPoint : Vector3.zero;
    }

    /// <summary>
    /// Change liquid color if detail is Default, DefaultNoFlask, BumpTexture or Reflections
    /// </summary>
    public IEnumerator ChangeLiquidColorWithTime(Color newColor1, Color newColor2, float time)
    {
        if (_liquidVolume != null)
        {
            if (_liquidVolume.detail == DETAIL.Default || _liquidVolume.detail == DETAIL.DefaultNoFlask)
            {
                Color initialColor1 = _liquidVolume.liquidColor1;
                Color initialColor2 = _liquidVolume.liquidColor2;
                float lerpValue = 0;

                while (lerpValue < 1)
                {
                    lerpValue += Time.deltaTime / time;
                    _liquidVolume.liquidColor1 = Color.Lerp(initialColor1, newColor1, lerpValue);
                    _liquidVolume.liquidColor2 = Color.Lerp(initialColor2, newColor2, lerpValue);
                    yield return null;
                }

                _liquidVolume.liquidColor1 = newColor1;
                _liquidVolume.liquidColor2 = newColor2;
            }
        }
        else
            Debug.Log("Color=#FFA725>This container doesn't have LiquidVolume reference</Color>");
    }

    /// <summary>
    /// Change liquid color with timer
    /// </summary>
    public IEnumerator ChangeLiquidColorWithTimer(Color newColor1, Color newColor2)
    {
        yield return new WaitForSeconds(0.5f);
        if (_liquidVolume == null || _timerHandler == null)
        {
            Debug.Log("<color=#FFA725>This container doesn't have LiquidVolume reference</color>");
            yield break;
        }

        if (_liquidVolume.detail != DETAIL.Default && _liquidVolume.detail != DETAIL.DefaultNoFlask)
            yield break;

        Color initialColor1 = _liquidVolume.liquidColor1;
        Color initialColor2 = _liquidVolume.liquidColor2;

        float initialTime = (float)_timerHandler.timer.GetInitialTimeInSeconds();

        if (initialTime <= 0f)
        {
            Debug.LogWarning("Initial time must be greater than zero.");
            yield break;
        }

        while (true)
        {
            float remainingTime = (float)_timerHandler.timer.GetRemainingTimeInSeconds();
            float elapsedTime = initialTime - remainingTime;
            float progress = Mathf.Clamp01(elapsedTime / initialTime);

            _liquidVolume.liquidColor1 = Color.Lerp(initialColor1, newColor1, progress);
            _liquidVolume.liquidColor2 = Color.Lerp(initialColor2, newColor2, progress);

            if (progress >= 1f)
                break;

            yield return null;
        }

        _liquidVolume.liquidColor1 = newColor1;
        _liquidVolume.liquidColor2 = newColor2;
    }


    /// <summary>
    /// Lerps color to a new one
    /// </summary>
    /// <param name="target">Which parameter in LiquidVolume to change(callback)</param>
    /// <param name="initValue">Parameter in LiquidVolume to change initial value before lerp</param>
    public IEnumerator LerpColor(Color newColor, float time, System.Action<Color> target, Color initValue)
    {
        if (_liquidVolume != null)
        {
            Color initialEmissionColor = initValue;
            float lerpValue = 0;

            while (lerpValue < 1)
            {
                lerpValue += Time.deltaTime / time;
                target(Color.Lerp(initialEmissionColor, newColor, lerpValue));
                yield return null;
            }

            yield return null;
            target(newColor);

        }
        else
            Debug.Log("Color=#FFA725>This container doesn't have LiquidVolume reference</Color>");
    }

    /// <summary>
    /// Lerps liquid volume level (capacity)
    /// </summary>
    /// <param name="valueToReach">New liquid level(volume)</param>
    public IEnumerator AdjustLiquidLevelWithTime(float valueToReach, float time)
    {
        if (_liquidVolume != null)
        {
            float lerpValue = 0;
            float initialValue = _liquidVolume.level;
            while (lerpValue < 1 && time != 0)
            {
                lerpValue += Time.deltaTime / time;
                _liquidVolume.level = Mathf.Lerp(initialValue, valueToReach, lerpValue);
                yield return null;
            }
            _liquidVolume.level = valueToReach;

        }
        else
            Debug.Log("Color=#FFA725>This container doesn't have LiquidVolume reference</Color>");
    }

    /// <summary>
    /// Lerps Float variable 
    /// </summary>
    /// <param name="target">Which parameter in LiquidVolume to change(callback)</param>
    /// <param name="initValue">Parameter in LiquidVolume to change initial value before lerp</param>
    public IEnumerator LerpFloat(float valueToReach, float time, System.Action<float> target, float initValue)
    {
        if (_liquidVolume != null)
        {
            float lerpValue = 0;
            float initialValue = initValue;

            while (lerpValue < 1 && time != 0)
            {
                lerpValue += Time.deltaTime / time;
                target(Mathf.Lerp(initialValue, valueToReach, lerpValue));

                yield return null;
            }
            yield return null;
            target(valueToReach);
        }
        else
            Debug.Log("Color=#FFA725>This container doesn't have LiquidVolume reference</Color>");
    }

    /// <summary>
    /// Lerps float vairable for liquid layers (if Multiple or MultipleNoFlask modes are selected)
    /// </summary>
    /// <param name="target">Which parameter in LiquidVolume to change(callback)</param>
    /// <param name="initValue">Parameter in LiquidVolume to change initial value before lerp</param>
    /// <param name="updateLayers">True or False to call UpdateLayers() function to update layers</param>
    public IEnumerator LerpFloatLayer(float valueToReach, float time, System.Action<float> target, float initValue, bool updateLayers)
    {
        if (_liquidVolume != null)
        {
            float lerpValue = 0;
            float initialValue = initValue;

            while (lerpValue < 1 && time != 0)
            {
                lerpValue += Time.deltaTime / time;
                target(Mathf.Lerp(initialValue, valueToReach, lerpValue));
                if (updateLayers)
                    _liquidVolume.UpdateLayers(false);

                yield return null;
            }
            yield return null;

            target(valueToReach);
            if (updateLayers)
                _liquidVolume.UpdateLayers(false);
        }
        else
            Debug.Log("Color=#FFA725>This container doesn't have LiquidVolume reference</Color>");
    }

    /// <summary>
    /// Lerps color for liquid layers (if Multiple or MultipleNoFlask modes are selected)
    /// </summary>
    /// <param name="target">Which parameter in LiquidVolume to change(callback)</param>
    /// <param name="initValue">Color initial value before lerp</param>
    /// <param name="updateLayers">True or False to call UpdateLayers() function to update layers</param>
    public IEnumerator LerpLayerLiquidColor(Color newColor, float time, System.Action<Color> target, Color initValue, bool updateLayers)
    {
        if (_liquidVolume != null)
        {
            Color initialColor = _liquidVolume.liquidColor1;
            float lerpValue = 0;

            while (lerpValue < 1 && time != 0)
            {
                lerpValue += Time.deltaTime / time;
                target(Color.Lerp(initialColor, newColor, lerpValue));
                if (updateLayers)
                    _liquidVolume.UpdateLayers(false);

                yield return null;
            }
            yield return null;

            target(newColor);
            if (updateLayers)
                _liquidVolume.UpdateLayers(false);
        }
        else
            Debug.Log("Color=#FFA725>This container doesn't have LiquidVolume reference</Color>");
    }

    public IEnumerator LerpDensity(float valueToReach, float time, int layerIndex, float initValue, bool updateLayers)
    {
        if (_liquidVolume != null)
        {
            float lerpValue = 0;
            float initialValue = initValue;

            while (lerpValue < 1 && time != 0)
            {
                lerpValue += Time.deltaTime / time;
                _liquidVolume.SetLayerDensity(layerIndex, lerpValue, true);
                if (updateLayers)
                    _liquidVolume.UpdateLayers(false);

                yield return null;
            }
            yield return null;

            _liquidVolume.SetLayerDensity(layerIndex, valueToReach, true);
            if (updateLayers)
                _liquidVolume.UpdateLayers(false);
        }
        else
            Debug.Log("Color=#FFA725>This container doesn't have LiquidVolume reference</Color>");
    }
}
