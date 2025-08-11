namespace Praxilabs.xNode
{
    [NodeTint("#102C57"), CreateNodeMenu("Subgraph Node", 2)]
    public class SubGraphStep : SelfExectutedStep
    {
        [Input(ShowBackingValue.Never)] public NodeObject entry;

        public StepsGraph subGraph;
        private StepsGraph _parentGraph;

        [Output] public NodeObject exit;

        public override void PrepareStep()
        {
            base.PrepareStep();
            ResolveObjects();

            _parentGraph = XnodeManager.Instance.runningGraph;
            Execute();
        }

        public override void Execute()
        {
            XnodeManager.Instance.subGraphSteps.Add(this);
            XnodeManager.Instance.StartGraph(subGraph);
        }

        public override void Exit()
        {
            XnodeManager.Instance.subGraphSteps.Remove(this);

            XnodeManager.Instance.runningGraph = _parentGraph;
            XnodeManager.Instance.CurrentStep = this;

            XnodeStepsRunner.Instance.StepIsDone();
        }
    }
}