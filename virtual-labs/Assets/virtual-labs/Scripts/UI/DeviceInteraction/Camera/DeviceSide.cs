using Cinemachine;
using DimmEffect;
using UltimateClean;
using UnityEngine;
using UnityEngine.Events;

public class DeviceSide : MonoBehaviour
{
    public DeviceUI DeviceUI {get; private set;} 
    public DeviceSideController DeviceSideController {get; private set;} 
    public ClickableTool ClickableTool {get; private set;}
    [field: SerializeField] public DeviceSideType DeviceSideType {get; private set;}
    [SerializeField] public CinemachineVirtualCamera _sideCamera;
    [SerializeField] private CleanButton _cameraSwitchButton;
    private Collider[] _colliders;
    private DimmEffectManager _dimmEffectManager;
    private GameObject _childGO;

    public UnityEvent OnDeviceSideEnter;
    public UnityEvent OnDeviceSideExit;

    private void OnEnable()
    {
        _cameraSwitchButton.onClick.AddListener(SwitchToThisSide);
    }

    private void OnDisable()
    {
        _cameraSwitchButton.onClick.RemoveListener(SwitchToThisSide);
    }

    private void Start()
    {
        ResolveObjects();
    }

    private void SwitchToThisSide()
    {
        DeviceSideController.SetActiveSide(this);
        _dimmEffectManager.UpdateDimmCameraFOV(_sideCamera);
    }

    public void ToggleCameraIcon(bool enable)
    {
        _cameraSwitchButton.gameObject.SetActive(enable);
    }

    public void ToggleDeviceSide(bool enable)
    {
        _childGO.SetActive(enable);
    }

    public void SetCameraPriority(int value)
    {
        _sideCamera.Priority = value;

        if(value == 1)
            OnDeviceSideEnter?.Invoke();
        else if(value == 0)
            OnDeviceSideExit?.Invoke();
    }

    public void SetCameraPriorityWithoutNotify(int value)
    {
        _sideCamera.Priority = value;
    }

    public void ToggleColliders(bool shouldEnable)
    {
        for(int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = shouldEnable;
        }
    }

    private void ResolveObjects()
    {
        DeviceUI = GetComponentInParent<DeviceUI>();
        DeviceSideController = GetComponentInParent<DeviceSideController>();
        ClickableTool = DeviceUI.GetComponent<ClickableTool>();
        _colliders = GetComponents<Collider>();
        _dimmEffectManager = ExperimentItemsContainer.Instance.Resolve<DimmEffectManager>();
        _childGO = transform.GetChild(0).gameObject;
    }
}