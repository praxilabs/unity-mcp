using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNode;

public static class xNodeUtility
{
    public static List<StepNode> GetConnectedNodes(Node node, string _exit)
    {
        foreach (NodePort p in node.Ports)
        {
            if (p.fieldName == _exit)
            {
                List<StepNode> steps = new List<StepNode>();
                List<NodePort> portsConnectedToRandomizerExitPort = new List<NodePort>();
                portsConnectedToRandomizerExitPort = p.GetConnections();

                for (int i = 0; i < portsConnectedToRandomizerExitPort.Count; i++)
                {
                    steps.Add(portsConnectedToRandomizerExitPort[i].node as StepNode);
                }
                return steps;
            }
        }
        return null;
    }

    public static StepNode GetConnectedStepById(List<StepNode> steps, string _id)
    {
        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i].stepId == _id)
            {
                return steps[i];
            }
        }
        return null;
    }

    public static List<string> GetStepsIds(List<StepNode> nodes)
    {
        List<string> ids = new List<string>();
        for (int i = 0; i < nodes.Count; i++)
        {
            ids.Add(nodes[i].stepId);
        }
        return ids;
    }

    public static string BuildID()
    {
        StringBuilder builder = new StringBuilder();
        Enumerable
           .Range(65, 26)
            .Select(e => ((char)e).ToString())
            .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
            .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
            .OrderBy(e => Guid.NewGuid())
            .Take(8)
            .ToList().ForEach(e => builder.Append(e));

        return builder.ToString();
    }
}
