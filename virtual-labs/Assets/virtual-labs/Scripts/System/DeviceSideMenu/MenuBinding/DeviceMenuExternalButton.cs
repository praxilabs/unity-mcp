using CursorStates;
using Praxilabs.DeviceSideMenu;
using UltimateClean;
using UnityEngine;
using UnityEngine.UI;

public class DeviceMenuExternalButton : MonoBehaviour
{
    [field: SerializeField] public CleanButton Button {get; private set;}
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Color _interactableColor;
    [SerializeField] private Color _notInteractableColor;
    private DeviceMenu _deviceMenu;
    private CursorStateHandler _cursorStateHandler;

    private void OnEnable()
    {
        if(_deviceMenu == null) return;

        _deviceMenu.SetExternalButtonState(true);
    }

    public void Setup(DeviceMenu deviceMenu)
    {
        _deviceMenu = deviceMenu;

        _cursorStateHandler = GetComponent<CursorStateHandler>();
        RectTransform buttonRectTransform = GetComponent<RectTransform>();
        Vector3 buttonCenter = buttonRectTransform.TransformPoint(buttonRectTransform.rect.center);
        _deviceMenu.SetExternalButtonState(true);
        _deviceMenu.SetExternalButtonPosition(transform.position);
        _deviceMenu.OnVisibilityChanged += HandleMenuVisibilityChanged;
        Button.onClick.AddListener(HandleButtonClicked);
    }

    private void HandleMenuVisibilityChanged(bool isMenuVisible)
    {
        UpdateInteractability(isMenuVisible);
        UpdateVisuals(isMenuVisible);        
    }

    private void UpdateInteractability(bool isMenuVisible)
    {
        Button.interactable = !isMenuVisible;
        _cursorStateHandler.canHandleCursorState = !isMenuVisible;
    }

    private void UpdateVisuals(bool isMenuVisible)
    {
        _buttonImage.color = isMenuVisible ? _notInteractableColor : _interactableColor;
    }

    private void HandleButtonClicked()
    {
        _deviceMenu.ExpandWindow();
    }

    private void OnDisable()
    {
        if(_deviceMenu == null) return;

        _deviceMenu.SetExternalButtonState(false);
    }

    private void OnDestroy()
    {
        _deviceMenu.OnVisibilityChanged -= HandleMenuVisibilityChanged;
        Button.onClick.RemoveListener(HandleButtonClicked);
    }
}
