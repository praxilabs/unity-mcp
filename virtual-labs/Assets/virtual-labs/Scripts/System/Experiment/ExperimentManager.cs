using Praxilabs;
using Praxilabs.CameraSystem;
using System;
using System.Collections;
using Table;
using UnityEngine;

public class ExperimentManager : Singleton<ExperimentManager>
{
    public int stageIndex = 0;
    public bool isStageReloading = false;

    [SerializeField] private FadeScreen fadeScreen;
    [SerializeField] private GameObject _tutorialCanvas;
    [SerializeField] private StageEndMessageHolder _stageEndMessageHolder;

    private ExperimentData _experimentData;
    private StepsGraph _stageGraph;
    private bool _isExperimentStarted;

    public static event Action OnStageStart;
    public static event Action OnStageEnd;

    private void Start()
    {
        Prepare();
        StartCoroutine(StartStage());
    }

    private void Prepare()
    {
        ExperimentItemsContainer.Instance.RegisterObjects();
        PlatformInitializer.Instance.CheckLabManagersPlatform();
    }

    private IEnumerator StartStage()
    {
        _experimentData = AssetBundlesManager.Instance.CurrentExperimentData;

        if (!_experimentData.hasTutorial)
            _tutorialCanvas.gameObject.SetActive(false);

        yield return StartCoroutine(PrepareStageCoroutine(stageIndex));
        if (!_experimentData.hasTutorial)
            StartExperiment(_stageGraph);
    }

    public void GoToStage(int StageIndex)
    {
        Debug.Log($"Experiment manager, switching stage to stage {StageIndex}");
        stageIndex = StageIndex;
        StartCoroutine(SwitchStageCoroutine(StageIndex));
    }

    public void GoToStage()
    {
        stageIndex++;
        Debug.Log($"Experiment manager, switching stage to stage {stageIndex}");
        StartCoroutine(SwitchStageCoroutine(stageIndex));
    }

    public void ReloadStage()
    {
        isStageReloading = true;
        GoToStage(stageIndex);
    }

    /// <summary>
    /// switch between stages
    /// </summary>
    public IEnumerator SwitchStageCoroutine(int stageIndex)
    {

        yield return StartCoroutine(EndCurrentStageCoroutine());
        yield return StartCoroutine(PrepareStageCoroutine(stageIndex));
        StartExperiment(_stageGraph);
    }

    private IEnumerator PrepareStageCoroutine(int stageIndex)
    {
        _stageGraph = _experimentData.experimentStages[stageIndex].experimentGraph;

        if (_stageGraph == null)
        {
            Debug.Log("<color=#E4003A>Stage _stageGraph not found</color>");
            yield break;
        }

        OnStageStart?.Invoke();
        yield return StartCoroutine(fadeScreen.EndFade(3));

        fadeScreen.gameObject.SetActive(false);
        isStageReloading = false;
    }

    public void StartExperimentAfterTutorial()
    {
        if (!_isExperimentStarted)
        {
            StartExperiment(_stageGraph);
            _isExperimentStarted = true;
        }
    }

    private void StartExperiment(StepsGraph stageGraph)
    {
        XnodeManager.Instance.StartGraph(stageGraph);
        CameraManager.Instance.stateRunner.StartCameraStateMachine();
    }

    private IEnumerator EndCurrentStageCoroutine()
    {
        EndStageInvoke();
        fadeScreen.gameObject.SetActive(true);

        yield return StartCoroutine(fadeScreen.StartFade(2));
        yield return new WaitForSeconds(0.2f);
    }

    public void EndStageInvoke()
    {
        OnStageEnd?.Invoke();
        OnStageEnd= null;
    }

    public void ToggleEndMessage(bool toggle)
    {
        foreach (var stageEndMessage in _stageEndMessageHolder.stageEndMessages)
        {
            if(stageEndMessage.stageEndMessageType == _experimentData.experimentStages[stageIndex].stageEndMessageType)
            {
                if (stageIndex == _experimentData.experimentStages.Count - 1)
                    stageEndMessage.stageEndMessage.ToggleGoToNextStageButton(!toggle);
                stageEndMessage.stageEndMessage.ToggleWindow(toggle);
            }
        }
    }
}