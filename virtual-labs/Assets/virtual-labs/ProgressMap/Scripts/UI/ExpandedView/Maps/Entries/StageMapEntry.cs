using ProgressMap.Structure;
using UnityEngine;

namespace ProgressMap.UI.ExpandedView.Maps.Entries
{
    public class StageMapEntry
    {
        #region fields
        [field: SerializeField] public Praxilabs.UIs.ProgressBarManager ProgressBarManager { get; set; }
        [field: SerializeField] public StageMapEntry PreviousStageEntry { get; set; }
        [SerializeField] private Stage _stage;
        private int _stageIndex;
        #endregion

        #region properties
        [field: SerializeField] public ProgressMapController ProgressMapController { get; set; }

        private int _currentStageIndex;
        /// <summary>
        /// return true is current stage index equals stage index
        /// </summary>
        public bool IsCurrentStage
        {
            get => _currentStageIndex == _stageIndex;

        }
        public bool IsStageIncomplete => _stage.Progress > 0 && _stage.Progress < _stage.TotalProgress;
        public bool IsStageComplete => _stage.Progress == _stage.TotalProgress;
        public bool IsStageEmpty => _stage.Progress == 0;
        #endregion

        #region methods
        [ContextMenu("UpdateUI")]
        public void UpdateUI() => UpdateUI(_currentStageIndex);
        /// <summary>
        /// Update UI and set current Stage index to <paramref name="currentStageIndex"/>
        /// </summary>
        /// <param name="currentStageIndex">Stage index to set current</param>
        public void UpdateUI(int currentStageIndex)
        {
            this._currentStageIndex = currentStageIndex;
            ProgressBarManager.UpdateProgressBarCircleState(
                _stageIndex,
                IsCurrentStage ? StateTypes.Current :
                IsStageEmpty || IsStageIncomplete ? StateTypes.Unfinished :
                StateTypes.Finished,

                PreviousStageEntry == null ? StateTypes.Unfinished :
                PreviousStageEntry.IsCurrentStage ? StateTypes.Current :
                PreviousStageEntry.IsStageEmpty || PreviousStageEntry.IsStageIncomplete ? StateTypes.Unfinished :
                StateTypes.Finished);

        }
        public void SetData(Stage stage, int currentStage, int stageIndex)
        {
            if (stage != null)
            {
                stage.ProgressUpdate -= UpdateUI;
            }
            this._stage = stage;
            stage.ProgressUpdate += UpdateUI;
            this._stageIndex = stageIndex;
            _currentStageIndex = currentStage;
        }
        public void SetAsSelected()
        {
            ProgressMapController.SelectedStageIndex = _stage.StageNumber - 1;
        }
        #endregion
    }
}