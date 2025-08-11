using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.Events;

public class StageEndMessaegMultiScenario : StageEndMessageBase
{

    [SerializeField] private TextMeshProUGUI _goToNextStageText;
    [SerializeField] private TextMeshProUGUI _endExperimentText;
    [SerializeField] private CleanButton _endExperiment;
    [SerializeField] private CleanButton _playScenario;
    [SerializeField] private TMP_Dropdown _scenariosDropdown;

    public static UnityEvent onPlayScenarioClick;
    public static UnityEvent<int> onScenarioSelectionClick;

    private UnityAction _goToNextStageDelegate;
    private UnityAction _endExperimentDelegate;

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


        _playScenario.onClick.AddListener(OnPlayClickInvoke);
        _scenariosDropdown.onValueChanged.AddListener(OnScenarioSelectionClickInvoke);
        _goToNextStage.onClick.AddListener(_goToNextStageDelegate);
        _endExperiment.onClick.AddListener(_endExperimentDelegate);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _playScenario.onClick.RemoveListener(OnPlayClickInvoke);
        _scenariosDropdown.onValueChanged.RemoveListener(OnScenarioSelectionClickInvoke);
        _goToNextStage.onClick.RemoveListener(_goToNextStageDelegate);
        _endExperiment.onClick.RemoveListener(_endExperimentDelegate);
    }

    protected override void UpdateText(StageEndMessageData endMessageData)
    {
        base.UpdateText(endMessageData);

        _goToNextStageText.text = endMessageData.goToNextStageText;
        _endExperimentText.text = endMessageData.endExperimentText;
    }

    private void OnPlayClickInvoke()
    {
        onPlayScenarioClick.Invoke();
    }

    private void OnScenarioSelectionClickInvoke(int index)
    {
        onScenarioSelectionClick.Invoke(index);
    }
}