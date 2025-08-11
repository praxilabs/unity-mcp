using LocalizationSystem;
using Newtonsoft.Json;
using Praxilabs.UIs;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class HintsJsonHelper : MonoBehaviour
{
    [SerializeField] private HintsManager _hintsManager;
    [SerializeField] private int _stepNumberToOpenWith;

    //Localization Variables
    private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
    private Dictionary<int, HintsList> _hintsData = new Dictionary<int, HintsList>();
    private Action _OnLanguageChangeDelegate;
    private bool _dataExists;

    private void OnEnable()
    {
        _OnLanguageChangeDelegate = () =>
        {
            if (!_dataExists) return;
            LoadHintsJson();
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
    }

    private void LoadJson()
    {
        _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.Hints.ToString());

        if (!_dataExists) return;

        _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.Hints.ToString()];
        LoadHintsJson();
    }

    public void LoadHintsJson()
    {
        if (!_dataExists) return;

        string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];
        string jsonString = RemoveTags(currentJson);

        _hintsData = JsonConvert.DeserializeObject<Dictionary<int, HintsList>>(jsonString);

        HintsJsonData hintsJson = new();
        hintsJson.MapDictionaryToList(_hintsData);

        _hintsManager.hintsData = hintsJson;
        _hintsManager.UpdateUIText();
    }

    public void OpenHintsWithStep(int stepNumber)
    {
        _hintsManager.gameObject.SetActive(true);
        _hintsManager.OpenHintsMenuSilent(stepNumber);
    }

    public string RemoveTags(string input)
    {
        string pattern = @"<[^>]*>";
        return Regex.Replace(input, pattern, string.Empty);
    }
}