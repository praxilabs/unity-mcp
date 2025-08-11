using LocalizationSystem;
using System;
using System.Collections;
using System.Collections.Generic;

public class LocalizationLoader : Singleton<LocalizationLoader>
{
    public static event Action OnLocalizationLoad;
    public Dictionary<string, Dictionary<string, string>> localizationTables = new Dictionary<string, Dictionary<string, string>>();

    private ILocalizationFileLoad _remoteFilesLoader = new LocalizationFileLoaderRemote();
    private LocalizationMetadata _metadata;
    private string _experimentName;

    public void LoadLocalization()
    {
        ExperimentData experimentData = AssetBundlesManager.Instance.CurrentExperimentData;
        _experimentName = experimentData.experimentName;

        if (string.IsNullOrEmpty(_experimentName))
        {
            OnLocalizationLoad.Invoke();
            return;
        }

        StartCoroutine(LoadLocalizationCoroutine());
    }

    private IEnumerator LoadLocalizationCoroutine()
    {
        yield return LoadMetadata();
        yield return LoadAllTables();
    }

    private IEnumerator LoadMetadata()
    {
        string filePath = $"{_experimentName}/LocalizationFiles/LocalizationMetadata.json";

        yield return _remoteFilesLoader.LoadFile(filePath, json =>
        {
            _metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalizationMetadata>(json);
        });
    }

    private IEnumerator LoadAllTables()
    {
        int totalTasks = _metadata.tables.Count * _metadata.locales.Count;
        int completedTasks = 0;

        foreach (string jsonName in _metadata.tables)
        {
            Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
            localizationTables[jsonName] = jsonDictionary; // initialize now

            foreach (var locale in _metadata.locales)
            {
                string filePath = $"{_experimentName}/LocalizationFiles/{jsonName}/{jsonName}_{locale}.json";

                StartCoroutine(_remoteFilesLoader.LoadFile(filePath, (jsonFile) =>
                {
                    jsonDictionary[locale] = jsonFile;

                    completedTasks++;
                    if (completedTasks == totalTasks)
                    {
                        OnLocalizationLoad?.Invoke();
                    }
                }));
            }
        }

        yield return null;
    }

    public IEnumerator LoadDiskJson(Dictionary<string, string> jsonDictionary, LocalizationDataTypes localizationDataType)
    {
        foreach (string language in LocalizationManager.Instance.AvailableLocales)
        {
            string filePath = $"{localizationDataType}/{localizationDataType}_StaticData_{language}";

            yield return LocalizationManager.Instance.diskFilesLoader.LoadFile(filePath,
           (jsonFile) =>
           {
               jsonDictionary[language] = jsonFile;
           });
        }
    }
}