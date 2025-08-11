using Praxilabs.CameraSystem;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialsManager : MonoBehaviour
{
    TutorialSO _tutorialSO;
    [SerializeField] private TutorialsDisplay _tutorialsDisplay;
    [SerializeField] private TutorialsAnimationManager _tutorialsAnimationManager;
    private int _currentModuleIndex = 0;
    private int _currentModuleInnerDataIndex = 0;
    private bool _finalCelebrationIsShownBefore;
    private RectTransform _tutorialCardRect;

    public void OpenFromSettings()
    {
        StartCoroutine(OpenFromMenu()); 
    }
    private IEnumerator OpenFromMenu()
    {
        yield return new WaitForSeconds(2f);
        GetComponentInParent<Canvas>().enabled = true;
    }

    public void InitializeTutorials(TutorialSO MySO)
    {
        _tutorialsDisplay.ExperimentManager = FindObjectOfType<ExperimentManager>();
        _tutorialsDisplay.CloseGuide.onClick.AddListener(_tutorialsDisplay.ExperimentManager.StartExperimentAfterTutorial);

        _tutorialSO = MySO;
        _tutorialCardRect = _tutorialsDisplay.TutorialCard.GetComponent<RectTransform>();
        _tutorialsAnimationManager.AssignTutorialsData(MySO, _tutorialCardRect);

        _tutorialsAnimationManager.AnimateWelcomingIconAndPanel();

        AssignTutorialUIData(_currentModuleInnerDataIndex, _currentModuleIndex);
        _tutorialsDisplay.NextButton.onClick.AddListener(MoveToNextItem);
        _tutorialsDisplay.SkipButton.onClick.AddListener(SkipExploring);
        _tutorialsDisplay.FinishButton.onClick.AddListener(FinishExplore);

        _tutorialsDisplay.SkipFromModuleButton.onClick.AddListener(SkipModule);
        _tutorialsDisplay.ContinueLearningNextModuleButton.onClick.AddListener(ContinueToNextModule);

        for (int i = 0; i < _tutorialSO._tutorialModules.Count; i++)
        {
            GameObject tmp = Instantiate(_tutorialsDisplay.BeginnerGuideItemPrefab, _tutorialsDisplay.BeginnerGuideScrollContent.transform);
            //Scroll Item Beginner guide title index
            tmp.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = _tutorialSO._tutorialModules[i]._title;

            int count = i;

            //Scroll Item Beginner guide button index
            tmp.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                _tutorialsDisplay.BeginnerGuideInterface.gameObject.SetActive(false);
                _tutorialsDisplay.TutorialCard.gameObject.SetActive(true);

                _tutorialsDisplay.SkipButton.gameObject.SetActive(true);
                _tutorialsDisplay.NextButton.gameObject.SetActive(true);
                _tutorialsDisplay.FinishButton.gameObject.SetActive(false);

                _tutorialsDisplay.TutorialIcon.gameObject.SetActive(false);

                _currentModuleIndex = count;
                AssignTutorialUIData(0 , _currentModuleIndex);
            });
        }

        //Update Scroll list Status On Starting
        for (int i = 0; i < _tutorialSO._tutorialModules.Count; i++)
        {
            if (_tutorialSO._tutorialModules[i].Status == TutorialModuleStatus.Finished)
            {
                //Checkmarks index
                _tutorialsDisplay.BeginnerGuideScrollContent.transform.GetChild(i).GetChild(4).GetChild(0).gameObject.SetActive(false);
                _tutorialsDisplay.BeginnerGuideScrollContent.transform.GetChild(i).GetChild(4).GetChild(1).gameObject.SetActive(true);
            }
        }

    }

    private void ContinueToNextModule()
    {
        _currentModuleInnerDataIndex = 0;

        if (_currentModuleIndex < (_tutorialSO._tutorialModules.Count))
        {
            _tutorialsDisplay.CongratulationsAfterModuleUI.gameObject.SetActive(false);
            _tutorialsDisplay.BeginnerGuideInterface.gameObject.SetActive(false);
            _tutorialsDisplay.TutorialCard.gameObject.SetActive(true);

            _currentModuleIndex++;
            AssignTutorialUIData(_currentModuleInnerDataIndex, _currentModuleIndex);
        }
        else
        {
            _tutorialsDisplay.BeginnerGuideInterface.gameObject.SetActive(true);
            _tutorialsDisplay.TutorialIcon.gameObject.SetActive(true);
            _tutorialsDisplay.TutorialCard.gameObject.SetActive(false);
            _tutorialsDisplay.TutorialSubImage.gameObject.SetActive(false);
        }

    }

    private void SkipModule()
    {
        _currentModuleInnerDataIndex = 0;

        _tutorialsDisplay.BeginnerGuideInterface.gameObject.SetActive(true);
        _tutorialsDisplay.TutorialIcon.gameObject.SetActive(true);
        _tutorialsDisplay.TutorialCard.gameObject.SetActive(false);
        _tutorialsDisplay.CongratulationsAfterModuleUI.gameObject.SetActive(false);
        _tutorialsDisplay.TutorialSubImage.gameObject.SetActive(false);
    }

    private void FinishExplore()
    {
        _currentModuleInnerDataIndex = 0;
        UpdateScrollListStatus();
        StartCoroutine(ShowFinalCelebration());

        if (_currentModuleIndex > 0  && _currentModuleIndex < _tutorialSO._tutorialModules.Count)
        {
            ShowCongratulationsOnModuleFinish();
        }
        else
        {
            SkipModule();
        }
    }

    private void SkipExploring()
    {
        _currentModuleInnerDataIndex = 0;
        AssignTutorialUIData(_currentModuleInnerDataIndex , _currentModuleIndex);

        _tutorialsDisplay.BeginnerGuideInterface.gameObject.SetActive(true);
        _tutorialsDisplay.TutorialIcon.gameObject.SetActive(true);
        _tutorialsDisplay.TutorialCard.gameObject.SetActive(false);
        _tutorialsDisplay.TutorialSubImage.gameObject.SetActive(false);


        UpdateScrollListStatus();
    }

    public void MoveToNextItem()
    {
        if (_tutorialSO._tutorialModules[_currentModuleIndex]._data.Count < 2) return;
        if (_currentModuleInnerDataIndex == _tutorialSO._tutorialModules[_currentModuleIndex]._data.Count - 1) return;

        _currentModuleInnerDataIndex++;
        AssignTutorialUIData(_currentModuleInnerDataIndex, _currentModuleIndex);
    }


    public void AssignTutorialUIData(int myIndex , int moduleIndex)
    {
        //module data must not be 0 and showing tutorial card data + Arrow Pointing
        _tutorialsDisplay.OuterTitleText.text = _tutorialSO._tutorialModules[moduleIndex]._title;
        _tutorialsDisplay.InnerTitleText.text = $"{myIndex+1}. {_tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._title}";

        _tutorialsDisplay.OuterProgressText.text = $"{myIndex+1} of {_tutorialSO._tutorialModules[moduleIndex]._data.Count}";

        _tutorialCardRect.anchoredPosition = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._cardPositionPlaceholder;
        _tutorialCardRect.anchorMin = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._cardAnchorMin;
        _tutorialCardRect.anchorMax = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._cardAnchorMax;

        _tutorialsDisplay.InnerBodyText.text = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._info;

        _tutorialsDisplay.ArrowDirectionObject.anchoredPosition = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._imageData._arrowImagePosition;
        _tutorialsDisplay.ArrowDirectionObject.eulerAngles =  new Vector3 (0,0,_tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._imageData._arrowImageAngle);


        //Showing Sub Image With Tutorial Card
        if(_tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._imageData._image != null)
        {
            _tutorialsDisplay.TutorialSubImage.gameObject.SetActive(true);
            _tutorialsDisplay.TutorialSubImage.sprite = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._imageData._image;
            _tutorialsDisplay.TutorialSubImage.rectTransform.sizeDelta = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._imageData._imageDimensions;
            _tutorialsDisplay.TutorialSubImage.rectTransform.anchorMin = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._imageData._imageAnchorMin;
            _tutorialsDisplay.TutorialSubImage.rectTransform.anchorMax = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._imageData._imageAnchorMax;
            _tutorialsDisplay.TutorialSubImage.rectTransform.anchoredPosition = _tutorialSO._tutorialModules[moduleIndex]._data[myIndex]._imageData._imagePlaceholderPosition;

        }
        else
        {
            _tutorialsDisplay.TutorialSubImage.gameObject.SetActive(false);
        }

        //When Reached end of current module mark as finished and show finish button
        if (_currentModuleInnerDataIndex == _tutorialSO._tutorialModules[moduleIndex]._data.Count - 1)
        {
            _tutorialSO._tutorialModules[moduleIndex].Status = TutorialModuleStatus.Finished;

            _tutorialsDisplay.SkipButton.gameObject.SetActive(false);
            _tutorialsDisplay.NextButton.gameObject.SetActive(false);
            _tutorialsDisplay.FinishButton.gameObject.SetActive(true);

        }
        else
        {
            _tutorialsDisplay.SkipButton.gameObject.SetActive(true);
            _tutorialsDisplay.NextButton.gameObject.SetActive(true);
            _tutorialsDisplay.FinishButton.gameObject.SetActive(false);
        }

        //Animate Card as per module
        _tutorialsAnimationManager.AnimateTutorialCard(_tutorialSO._tutorialModules[_currentModuleIndex]._data[_currentModuleInnerDataIndex], _tutorialCardRect);
    }

    private void UpdateScrollListStatus()
    {
        if (_tutorialSO._tutorialModules[_currentModuleIndex].Status == TutorialModuleStatus.Finished)
        {
            //Checkmarks index
            _tutorialsDisplay.BeginnerGuideScrollContent.transform.GetChild(_currentModuleIndex).GetChild(4).GetChild(0).gameObject.SetActive(false);
            _tutorialsDisplay.BeginnerGuideScrollContent.transform.GetChild(_currentModuleIndex).GetChild(4).GetChild(1).gameObject.SetActive(true);
        }
    }

    private IEnumerator ShowFinalCelebration()
    {
        if (_finalCelebrationIsShownBefore) yield break;

        bool allFinished = true;

        for (int i = 0; i < _tutorialSO._tutorialModules.Count; i++)
        {
            if (_tutorialSO._tutorialModules[i].Status != TutorialModuleStatus.Finished)
            {
                allFinished = false;
                break;
            }
        }

        if (allFinished) 
        {
            _tutorialsDisplay.CongratulationsFullFinished.SetActive(true);
            yield return new WaitForSeconds(3f);
            _tutorialsDisplay.CongratulationsFullFinished.SetActive(false);
            _finalCelebrationIsShownBefore = true;


            _tutorialsDisplay.ExperimentManager.StartExperimentAfterTutorial();
            _tutorialsDisplay.CloseGuide.onClick.Invoke();
        }

    }

    private void ShowCongratulationsOnModuleFinish()
    {
        _tutorialsDisplay.CongratulationsAfterModuleUI.gameObject.SetActive(true);
        _tutorialsDisplay.TutorialCard.gameObject.SetActive(false);
        _tutorialsDisplay.TutorialIcon.gameObject.SetActive(false);
        _tutorialsDisplay.BeginnerGuideInterface.gameObject.SetActive(false);
        _tutorialsDisplay.TutorialSubImage.gameObject.SetActive(false);

        _tutorialsDisplay.CurrentModuleCongratsMessage.text = $"Well done, you completed learning about {_tutorialSO._tutorialModules[_currentModuleIndex]._title}";

        if(_currentModuleIndex < _tutorialSO._tutorialModules.Count-1)
        {
            _tutorialsDisplay.NextModuleMessage.text = $"Would you like to learn about {_tutorialSO._tutorialModules[_currentModuleIndex + 1]._title}?";
        }
        else
        {
            _tutorialsDisplay.BeginnerGuideInterface.gameObject.SetActive(true);
            _tutorialsDisplay.TutorialIcon.gameObject.SetActive(true);
            _tutorialsDisplay.TutorialCard.gameObject.SetActive(false);
            _tutorialsDisplay.CongratulationsAfterModuleUI.gameObject.SetActive(false);
        }
        
    }

}
