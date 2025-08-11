using Praxilabs.UIs;
using UnityEngine;

public class HintsStepProgressManager : MonoBehaviour
{
    [SerializeField] private HintsDisplay _hintsDisplayObject;
    [SerializeField] private HintsManager _uiManager;
    [SerializeField] private ProgressBarManager _progressBarManager;

    private int _stepIndex;
    private bool _isStepsProgressInitialized;

    public void InitializeStep(int stepNumber)
    {
        _stepIndex = stepNumber - 1;
        _progressBarManager.InitializeProgressBar(_hintsDisplayObject.StepPorgressScrollableContainerObject, _uiManager.hintsData.StepsData.Count, AddListenerToProgressButton);
        UpdateStepsProgress();
    }

    public void ToggleProgress()
    {
        _hintsDisplayObject.CurrentStepButton.gameObject.SetActive(!_hintsDisplayObject.CurrentStepButton.gameObject.activeSelf);
        _hintsDisplayObject.ProgressBarObject.gameObject.SetActive(!_hintsDisplayObject.ProgressBarObject.gameObject.activeSelf);
    }

    private void UpdateStepsProgress()
    {
        if (_uiManager.hintsData == null) return;

        for (int i = 0; i < _uiManager.hintsData.StepsData.Count; i++)
        {
            _progressBarManager.InitializeProgressBarStates(i, _uiManager.hintsData.StepsData[i].State);
        }
    }

    private void AddListenerToProgressButton(int stepNumber)
    {
        NavigateSteps(stepNumber);
    }

    private void NavigateSteps(int StepNumber)
    {
        if (StepNumber == _stepIndex) return;

        _stepIndex = StepNumber;
        _uiManager.UpdateUIView(_stepIndex + 1);
    }

}
