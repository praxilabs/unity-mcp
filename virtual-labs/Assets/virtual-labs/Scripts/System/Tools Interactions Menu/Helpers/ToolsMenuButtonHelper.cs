using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsMenuButtonHelper : MonoBehaviour
{
    private ToolsInteractionsMenu _manager;

    private void Start()
    {
        _manager = GetComponentInParent<ToolsInteractionsMenu>(true);
    }
    public void EndStep()
    {
        _manager.ResetMenu();
        XnodeStepsRunner.Instance.StepIsDone();
    }
}
