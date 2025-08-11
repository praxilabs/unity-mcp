using Praxilabs.UIs;
using ProgressMap.UI.ExpandedView.Maps.Entries;
using UnityEngine;

namespace ProgressMap.UI.ExpandedView.Maps
{
    public class StageMap : MonoBehaviour
    {
        #region fields
        [SerializeField] private Structure.Experiment _experiment;
        [SerializeField] private StageMapEntry _stageCirclePrefab;
        [SerializeField] private Transform _stagesParent;
        [SerializeField] private ProgressBarManager _progressBarManager;
        [SerializeField, Header("Stage objects to disable when there is only 1 stage")]
        private GameObject[] _stageObjects;
        private StageMapEntry[] _stagesInStageMap;
        [SerializeField] private ProgressMapController _progressMapController;
        #endregion

        #region properties
        public Structure.Experiment Experiment
        {
            get { return _experiment; }
            set { _experiment = value; Initialize(); }
        }
        #endregion

        #region methods
        [ContextMenu("Initialize")]
        public void Initialize()
        {
            // if there is only one stage, don't show stage map
            if (_experiment.Stages.Length == 1)
            {
                foreach (var gameObject in _stageObjects)
                    gameObject.SetActive(false);
                return;
            }
            else
            {
                foreach (var gameObject in _stageObjects)
                    gameObject.SetActive(true);
            }
            _stagesInStageMap = new StageMapEntry[_experiment.Stages.Length];

            _progressBarManager.InitializeProgressBar(_stagesParent.gameObject, _experiment.Stages.Length, (x) => _stagesInStageMap[x].SetAsSelected());
            for (int i = 0; i < _experiment.Stages.Length; i++)
            {
                _progressBarManager.InitializeProgressBarStates(i, StateTypes.Unfinished);
                _experiment.Stages[i].CalculateStageProgress();
                _stagesInStageMap[i] = GetNewStage();
                _stagesInStageMap[i].PreviousStageEntry = i == 0 ? null : _stagesInStageMap[i - 1];
                _stagesInStageMap[i].ProgressBarManager = _progressBarManager;
                _experiment.OnCurrentStageChange += _stagesInStageMap[i].UpdateUI;
                _stagesInStageMap[i].SetData(stage: _experiment.Stages[i],
                    currentStage: _experiment.CurrentStageIndex,
                    stageIndex: i);
                _stagesInStageMap[i].UpdateUI();
                _stagesInStageMap[i].ProgressMapController = _progressMapController;
            }
        }

        public void UpdateStages()
        {
            if (_experiment.Stages.Length == 1)
            {
                return;
            }
            for (int i = 0; i < _experiment.Stages.Length; i++)
            {
                _stagesInStageMap[i].SetData(_experiment.Stages[i], _experiment.CurrentStageIndex, i);
                _stagesInStageMap[i].UpdateUI();
            }
        }

        private StageMapEntry GetNewStage()
        {
            return new StageMapEntry();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _experiment.Stages.Length; i++)
            {
                _experiment.OnCurrentStageChange -= _stagesInStageMap[i].UpdateUI;
            }
        }
        #endregion
    }
}