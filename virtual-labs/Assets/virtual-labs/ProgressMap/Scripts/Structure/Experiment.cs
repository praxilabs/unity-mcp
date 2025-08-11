using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
namespace ProgressMap.Structure
{

    public delegate void StateChange(int StageIndex);
    public delegate void StateChangeEmpty();
    [System.Serializable]
    public class Experiment
    {
        #region fields
        public event Action<float> ProgressUpdateFloat;
        public event Action ProgressUpdate;

        public event StateChange OnCurrentStageChange;
        public event StateChangeEmpty OnCurrentStageChangeEmpty;
        #endregion
        #region properties
        [field: SerializeField, Tooltip("First words before \'...\' should appear")][JsonProperty("ExperimentTitle")] public string ExperimentTitle { get; set; } = "New";
        [field: SerializeField, TextArea(5, 1203)][JsonProperty("ExperimentSubtitle")] public string ExperimentSubtitle { get; protected set; } = "New Experiment";
        [field: SerializeField][JsonProperty("Stages")] public Stage[] Stages { get; set; }
        [field: SerializeField][JsonProperty("CurrentStageIndex")] public int CurrentStageIndex { get; protected set; } = 0;
        [field: SerializeField][JsonIgnore] public float OverallProgress { get; protected set; } = 0;
        [JsonIgnore] public Stage CurrentStage => Stages[CurrentStageIndex];
        /// <summary>
        /// Total sum of progress for all stages if 100% complete.
        /// </summary>
        [JsonIgnore] public float TotalProgress { get; protected set; }
        [JsonIgnore] public bool HasNextStep => Stages[CurrentStageIndex].HasNextStep;
        /// <summary>
        /// Returns true if current stage isn't last stage, false if current stage is last stage
        /// </summary>
        [JsonIgnore] public bool HasNextStage => CurrentStageIndex < (Stages.Length - 1);
        #endregion

        #region methods

        public Experiment()
        {
            Initialize();
        }
        public static Experiment ExperimentFromJson(string json, Experiment oldExperiment)
        {
            Experiment experiment = ExperimentProgressJsonConverter.Instance.FromJson(json);
            experiment.OnCurrentStageChange = oldExperiment.OnCurrentStageChange;
            experiment.OnCurrentStageChangeEmpty = oldExperiment.OnCurrentStageChangeEmpty;
            return experiment;
        }
        /// <summary>
        /// Initializes experiment and stages progresses
        /// </summary>
        public void Initialize()
        {
            if (Stages == null)
                return;
            TotalProgress = 0;
            for (int i = 0; i < Stages.Length; i++)
            {
                TotalProgress += Stages[i].TotalProgress;
                Stages[i].CalculateStageProgress();
            }
            for (int i = 0; i < Stages.Length; i++)
            {
                Stages[i].ProgressUpdate += CalculateOverallProgress;
            }
            CalculateOverallProgress();
        }

        ~Experiment()
        {
            for(int i = 0; i < Stages.Length; i++)
            {
                Stages[i].ProgressUpdate -= CalculateOverallProgress;
            }
        }

        /// <summary>
        /// Calculates and updates overall progress
        /// </summary>
        public void CalculateOverallProgress()
        {
            float totalProgressDone = 0 ;

            for (int i = 0; i < Stages.Length; i++)
            {
                totalProgressDone += Stages[i].Progress;

            }
            OverallProgress = totalProgressDone;
            ProgressUpdate?.Invoke();
            ProgressUpdateFloat?.Invoke(OverallProgress);
        }

        /// <summary>
        /// Set progress to zero for stage of index <paramref name="stageIndex"/>.
        /// </summary>
        /// <param name="stageIndex">stage index</param>
        public void ResetStage(int stageIndex)
        {
            if(stageIndex>=Stages.Length)
            {
                Debug.LogError($"Cannot find stage <{stageIndex}> of stages <{Stages.Length}> to reset!" +
                    $"\n" +
                    $" Index out of range!");
                return;
            }
            Stages[stageIndex].ResetStage();
        }
        /// <summary>
        /// resets stage of index <see cref="CurrentStageIndex"/>
        /// </summary>
        public void ResetCurrentStage()
        {
            Stages[CurrentStageIndex].ResetStage();
        }

        public void NextStep()
        {
            Stages[CurrentStageIndex].Next();
        }

        public void NextStep(int stepIndex)
        {
            Stages[CurrentStageIndex].Next(stepIndex);
        }

        public void NextStage()
        {
            SetCurrentStage(CurrentStageIndex + 1);
        }
        /// <summary>
        /// Set current stage to stage <paramref name="stageIndex"/>.
        /// </summary>
        /// <param name="stageIndex">stage index</param>
        public void SetCurrentStage(int stageIndex)
        {
            if (stageIndex > Stages.Length || stageIndex < 0 || this.CurrentStageIndex == stageIndex)
                return;
            CurrentStageIndex = stageIndex;
            OnCurrentStageChange?.Invoke(CurrentStageIndex);
            OnCurrentStageChangeEmpty?.Invoke();
        }

        #endregion
    }
}
