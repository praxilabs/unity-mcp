using ProgressMap.UI;
using UnityEngine;

namespace ProgressMap.Xnode
{
    [NodeTint("#3B1C32"), CreateNodeMenu("Progress map/Progress Map Manual Next step")]
    public class ProgressMapManualNextStep : SelfExectutedStep
    {
        [Input(ShowBackingValue.Never)] public NodeObject entry;

        [SerializeField] private int _nextStepIndex;

        [Output] public NodeObject exit;

        public override void PrepareStep()
        {
            base.PrepareStep();
            Execute();
        }
        public override void Execute()
        {
            base.Execute();
            ProgressMapController.Instance.NextStep(_nextStepIndex);
            XnodeStepsRunner.Instance.StepIsDone();
        }
    }
}