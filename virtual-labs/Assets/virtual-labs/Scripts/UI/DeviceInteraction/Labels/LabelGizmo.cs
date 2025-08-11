using System;
using System.Collections;
using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LabelGizmo : MonoBehaviour
{
    public int labelID;

    [SerializeField] private LabelDirection _labelDirection;
    [SerializeField] private LabelColor _labelColor;

    [SerializeField] private LabelColorPrefab _labelUp;
    [SerializeField] private LabelColorPrefab _labelDown;
    [SerializeField] private LabelColorPrefab _labelLeft;
    [SerializeField] private LabelColorPrefab _labelRight;

    [Tooltip("This is set automatically when you click Instantiate label in inspector")]
    [SerializeField] private RectTransform _label;

    private LabelsController _labelsController;
    private CleanButton _labelGizmoButton;
    private float _offset;
    private bool _showLabel = false;
    private bool _canUpdatePosition = false;
    private UnityAction _closeClickDelegate;

    private void Start()
    {
        _labelGizmoButton = this.GetComponent<CleanButton>();
        _labelsController = transform.GetComponentInParent<LabelsController>();
        _closeClickDelegate = () => ToggleLabel(!_showLabel);

        //added event in start because adding it in OnEnable causes bugs,
        //because this gameObject is enabled/disabled a lot
        _labelGizmoButton.onClick.AddListener(_closeClickDelegate);        
    }
 
    private void OnDestroy()
    {
        _labelGizmoButton.onClick.RemoveListener(_closeClickDelegate);
    }

    private void Update()
    {
        if (_canUpdatePosition)
            UpdatePosition();
    }

    public void UpdateText(string labelName)
    {
        TMP_Text text = _label.GetComponentInChildren<TMP_Text>();
        text.text = labelName;
    }

    public void ToggleLabel(bool setActive)
    {
        _showLabel = setActive;

        if (_showLabel)
        {
            _canUpdatePosition = true;
            _labelsController.ClosePreviousLabel(_label.gameObject);
        }
        else
            _canUpdatePosition = false;

        _label.gameObject.SetActive(_showLabel);
    }

    public void InstantiateLabels()
    {
        switch (_labelDirection)
        {
            case LabelDirection.Up:
                InstantiateLabelbyColor(_labelUp);
                break;
            case LabelDirection.Down:
                InstantiateLabelbyColor(_labelDown);
                break;
            case LabelDirection.Left:
                InstantiateLabelbyColor(_labelLeft);
                break;
            case LabelDirection.Right:
                InstantiateLabelbyColor(_labelRight);
                break;
        }
    }

    public void DestroyLabel()
    {
        if ( _label != null )
            DestroyImmediate(_label.gameObject);
    }

    private void InstantiateLabelbyColor(LabelColorPrefab labelColorPrefab)
    {
        if (_label != null)
            return;

        if (_labelColor == LabelColor.White)
            InstantiateLabel(labelColorPrefab.whiteLabel);
        else
            InstantiateLabel(labelColorPrefab.blackLabel);
    }

    private void InstantiateLabel(GameObject labelDirectionPrefab)
    {
        _label = Instantiate(labelDirectionPrefab, transform).GetComponent<RectTransform>();
        StartCoroutine(PositionLabel());
    }

    private IEnumerator PositionLabel()
    {
        yield return new WaitForSeconds(0.5f);

        _label.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        UpdatePosition();
        _label.gameObject.SetActive(false);
    }
   
    private void UpdatePosition()
    {
        Vector2 localPos = Vector2.zero;
        float fixedMargin = 50f; // The minimum gap you want between the two UI elements

        switch (_labelDirection)
        {
            case LabelDirection.Up:
                // Use the dynamic height + fixed margin to avoid overlap
                _offset = _label.rect.height / 2 + fixedMargin;
                localPos += new Vector2(0, _offset);
                break;
            case LabelDirection.Down:
                _offset = _label.rect.height / 2 + fixedMargin;
                localPos += new Vector2(0, -_offset);
                break;
            case LabelDirection.Left:
                // Use the dynamic width + fixed margin to avoid overlap
                _offset = _label.rect.width / 2 + fixedMargin;
                localPos += new Vector2(-_offset, 0);
                break;
            case LabelDirection.Right:
                _offset = _label.rect.width / 2 + fixedMargin;
                localPos += new Vector2(_offset, 0);
                break;
        }

        _label.localPosition = localPos;
    }
}

[Serializable]
public class LabelColorPrefab
{
    public GameObject whiteLabel;
    public GameObject blackLabel;
}

public enum LabelColor
{
    White, Black
}

public enum LabelDirection
{
    Up, Down, Left, Right
}