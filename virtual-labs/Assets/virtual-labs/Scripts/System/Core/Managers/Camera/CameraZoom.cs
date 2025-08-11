using Praxilabs.CameraSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraZoom : MonoBehaviour
{
    public event System.Action OnMouseClick;
    public event System.Action OnCameraMoveComplete;
    public event System.Action OnCameraMovementStarted;

    public static ZeroParamEventKey OnCameraReachedTarget = new ZeroParamEventKey("OnCameraReachedTarget");
    public static ZeroParamEventKey OnCameraStartedMoving = new ZeroParamEventKey("OnCameraStartedMoving");

    public Camera cameraTarget;
    // private CameraManager cameraManager;
    private string gameObjectName;
    [field: SerializeField] public bool CameraReachedTarget { get; private set; }
    public bool ignoreInput;
    bool ignoreMouseClick;

    private void Awake()
    {
        // cameraManager = Camera.main.GetComponent<CameraManager>();
    }

    private void OnEnable()
    {
        // cameraManager.ResetCamera += SetCameraReachedTarget;
        // EventsManager.AddListener(CameraZoom.OnCameraReachedTarget, SetCameraReachedTarget);
        // EventsManager.AddListener(OnCameraStartedMoving, CameraStartedMoving);
    }
    private void OnDisable()
    {
        // cameraManager.ResetCamera -= SetCameraReachedTarget;
        // EventsManager.AddListener(ZeroParamEventManagerEnum.OnCameraReachedTarget, SetCameraReachedTarget);
        // EventsManager.AddListener(ZeroParamEventManagerEnum.OnCameraStartedMoving, CameraStartedMoving);
    }

    // Start is called before the first frame update
    void Start()
    {
        // cameraManager = Camera.main.GetComponent<CameraManager>();
        gameObjectName = gameObject.name;
        ignoreMouseClick = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraManager.Instance.eventSystem.IsPointerOverGameObject())
        {
            ignoreMouseClick = true;
        }
        else
        {
            ignoreMouseClick = false;
        }
    }

    private void OnMouseDown()
    {
        if (!ignoreMouseClick)
        {
            MoveCamToObject();
        }
    }

    public void IgnoreCameraZoom(string ignore)
    {
        ignoreInput = bool.Parse(ignore);

        if (OnIgnoreCameraZoom != null) { OnIgnoreCameraZoom(gameObject, "IgnoreCameraZoom"); }
    }
    public event System.Action<GameObject, string> OnIgnoreCameraZoom;

    public void ChangeGameObjectName(string newName)
    {
        gameObjectName = newName;
    }

    public void MoveCamToObject()
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamToObjectCoroutine());
    }

    IEnumerator MoveCamToObjectCoroutine()
    {
        // Wait before checking if the mouse is held down
        yield return new WaitForSeconds(0.15f);

        // If mouse button is not being held down, zoom in. Otherwise it means the user is dragging the object
        if (!Input.GetMouseButton(0) && !ignoreInput)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                OnMouseClick?.Invoke();

                MoveCameraToObject();
            }
        }
    }

    public void MoveCameraToObject()
    {
    //     if (!CameraReachedTarget)
    //     {
    //         cameraManager.MoveCameraToObject(gameObjectName);
    //     }
    //     else
    //     {
    //         cameraManager.ReturnCamera("test");
    //     }
    }

    private void SetCameraReachedTarget()
    {
        // CameraReachedTarget = cameraManager.transform.position == cameraTarget.transform.position;

        OnCameraMoveComplete?.Invoke();
    }

    void CameraStartedMoving()
    {
        OnCameraMovementStarted?.Invoke();
    }
}
