using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentCameras : MonoBehaviour
{
    public CinemachineVirtualCamera mainBenchCamera;
    public CinemachineVirtualCamera settingsMenuCamera;
    public List<GameObject> interestPoints = new List<GameObject>();
    public List<GameObject> interestPointsEffect = new List<GameObject>();

    public void UpdateMainBenchCamera(InterestPoint interestPoint)
    {
        foreach (GameObject go in interestPoints)
        {
            InterestPoint poi = go.GetComponent<InterestPoint>();
            if (poi == interestPoint)
            {
                poi.poiType = InterestPointType.MainBench;
                mainBenchCamera = poi.poiCamera;
            }
            else if (poi != interestPoint && poi.poiType == InterestPointType.MainBench)
                poi.poiType = InterestPointType.InterestPoint;
        }
    }
}
