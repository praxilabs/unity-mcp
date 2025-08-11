using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StageEndMessageDependent : StageEndMessageBase
{
    [SerializeField] private TextMeshProUGUI _goToNextStageText;
    private UnityAction _goToNextStageDelegate;

    protected override void OnEnable()
    {
        base.OnEnable();
        _goToNextStageDelegate = () =>
        {
            ExperimentManager.Instance.GoToStage();
            ToggleWindow(false);
        };
        _goToNextStage.onClick.AddListener(_goToNextStageDelegate);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _goToNextStage.onClick.RemoveListener(_goToNextStageDelegate);
    }

    protected override void UpdateText(StageEndMessageData endMessageData)
    {
        base.UpdateText(endMessageData);

        _goToNextStageText.text = endMessageData.goToNextStageText;
    }
}