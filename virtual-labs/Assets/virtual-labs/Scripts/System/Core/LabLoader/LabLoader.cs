using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LabLoader : MonoBehaviour
{
    private Action _OnLocalizationLoadedDelegate;
    private void OnEnable()
    {
        _OnLocalizationLoadedDelegate =() => LoadLab();

        LocalizationLoader.OnLocalizationLoad += _OnLocalizationLoadedDelegate;
    }

    private void OnDisable()
    {
        LocalizationLoader.OnLocalizationLoad -= _OnLocalizationLoadedDelegate;
    }

    public void LoadLab()
    {
        ExperimentData experimentData = AssetBundlesManager.Instance.CurrentExperimentData;

        switch (experimentData.labType)
        {
            case LabTypes.Biology:
                SceneManager.LoadScene("biology lab");
                break;
            case LabTypes.Chemistry:
                SceneManager.LoadScene("chem lab");
                break;
            case LabTypes.Physics:
                SceneManager.LoadScene("physics lab");
                break;
            case LabTypes.PhysicsNoSink:
                SceneManager.LoadScene("physics_Without_sink");
                break;
        }
    }
}