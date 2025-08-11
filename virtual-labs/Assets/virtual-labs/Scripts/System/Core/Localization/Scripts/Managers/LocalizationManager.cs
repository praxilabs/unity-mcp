using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LocalizationSystem
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        public string defaultLocale = "en";
        public static event Action OnLocaleChanged;
        public ILocalizationFileLoad diskFilesLoader = new LocalizationFileLoaderDisk();

        private string _currentLocale;
        public string CurrentLocale => _currentLocale;
        public List<string> AvailableTables => _metadata.tables;
        public List<string> AvailableLocales => _metadata.locales;

        public Dictionary<string, Dictionary<string, string>> _localizationTables = new Dictionary<string, Dictionary<string, string>>();

        private LocalizationMetadata _metadata;
        private UnityAction _labLoadedDelegate;

        protected override void Awake()
        {
            _addToDontDestroyOnLoad = false;

            base.Awake();

            _currentLocale = defaultLocale;
        }

        private void OnEnable()
        {
            _labLoadedDelegate = () => LoadLocalization();

            EventsManager.Instance.AddListener(AssetBundlesEvents.webRequestCompleted, _labLoadedDelegate);
        }

        private void OnDisable()
        {
            if (EventsManager.Instance == null) return;
            EventsManager.Instance.RemoveListener(AssetBundlesEvents.webRequestCompleted, _labLoadedDelegate);
        }

        public void LoadLocalization()
        {
            ExperimentData experimentData = AssetBundlesManager.Instance.CurrentExperimentData;

            if (experimentData == null)
            {
                Debug.Log("<Color=#77B254><size=15><b>ExperimentData is null, please assign <Color=#FE4F2D>ExperimentData</Color> to <Color=#FE4F2D>AssetBundleManager</Color>~!!!</b></size></Color>");
                return;
            }

            StartCoroutine(Initialize(diskFilesLoader));
        }

        /// <summary>
        /// Used for testing language change
        /// </summary>
        //private void Update()
        //{
        //    if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        //        StartCoroutine(SetLocale("en"));
        //    if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        //        StartCoroutine(SetLocale("ar"));
        //}

        public IEnumerator Initialize(ILocalizationFileLoad loader)
        {
            diskFilesLoader = loader;
            yield return LoadMetadataAndDefaultLocale();

            LocalizationLoader.Instance.LoadLocalization();
        }

        private IEnumerator LoadMetadataAndDefaultLocale()
        {
            yield return LoadMetadata();
            yield return SetLocale(defaultLocale);
        }

        private IEnumerator LoadMetadata()
        {
            yield return diskFilesLoader.LoadFile("LocalizationMetadata", json =>
            {
                _metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalizationMetadata>(json);
            });
        }

        public IEnumerator SetLocale(string locale)
        {
            _localizationTables.Clear();

            yield return LoadAllTablesForLocale(locale);

            _currentLocale = locale;
            OnLocaleChanged?.Invoke();
        }

        private IEnumerator LoadAllTablesForLocale(string locale)
        {
            foreach (string tableName in _metadata.tables)
            {
                yield return LoadTable(locale, tableName);
            }
        }

        private IEnumerator LoadTable(string locale, string tableName)
        {
            string fileName = $"{tableName}_{locale}";
            string filePath=$"{tableName.Replace("_StaticData", "")}/{fileName}";

            yield return diskFilesLoader.LoadFile(filePath, json =>
            {
                if (!string.IsNullOrEmpty(json) && json[0] == '\uFEFF')
                {
                    json = json.Substring(1);
                }
                _localizationTables[tableName] = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            });
        }

        public string GetLocalizedString(string table, string key)
        {
            bool tableNull = string.IsNullOrEmpty(table);
            bool keyNull = string.IsNullOrEmpty(key);

            if (keyNull || tableNull)
            {
                return $"{(tableNull ? "table is null or empty" : "")} {(keyNull ? "key is null or empty" : "")}";
            }
            if (_localizationTables.TryGetValue(table, out var dict) && dict.TryGetValue(key, out var value))
            {
                return value;
            }
            return $"[MISSING:{key}]";
        }
    }

    public enum LocalizationDataTypes
    {
        ProgressMap,
        Hints,
        DeviceMenu,
        MCQ,
        SafetyTools,
        SettingsMenu,
        IntroEnd,
        SideMessages,
        MiddleMessages,
        Timer,
        Exploration,
        Tables,
        EndMessage,
        UILabels,
        DeviceLabels,
        DeviceTooltip
    }
}