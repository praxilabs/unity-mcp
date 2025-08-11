using Cinemachine;
using UnityEngine;

public class InterestPoint : MonoBehaviour
{
    public CinemachineVirtualCamera poiCamera;
    public InterestPointType poiType;

    public void SetMainBenchCamera()
    {
        GetComponentInParent<ExperimentCameras>().UpdateMainBenchCamera(this);
    }
}

public enum InterestPointType
{
    MainBench,
    InterestPoint,
    SettingsMenu
}