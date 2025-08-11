using UltimateClean;
using UnityEngine;

public class BirdEyeUIHolder : MonoBehaviour
{
    public CleanButton firstPersonPOV;

    [SerializeField] private Canvas _birdEyeCanvas;

    public void ToggleBirdEyeUI(bool enable)
    {
        _birdEyeCanvas.enabled = enable;
    }
}