using UnityEngine;

public class LiquidVolumeSpill : MonoBehaviour
{
    // private SpillController _spillController;

    private void Start()
    {
        // _spillController = GetComponent<LiquidVolumeController>().spillController;
    }

    /// <summary>
    /// Enables the container ability to spill liquid when rotated
    /// </summary>
    public void ActivateSpill(bool activate)
    {
        // if (_spillController == null) return;

        // _spillController.enabled = activate;
    }

    /// <summary>
    /// Minimum level of liquid to stop container spill at
    /// </summary>
    /// <param name="minLevel">From 0 to 1</param>
    public void SetSpillMinCapacity(float minCapacity)
    {
        // if (_spillController == null) return;

        // _spillController.minCapacity = minCapacity;
    }

    public void SetParticleDestroyTime(float destroyTime)
    {
        // if (_spillController == null) return;

        // _spillController.particleDestroyTime = destroyTime;
    }
}
