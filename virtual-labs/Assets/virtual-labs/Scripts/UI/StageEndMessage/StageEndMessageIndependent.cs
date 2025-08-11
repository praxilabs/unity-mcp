using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.Events;

public class StageEndMessageIndependent : StageEndMessageBase
{
    [SerializeField] private TextMeshProUGUI _goToNextStageText;
    [SerializeField] private TextMeshProUGUI _endExperimentText;
    [SerializeField] private CleanButton _endExperiment;
    [SerializeField] private CleanButton _restartStage;

    private UnityAction _goToNextStageDelegate;
    private UnityAction _endExperimentDelegate;
    private UnityAction _restartStageDelegate;

    protected override void OnEnable()
    {
        base.OnEnable();
        _goToNextStageDelegate = () =>
        {
            ExperimentManager.Instance.GoToStage();
            ToggleWindow(false);
        };

        _endExperimentDelegate = () =>
        {
            ExperimentManager.Instance.EndStageInvoke();
            ToggleWindow(false);
        };

        _restartStageDelegate = () =>
        {
            ExperimentManager.Instance.ReloadStage();
            ToggleWindow(false);
        };

        _goToNextStage.onClick.AddListener(_goToNextStageDelegate);
        _endExperiment.onClick.AddListener(_endExperimentDelegate);
        _restartStage.onClick.AddListener(_restartStageDelegate);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _goToNextStage.onClick.RemoveListener(_goToNextStageDelegate);
        _endExperiment.onClick.RemoveListener(_endExperimentDelegate);
        _restartStage.onClick.RemoveListener(_restartStageDelegate);
    }

    protected override void UpdateText(StageEndMessageData endMessageData)
    {
        base.UpdateText(endMessageData);

        _goToNextStageText.text = endMessageData.goToNextStageText;
        _endExperimentText.text = endMessageData.endExperimentText;
    }
}