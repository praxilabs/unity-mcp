using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressMap.UI;

namespace ProgressMap.Xnode
{
    [NodeTint("#3B1C32"), CreateNodeMenu("Progress map/Reset stage by number")]
    public class ResetStage : SelfExectutedStep
    {
        [Input(ShowBackingValue.Never)] public NodeObject entry;
        [Input] public int stageNumber;
        [Output] public NodeObject exit;
        public override void PrepareStep()
        {
            base.PrepareStep();
            Execute();
        }
        public override void Execute()
        {
            base.Execute();
            ProgressMapController.Instance.ResetStage(stageNumber);
            XnodeStepsRunner.Instance.StepIsDone();
        }
    }
}