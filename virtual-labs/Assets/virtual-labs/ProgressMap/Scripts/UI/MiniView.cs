using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProgressMap.UI
{
    public class MiniView : MonoBehaviour
    {
        #region fields
        [SerializeField] private ProgressVisualizerBase[] _overallVisualizers;
        [SerializeField] private ProgressVisualizerBase[] _currentStageVisualizers;
        private ProgressMap.Structure.Experiment _experiment;
        private ProgressMap.Structure.Stage _currentStage;
        #endregion

        #region properties
        public ProgressMap.Structure.Experiment Experiment
        {
            get => _experiment;
            set
            {
                if (_experiment != null)
                    foreach (var vizualizer in _overallVisualizers)
                    {
                        _experiment.ProgressUpdateFloat -= vizualizer.SetProgress;
                    }
                _experiment = value;
                foreach (var vizualizer in _overallVisualizers)
                {
                    vizualizer.TotalProgress = _experiment.TotalProgress;
                    vizualizer.Progress = _experiment.OverallProgress;
                    _experiment.ProgressUpdateFloat += vizualizer.SetProgress;
                }
            }
        }
        public ProgressMap.Structure.Stage CurrentStage
        {
            get => _currentStage;
            set
            {
                if (_currentStage != null)
                    foreach (var vizualizer in _currentStageVisualizers)
                    {

                        _currentStage.ProgressUpdateFloat -= vizualizer.SetProgress;
                    }
                _currentStage = value;
                foreach (var vizualizer in _currentStageVisualizers)
                {

                    vizualizer.TotalProgress = _currentStage.TotalProgress;
                    vizualizer.Progress = _currentStage.Progress;
                    _currentStage.ProgressUpdateFloat += vizualizer.SetProgress;
                }
            }
        }

        public void UpdateUI()
        {
            foreach(var vizualizer in _overallVisualizers)
            {
                vizualizer.UpdateVisualization();
            }
            foreach (var vizualizer in _currentStageVisualizers)
            {
                vizualizer.UpdateVisualization();
            }
        }
        #endregion
    }
}