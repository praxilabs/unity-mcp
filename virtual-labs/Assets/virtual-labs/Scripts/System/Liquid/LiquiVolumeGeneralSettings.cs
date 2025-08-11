using LiquidVolumeFX;
using UnityEngine;

public class LiquiVolumeGeneralSettings : MonoBehaviour
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
    /// Adjusts the overall transparency of liquid, smoke and foam
    /// </summary>
    public virtual void ChangeGlobalAlpha(float newAlpha, float timeToReach)
    {
        float alpha2Reach = newAlpha;
        float time2Reach =timeToReach;

        StartCoroutine(_helper.LerpFloat(alpha2Reach, time2Reach, (x) => _liquidVolume.alpha = x, _liquidVolume.alpha));
    }

    /// <summary>
    /// Change container toplogy (geometry)
    /// </summary>
    public virtual void ChangeContainerTopology(TOPOLOGY topology)
    {
        _liquidVolume.topology = topology;
    }

    /// <summary>
    /// Adjust container detail can be used to change to another container detail that supports different liquid features 
    /// </summary>
    public virtual void ChangeContainerDetail(DETAIL detail)
    {
        _liquidVolume.detail = detail;
    }

    /// <summary>
    /// Enables z-testing inside liquid volume. Useful if volume contains other objects in addition to liquid
    /// </summary>
    public void SetDepthAware(bool isDepthAware)
    {
        _liquidVolume.depthAware = isDepthAware;
    }
}
