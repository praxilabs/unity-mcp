using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Transform toolsParent;
    private ExperimentData _experimentData;
    private GameObject _toolsPrefab;
    private GameObject _stageCameras;

    private int _stageIndex = 0;
    private Action _stageStartPreparationsAction;

    private void OnEnable()
    {
        if (_stageStartPreparationsAction == null)
        {
            _stageStartPreparationsAction = () =>
            {
                _stageIndex = ExperimentManager.Instance.stageIndex;
                RemoveStagePrefab();
                InstantiateSceneTools();
            };
        }
        ExperimentManager.OnStageStart += _stageStartPreparationsAction;
    }

    private void OnDisable()
    {
        ExperimentManager.OnStageStart -= _stageStartPreparationsAction;
    }

    private void InstantiateSceneTools()
    {
        _experimentData = AssetBundlesManager.Instance.CurrentExperimentData;
        GameObject stagePrefab = _experimentData.experimentStages[_stageIndex].experimentTools;
        GameObject stageCameras = _experimentData.experimentCameras;
        
        if (stagePrefab == null || stageCameras == null)
        {
            Debug.Log("<color=#E4003A>Stage prefab not found</color>");
            return;
        }

        _toolsPrefab = Instantiate(stagePrefab, toolsParent);
        _toolsPrefab.name = stagePrefab.name;


        if (_stageCameras == null)
        {
            _stageCameras = Instantiate(stageCameras, toolsParent);
            _stageCameras.name = stageCameras.name;
            ActivateStageCameras();
        }
        else
            ActivateStageCameras();
    }

    private void ActivateStageCameras()
    {
        foreach (var interestPoint in _stageCameras.GetComponent<ExperimentCameras>().interestPoints)
        {
            InterestPointStagesToggle interestPointToggle = interestPoint.GetComponent<InterestPointStagesToggle>();

            if (interestPointToggle.activeInStages[_stageIndex] == InterestPointToggle.On)
                interestPoint.SetActive(true);
            else
                interestPoint.SetActive(false);
        }
    }

    private void RemoveStagePrefab()
    {
        if (_toolsPrefab != null && _toolsPrefab.GetComponent<RegisterObject>())
            _toolsPrefab.GetComponent<RegisterObject>().ClearStagePrefabRegistery();
        Destroy(_toolsPrefab);
    }
}
