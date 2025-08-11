using ProgressMap.UI;
using UnityEngine;

namespace ProgressMap.Xnode
{
    [NodeTint("#3B1C32"), CreateNodeMenu("Progress map/Progress Map Auto Next step")]
    public class ProgressMapAutoNextStep : SelfExectutedStep
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
            ProgressMapController.Instance.NextStepOrStage();
            XnodeStepsRunner.Instance.StepIsDone();
        }
    }
}