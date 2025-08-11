using LocalizationSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplorationHintsContainer : MonoBehaviour
{
    public class ExplorationGroup
    {
        public int ExploredHints { get; set; }
        public int TotalHints { get; set; }
        public List<ExploreHintItem> unexploredHints = new();
        public List<ExploreHintItem> allHints = new();
        public GameObject groupGO;
    }

    [SerializeField] private GameObject _hintsContainerCanvas;
    private Dictionary<int, ExplorationGroup> _explorationGroupDict = new();
    private List<ExploreHintItem> _allHints = new();

    private int _currentGroupIndex;

    //Localization Variables
    private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
    private Dictionary<int, ExplorationData> _explorationData = new();
    private LocalizedString _hintsCountLocalization = new();
    private Action _OnLanguageChangeDelegate;
    private bool _isInitialized = true;
    private bool _dataExists;

    private void OnEnable()
    {
        _OnLanguageChangeDelegate = () =>
        {
            if (!_dataExists) return;
            LoadExplorationJson();
        };
        LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
    }

    private void Start()
    {
        LoadJson();
        ExplorationHandler.Instance.SetExplorationContainer(this);
    }

    public void Initialization()
    {
        if (_hintsContainerCanvas == null)
        {
            Debug.LogError("Exploration Hints Container Canvas Is Null!");
            return;
        }

        List<Transform> childernGO = new();
        for (int i = 0; i < _hintsContainerCanvas.transform.childCount; i++)
        {
            Transform child = _hintsContainerCanvas.transform.GetChild(i);
            childernGO.Add(child);
        }

        List<ExploreHintItem> allHints = new();

        for (int i = 0; i < childernGO.Count; i++)
        {
            int index = i;
            allHints.Clear();
            allHints = childernGO[index].GetComponentsInChildren<ExploreHintItem>(true).ToList();
            _allHints = allHints;
            if (allHints.Count == 0) // Ignore game objects that has no children
                continue;

            childernGO[index].name += $"_{index}";
            ExplorationGroup explorationGroup = new();
            explorationGroup.allHints = new List<ExploreHintItem>(allHints);

            _explorationGroupDict.Add(index, explorationGroup);

            int counter = 1;

            foreach (ExploreHintItem hint in explorationGroup.allHints)
            {
                explorationGroup.unexploredHints.Add(hint);
                hint.HintBtn.onClick.AddListener(() => HandleHintButtonClicked(hint, index));
                hint.Setup(this, _explorationData[counter].hintText);
                counter++;
            }

            explorationGroup.TotalHints = explorationGroup.unexploredHints.Count;
            explorationGroup.groupGO = childernGO[index].gameObject;
            explorationGroup.groupGO.SetActive(false);
        }
    }

    private void HandleHintButtonClicked(ExploreHintItem hint, int groupIndex)
    {
        ToggleHintBtns(false);
        if (!GetGroup(groupIndex).unexploredHints.Contains(hint)) return;
        GetGroup(groupIndex).ExploredHints++;
        GetGroup(groupIndex).unexploredHints.Remove(hint);
    }

    public void SetGroupIndex(int groupIndex)
    {
        _currentGroupIndex = groupIndex;
    }

    public bool IsValidGroupIndex(int groupIndex)
    {
        return _explorationGroupDict.ContainsKey(groupIndex);
    }

    public int GetExploredHintsCount()
    {
        return GetGroup(_currentGroupIndex).ExploredHints;
    }

    public void ToggleExplorationContainer(bool enable)
    {
        _hintsContainerCanvas.SetActive(enable);
        ToggleExplorationGroup(_currentGroupIndex, enable);
        if (enable == false)
        {
            ResetGroupFlashing();
        }
    }

    public void CheckIfCompletedGroupHints()
    {
        if (GetGroup(_currentGroupIndex).TotalHints == GetGroup(_currentGroupIndex).ExploredHints)
        {
            ExplorationHandler.Instance.CompletedExploringHints();
        }
    }

    public void ToggleHintBtns(bool enable)
    {
        foreach (var hint in GetGroup(_currentGroupIndex).allHints)
        {
            hint.ToggleHintBtn(enable);
        }
    }

    public void ResetGroupFlashing()
    {
        foreach (var hint in GetGroup(_currentGroupIndex).allHints)
        {
            hint.ResetFlashing();
        }
    }

    public string ProvideExploredOfTotalHints()
    {
        if (_hintsCountLocalization.table == null || _hintsCountLocalization.key == null)
        {
            _hintsCountLocalization.table = "Exploration_StaticData";
            _hintsCountLocalization.key = "ExplorationHintsCount";
        }
        _hintsCountLocalization.Refresh();
        return $"{GetGroup(_currentGroupIndex).ExploredHints} {_hintsCountLocalization.Value} {GetGroup(_currentGroupIndex).TotalHints}";
    }

    public List<Hologram> GetHintsFlashableObjectHologram(int groupIndex)
    {
        if (!IsValidGroupIndex(groupIndex)) return null;

        List<Hologram> holograms = new();

        foreach (var hint in GetGroup(groupIndex).allHints)
        {
            if (hint.FlashableObject == null) continue;
            var hologram = hint.FlashableObject.GetComponentInParent<Hologram>();
            if (hologram == null) continue;
            holograms.Add(hologram);
        }

        return holograms;
    }

    private void ToggleExplorationGroup(int groupIndex, bool enable)
    {
        GetGroup(groupIndex).groupGO.SetActive(enable);
    }

    private ExplorationGroup GetGroup(int groupIndex)
    {
        return _explorationGroupDict[groupIndex];
    }

    #region Localization
    private void LoadJson()
    {
        _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.Exploration.ToString());

        if (!_dataExists) return;

        _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.Exploration.ToString()];
        LoadExplorationJson();
    }

    public void LoadExplorationJson()
    {
        if (!_dataExists) return;

        string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
        _explorationData = JsonConvert.DeserializeObject<Dictionary<int, ExplorationData>>(currentJson);

        if (_isInitialized)
        {
            Initialization();
            _isInitialized = false;
        }
        else
            RefreshUI();
    }

    public void RefreshUI()
    {
        for (int j = 0; j < _allHints.Count; j++)
        {
            ExploreHintItem hint = _allHints[j];
            hint.SetHintText(_explorationData[j + 1].hintText);
        }
    }
    #endregion
}

public class ExplorationData
{
    public string hintText;
}