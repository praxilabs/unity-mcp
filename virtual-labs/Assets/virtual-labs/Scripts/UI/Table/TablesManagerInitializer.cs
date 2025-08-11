using System;
using Table;
using Table.UI.SideMenuButtons;
using UnityEngine;

public class TablesManagerInitializer : MonoBehaviour
{
    private TablesManager _tablesManager;
    private Action _prepareForNewStageDelegate;

    private void OnEnable()
    {
        ResolveObjects();
        _prepareForNewStageDelegate = PrepareForNewStage;

        ExperimentManager.OnStageStart += _prepareForNewStageDelegate;
    }

    private void OnDisable()
    {
        ExperimentManager.OnStageStart -= _prepareForNewStageDelegate;
    }

    private void PrepareForNewStage()
    {
        _tablesManager.stagesAvailable = AssetBundlesManager.Instance.CurrentExperimentData.tableStages.ToArray();
        _tablesManager.Initialize(ExperimentManager.Instance.stageIndex);
        RemoveRecordButton();
        ResetTable();
    }

    private void RemoveRecordButton()
    {
        RecordReadingsButton recordButton = _tablesManager.GetRecordButton();

        if (recordButton == null) return; 

        if (ExperimentItemsContainer.Instance.experimentItems.Contains(recordButton.gameObject))
            ExperimentItemsContainer.Instance.experimentItems.Remove(recordButton.gameObject);
        _tablesManager.RemoveRecordButton();
    }

    private void ResetTable()
    {
        if (ExperimentManager.Instance.isStageReloading)
        {
            _tablesManager.RemoveTrials();
            _tablesManager.ResetTable(ExperimentManager.Instance.stageIndex + 1);

        }
        else
            _tablesManager.ResetTable(ExperimentManager.Instance.stageIndex);
    }

    private void ResolveObjects()
    {
        _tablesManager = ExperimentItemsContainer.Instance.Resolve<TablesManager>();
    }
}
