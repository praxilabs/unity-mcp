using System.Collections;
using UnityEngine;

/// <summary>
/// This class has fields for the classes that control canvas render mode, the depth of field effect (DOF), and the loading screen animations.
/// Each class has a set of functions that make up part of the loading screen effect.
/// The Start function calls the functions to create the final effect.
/// </summary>
public class NewLoadingScreenController : MonoBehaviour
{
    private CanvasRenderModeController _canvasRenderModeController;
    private DepthOfFieldController _depthOfFieldController;
    private LoadingScreenAnimationController _loadingScreenAnimationController;
    [SerializeField] private CanvasGroup _canvasGroup;
    private float _blurTransitionDuration;

    private Coroutine _sharpenScreenCoroutine;
    private Coroutine _setCanvasGroupAlphaCoroutine;
    private Coroutine _updateCanvasListCoroutine;

    private void Awake()
    {
        _canvasRenderModeController = GetComponent<CanvasRenderModeController>();
        _depthOfFieldController = GetComponent<DepthOfFieldController>();
        _loadingScreenAnimationController = GetComponent<LoadingScreenAnimationController>();
    }

    private void Start()
    {
        _blurTransitionDuration = _depthOfFieldController.DistanceChangeTime;
    }

    #region DOF Related Functions
    public void BlurScreen(float blurTime)
    {
        AddSpawnedCanvasesToOverlayCanvases();
        SetFocusDistanceToMin();
        SetCanvasGroupAlpha(1);
        StartLoadingScreenAnimations();
        // Sharpen Screen calls a coroutine that will disable the DOF effect after the blur duration has passed
        SharpenScreen(blurTime);
    }

    void SharpenScreen(float blurTime)
    {
        GameHelper.RestartCoroutine(this, ref _sharpenScreenCoroutine, SharpenScreenCoroutine(blurTime));
    }
    void SetFocusDistanceToMax()
    {
        _depthOfFieldController?.SetFocusDistanceToMax();
    }
    void SetFocusDistanceToMin()
    {
        _depthOfFieldController?.SetFocusDistanceToMin();
    }
    IEnumerator SharpenScreenCoroutine(float blurTime)
    {
        yield return new WaitForSeconds(blurTime);

        SetFocusDistanceToMax();
        SetRenderModeScreenSpaceOverlay();
        FixOverlayCanvasesRenderingOrder();
        SetCanvasGroupAlpha(0);
        GameHelper.StopAndNullifyCoroutine(this, ref _updateCanvasListCoroutine);

        yield return new WaitForSeconds(0.1f);
        StopLoadingScreenAnimations();
    }
    #endregion

    #region Camera Render Mode Functions
    void PopulateOverlayCanvasList()
    {
        _canvasRenderModeController?.PopulateOverlayCanvasList();
    }
    void SetRenderModeScreenSpaceCamera()
    {
        _canvasRenderModeController?.SetCanvasesScreenSpaceCamera();
    }
    void SetRenderModeScreenSpaceOverlay()
    {
        _canvasRenderModeController?.SetCanvasesScreenSpaceOverlay();
    }
    void AddSpawnedCanvasesToOverlayCanvases()
    {
        GameHelper.RestartCoroutine(this, ref _updateCanvasListCoroutine, UpdateCanvasListCoroutine());
    }
    IEnumerator UpdateCanvasListCoroutine()
    {
        // This coroutine checks for new Canvas game objects 10 times per second as opposed to the default 60 times per second.
        while (true)
        {
            _canvasRenderModeController?.AddSpawnedCanvasesToOverlayCanvases();
            SetRenderModeScreenSpaceCamera();
            yield return new WaitForSeconds(0.2f);
        }
    }
    void FixOverlayCanvasesRenderingOrder()
    {
        _canvasRenderModeController?.FixOverlayCanvasesRenderingOrder();
    }
    #endregion

    #region Loading Screen Functions
    void SetCanvasGroupAlpha(float alpha)
    {
        GameHelper.RestartCoroutine(this, ref _setCanvasGroupAlphaCoroutine, SetCanvasGroupAlphaCoroutine(alpha));
    }
    IEnumerator SetCanvasGroupAlphaCoroutine(float targetAlpha)
    {
        float t = 0;

        float alpha = _canvasGroup.alpha;

        while (t < 1)
        {
            _canvasGroup.alpha = Mathf.Lerp(alpha, targetAlpha, t);
            t += Time.deltaTime / _blurTransitionDuration;
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
    }
    void StartLoadingScreenAnimations()
    {
        _loadingScreenAnimationController?.StartLoadingScreenAnimations();
    }
    void StopLoadingScreenAnimations()
    {
        _loadingScreenAnimationController?.StopLoadingScreenAnimations();
    }
    #endregion    
}