using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestPointStagesToggle : MonoBehaviour
{
    public List<InterestPointToggle> activeInStages = new List<InterestPointToggle>();

    public void UpdateCameraStages (int count)
    {
        if (activeInStages != null &&
              activeInStages.Count != count)
        {
            activeInStages.Clear();

            for (int i = 0; i < count; i++)
                activeInStages.Add(InterestPointToggle.On);
        }
    }
}
public enum InterestPointToggle
{
    Off = 0,
    On = 1
}