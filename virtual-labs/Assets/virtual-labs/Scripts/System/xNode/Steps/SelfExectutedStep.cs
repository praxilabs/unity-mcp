using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateNodeMenu("")]
public class SelfExectutedStep : StepNode
{
    protected Coroutine co;
    public override void AutomateStep()
    {
        //you will have to write automation behavior for any node that extend this 
    }

    
}
