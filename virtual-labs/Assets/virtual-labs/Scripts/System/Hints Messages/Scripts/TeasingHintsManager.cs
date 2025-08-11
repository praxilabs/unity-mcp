using UnityEngine;
using Praxilabs.UIs;

public class TeaseHintManager : MonoBehaviour
{
    [SerializeField] private HintsDisplay _hintsDisplayObject;
    [SerializeField] private TeaseHintsDisplay _teaseHintsDisplayObject;
    
    [SerializeField] private HintsManager _uiManager; // Reference to UIManager

    private int _stepNumber;
    private int _teasingHintIndex;

    private void Start()
    {
        _teaseHintsDisplayObject.ConfirmMoreHelp.onClick.AddListener(ConfirmNeedingMoreHelp);
        _teaseHintsDisplayObject.Cancel.onClick.AddListener(CancelTeasingHintHelp);
    }

    public void InitializeHints(int stepNumber )
    {
        _stepNumber = stepNumber - 1; // Adjust for zero-based index
        _teasingHintIndex = 0;
 
        ConfirmNeedingMoreHelp();
    }

    public async void ConfirmNeedingMoreHelp()
    {
        if (_teasingHintIndex < _uiManager.hintsData.StepsData[_stepNumber].TeaseHints.Count)
        {
            #if UNITY_EDITOR

            _hintsDisplayObject.HintsTextEditorMode.text = _uiManager.hintsData.StepsData[_stepNumber].TeaseHints[_teasingHintIndex];

            #else

             await _hintsDisplayObject.MainCanvasWebViewPrefab.WaitUntilInitialized();
             _teaseHintsDisplayObject.MainCanvasWebPrefab.WebView.LoadHtml(_uiManager.hintsData.StepsData[_stepNumber].TeaseHints[_teasingHintIndex]);

            #endif

            _teasingHintIndex++;

            _hintsDisplayObject.NextBackContainer.SetActive(false);
            _hintsDisplayObject.LineSegmentContainer.SetActive(false);
            _hintsDisplayObject.MoreHelpContainer.SetActive(true);
        }
        else
        {
            _hintsDisplayObject.MoreHelpContainer.SetActive(false);
            if (_uiManager.hintsData.StepsData[_stepNumber].Hints.Count > 1)
            {
                _hintsDisplayObject.NextBackContainer.SetActive(true);
                _hintsDisplayObject.LineSegmentContainer.SetActive(true);
            }

            _uiManager.UpdateBodyText(0);
            _uiManager.ClearPreviousLineSegments();
            _uiManager.GenerateLineSegments();
            _uiManager.AdjustSizeOnConfirmNeedingMoreHelp();
            StartCoroutine(_uiManager.UpdateLineSegments());
        }
    }

    public void CancelTeasingHintHelp()
    {
        _uiManager.ForceCloseHintBox();
    }

   
}
