using Cinemachine;
using UltimateClean;
using UnityEngine;
using UnityEngine.Events;

public class CameraFirstPersonUI : MonoBehaviour
{
    public CleanButton zoomin;
    public CleanButton zoomout;

    public CleanButton reset;
    public CleanButton thirdPersonPOV;

    public CleanButton leftInterestPointButton;
    public CleanButton rightInterestPointButton;
    public CinemachineVirtualCamera leftInterestPoint;
    public CinemachineVirtualCamera rightInterestPoint;

    [HideInInspector] public CleanButton interestPointButton;
    [HideInInspector] public CleanButton goBackButton;

    private FirstPersonUIHolder _firstPersonUIHolder;
    private event UnityAction OnThirdPersonSwitchDelegate;

    private void OnEnable()
    {
        OnThirdPersonSwitchDelegate = () =>
        {
            _firstPersonUIHolder.ToggleFirstPersonUI(false);
            _firstPersonUIHolder.ToggleSideMenu(false);
        };

        thirdPersonPOV.onClick.AddListener(OnThirdPersonSwitchDelegate);
    }

    private void OnDisable()
    {
        thirdPersonPOV.onClick.RemoveListener(OnThirdPersonSwitchDelegate);
    }

    private void Start()
    {
        ResolveObjects();
        CheckNearbyInterestPoints();
    }

    public void CheckNearbyInterestPoints()
    {
        if (leftInterestPoint != null)
            leftInterestPointButton.gameObject.SetActive(true);

        if (rightInterestPoint != null)
            rightInterestPointButton.gameObject.SetActive(true);
    }

    public void DisableInterestPointsButons()
    {
        leftInterestPointButton.gameObject.SetActive(false);
        rightInterestPointButton.gameObject.SetActive(false);
    }

    private void ResolveObjects()
    {
        _firstPersonUIHolder = ExperimentItemsContainer.Instance.Resolve<FirstPersonUIHolder>();
    }
}