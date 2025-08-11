using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EnhancedKnobController : MonoBehaviour
{
    public event Action<float> OnKnobRotation;
    public event System.Action OnViolateIgnore;
    public UnityEvent<bool> OnKnobInteractabilityChanged;

    public Transform knob;
    public enum RotationAxis { X, Y, Z }
    public enum Direction { ClockWise, CounterClockWise, NotRotated }
    public Direction direction;

    [Header("Knob Settings")]
    [SerializeField] private RotationAxis _rotationAxis = RotationAxis.Z;
    [field: SerializeField] public NumericalRange AngleRange { get; private set; }
    [SerializeField] private bool _clampRotation;
    [SerializeField] private float _startingAngle;
    [SerializeField] private float _stepAngle;
    [field: SerializeField] public bool IgnoreInput { get; set; }
    [field: SerializeField] public PredefinedAnglesData PredefinedAnglesData { get; set; } = new();

    [Header("Knob Rotation Data")]
    [Space(15)]
    [SerializeField] private float _actualAngle;
    [field: SerializeField] public float AccumulatedAngle { get; private set; }
    [SerializeField] private bool _invertRotationPercentage;
    [field: SerializeField] public float RotationPercentage { get; private set; }

    private Camera _mainCamera;
    private float _angleOffset;
    private float _previousAngle;
    private Quaternion _initialRotation;
    private bool _isOverUI = false;

    public bool canRotateFromStep = false;

    private Vector3 MouseToKnobPosition
    {
        get
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 mousePosition3D = new Vector3(mousePosition.x, mousePosition.y, 0f);
            Vector3 knobScreenPos = _mainCamera.WorldToScreenPoint(knob.position);
            
            Vector3 difference = mousePosition3D - new Vector3(knobScreenPos.x, knobScreenPos.y, 0f);
            
            return _rotationAxis switch
            {
                RotationAxis.X => Quaternion.Inverse(_initialRotation) * new Vector3(difference.x, difference.y, 0f),
                RotationAxis.Y => Quaternion.Inverse(_initialRotation) * new Vector3(difference.x, 0f, difference.y),
                _ => difference // Z-axis (default)
            };
        }
    }

    private float _inputAngle;
    public float InputAngle
    {
        get
        {
            Vector3 relativePos = MouseToKnobPosition;
            _inputAngle = _rotationAxis == RotationAxis.Y
                ? Mathf.Atan2(relativePos.z, relativePos.x) * Mathf.Rad2Deg + _angleOffset
                : Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg + _angleOffset;
            return _inputAngle;
        }
        set { _inputAngle = value; }
    }

    private Vector3 GetRotationAxis()
    {
        return _rotationAxis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            _ => Vector3.forward // Z-axis (default)
        };
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _initialRotation = knob.rotation;
        AccumulatedAngle = _startingAngle;
        SetInitialRotation(_startingAngle);
    }

    public void UpdateKnobRotationData(float accumulatedAngle)
    {
        this.AccumulatedAngle = accumulatedAngle;
        RotateKnob();
        CalculateRotationPercentage();
        SetActualAngle();
        OnKnobRotation?.Invoke(RotationPercentage);
    }

    public void UpdateRotationPercentage(float newValue)
    {
        // Store the new percentage
        RotationPercentage = newValue;

        // Calculate the corresponding angle based on the percentage
        float angle = Mathf.Lerp(AngleRange.min, AngleRange.max, RotationPercentage);

        // Set the knob rotation to the calculated angle
        UpdateKnobRotationData(angle); // This calls SetKnobRotation indirectly
    }

    public void UpdateKnobRange(float min, float max)
    {
        AngleRange.min = min;
        AngleRange.max = max;
    }

    public void SetInitialRotation(float angle)
    {
        Vector3 rotationAxis = GetRotationAxis();
        knob.rotation = _initialRotation * Quaternion.AngleAxis(angle, rotationAxis);
        CalculateRotationPercentage();

        if (AngleRange.max > 0)
        {
            OnKnobRotation?.Invoke(1 - (_startingAngle / AngleRange.max));
        }
    }

    public void SetKnobInteractability(bool isInteractable)
    {
        IgnoreInput = !isInteractable;
        OnKnobInteractabilityChanged?.Invoke(isInteractable);
    }

    public void HandleKnobStepExecution()
    {
        if (canRotateFromStep)
            ExecuteKnobStep();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) _isOverUI = true;

        if (canRotateFromStep)
            StopKnobStepFlashing();

        if (IgnoreInput || _isOverUI)
        {
            OnViolateIgnore?.Invoke();
            return;
        }

        CalculateAngleOffset();
        SetActualAngle();
        _previousAngle = _actualAngle;
    }

    private void OnMouseDrag()
    {
        if (IgnoreInput || _isOverUI) return;

        float inputAngle = InputAngle;

        if (Mathf.Abs(_stepAngle) > 0)
        {
            _stepAngle = Mathf.Abs(_stepAngle);
            inputAngle -= inputAngle % _stepAngle;
        }

        float deltaAngle = Mathf.DeltaAngle(_previousAngle, inputAngle);
        
        SetDirection(_previousAngle, inputAngle);
        AccumulatedAngle += deltaAngle;
        _previousAngle = inputAngle;

        UpdateKnobRotationData(AccumulatedAngle);
    }

    private void OnMouseUp()
    {
        if(PredefinedAnglesData.angles != null && PredefinedAnglesData.angles.Length > 0)
            SmoothRotateToNearestAngle();
        else
            HandleKnobStepExecution();

        _isOverUI = false;
    }

    private void CalculateAngleOffset()
    {
        Vector3 relativePos = MouseToKnobPosition;
        float mouseAngle = _rotationAxis == RotationAxis.Y
            ? Mathf.Atan2(relativePos.z, relativePos.x) * Mathf.Rad2Deg
            : Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        
        Quaternion relativeRot = Quaternion.Inverse(_initialRotation) * knob.rotation;
        Vector3 rightVector = relativeRot * Vector3.right;
        
        float currentKnobAngle = _rotationAxis == RotationAxis.Y
            ? Mathf.Atan2(rightVector.z, rightVector.x) * Mathf.Rad2Deg
            : Mathf.Atan2(rightVector.y, rightVector.x) * Mathf.Rad2Deg;
        
        _angleOffset = currentKnobAngle - mouseAngle;
    }

    private void SetActualAngle()
    {
        Quaternion relativeRotation = Quaternion.Inverse(_initialRotation) * knob.rotation;
        Vector3 angles = relativeRotation.eulerAngles;
        
        _actualAngle = _rotationAxis switch
        {
            RotationAxis.X => angles.x,
            RotationAxis.Y => angles.y,
            _ => angles.z
        };
    }

    private void RotateKnob()
    {
        if (_clampRotation)
        {
            AccumulatedAngle = Mathf.Clamp(AccumulatedAngle, AngleRange.min, AngleRange.max);
        }

        Vector3 rotationAxis = GetRotationAxis();
        knob.rotation = _initialRotation * Quaternion.AngleAxis(AccumulatedAngle, rotationAxis);
    }

    private void CalculateRotationPercentage()
    {
        float percentage = (AngleRange.max - AccumulatedAngle) / (AngleRange.max - AngleRange.min);

        RotationPercentage = _invertRotationPercentage ? 1 - percentage : percentage;
        RotationPercentage = Mathf.Clamp(RotationPercentage, 0, 1f);
    }

    private void SetDirection(float previousAngle, float nextAngle)
    {
        if (nextAngle > previousAngle)
            direction = Direction.CounterClockWise;
        else if (nextAngle < previousAngle)
            direction = Direction.ClockWise;
        else
            direction = Direction.NotRotated;
    }

    private float CalculateNearestPredefinedAngle()
    {
        float nearestAngle = PredefinedAnglesData.angles
            .OrderBy(angle => Mathf.Abs(AccumulatedAngle - angle))
            .FirstOrDefault();

        return nearestAngle;
    }

    private void SmoothRotateToNearestAngle()
    {
        float targetAngle = CalculateNearestPredefinedAngle();

        StartCoroutine(SmoothRotateCoroutine(targetAngle));
    }

    private IEnumerator SmoothRotateCoroutine(float targetAngle)
    {
        float currentAngle = AccumulatedAngle;
        float elapsedTime = 0f;

        while (elapsedTime < PredefinedAnglesData.smoothRotateDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, elapsedTime / PredefinedAnglesData.smoothRotateDuration);

            UpdateKnobRotationData(newAngle);
            yield return null;
        }
        UpdateKnobRotationData(targetAngle);
        HandleKnobStepExecution();
    }

    private void ExecuteKnobStep()
    {
        if (XnodeManager.Instance.CurrentStep is IProvideActionSteps)
            BranchingExecute();
        else if (XnodeManager.Instance.CurrentStep is RotateKnobStep)
            SoloExecute();
    }

    private void BranchingExecute()
    {
        if (CheckAvailableSteps())
            XnodeManager.Instance.CurrentStep.Execute();
    }

    private void SoloExecute()
    {
        RotateKnobStep knobStep = (RotateKnobStep)XnodeManager.Instance.CurrentStep;
        knobStep.Execute();
    }

    private bool CheckAvailableSteps()
    {
        for (int i = 0; i < XnodeStepsRunner.Instance.availableSteps.Count; i++)
        {
            if (XnodeStepsRunner.Instance.availableSteps[i] is RotateKnobStep)
            {
                RotateKnobStep knobStep = (RotateKnobStep)XnodeStepsRunner.Instance.availableSteps[i];

                if (knobStep.knobAngle == AccumulatedAngle)
                {
                    XnodeManager.Instance.CurrentStep = knobStep;
                    return true;
                }
            }
        }
        return false;
    }

    private void StopKnobStepFlashing()
    {
        if (XnodeManager.Instance.CurrentStep is IProvideActionSteps)
            StopBranchFlashing();
        else if (XnodeManager.Instance.CurrentStep is RotateKnobStep)
            StopSoloFlashing();
    }

    private void StopBranchFlashing()
    {
        for (int i = 0; i < XnodeStepsRunner.Instance.availableSteps.Count; i++)
        {
            if (XnodeStepsRunner.Instance.availableSteps[i] is RotateKnobStep)
            {
                RotateKnobStep knobStep = (RotateKnobStep)XnodeStepsRunner.Instance.availableSteps[i];
                knobStep.StopFlashing();
            }
        }
    }

    private void StopSoloFlashing()
    {
        RotateKnobStep knobStep = (RotateKnobStep)XnodeManager.Instance.CurrentStep;
        knobStep.StopFlashing();
    }
}

[Serializable]
public class NumericalRange
{
    public float min;
    public float max;
}
[Serializable]
public class PredefinedAnglesData
{
    public float[] angles;// List of angles to snap to
    public float smoothRotateDuration = 0.3f;
}