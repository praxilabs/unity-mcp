using System.Collections;
using UltimateClean;
using UnityEngine;
using UnityEngine.UI;

public class ExploreHintItem : MonoBehaviour
{
    [field: SerializeField, Header("Target Object")] public GameObject FlashableObject {get; private set;}
    [field: SerializeField, Header("Elements"), Space] public CleanButton HintBtn {get; private set;}
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private ExploreHintPopupWindow _hintPopupWindow;
    [Header("Settings")]
    [SerializeField] private Sprite _checkMark;
    [SerializeField] private Color _unopenedColor;
    [SerializeField] private Color _openedColor;
    private bool _isFirstTime = true;
    private float _initialFlashingDelay;

    private UnityEngine.Events.UnityAction _hintBtnAction;
    private Hologram _flashableObjectHologram;
    private Coroutine _flashingCoroutine;
    private WaitForSeconds _flashingDuration = new(FLASHING_DURATION);
    private const float FLASHING_DURATION = 4f;

    public void Setup(ExplorationHintsContainer explorationHintsContainer, string hintText)
    {
        _hintBtnAction = HandleButtonClicked;
        HintBtn.onClick.AddListener(_hintBtnAction);
        _hintPopupWindow.Setup(explorationHintsContainer, ResetFlashing, hintText);
    }

    public void SetHintText(string hintText)
    {
        _hintPopupWindow.SetHintText(hintText);
        _hintPopupWindow.SetHintsCountText();
    }

    public void ToggleHintBtn(bool enable)
    {
        HintBtn.gameObject.SetActive(enable);
    }

    private void HandleButtonClicked()
    {
        _hintPopupWindow.TogglePopupWindow(true);
        if(_isFirstTime)
        {
            _isFirstTime = false;
            UpdateHintBtnVisuals();
            Flashing();
        }
    }

    private void UpdateHintBtnVisuals()
    {
        _backgroundImage.color = _openedColor;
        _iconImage.sprite = _checkMark;
    }

    private void Flashing()
    {
        if(FlashableObject)
        {
            _flashableObjectHologram = FlashableObject.GetComponentInParent<Hologram>();
            if(_flashableObjectHologram == null) return;
            _initialFlashingDelay = _flashableObjectHologram.PulseDelay;

            _flashingCoroutine = StartCoroutine(FlashingCoroutine());
        }
    }

    private IEnumerator FlashingCoroutine()
    {
        AdjustFlashingDelay(0f);
        StartFlashing();
        yield return _flashingDuration;
        StopFlashing();
        AdjustFlashingDelay(_initialFlashingDelay);
    }

    private void StartFlashing()
    {
        ToolsFlashManager.Instance.flashingTools.Add(FlashableObject);
        ToolsFlashManager.Instance.StartFlashing(FlashableObject);
    }

    private void StopFlashing()
    {
        ToolsFlashManager.Instance.StopFlashing(FlashableObject);
        ToolsFlashManager.Instance.flashingTools.Remove(FlashableObject);
    }

    public void ResetFlashing()
    {
        if(_flashingCoroutine != null)
            StopCoroutine(_flashingCoroutine);
        StopFlashing();
        if(_flashableObjectHologram)
            AdjustFlashingDelay(_initialFlashingDelay);
    }

    private void AdjustFlashingDelay(float value)
    {
        _flashableObjectHologram.PulseDelay = value;
    }
}
