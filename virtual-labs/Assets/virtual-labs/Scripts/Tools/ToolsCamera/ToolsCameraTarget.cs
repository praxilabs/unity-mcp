using Cinemachine;
using UnityEngine;

public class ToolsCameraTarget : MonoBehaviour
{
    public CinemachineVirtualCamera cameraTarget;
    [HideInInspector] public string targetTag;

    private void Start()
    {
        targetTag = gameObject.tag;
    }
}