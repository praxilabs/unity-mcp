using UnityEngine;
using UnityEngine.EventSystems;

public class PushButtonSwitch : MonoBehaviour
{
    /// <summary>
    /// The Action doesn't pass a value in the event there's a function that listens for the event but doesn't have parameters
    /// </summary>
    public event System.Action OnButtonPushed;

    public Collider buttonColider;
    public bool ignoreMouseInput;
    [field: SerializeField] public bool On { get; set; }
    [SerializeField] private GameEventScriptableObject _onButtonPushedGameEvent;
    [SerializeField] private float _pressedPosition;
    [SerializeField] private float _onPosition;
    [SerializeField] private float _offPosition;
    [SerializeField] private Transform _button;
    [SerializeField] private Vector3 _buttonDirection = Vector3.forward;
    private ClickableTool _clickableTool;
    private bool _onButtonChanged;

    void Start()
    {
        _clickableTool = GetComponent<ClickableTool>();
        SetButtonReleasedPosition();
    }

    private void OnMouseDown()
    {
        if (ignoreMouseInput || EventSystem.current.IsPointerOverGameObject())
            return;

        On = !On;
        _button.localPosition = _buttonDirection * _pressedPosition;

        // OnMouseUpLogic
        OnButtonPushed?.Invoke();
        _onButtonPushedGameEvent?.RaiseEvent(On);
        _onButtonChanged = true;
    }

    private void OnMouseUp()
    {
        if(_onButtonChanged)
        {
            SetButtonReleasedPosition();
            _onButtonChanged = false;
        }
    }

    public void ToggleIgnoreInput(bool enableInput)
    {
        ignoreMouseInput = !enableInput;
        buttonColider.enabled = enableInput;
    }

    public void ListenOnDropdownValueChanged(object data)
    {
        int dropdownIndex = (int) data;
        SetButtonFromDeviceMenu(dropdownIndex);
        // To execute knob step if possible
        if(_clickableTool)
        {
            _clickableTool.HandleClickStepExecution(false);
        }
    }

    private void SetButtonFromDeviceMenu(int index)
    {
        if(index != 0 && index != 1) return;
        // Off = 0, On = 1
        On = index == 1;

        OnButtonPushed?.Invoke();
        SetButtonReleasedPosition();
    }

    private void SetButtonReleasedPosition()
    {
        float pos = On ? _onPosition : _offPosition;
        _button.localPosition = _buttonDirection * pos;
    }
}