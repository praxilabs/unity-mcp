using System;
using System.Collections;
using System.Collections.Generic;
using Praxilabs.CameraSystem;
using UnityEngine;
using UnityEngine.UI;

public class ExplorationHandler : Singleton<ExplorationHandler>
{
    [SerializeField] private GameObject _confirmationPopupWindow;
    [SerializeField] private Button _startExplorationBtn;
    [SerializeField] private Button _cancelExplorationBtn;
    [SerializeField] private Button _skipExplorationBtn;
    private ExplorationHintsContainer _explorationHintsContainer;
    private Action _onActionCallback;
    private int _currentGroupIndex = -1;
    private WaitForSeconds _waitTime = new WaitForSeconds(2f);

    private void Start()
    {
        _startExplorationBtn.onClick.AddListener(() =>
        {
            ToggleConfirmationPopupWindow(false);
            StartExplorationMode();
        });
        _cancelExplorationBtn.onClick.AddListener(() =>
        {
            ToggleConfirmationPopupWindow(false);
            EndExploringInstantly();
        });
        _skipExplorationBtn.onClick.AddListener(() =>
        {
            HandleSkipExploration();
        });
    }

    public void SetExplorationContainer(ExplorationHintsContainer explorationContainer) => _explorationHintsContainer = explorationContainer;

    // Exploration Confirmation Popup Window
    public void PrepareExplorationPopup(int explorationGroupIndex, Action OnActionCallback)
    {
        _currentGroupIndex = explorationGroupIndex;
        _onActionCallback = OnActionCallback;

        if (!_explorationHintsContainer.IsValidGroupIndex(explorationGroupIndex))
        {
            EndExploringInstantly();
            Debug.LogWarning("Invalid exploration group index");
            return;
        }
        _explorationHintsContainer.SetGroupIndex(_currentGroupIndex);

        CameraManager.Instance.FreezeAll(true);
        ToggleConfirmationPopupWindow(true);
    }

    public void CompletedExploringHints()
    {
        StartCoroutine(EndExploringCoroutine());
    }

    public List<Hologram> GetHintsFlashableObjectHologram()
    {
        if (_currentGroupIndex == -1)
        {
            Debug.LogWarning("Invalid Group Index");
            return null;
        }
        return _explorationHintsContainer.GetHintsFlashableObjectHologram(_currentGroupIndex);
    }

    private void StartExplorationMode()
    {
        UIManager.Instance.ToggleTargetUI(false);
        ToggleExplorationHints(true);
        ToggleSkipExplorationButton(true);
        ExplorationHologramColorHelper.Instance.SetExplorationColor();
    }

    private IEnumerator EndExploringCoroutine()
    {
        ToggleSkipExplorationButton(false);
        yield return _waitTime;
        ToggleExplorationHints(false);
        UIManager.Instance.ToggleTargetUI(true);
        yield return new WaitForEndOfFrame();
        EndExploringInstantly();
    }

    private void EndExploringInstantly()
    {
        CameraManager.Instance.FreezeAll(false);
        ExplorationHologramColorHelper.Instance.SetDefaultColor();
        _onActionCallback?.Invoke();
    }

    private void HandleSkipExploration()
    {
        ToggleSkipExplorationButton(false);
        ToggleExplorationHints(false);
        UIManager.Instance.ToggleTargetUI(true);
        EndExploringInstantly();
    }

    private void ToggleConfirmationPopupWindow(bool enable)
    {
        _confirmationPopupWindow.SetActive(enable);
    }
    
    private void ToggleExplorationHints(bool enable)
    {
        _explorationHintsContainer.ToggleExplorationContainer(enable);
    }

    private void ToggleSkipExplorationButton(bool enable)
    {
        _skipExplorationBtn.gameObject.SetActive(enable);
    }
}