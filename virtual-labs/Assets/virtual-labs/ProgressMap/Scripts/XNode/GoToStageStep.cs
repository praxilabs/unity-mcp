using ProgressMap.UI;
using UnityEngine;

namespace ProgressMap.Xnode
{
    [NodeTint("#3B1C32"), CreateNodeMenu("Go To Stage", 100)]
    public class GoToStageStep : SelfExectutedStep
    {
        [Input(ShowBackingValue.Never)] public NodeObject entry;
        public int stageNumber;
        [Output] public NodeObject exit;

        public override void PrepareStep()
        {
            base.PrepareStep();
            Execute();
        }

        public override void Execute()
        {
            base.Execute();

            ExperimentManager.Instance.GoToStage(stageNumber);
            XnodeStepsRunner.Instance.StepIsDone();
        }
    }
}