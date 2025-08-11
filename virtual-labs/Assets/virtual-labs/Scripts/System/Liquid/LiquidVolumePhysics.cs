using LiquidVolumeFX;
using UnityEngine;

public class LiquidVolumePhysics : MonoBehaviour
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
    /// when enabled, the turbulence of the liquid will react to external forces or accelerations
    /// P.S. The sphere topology allows rotation of liquid inside the container as well as change in turbulence. Cylinder and Cube topologies will only alter the turbulence
    /// </summary>
    /// <param name="react">True or False</param>
    public void SetReactToForces(bool react)
    {
        _liquidVolume.reactToForces = react;
    }

    /// <summary>
    /// Adjust mass to a higher value will increase the weight of the liquid thus it will move less and vice versa
    /// </summary>
    public void ChangeMass(float liquidMass, float timeToReach)
    {
        float mass2Reach = Mathf.Clamp(liquidMass, 0, 5f);
        float time2Reach = timeToReach;

        StartCoroutine(_helper.LerpFloat(mass2Reach, time2Reach, (x) => _liquidVolume.physicsMass = x, _liquidVolume.physicsMass));
    }

    /// <summary>
    /// Angular damp defines the internal friction of the liquid. A higher value will make the liquid return to calm state faster
    /// </summary>
    public void ChangeAngularDamp(float newAngularDamp, float timeToReach)
    {
        float angularDamp2Reach = Mathf.Clamp(newAngularDamp, 0, 0.2f);
        float time2Reach = timeToReach;

        StartCoroutine(_helper.LerpFloat(angularDamp2Reach, time2Reach, (x) => _liquidVolume.physicsAngularDamp = x, _liquidVolume.physicsAngularDamp));
    }

    /// <summary>
    /// Enable this option to force liquid to rotate with the flask. Note that React To Forces option disables this
    /// </summary>
    public void SetIgnoreGravity(bool ignoreGravity)
    {
        _liquidVolume.ignoreGravity = ignoreGravity;
    }
}
