using LiquidVolumeFX;
using UnityEngine;

public class LiquidVolumeAdvancedSettings : MonoBehaviour
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
    /// if called with true the asset will detect if the camera enters the liquid container and will render appropiately. Note that this is an experimental option and some
    /// other properties are not supported when the camera is inside the container, like turbulence
    /// </summary>
    public void SetAllowViewFromInside(bool allowViewFromInside)
    {
        _liquidVolume.allowViewFromInside = allowViewFromInside;
    }

    /// <summary>
    /// Change the render queue of the liquid material, the default is 3001
    /// </summary>
    /// <param name="newRenderQueue">New render queue value</param>
    public void AdjustRenderQueue(int newRenderQueue)
    {
        _liquidVolume.renderQueue = newRenderQueue;
    }

    /// <summary>
    /// Liquid Volume includes three precomputed 3D noise textures. change the index from 1 to 3 to switch between them
    /// </summary>
    /// <param name="noiseIndex">Index of the used texture from 1 to 3 </param>
    public void ChangeNoiseTexture(int noiseIndex)
    {
        int noise = noiseIndex;
        if (noise == 1)
            _liquidVolume.noiseVariation = 1;
        else if (noise == 2)
            _liquidVolume.noiseVariation = 2;
        else if (noise == 3)
            _liquidVolume.noiseVariation = 3;
        else
            Debug.LogWarning("There's Only 3 noise textures set index from 1 to 3");
    }
}