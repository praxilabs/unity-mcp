using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressMap.UI;
namespace ProgressMap.Xnode
{
    [NodeTint("#3B1C32"), CreateNodeMenu("Progress map/Reset current stage")]
    public class ResetCurrentStage : SelfExectutedStep
    {
        [Input(ShowBackingValue.Never)] public NodeObject entry;
        [Output] public NodeObject exit;

        public override void PrepareStep()
        {
            base.PrepareStep();
            Execute();
        }

        public override void Execute()
        {
            base.Execute();
            ProgressMapController.Instance.ResetCurrentStage();
            XnodeStepsRunner.Instance.StepIsDone();
        }
    }
}