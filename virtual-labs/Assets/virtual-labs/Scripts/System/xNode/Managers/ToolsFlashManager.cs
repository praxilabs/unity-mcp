using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolsFlashManager : Singleton<ToolsFlashManager>
{
    public List<GameObject> flashingTools = new List<GameObject>();
    public bool canFlash;
    private Action _prepareForNewStageAction;

    private void OnEnable()
    {
        _prepareForNewStageAction = PrepareForNewStage;
        ExperimentManager.OnStageStart += _prepareForNewStageAction;
    }

    private void OnDisable()
    {
        ExperimentManager.OnStageStart -= _prepareForNewStageAction;
    }

    public void StartFlashing(GameObject objectTOFlash)
    {
        Hologram toolHologram = objectTOFlash.GetComponentInParent<Hologram>();

        if (toolHologram != null)
            toolHologram.StartPulsating();
    }

    public void StopFlashing(GameObject objectToFlash)
    {
        Hologram toolHologram = objectToFlash.GetComponentInParent<Hologram>();

        if (toolHologram != null)
            toolHologram.StopPulsating();
    }

    private void PrepareForNewStage()
    {
        flashingTools.Clear();
    }
}