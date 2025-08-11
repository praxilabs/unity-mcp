using LiquidVolumeFX;
using System;
using System.Linq;
using UnityEngine;

public class LiquidVolumeMultipleLiquidSettings : MonoBehaviour
{
    private LiquidVolume _liquidVolume;
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
    /// Add a new layer to the Multiple or MultipleNoFlask Liquid
    /// </summary>
    /// <param name="amount">New liquid amount</param>
    /// <param name="hexColor">New layer color in Hex Code</param>
    /// <param name="layerDensity">New layer density</param>
    public void AddLayer(float amount, string hexColor, float layerDensity, float time, bool updateLayers)
    {
        LiquidVolume.LiquidLayer[] tempLayer = _liquidVolume.liquidLayers;
        Array.Resize<LiquidVolume.LiquidLayer>(ref tempLayer, _liquidVolume.liquidLayers.Length + 1);
       
        _liquidVolume.liquidLayers = tempLayer;
        _liquidVolume.liquidLayers[_liquidVolume.liquidLayers.Length - 1].amount = 0;

        Color newColor = GameHelper.ConvertHexToColor(hexColor);
        _liquidVolume.liquidLayers[_liquidVolume.liquidLayers.Length - 1].color = newColor;
        _liquidVolume.liquidLayers[_liquidVolume.liquidLayers.Length - 1].density = layerDensity;

        LiquidLevel = amount / ContainerCapacity;
        StartCoroutine(_helper.LerpFloatLayer(Mathf.Clamp(LiquidLevel, 0, 1), time, (x) => _liquidVolume.liquidLayers[_liquidVolume.liquidLayers.Length - 1].amount = x,
            0, updateLayers));
    }

    /// <summary>
    /// Delete a layer from the layers in Multiple or MultipleNoFlask Liquid
    /// </summary>
    public void DeleteLayer(float indexToRemove)
    {
        float iTR = indexToRemove;

        _liquidVolume.sortedLayers = _liquidVolume.sortedLayers.Where(x => x != iTR).ToArray();
        _liquidVolume.liquidLayers = _liquidVolume.liquidLayers.Where((source, index) => index != iTR).ToArray();
        _liquidVolume.sortedLayersCount--;

        for (int i = 0; i < _liquidVolume.sortedLayers.Length; i++)
        {
            if (_liquidVolume.sortedLayers[i] > iTR)
                _liquidVolume.sortedLayers[i]--;
        }
    }

