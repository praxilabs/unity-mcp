using DG.Tweening;
using LocalizationSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.UIs
{
    public class HintsManager : MenuWithWebview
    {

        private const int DEFAULT_MAIN_CONTAINER_PADDING = 80;
        private const int MAIN_CONTAINER_PADDING_BOTTOM = 60;
        private const float DEFAULT_SEGMENT_WIDTH = 46f;

        public HintsJsonData hintsData;

        [SerializeField] private HintsDisplay _hintsDisplayObject;

        private HintsStepProgressManager _stepProgressManager;
        private TeaseHintManager _teaseHintManager;
        private HintsAnimationManager _animationManager;

        private int _stepNumber;
        private int _teasingHintIndex;
        private int _currentIndex;
        private int _lastStepNumber;

        
        private void Awake()
        {
            _stepProgressManager = GetComponent<HintsStepProgressManager>();
            _teaseHintManager = GetComponent<TeaseHintManager>();
            _animationManager = GetComponent<HintsAnimationManager>();

        }

        private void Start()
        {
            InitializeButtonEvents();

            
            #if UNITY_EDITOR

            Debug.Log("Unity Editor Hints");
            _canvasWebViewPrefabList.Remove(_hintsDisplayObject.MainCanvasWebViewPrefab);
            Destroy(_hintsDisplayObject.MainCanvasWebViewPrefab);

#else

            Destroy(_hintsDisplayObject.ScrollViewEditorMode);
            InitializeWebView();

#endif
        }

        public void OpenHintsMenuSilent(int stepNumber)
        {
            OpenHintsMenu(stepNumber);
            if(!_animationManager.IsPinned) ForceCloseHintBox();
        }

        public void OpenHintsMenu(int stepNumber)
        {
            if (stepNumber <= 0 || stepNumber > hintsData.StepsData.Count)
            {
                gameObject.SetActive(false);
                return;
            }

            _stepNumber = stepNumber - 1;

            if (_stepNumber != _lastStepNumber)
            {
                if (this.hintsData.StepsData[_stepNumber].State != StateTypes.Finished)
                {
                    this.hintsData.StepsData[_stepNumber].State = StateTypes.Current;
                }
                this.hintsData.StepsData[_lastStepNumber].State = StateTypes.Finished;
            }

            _lastStepNumber = _stepNumber;
            _hintsDisplayObject.CurrentStepButtonText.text = (_lastStepNumber + 1).ToString("D2"); // To display 1 instead of 0 ,  minimze the size of webview to show it all the time
            _stepProgressManager.InitializeStep(_stepNumber + 1);
            //_teaseHintManager.InitializeHints(_hintsSO, _stepNumber + 1);
            ShowHintsBox();
            UpdateUIView(_stepNumber + 1);

            OpenBaseCall();

        }

        public void ShowHintsBox()
        {
            _animationManager.ShowHintBoxRect();
        }

        public void ShowHintsBoxWhenPinned()
        {
            if(_animationManager == null) return;

            Debug.Log("Pinned : " + _animationManager.IsPinned);
              
            if(_animationManager.IsPinned)
            {
                _animationManager.ShowHintBoxRect();
            }
        }

        public void UpdateUIText()
        {
            UpdateBodyText(_currentIndex);
            UpdateBodyHeaderTitle(_stepNumber + 1);
        }

        public void UpdateUIView(int stepNumber)
        {
            _stepNumber = stepNumber - 1;
            _teasingHintIndex = 0;
            _currentIndex = 0;

            if (hintsData.StepsData[_stepNumber].TeaseHints.Count == 0)
            {
                GenerateLineSegments();
                StartCoroutine(UpdateLineSegments());
            }
            else
            {
                ClearPreviousLineSegments();
            }

            SetNextButtonToEnabled();
            SetBackButtonToDisabled();

            _teaseHintManager.InitializeHints(_stepNumber + 1);

            UpdateBodyHeaderTitle(_stepNumber + 1);

            AdjustUISizes();
            ForceUIRedraw();   
        }

        public void OpenBaseCall()
        {
            #if UNITY_EDITOR

                     Debug.Log("Unity Editor Hints");

            #else
            
                     base.Open();

            #endif
        }

        private async void InitializeWebView()
        {
            await _hintsDisplayObject.MainCanvasWebViewPrefab.WaitUntilInitialized();
            _hintsDisplayObject.MainCanvasWebViewPrefab.WebView.SetDefaultBackgroundEnabled(false);
        }

        private void InitializeButtonEvents()
        {
            _hintsDisplayObject.AllStepsButton.onClick.AddListener(() => _stepProgressManager.ToggleProgress());
            _hintsDisplayObject.CurrentStepButton.onClick.AddListener(NavigateBackToCurrentStep);
            _hintsDisplayObject.FullScreenButton.onClick.AddListener(ToggleFullScreen);


            _hintsDisplayObject.BackButton.GetComponent<Button>().onClick
                .AddListener(UpdateBackButtonStateWithMessagesCount);
            _hintsDisplayObject.NextButton.GetComponent<Button>().onClick
                .AddListener(UpdateNextButtonStateWithMessagesCount);

            AdjustUISizes();

            _hintsDisplayObject.FullScreenButton.onClick.AddListener(_animationManager.ToggleFullScreenOn);

            ForceUIRedraw();
        }

        public void AdjustSizeOnConfirmNeedingMoreHelp()
        {
            if (hintsData.StepsData[_stepNumber].Hints.Count > 1)
            {
                _hintsDisplayObject.MainRectTransform.GetComponent<VerticalLayoutGroup>().padding.bottom = DEFAULT_MAIN_CONTAINER_PADDING;
            }
            else
            {
                _hintsDisplayObject.MainRectTransform.GetComponent<VerticalLayoutGroup>().padding.bottom = DEFAULT_MAIN_CONTAINER_PADDING - MAIN_CONTAINER_PADDING_BOTTOM;
            }
               
            _animationManager.ResetProgressBarHeight();
            ForceUIRedraw();
        }

        private void ForceUIRedraw()
        {
            _hintsDisplayObject.MainRectTransform.GetComponent<VerticalLayoutGroup>().enabled = false;
            _hintsDisplayObject.MainRectTransform.GetComponent<VerticalLayoutGroup>().enabled = true;
        }

        private void AdjustUISizes()
        {
            if (hintsData.StepsData[_stepNumber].Hints.Count > 1)
            {
                if (hintsData.StepsData[_stepNumber].TeaseHints.Count > 0)
                {
                    _hintsDisplayObject.NextBackContainer.SetActive(false);
                    _hintsDisplayObject.MainRectTransform.GetComponent<VerticalLayoutGroup>().padding.bottom = DEFAULT_MAIN_CONTAINER_PADDING - MAIN_CONTAINER_PADDING_BOTTOM;
                    _animationManager.SetProgressBarHeight();
                }
                else
                {
                    Debug.Log("No Teasing Hints");
                    _hintsDisplayObject.NextBackContainer.SetActive(true);
                    _hintsDisplayObject.MainRectTransform.GetComponent<VerticalLayoutGroup>().padding.bottom = DEFAULT_MAIN_CONTAINER_PADDING;
                    _animationManager.ResetProgressBarHeight();
                }
            }
            else
            {
                _hintsDisplayObject.MainRectTransform.GetComponent<VerticalLayoutGroup>().padding.bottom = DEFAULT_MAIN_CONTAINER_PADDING - MAIN_CONTAINER_PADDING_BOTTOM;
                _hintsDisplayObject.NextBackContainer.SetActive(false);
            }
        }

        private void UpdateBodyHeaderTitle(int StepNumber)
        {
            string localizedTemplate = LocalizationManager.Instance.GetLocalizedString("Hints_StaticData", "Step Number");
            _hintsDisplayObject.BodyHeaderTitleText.text = string.Format(localizedTemplate, StepNumber);
        }

        private void UpdateBackButtonStateWithMessagesCount()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                UpdateBodyText(_currentIndex);
                SetNextButtonToEnabled();
            }

            if (_currentIndex == 0)
            {
                SetBackButtonToDisabled();
            }
            StartCoroutine(UpdateLineSegments());
            UpdateBodyText(_currentIndex);
        }

        private void UpdateNextButtonStateWithMessagesCount()
        {
            if (_currentIndex < hintsData.StepsData[_stepNumber].Hints.Count - 1)
            {
                _currentIndex++;
                UpdateBodyText(_currentIndex);
                SetBackButtonToEnabled();
            }

            if (_currentIndex == hintsData.StepsData[_stepNumber].Hints.Count - 1)
            {
                SetNextButtonToDisabled();
            }
            StartCoroutine(UpdateLineSegments());
            UpdateBodyText(_currentIndex);
        }

        public async void UpdateBodyText(int hintsIdx)
        {

        #if UNITY_EDITOR
            _hintsDisplayObject.HintsTextEditorMode.text = hintsData.StepsData[_stepNumber].Hints[hintsIdx];
        #else

              await _hintsDisplayObject.MainCanvasWebViewPrefab.WaitUntilInitialized();
              _hintsDisplayObject.MainCanvasWebViewPrefab.WebView.LoadHtml(hintsData.StepsData[_stepNumber].Hints[hintsIdx]);
        #endif

        }

        private void SetBackButtonToEnabled()
        {
            _hintsDisplayObject.BackButton.interactable = true;
            _hintsDisplayObject.BackButton.image.color = _hintsDisplayObject.ActiveColor;
        }

        private void SetBackButtonToDisabled()
        {
            _hintsDisplayObject.BackButton.interactable = false;
            _hintsDisplayObject.BackButton.image.color = _hintsDisplayObject.DisabledColor;
        }

        private void SetNextButtonToEnabled()
        {
            _hintsDisplayObject.NextButton.interactable = true;
            _hintsDisplayObject.NextButton.image.color = _hintsDisplayObject.ActiveColor;
        }

        private void SetNextButtonToDisabled()
        {
            _hintsDisplayObject.NextButton.interactable = false;
            _hintsDisplayObject.NextButton.image.color = _hintsDisplayObject.DisabledColor;
        }

        public void HideHintBoxRect()
        {
            _animationManager.HideHintBoxRect();
        }

        public void HideHintBoxWhilePinned()
        {
            if(_animationManager != null) 
            _animationManager.HideHintBoxRectWhilePinned();
        }

        public void ForceCloseHintBox()
        {
            if(_animationManager==null) return;

            _animationManager.Unpin();
            _animationManager.ForceHideHintBoxRect();
        }

        public void TogglePinning()
        {
            if(_animationManager.IsPinned) _animationManager.Unpin();
            else _animationManager.Pin();
        }

        private void ToggleFullScreen()
        {
            if (_animationManager.IsFullScreen)
            {
                _animationManager.ToggleFullScreenOff();
            }
            else
            {
                _animationManager.ActivateFullScreen();
            }
        }

        public void ClearPreviousLineSegments()
        {
            if (_hintsDisplayObject.LineSegmentContainer.transform.childCount > 0)
            {
                for (int i = 0; i < _hintsDisplayObject.LineSegmentContainer.transform.childCount; i++)
                {
                    Destroy(_hintsDisplayObject.LineSegmentContainer.transform.GetChild(i).gameObject);
                }
            }
        }

        public void GenerateLineSegments()
        {
            if (hintsData.StepsData[_stepNumber].Hints.Count == 1)
            {
                _hintsDisplayObject.NextBackContainer.SetActive(false);
                return;
            }

            for (int i = 0; i < hintsData.StepsData[_stepNumber].Hints.Count; i++)
            {
                GameObject tmp = Instantiate(_hintsDisplayObject.LineSegmentPrefab);
                tmp.transform.SetParent(_hintsDisplayObject.LineSegmentContainer.transform, false);
            }
        }

        public IEnumerator UpdateLineSegments()
        {
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < _hintsDisplayObject.LineSegmentContainer.transform.childCount; i++)
            {
                RectTransform tmpRect = _hintsDisplayObject.LineSegmentContainer.transform.GetChild(i).GetComponent<RectTransform>();
                float currentWidth = tmpRect.sizeDelta.x;

                if (i == _currentIndex)
                {
                    tmpRect.DOSizeDelta(new Vector2(currentWidth * 2.435f, tmpRect.sizeDelta.y), 0.5f);
                    tmpRect.gameObject.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    tmpRect.DOSizeDelta(new Vector2(DEFAULT_SEGMENT_WIDTH, tmpRect.sizeDelta.y), 0.5f);
                    tmpRect.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.25f);
                }
            }
        }

        private void NavigateBackToCurrentStep()
        {
            if (hintsData != null)
            {
                OpenHintsMenu(_lastStepNumber + 1);
            }
        }

        public void NavigateSteps(int StepNumber)
        {
            if (StepNumber == _stepNumber) return;
           UpdateUIView(_stepNumber);
        }

    }

}