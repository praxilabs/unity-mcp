 
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;


namespace Praxilabs.UIs
{
    public class HintsDisplay : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _allStepsButton;
        [SerializeField] private Button _currentStepButton;
        [SerializeField] private Button _FullScreenButton;
        [SerializeField]private Button _oxiButtonOpen;
        [SerializeField] private Button _oxiButtonClose;
        [Space(20)]
        
        [SerializeField] private GameObject _progressBarObject;
        [SerializeField] private GameObject _stepPorgressScrollableContainerObject;
        
        [SerializeField] private TextMeshProUGUI _currentStepButtonText;
        [SerializeField] private TextMeshProUGUI _bodyHeaderTitleText;
        [SerializeField] private TextMeshProUGUI _hintsTextEditorMode;


        [SerializeField] private RectTransform _mainRectTransform;
        [SerializeField] private CanvasWebViewPrefab _mainCanvasWebViewPrefab;


        [SerializeField] private GameObject _MoreHelpContainer;
        [SerializeField] private GameObject _NextBackContainer;


        

        [SerializeField] private GameObject _scrollViewBodyContent;
        [SerializeField] private RectTransform _scrollViewObject;
        [SerializeField] private GameObject _scrollViewEditorMode;
        


        [Header("Nav Segment")]
        [SerializeField] private GameObject _lineSegmentContainer;
        [SerializeField] private GameObject _lineSegmentPrefab;

        [Header("Colors of Buttons")] 
        [SerializeField] private Color _disabledColor;
        [SerializeField] private Color _activeColor;



        public Button NextButton
        {
            get => _nextButton;
            set => _nextButton = value;
        }

        public Button BackButton
        {
            get => _backButton;
            set => _backButton = value;
        }

        public GameObject LineSegmentContainer
        {
            get => _lineSegmentContainer;
            set => _lineSegmentContainer = value;
        }

        public GameObject LineSegmentPrefab
        {
            get => _lineSegmentPrefab;
            set => _lineSegmentPrefab = value;
        }

        public Color DisabledColor
        {
            get => _disabledColor;
            set => _disabledColor = value;
        }

        public Color ActiveColor
        {
            get => _activeColor;
            set => _activeColor = value;
        }

        public Button CurrentStepButton
        {
            get => _currentStepButton;
            set => _currentStepButton = value;
        }

        public TextMeshProUGUI CurrentStepButtonText
        {
            get => _currentStepButtonText;
            set => _currentStepButtonText = value;
        }

        public Button AllStepsButton
        {
            get => _allStepsButton;
            set => _allStepsButton = value;
        }

        public RectTransform MainRectTransform
        {
            get => _mainRectTransform;
            set => _mainRectTransform = value;
        }

        public Button FullScreenButton
        {
            get => _FullScreenButton;
            set => _FullScreenButton = value;
        }

        public GameObject ScrollViewBodyContent
        {
            get => _scrollViewBodyContent;
            set => _scrollViewBodyContent = value;
        }

        public RectTransform ScrollViewObject
        {
            get => _scrollViewObject;
            set => _scrollViewObject = value;
        }
        public GameObject StepPorgressScrollableContainerObject
        {
            get => _stepPorgressScrollableContainerObject;
            set => _stepPorgressScrollableContainerObject = value;
        }

        public GameObject ProgressBarObject
        {
            get => _progressBarObject;
            set => _progressBarObject = value;
        }
        public TextMeshProUGUI BodyHeaderTitleText
        {
            get => _bodyHeaderTitleText;
            set => _bodyHeaderTitleText = value;
        }
        public GameObject MoreHelpContainer
        {
            get => _MoreHelpContainer;
            set => _MoreHelpContainer = value;
        }

        public GameObject NextBackContainer
        {
            get => _NextBackContainer;
            set => _NextBackContainer = value;
        }

        public CanvasWebViewPrefab MainCanvasWebViewPrefab
        {
            get => _mainCanvasWebViewPrefab;
            set => _mainCanvasWebViewPrefab = value;
        }
        public Button OxiButtonOpen 
        { 
            get => _oxiButtonOpen; 
            set => _oxiButtonOpen = value; 
        }
        public Button OxiButtonClose 
        { 
            get => _oxiButtonClose; 
            set => _oxiButtonClose = value; 
        }
        public TextMeshProUGUI HintsTextEditorMode { get => _hintsTextEditorMode; set => _hintsTextEditorMode = value; }
        public GameObject ScrollViewEditorMode { get => _scrollViewEditorMode; set => _scrollViewEditorMode = value; }
    }
}