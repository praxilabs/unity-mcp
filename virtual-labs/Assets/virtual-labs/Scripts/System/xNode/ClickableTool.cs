using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableTool : MonoBehaviour
{
    public bool canClick = false;

    public void HandleClickStepExecution(bool isPointerBlocked = true)
    {
        if (canClick)
            ExecuteClickStep(isPointerBlocked);
    }

    private void OnMouseDown()
    {
        HandleClickStepExecution();
    }

    private void ExecuteClickStep(bool isPointerRestricted = true)
    {
        if (XnodeManager.Instance.CurrentStep is IProvideActionSteps)
            BranchingExecute(isPointerRestricted);
        else if (XnodeManager.Instance.CurrentStep is ClickStep)
            SoloExecute(isPointerRestricted);
    }

    private void BranchingExecute(bool isPointerRestricted = true)
    {
        if(isPointerRestricted && EventSystem.current.IsPointerOverGameObject())
            return;

        if (!CheckAvailableSteps(this.name))
            return;

        XnodeManager.Instance.CurrentStep.Execute(this.gameObject);
    }

    private void SoloExecute(bool isPointerRestricted = true)
    {
        if(isPointerRestricted && EventSystem.current.IsPointerOverGameObject())
            return;

        ClickStep clickStep = (ClickStep)XnodeManager.Instance.CurrentStep;
        if (this.name != clickStep.targetName) 
            return;

        XnodeManager.Instance.CurrentStep.Execute(this.gameObject);
    }

    private bool CheckAvailableSteps(string objectToClick)
    {
        for (int i = 0; i < XnodeStepsRunner.Instance.availableSteps.Count; i++)
        {
            if (XnodeStepsRunner.Instance.availableSteps[i] is ClickStep)
            {
                ClickStep clickStep = (ClickStep)XnodeStepsRunner.Instance.availableSteps[i];
                if (objectToClick == clickStep.targetName)
                {
                    XnodeManager.Instance.CurrentStep = clickStep;
                    return true;
                }
            }
        }
        return false;
    }
}