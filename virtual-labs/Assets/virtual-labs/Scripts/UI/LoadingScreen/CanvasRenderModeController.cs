using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This list of canvases is populated in the scene view of each even that has an instance
/// of the NewLoadingScreenController prefab. By default, the canvases are set to Screen Space - Overlay, which is unaffected
/// by post processing effects. Setting the render mode to Screen Space - Camera allows the UI to be affected by the 
/// post processing effects.
/// </summary>
public class CanvasRenderModeController : MonoBehaviour
{
    [SerializeField] private List<Canvas> _overlayCanvases;
    [SerializeField] private List<Canvas> _newlySpawnedCavases;
    [SerializeField] private Canvas _loadingScreenCanvas;
    [field: SerializeField] public float CanvasPlaneDistance { get; set; } = 0.0101f;
    [field: SerializeField] public int CanvasNewSortingOrder { get; set; } = 1;
    [SerializeField] private List<int> _canvasOldSortOrderList;

    public void PopulateOverlayCanvasList()
    {
        _overlayCanvases = new List<Canvas>();
        _canvasOldSortOrderList = new List<int>();

        _overlayCanvases = PopulateCanvasList();
        UpdateOldSortingOrderList(_overlayCanvases);
    }

    [ContextMenu("Populate Canvas List")]
    public void AddSpawnedCanvasesToOverlayCanvases()
    {
        _newlySpawnedCavases = new List<Canvas>();
        _newlySpawnedCavases = PopulateCanvasList();
        UpdateOldSortingOrderList(_newlySpawnedCavases);

        _overlayCanvases.AddRange(_newlySpawnedCavases);
    }

    void UpdateOldSortingOrderList(List<Canvas> canvases)
    {
        for (int i = 0; i < canvases.Count; i++)
        {
            if (canvases[i] == null) continue;

            _canvasOldSortOrderList.Add(canvases[i].sortingOrder);
        }
    }

    public List<Canvas> PopulateCanvasList()
    {
        List<Canvas> canvases = new List<Canvas>();
        canvases = FindObjectsOfType<Canvas>(true).ToList();

        if (canvases.Contains(_loadingScreenCanvas))
            canvases.Remove(_loadingScreenCanvas);

        RemoveNullCanvass(canvases);
        RemoveInactiveCanvases(canvases);
        FilterOutOverlayCanvases(canvases);

        return canvases;
    }

    void RemoveNullCanvass(List<Canvas> canvases)
    {
        for (int i = canvases.Count - 1; i >= 0; i--)
        {
            if (canvases[i] == null)
            {
                canvases.RemoveAt(i);
            }
        }
    }
    void RemoveInactiveCanvases(List<Canvas> canvases)
    {
        for (int i = canvases.Count - 1; i >= 0; i--)
        {
            if (!canvases[i].gameObject.activeInHierarchy)
            {
                canvases.RemoveAt(i);
            }
        }
    }
    void FilterOutOverlayCanvases(List<Canvas> canvases)
    {
        for (int i = canvases.Count - 1; i >= 0; i--)
        {
            if (canvases[i].renderMode == RenderMode.ScreenSpaceOverlay)
            {
                continue;
            }
            else
            {
                canvases.RemoveAt(i);
            }
        }
    }

    [ContextMenu("Screen Space Camera")]
    public void SetCanvasesScreenSpaceCamera()
    {
        foreach (var canvas in _overlayCanvases)
        {
            if (canvas == null) continue;
            SetRenderModeScreenSpaceCamera(canvas);
        }
    }

    [ContextMenu("Screen Space Overlay")]
    public void SetCanvasesScreenSpaceOverlay()
    {
        for (int i = 0; i < _overlayCanvases.Count; i++)
        {
            if (_overlayCanvases[i] == null) continue;
            _overlayCanvases[i].sortingOrder = _canvasOldSortOrderList[i];
            RestoreCanvasOriginalSettings(_overlayCanvases[i]);
        }
    }

    void SetRenderModeScreenSpaceCamera(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = CanvasPlaneDistance;
        canvas.sortingOrder = CanvasNewSortingOrder;
    }

    void RestoreCanvasOriginalSettings(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    public void FixOverlayCanvasesRenderingOrder()
    {
        for (int i = _overlayCanvases.Count - 1; i >= 0; i--)
        {
            if (_overlayCanvases[i] == null)
            {
                _overlayCanvases.RemoveAt(i);
                continue;
            }

            if (!_overlayCanvases[i].gameObject.activeInHierarchy)
            {
                _overlayCanvases.RemoveAt(i);
                continue;
            }

            _overlayCanvases[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _overlayCanvases.Count; i++)
        {
            _overlayCanvases[i].gameObject.SetActive(true);
        }
    }
}
