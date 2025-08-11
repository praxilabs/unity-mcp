using LocalizationSystem;
using Newtonsoft.Json;
using ProgressMap.Structure;
using ProgressMap.UI.ExpandedView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProgressMap.UI
{
    public class ProgressMapController : Singleton<ProgressMapController>
    {
        #region fields
        private Stage _selectedStage;
        [SerializeField] private int _selectedStageIndex;
        [SerializeField] private Experiment _experiment;
        [SerializeField] private ExpandedView.Maps.StageMap _stageMap;
        [SerializeField] private SelectedStageUI _currentStageUI;
        [SerializeField] private ExpandedView.Maps.SubstageMap _substageMap;
        [SerializeField] private JumpButton _jumpButton;
        [SerializeField] private MiniView _miniView;
        [SerializeField] private ExperimentTitle _experimentTitle;
        [SerializeField] private ProgressMapTransitions _progressMapTransitions;

        private Dictionary<string, string> _jsonDictionary = new Dictionary<string, string>();
        private Experiment _loadedExperiment;
        private bool _isProgressMapFirstTimeLoad = true;
        private Action _stageChangeAction;
        private Action _OnLanguageChangeDelegate;
        private bool _dataExists;
        #endregion

        #region properties
        public Structure.Experiment Experiment => _experiment;
        public int SelectedStageIndex
        {
            get => _selectedStageIndex;
            set
            {
                _selectedStageIndex = value;
                _selectedStage = _experiment.Stages[_selectedStageIndex];
                UpdateUI();
            }
        }
        #endregion

        #region methods
        protected override void Awake()
        {
            _addToDontDestroyOnLoad = false;
            base.Awake();
        }
        private void OnEnable()
        {
            if (_experiment.Stages.Length == 0)
                return;

            if (_stageChangeAction == null)
                _stageChangeAction = () => {
                    SetCurrentStage(ExperimentManager.Instance.stageIndex); 
                    _selectedStage.ResetStage();
                };

            _OnLanguageChangeDelegate = () =>
            {
                if (!_dataExists) return;
                LoadProgressMapJson();
            };

            OnCurrentStageChange(_experiment.CurrentStageIndex);

            _experiment.OnCurrentStageChange += OnCurrentStageChange;
            ExperimentManager.OnStageStart += _stageChangeAction;
            LocalizationManager.OnLocaleChanged += _OnLanguageChangeDelegate;
        }

        private void OnDisable()
        {
            _experiment.OnCurrentStageChange -= OnCurrentStageChange;
            ExperimentManager.OnStageStart -= _stageChangeAction;
            LocalizationManager.OnLocaleChanged -= _OnLanguageChangeDelegate;
        }

        private void Start()
        {
            LoadJson();
        }

        private void OnCurrentStageChange(int Index)
        {
            SelectedStageIndex = Index;
            _jumpButton.CurrentStage = _experiment.CurrentStage;
            _miniView.CurrentStage = _experiment.CurrentStage;
        }
 
        public void UpdateUI()
        {
            _substageMap.SelectedStage = _selectedStage;
            _currentStageUI.SelectedStage = _selectedStage;
            _jumpButton.SelectedStage = _selectedStage;
        }

        public void Initialize(string currentJson)
        {
            _experiment = ProgressMap.Structure.Experiment.ExperimentFromJson(currentJson, _experiment);
            Initialize();
        }

        public new void Initialize()
        {
            _experiment.Initialize();
            _experimentTitle.SubtitleText = _experiment.ExperimentSubtitle;
            _experimentTitle.TitleText = _experiment.ExperimentTitle;
            _experimentTitle.Initialize();
            OnCurrentStageChange(_experiment.CurrentStageIndex);
            _stageMap.Experiment = _experiment;
            _miniView.Experiment = _experiment;
            OnDisable();
            OnEnable();
        }
        public void NextStepOrStage()
        {
            if (_experiment.HasNextStep)
                _experiment.NextStep();
        }

        public void NextStep(int stepIndex)
        {
            _experiment.NextStep(stepIndex);
        }

        public void ResetStage(int stageIndex)
        {
            _experiment.ResetStage(stageIndex);
        }
        public void ResetCurrentStage()
        {
            _experiment.ResetCurrentStage();
        }
        public void SetCurrentStage(int stageIndex)
        {
            if (stageIndex >= 0 && stageIndex < _experiment.Stages.Length)
                _experiment.SetCurrentStage(stageIndex);
        }

        public void SelectedEqualCurrent()
        {
            ExperimentManager.Instance.GoToStage(SelectedStageIndex);
        }

        public void ToggleMiniUI(bool toggle)
        {
            if (toggle)
            {
                _progressMapTransitions.ToggleBGImage(toggle);
                _progressMapTransitions.ShowMini();
            }
            else
                StartCoroutine(HideUICoroutine());
        }

        public IEnumerator HideUICoroutine()
        {
            if (_progressMapTransitions.IsExpandedToFull())
            {
                _progressMapTransitions.HideExpandedFull();
                yield return new WaitForSeconds(_progressMapTransitions.GetFadeSpeed() * 1e-3f + 0.7f);
            }

            _progressMapTransitions.ToggleBGImage(false);
            _progressMapTransitions.HideMini(null);
        }
        #endregion

        #region Localization
        private void LoadJson()
        {
            _dataExists = LocalizationLoader.Instance.localizationTables.ContainsKey(LocalizationDataTypes.ProgressMap.ToString());

            if (!_dataExists) return;

            _jsonDictionary = LocalizationLoader.Instance.localizationTables[LocalizationDataTypes.ProgressMap.ToString()];
            LoadProgressMapJson();
        }

        private void LoadProgressMapJson()
        {
            if (!_dataExists) return;

            string currentJson = _jsonDictionary[LocalizationManager.Instance.CurrentLocale];

            if (_isProgressMapFirstTimeLoad)
            {
                Initialize(currentJson);
                _isProgressMapFirstTimeLoad = false;
            }
            else
                UpdateUILocalization(currentJson);
        }

        private void UpdateUILocalization(string currentJson)
        {
            _loadedExperiment = JsonConvert.DeserializeObject<Experiment>(currentJson);

            _experimentTitle.SubtitleText = _loadedExperiment.ExperimentSubtitle;
            _experimentTitle.TitleText = _loadedExperiment.ExperimentTitle;

            int counter = 0;
            foreach (var stage in _loadedExperiment.Stages)
            {
                _experiment.Stages[counter].UpdateText(stage.Description, stage.StageName, stage.Substages);
                counter++;
            }

            _miniView.UpdateUI();
            _experimentTitle.UpdateText();
            _currentStageUI.UpdateText();
            _substageMap.UpdateText();
        }
        #endregion
    }
}