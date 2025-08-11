using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialsDisplay : MonoBehaviour
{
    [SerializeField] private Button _tutorialIcon;
    [SerializeField] private RectTransform _tutorialWelcomingPanel;

    [Header("Tutorial Card Buttons")]
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _skipButton;
    [SerializeField] private Button _finishButton;

    [Header("Beginner Guide Buttons")]
    [SerializeField] private Button _closeGuide;
    
    [Header("Congratulations After Module")]
    [SerializeField] private GameObject _congratulationsAfterModuleUI;
    [SerializeField] private Button _skipFromModuleButton;
    [SerializeField] private Button _continueLearningNextModuleButton;
    [SerializeField] private TextMeshProUGUI _currentModuleCongratsMessage;
    [SerializeField] private TextMeshProUGUI _nextModuleMessage;

    [Header("Final Congratulations After Finish")]
    [SerializeField] private GameObject _congratulationsFullFinished;

    [Header("Inner Module Text")]
    [SerializeField] private TextMeshProUGUI _innerTitleText;
    [SerializeField] private TextMeshProUGUI _innerBodyText;

    [Header("Outer Card Text")]
    [SerializeField] private TextMeshProUGUI _outerTitleText;
    [SerializeField] private TextMeshProUGUI _outerProgressText;

    [Header("Other")]
    [SerializeField] private GameObject _exploringInterfaceUI;
    [SerializeField] private GameObject _beginnerGuideInterface;
    [SerializeField] private GameObject _beginnerGuideScrollContent;
    [SerializeField] private GameObject _beginnerGuideItemPrefab;
    [SerializeField] private RectTransform _arrowDirectionObject;
    [SerializeField] private Image _tutorialSubImage;
    [HideInInspector][SerializeField] private ExperimentManager _experimentManager;

    public Button TutorialIcon { get => _tutorialIcon; set => _tutorialIcon = value; }
    public Button NextButton { get => _nextButton; set => _nextButton = value; }
    public Button SkipButton { get => _skipButton; set => _skipButton = value; }
    public Button FinishButton { get => _finishButton; set => _finishButton = value; }
    public GameObject CongratulationsAfterModuleUI { get => _congratulationsAfterModuleUI; set => _congratulationsAfterModuleUI = value; }
    public Button SkipFromModuleButton { get => _skipFromModuleButton; set => _skipFromModuleButton = value; }
    public Button ContinueLearningNextModuleButton { get => _continueLearningNextModuleButton; set => _continueLearningNextModuleButton = value; }
    public TextMeshProUGUI CurrentModuleCongratsMessage { get => _currentModuleCongratsMessage; set => _currentModuleCongratsMessage = value; }
    public TextMeshProUGUI NextModuleMessage { get => _nextModuleMessage; set => _nextModuleMessage = value; }
    public GameObject CongratulationsFullFinished { get => _congratulationsFullFinished; set => _congratulationsFullFinished = value; }
    public TextMeshProUGUI InnerTitleText { get => _innerTitleText; set => _innerTitleText = value; }
    public TextMeshProUGUI InnerBodyText { get => _innerBodyText; set => _innerBodyText = value; }
    public TextMeshProUGUI OuterTitleText { get => _outerTitleText; set => _outerTitleText = value; }
    public TextMeshProUGUI OuterProgressText { get => _outerProgressText; set => _outerProgressText = value; }
    public GameObject TutorialCard { get => _exploringInterfaceUI; set => _exploringInterfaceUI = value; }
    public GameObject BeginnerGuideInterface { get => _beginnerGuideInterface; set => _beginnerGuideInterface = value; }
    public GameObject BeginnerGuideScrollContent { get => _beginnerGuideScrollContent; set => _beginnerGuideScrollContent = value; }
    public GameObject BeginnerGuideItemPrefab { get => _beginnerGuideItemPrefab; set => _beginnerGuideItemPrefab = value; }
    public RectTransform ArrowDirectionObject { get => _arrowDirectionObject; set => _arrowDirectionObject = value; }
    public Image TutorialSubImage { get => _tutorialSubImage; set => _tutorialSubImage = value; }
    public ExperimentManager ExperimentManager { get => _experimentManager; set => _experimentManager = value; }
    public Button CloseGuide { get => _closeGuide; set => _closeGuide = value; }
    public RectTransform TutorialWelcomingPanel { get => _tutorialWelcomingPanel; set => _tutorialWelcomingPanel = value; }
}
