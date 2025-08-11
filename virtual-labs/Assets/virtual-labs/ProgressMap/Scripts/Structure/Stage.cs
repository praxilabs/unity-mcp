using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProgressMap.Structure
{
    [System.Serializable]
    public class Stage
    {
        #region fields

        public event System.Action<float> ProgressUpdateFloat;
        public event System.Action ProgressUpdate;
        public event System.Action StageComplete;

        private int _totalStepCount = -1;
        private float _totalProgress = -1;
        [SerializeField] private int _lastStepDone = -1;
        #endregion

        #region properties
        /// <summary>
        /// Sets stage number for writing
        /// </summary>
        [field: SerializeField][JsonProperty("StageNumber")] public int StageNumber { get; protected set; }
        [field: SerializeField, Multiline][JsonProperty("Description")] public string Description { get; protected set; }
        [JsonProperty("StageName")] public string StageName {  get; protected set; }
        [field: SerializeField] [JsonProperty("Progress")]public float Progress { get; protected set; }
        /// <summary>
        /// Total progress as if the stage is 100% complete
        /// <br/>
        /// Accumilation of (step x weight) for all steps in stage.
        /// </summary>
        [JsonIgnore]public float TotalProgress
        {
            get
            {
                // calculates total progress if first time, and caches value
                if (_totalProgress == -1)
                {
                    _totalProgress = 0;
                    for (int i = 0; i < Steps.Length; i++)
                    {
                        _totalProgress += Steps[i].Weight;
                    }
                }
                return _totalProgress;
            }
        }
        /// <summary>
        /// returns false if last step in stage is reached
        /// </summary>
        [JsonIgnore] public bool HasNextStep => _lastStepDone < (Steps.Length - 1);
        /// <summary>
        /// can you change current stage from another stage to this stage by jump button.
        /// </summary>
        [field: SerializeField][JsonProperty("CanJumpTo")] public bool CanJumpTo { get; protected set; }
        /// <summary>
        /// Can you change current stage from this stage to another stage by jump button.
        /// </summary>
        [field: SerializeField][JsonProperty("CanJumpFrom")] public bool CanJumpFrom { get; protected set; }

        [field: SerializeField][JsonProperty("Steps")] public Step[] Steps { get; protected set; }
        [field: SerializeField][JsonProperty("SubStages")] public Substage[] Substages { get; protected set; }
        /// <summary>
        /// returns number of last step done, begining from 0 = no steps done, to steps count = all steps done
        /// </summary>
        [JsonIgnore] public int LastStepDone => _lastStepDone + 1;
        [JsonIgnore] public int TotalStepCount

        {
            get
            {
                if (_totalStepCount == -1)
                {
                    _totalStepCount = Steps.Length;
                }
                return _totalStepCount;
            }
        }

        #endregion

        #region methods
        public void UpdateText(string description, string stageName, Substage[] subStages)
        {
            Description = description;
            StageName = stageName;
            Substages = subStages;
        }

        public void CalculateStageProgress()
        {
            float stageProgressDone = 0;
            if (_lastStepDone >= 0 || _lastStepDone < Steps.Length)
            {
                for (int i = 0; i <= _lastStepDone && i < Steps.Length; i++)
                {
                    stageProgressDone += Steps[i].Weight;
                }
            }
            Progress = stageProgressDone;
            ProgressUpdateFloat?.Invoke(Progress);
            ProgressUpdate?.Invoke();

            if (stageProgressDone == 1)
                StageComplete?.Invoke();
        }

        [ContextMenu("NextStep")]
        public void Next()
        {
            if (!HasNextStep)
                return;
            _lastStepDone += 1;
            CalculateStageProgress();
        }

        public void Next(int stepIndex)
        {
            _lastStepDone = stepIndex;
            CalculateStageProgress();
        }

        [ContextMenu("ResetStage")]
        public void ResetStage()
        {
            _lastStepDone = -1;

            CalculateStageProgress();
        }
        [ContextMenu("CompleteStage")]
        public void CompleteStage()
        {
            _lastStepDone = Steps.Length - 1;
            CalculateStageProgress();
        }
        #endregion
    }
}
