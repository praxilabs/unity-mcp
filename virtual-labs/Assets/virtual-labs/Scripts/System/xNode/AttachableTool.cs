using Praxilabs.Input;
using UnityEngine;

public class AttachableTool : MonoBehaviour
{
    public bool canAttach = false;

    private void OnTriggerEnter(Collider other)
    {
        if (canAttach)
            ExecuteAttachStep(other);
    }

    private void ExecuteAttachStep(Collider other)
    {
        if (XnodeManager.Instance.CurrentStep is IProvideActionSteps && DragDropTarget.draggableTarget != null)
            BranchingExecute(other);
        else if (DragDropTarget.draggableTarget != null && XnodeManager.Instance.CurrentStep is AttachStep)
            SoloExecute(other);
    }

    private void BranchingExecute(Collider other)
    {
        if (CheckAvailableSteps(DragDropTarget.draggableTarget.name, other.name))
        {
            AttachStep attachStep = XnodeManager.Instance.CurrentStep as AttachStep;
            attachStep.followerObject.GetComponent<DraggableObject>().resetPositionOnDrop = false;

            attachStep.Execute();
        }
    }

    private void SoloExecute(Collider other)
    {
        AttachStep attachStep = XnodeManager.Instance.CurrentStep as AttachStep;

        if (XnodeManager.Instance.CurrentStep is AttachStep &&
            DragDropTarget.draggableTarget == attachStep.followerObject &&
            other.gameObject == attachStep.receiverObject)
        {
            attachStep.followerObject.GetComponent<DraggableObject>().resetPositionOnDrop = false;

            attachStep.Execute();
        }
    }

    private bool CheckAvailableSteps(string follower, string receiver)
    {
        for (int i = 0; i < XnodeStepsRunner.Instance.availableSteps.Count; i++)
        {
            if (XnodeStepsRunner.Instance.availableSteps[i] is AttachStep)
            {
                AttachStep attach = (AttachStep)XnodeStepsRunner.Instance.availableSteps[i];
                if (follower == attach.followerObject.name && receiver == attach.receiverObject.name)
                {
                    XnodeManager.Instance.CurrentStep = attach;
                    return true;
                }
            }
        }
        return false;
    }
}