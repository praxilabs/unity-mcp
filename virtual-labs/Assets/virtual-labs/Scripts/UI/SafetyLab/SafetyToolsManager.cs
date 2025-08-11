using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafetyToolsManager : Singleton<SafetyToolsManager>
{
    [HideInInspector] public List<SafetyTool> equippableSafetyTools;
  
    [SerializeField] private Button _goToLabButton;
    [SerializeField] private Button _skipButton;
    private int _equippedToolsCounter = 0;
    public Button GoToLabButton => _goToLabButton;

    private void OnEnable()
    {
        _skipButton.onClick.AddListener(SkipTutorial);
    }

    private void OnDestroy()
    {
        _skipButton.onClick.RemoveListener(SkipTutorial);
    }

    protected override void Awake()
    {
        _addToDontDestroyOnLoad = false;
        base.Awake();
    }

    public void UpdateCounter()
    {
        _equippedToolsCounter++;
        if (_equippedToolsCounter == equippableSafetyTools.Count)
        {
            _goToLabButton.interactable = true;
            _skipButton.interactable = false;
        }
    }

    public void UpdateGoToLabState()
    {
        if (AreAllToolsEquipped())
            EnableGoToLabButton();
    }

    private bool AreAllToolsEquipped()
    {
        return _equippedToolsCounter == equippableSafetyTools.Count;
    }

    private void EnableGoToLabButton()
    {
        _goToLabButton.interactable = true;
        _skipButton.interactable = false;
    }

    private void SkipTutorial()
    {
        foreach (SafetyTool tool in equippableSafetyTools)
            tool.EquipRightTool();

        _goToLabButton.interactable = true;
        _skipButton.interactable = false;
    }
}