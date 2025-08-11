using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// This class takes a reference to the post processing volume in the scene and, if successful, takes a reference to the depth of field effect.
/// </summary>
public class DepthOfFieldController : MonoBehaviour
{
    [field: SerializeField] public float DistanceChangeTime { get; private set; } = 0.5f;
    [SerializeField, Tooltip("The higher the value, the sharper the image. And vice versa")] private float _maxFocusDistance = 1f;
    [SerializeField] private float _disableDepthOfFieldWaitTime = 0.75f;
    private Volume _localVolume;
    private DepthOfField _depthOfField;
    private float _minFocusDistance;

    private Coroutine _setFocusDistanceByTimeCoroutine;
    private Coroutine _disableDOFCoroutine;

    private void Awake()
    {
    }

    private void Start()
    {
        _localVolume = FindObjectOfType<Volume>();
        _localVolume.profile.TryGet(out _depthOfField);
        _minFocusDistance = _depthOfField.focusDistance.min;
    }

    public void SetFocusDistanceToMax()
    {
        GameHelper.RestartCoroutine(this, ref _setFocusDistanceByTimeCoroutine, SetFocusDistanceByTimeCoroutine(_minFocusDistance, _maxFocusDistance));

        GameHelper.RestartCoroutine(this, ref _disableDOFCoroutine, DisableDOFCoroutine());
    }

    public void SetFocusDistanceToMin()
    {
        _depthOfField.active = true;

        GameHelper.RestartCoroutine(this, ref _setFocusDistanceByTimeCoroutine, SetFocusDistanceByTimeCoroutine(_maxFocusDistance, _minFocusDistance));
    }

    IEnumerator SetFocusDistanceByTimeCoroutine(float from, float to)
    {
        float t = 0;

        float distance = from;

        while (t < 1)
        {
            t += Time.deltaTime / DistanceChangeTime;
            distance = Mathf.Lerp(from, to, t);
            SetFocusDistance(distance);
            yield return null;
        }
        SetFocusDistance(to);
    }

    public void SetFocusDistance(float distance)
    {
        _depthOfField.focusDistance.value = distance;
    }

    IEnumerator DisableDOFCoroutine()
    {
        yield return new WaitForSeconds(_disableDepthOfFieldWaitTime);
        _depthOfField.active = false;
    }
}