    /// <summary>
    /// Lerps Global Adjustment Speed
    /// liquid layers are sorted by density. When density is changed and  the layer positions change, this option controls the global speed of change
    /// </summary>
    /// <param name="adjustmentSpeed">New Adjustment Speed from 0 to 10</param>
    public void ChangeLayersAdjustmentSpeed(float adjustmentSpeed, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(adjustmentSpeed, 0, 10), time, (x) => _liquidVolume.layersAdjustmentSpeed = x, _liquidVolume.layersAdjustmentSpeed));
    }

    /// <summary>
    /// Lerps dither strength which helps reducing banding effect between layers
    /// </summary>
    /// <param name="ditherStrength">Dither Strength from 0 to 1</param>
    public void ChangeLayersDitherStrength(float ditherStrength, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(ditherStrength, 0, 1), time, (x) => _liquidVolume.ditherStrength = x, _liquidVolume.ditherStrength));
    }

    /// <summary>
    /// Lerps the blending between surface layers
    /// It makes the layers look a bit blurry
    /// </summary>
    /// <param name="contactSurfaceSmoothness">New smoothness value from 0 to 16</param>
    public void ChangeLayersContactSurfaceSmoothness(float contactSurfaceSmoothness, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(contactSurfaceSmoothness, 0, 16), time, (x) => _liquidVolume.smoothContactSurface = (int)x, _liquidVolume.smoothContactSurface));
    }

    /// <summary>
    /// Avoid gaps between layers when reordering layers
    /// </summary>
    public void CompactLayers(bool isCompact)
    {
        _liquidVolume.layersAdjustmentCompact = isCompact;
    }

    /// <summary>
    /// Adds or remove X amount of liquid to a layer
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    /// <param name="newLayerAmount">New liquid amount from 0 to 1 P.S. 1 means that the liquid only contains 1 layer that fills the container. from 0 to 1</param>
    /// <param name="updateLayers">Call UpdateLayers function in LiquidVolume script
    /// (operations on layers requires call to update layer function in order for the layers to update visually)</param>
    public void ChangeLayerAmount(int layerIndex, float newLayerAmount, float time, bool updateLayers)
    {
        LiquidLevel = newLayerAmount / ContainerCapacity;

        StartCoroutine(_helper.LerpFloatLayer(Mathf.Clamp(LiquidLevel, 0, 1), time, (x) => _liquidVolume.liquidLayers[layerIndex].amount = x,
            _liquidVolume.liquidLayers[layerIndex].amount, updateLayers));
    }

    /// <summary>
    /// Call UpdateLayers function in LiquidVolume script
    /// </summary>
    public void UpdateLayers(bool immediate)
    {
            _liquidVolume.UpdateLayers(immediate);
    }

    /// <summary>
    /// Lerps a layer's density. Keep in mind that heavier layers go to the bottom of the container
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    /// <param name="newDensity">New layer's density from 0 to 1</param>
    /// <param name="updateLayers">Call UpdateLayers function in LiquidVolume script
    /// (operations on layers requires call to update layer function in order for the layers to update visually)</param>
    public void ChangeLayerDensity(int layerIndex, float newDensity, float time, bool updateLayers)
    {
        StartCoroutine(_helper.LerpFloatLayer(newDensity, time, (x) => _liquidVolume.liquidLayers[layerIndex].density = x,
            _liquidVolume.liquidLayers[layerIndex].density, updateLayers));
    }

    /// <summary>
    /// Change layer density while keeping layer amount the same
    /// </summary>
    /// <param name="layerIndex">Which layer to alter it's density</param>
    /// <param name="newDensity">New layer density</param>
    public void ChangeLayerDensityWhilePreservingAmount(int layerIndex, float newDensity, float time, bool updateLayers)
    {
        StartCoroutine(_helper.LerpDensity(newDensity, time, layerIndex, _liquidVolume.liquidLayers[layerIndex].density, updateLayers));
    }

    /// <summary>
    /// Change a layer's Miscibility (layers with same density will only mix if Miscible is set to true)
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    public void ChangeMiscibility(int layerIndex, bool isMiscible)
    {
        _liquidVolume.liquidLayers[layerIndex].miscible = isMiscible;
    }

    /// <summary>
    /// Lerps how turpid and murky a layer looks
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    /// <param name="newMurkiness">New murkiness value from 0 to 1</param>
    /// <param name="updateLayers">Call UpdateLayers function in LiquidVolume script
    /// (operations on layers requires call to update layer function in order for the layers to update visually)</param>
    public void ChangeLayerMurkiness(int layerIndex, float newMurkiness, float time, bool updateLayers)
    {
        StartCoroutine(_helper.LerpFloatLayer(Mathf.Clamp(newMurkiness, 0, 1), time, (x) => _liquidVolume.liquidLayers[layerIndex].murkiness = x,
            _liquidVolume.liquidLayers[layerIndex].murkiness, updateLayers));
    }

    /// <summary>
    /// Lerps the scale of the noise used for the murkiness effect.
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    /// <param name="newScale">New noise scale value from 0 to 0.48</param>
    /// <param name="updateLayers">Call UpdateLayers function in LiquidVolume script
    /// (operations on layers requires call to update layer function in order for the layers to update visually)</param>
    public void ChangeLayerScale(int layerIndex, float newScale, float time, bool updateLayers)
    {
        StartCoroutine(_helper.LerpFloatLayer(Mathf.Clamp(newScale, 0, 0.48f), time, (x) => _liquidVolume.liquidLayers[layerIndex].scale = x,
            _liquidVolume.liquidLayers[layerIndex].scale, updateLayers));
    }

    /// <summary>
    /// Lerps the influence of external forces / turbulences on this layer. Some liquids are stickier than others(ie.honey vs alcohol).
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    /// <param name="newViscosity">How heavy is the layer from -5 to 5</param>
    /// <param name="updateLayers">Call UpdateLayers function in LiquidVolume script
    /// (operations on layers requires call to update layer function in order for the layers to update visually)</param>
    public void ChangeLayerViscosity(int layerIndex, float newViscosity, float time, bool updateLayers)
    {
        StartCoroutine(_helper.LerpFloatLayer(Mathf.Clamp(newViscosity, -5, 5f), time, (x) => _liquidVolume.liquidLayers[layerIndex].viscosity = x,
            _liquidVolume.liquidLayers[layerIndex].viscosity, updateLayers));
    }

    /// <summary>
    /// Lerps the opacity of bubbles in a given layer
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    /// <param name="newBubblesOpacity">New opacity for the bubbles from -100 to 100 P.S. using -ve values will produce dark bubbles</param>
    /// <param name="updateLayers">Call UpdateLayers function in LiquidVolume script
    /// (operations on layers requires call to update layer function in order for the layers to update visually)</param>
    public void ChangeLayerBubblesOpacity(int layerIndex, float newBubblesOpacity, float time, bool updateLayers)
    {
        StartCoroutine(_helper.LerpFloatLayer(Mathf.Clamp(newBubblesOpacity, -100, 100f), time, (x) => _liquidVolume.liquidLayers[layerIndex].bubblesOpacity = x,
            _liquidVolume.liquidLayers[layerIndex].bubblesOpacity, updateLayers));
    }

    /// <summary>
    /// Lerps the seed of bubbles across all layers
    /// </summary>
    /// <param name="newBubblesSeed">New bubbles seed</param>
    public void ChangeBubblesSeed(float newBubblesSeed, float time)
    {
        StartCoroutine(_helper.LerpFloat(newBubblesSeed, time, (x) => _liquidVolume.bubblesSeed = (int)x,
           _liquidVolume.bubblesSeed));
    }

    /// <summary>
    /// Lerps the amount of bubbles across all layers
    /// </summary>
    /// <param name="newBubblesAmount">New bubbles amount</param>
    public void ChangeBubblesAmount(float newBubblesAmount, float time)
    {
        StartCoroutine(_helper.LerpFloat(newBubblesAmount, time, (x) => _liquidVolume.bubblesAmount = (int)x,
           _liquidVolume.bubblesAmount));
    }

    /// <summary>
    /// Lerps bubbles min value across all layers
    /// </summary>
    /// <param name="newBubblesSizeMin">New bubbles min value</param>
    public void ChangeBubblesSizeMin(float newBubblesSizeMin, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(newBubblesSizeMin, 0, 16f), time, (x) => _liquidVolume.bubblesSizeMin = (int)x,
           _liquidVolume.bubblesSizeMin));
    }

    /// <summary>
    /// Lerps bubbles max value across all layers
    /// </summary>
    /// <param name="newBubblesSizeMax">New bubbles max value</param>
    public void ChangeBubblesSizeMax(float newBubblesSizeMax, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(newBubblesSizeMax, 0, 16f), time, (x) => _liquidVolume.bubblesSizeMax = (int)x,
           _liquidVolume.bubblesSizeMax));
    }

    /// <summary>
    /// Lerps bubbles vertical speed
    /// P.S. if -ve values are passed to this function the bubbles withh go down instead of up
    /// </summary>
    /// <param name="newBubblesVerticalSpeed">New bubbles vertical speed from -10 to 10</param>
    public void ChangeBubblesVerticalSpeed(float newBubblesVerticalSpeed, float time)
    {
        StartCoroutine(_helper.LerpFloat(Mathf.Clamp(newBubblesVerticalSpeed, -10, 10f), time, (x) => _liquidVolume.bubblesVerticalSpeed = x,
           _liquidVolume.bubblesVerticalSpeed));
    }

    /// <summary>
    /// Lerps the murkiness color of a layer
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    /// <param name="hexColor">New murkiness color in Hex Code</param>
    /// <param name="updateLayers">Call UpdateLayers function in LiquidVolume script
    /// (operations on layers requires call to update layer function in order for the layers to update visually)</param>
    public void ChangeLayerMurkinessColor(int layerIndex, string hexColor, float time, bool updateLayers)
    {
        Color newColor = GameHelper.ConvertHexToColor(hexColor);

        StartCoroutine(_helper.LerpLayerLiquidColor(newColor, time, (x) => _liquidVolume.liquidLayers[layerIndex].murkColor = x,
            _liquidVolume.liquidLayers[layerIndex].murkColor, updateLayers));
    }

    /// <summary>
    /// Lerps a layer color
    /// </summary>
    /// <param name="layerIndex">Index of the layer to adjust. layers start from index 0</param>
    /// <param name="hexColor">New layer color in Hex Code</param>
    /// <param name="updateLayers">Call UpdateLayers function in LiquidVolume script
    /// (operations on layers requires call to update layer function in order for the layers to update visually)</param>
    public virtual void ChangeLayerColor(int layerIndex, string hexColor, float time, bool updateLayers)
    {
        Color newColor = GameHelper.ConvertHexToColor(hexColor);

        StartCoroutine(_helper.LerpLayerLiquidColor(newColor, time, (x) => _liquidVolume.liquidLayers[layerIndex].color = x,
            _liquidVolume.liquidLayers[layerIndex].color, updateLayers));
    }
}
