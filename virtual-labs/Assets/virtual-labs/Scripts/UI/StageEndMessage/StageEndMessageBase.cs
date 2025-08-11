using LocalizationSystem;
using Newtonsoft.Json;
using ProgressMap.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateClean;
using UnityEngine;

public class StageEndMessageBase : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected CleanButton _goToNextStage;

    [SerializeField] protected TextMeshProUGUI _titleText;
    [SerializeField] protected TextMeshProUGUI _descriptionText;

    private Action _OnLanguageChangeDelegate;
    private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
    private Dictionary<int, StageEndMessageData> _endMessages = new Dictionary<int, StageEndMessageData>();
    private StageEndMessageData _currentEndMessageData;
    private bool _dataExists;

    protected virtual void OnEnable()
    {
        _OnLanguageChangeDelegate = () =>
        {
            if (!_dataExists) return;
            LoadEndMessageJson();
        };
        LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
    }

    protected virtual void OnDisable()
    {
        LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
    }

    protected virtual void Start()
    {
        LoadJson();
    }

    public virtual void ToggleWindow(bool toggle)
    {
        if (toggle)
            StartCoroutine(OpenWindowCoroutine());
        else
            StartCoroutine(CloseWindowCoroutine());
    }

    protected virtual IEnumerator OpenWindowCoroutine()
    {
        yield return StartCoroutine(ProgressMapController.Instance.HideUICoroutine());
        _animator.SetTrigger("Open");
    }

    protected virtual IEnumerator CloseWindowCoroutine()
    {
        _animator.SetTrigger("Close");
        yield return new WaitForSeconds(1.2f);
        ProgressMapController.Instance.ToggleMiniUI(true);
    }

    public virtual void ToggleGoToNextStageButton(bool toggle)
    {
        _goToNextStage.gameObject.SetActive(toggle);
    }
    #region Localization

    private void LoadJson()
    {
        _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.EndMessage.ToString());

        if (!_dataExists) return;

        _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.EndMessage.ToString()];
        LoadEndMessageJson();
    }

    private void LoadEndMessageJson()
    {
        if (!_dataExists) return;

        string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
        int stageIndex = ExperimentManager.Instance.stageIndex;

        if (stageIndex == 0) 
            stageIndex = 1;
       
        _endMessages = JsonConvert.DeserializeObject<Dictionary<int, StageEndMessageData>>(currentJson);
        
        _currentEndMessageData = _endMessages[stageIndex];

        UpdateText(_currentEndMessageData);
    }

    protected virtual void UpdateText(StageEndMessageData endMessageData)
    {
        _titleText.text = endMessageData.title;
        _descriptionText.text = endMessageData.description;
    }
    #endregion
}

[Serializable]
public class StageEndMessageData
{
    public string title;
    public string description;
    public string goToNextStageText;
    public string endExperimentText;
}