using System.Linq;
using UnityEngine;

public class InteractionBlockerManager : MonoBehaviour
{
    [SerializeField] private GameObject interactionBlockerPrefab;
    [SerializeField] private Canvas _overlayCanvas;
    
    private GameObject _interactionBlockerInstance;
    private int _highestSortOrder;
    private int _lastSortOrder;
    private void Start()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        _highestSortOrder = canvases.Select(canvas => canvas.sortingOrder).Max();
        
        if(!_overlayCanvas) _overlayCanvas = GetComponent<Canvas>();
        
        InstanceTheBlocker();
    }
    private void InstanceTheBlocker()
    {
        _lastSortOrder = _overlayCanvas.sortingOrder;
        _interactionBlockerInstance = Instantiate(interactionBlockerPrefab);
        
        _interactionBlockerInstance.GetComponent<Canvas>().sortingOrder = _highestSortOrder + 1;
        _overlayCanvas.sortingOrder = _highestSortOrder + 1;

    }
    public void DestroyTheBlocker()
    {
        if (_interactionBlockerInstance)
        {
            Destroy(_interactionBlockerInstance);
        }

        ResetSortOrder();
    }
    private void ResetSortOrder()
    {
        _overlayCanvas.sortingOrder = _lastSortOrder;
    }
}
