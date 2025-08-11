using System.Collections;
using UltimateClean;
using UnityEngine;
using UnityEngine.UI;
using static SideMessagesManager;

public class SafetyTool : MonoBehaviour
{
    public bool isEquippable;
    public GameObject equippedImage;
    [SerializeField] private string _toolName;
    [SerializeField] private CleanButton _toolButton;
    [SerializeField] private Image _equippedImage;
    [SerializeField] private Image _errorImage;
    [SerializeField] private Vector2 _errorMessageOffset = new Vector2(0, -145f);
    private void OnEnable()
    {
        _toolButton.onClick.AddListener(ValidateTool);
    }

    private void OnDisable()
    {
        _toolButton.onClick.RemoveListener(ValidateTool);
    }

    private void ValidateTool()
    {
        if (SafetyToolsManager.Instance.equippableSafetyTools.Contains(this))
            EquipRightTool();
        else
            EquipWrongTool();
    }

    public void EquipRightTool()
    {
        _equippedImage.gameObject.SetActive(true);
        equippedImage.SetActive(true);

        SafetyToolsManager.Instance.UpdateCounter();
        SafetyToolsManager.Instance.UpdateGoToLabState();
    }

    public void EquipWrongTool()
    {
        StartCoroutine(WrongToolCoroutine());
    }

    private IEnumerator WrongToolCoroutine()
    {
        _errorImage.gameObject.SetActive(true);
        LogWrongToolMessage();
        yield return new WaitForSeconds(5f);

        _errorImage.gameObject.SetActive(false);
    }

    private void LogWrongToolMessage()
    {
        NotificationType animationType = NotificationType.SlideRightAndFade;

        SideMessageData messageData = new SideMessageData()
        {
            Title = "Not Required",
            Message = $"{_toolName} is not needed for this experiment.",
            Animation = animationType,
            Duration = 5
        };

        CreateSideMessages.CreateSideMessage(MessageType.Error, messageData, _errorMessageOffset);
    }
}