using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for the loading screen animations. It's responsible for spinning the quarter circle sprite and 
/// for moving the dots up/down in the sequence specified by the UI/UX team.
/// </summary>

public class LoadingScreenAnimationController : MonoBehaviour
{
    [SerializeField] private RectTransform _spinnerPivot;
    [SerializeField] private List<RectTransform> _dots;
    [SerializeField] private float _rotationSpeed = 30f;
    [SerializeField] private float _dotAnimationTime;
    [SerializeField] private float _dotMaxYPostion;
    [SerializeField] private float _dotMinYPostion;
    [SerializeField] private float _dotMaxAlpha;
    [SerializeField] private float _dotMinAlpha;

    [SerializeField, ReadOnly] private List<Image> _dotSprites;

    private Coroutine _dotsAnimationCycleCoroutine;
    private Coroutine _spinnerAnimationCoroutine;
    private RectTransform _dot;
    private Image _dotSprite;
    private float _halfTime;

    private void Start()
    {
        _dotSprites = new List<Image>();
        foreach (var dot in _dots)
        {
            _dotSprites.Add(dot.GetComponent<Image>());
        }

        _halfTime = _dotAnimationTime / 2;
    }

    public void StartLoadingScreenAnimations()
    {
        StartDotsAnimationCycle(0);
        StartSpinnerAnimation();
    }
    public void StopLoadingScreenAnimations()
    {
        StopDotsAnimation();
        StopSpinnerAnimation();
    }

    void StartDotsAnimationCycle(int index)
    {
        GameHelper.RestartCoroutine(this, ref _dotsAnimationCycleCoroutine, DotsAnimationCycleCoroutine(index));
    }

    void StartSpinnerAnimation()
    {
        GameHelper.RestartCoroutine(this, ref _spinnerAnimationCoroutine, SpinnerAnimationCoroutine());
    }

    /// <summary>
    /// This coroutine works recursively. It takes the index of the sprite that moves up then back down. Once the coroutine is done
    /// it calls itself with the next index. The index is reset to 0 when it reaches the end of the list.
    /// </summary>
    IEnumerator DotsAnimationCycleCoroutine(int i)
    {
        _dot = _dots[i];
        _dotSprite = _dotSprites[i];

        float t = 0f;

        // Move sprite up
        while (t < 1)
        {
            SetImageYPos(_dot, _dotMaxYPostion, t);
            SetImageAlpha(_dotSprite, _dotMaxAlpha, t);
            t += Time.deltaTime / _halfTime;
            yield return null;
        }

        SetImageYPos(_dot, _dotMaxYPostion, 1);
        SetImageAlpha(_dotSprite, _dotMaxAlpha, 1);

        t = 0;

        // Move sprite down
        while (t < 1)
        {
            SetImageYPos(_dot, _dotMinYPostion, t);
            SetImageAlpha(_dotSprite, _dotMinAlpha, t);
            t += Time.deltaTime / _halfTime;
            yield return null;
        }

        SetImageYPos(_dot, _dotMinYPostion, 1);
        SetImageAlpha(_dotSprite, _dotMinAlpha, 1);

        i++;
        int dotCount = _dots.Count;

        // If the value of i returns to zer0, wait for twice the animation time before restarting the animation cycle.
        if (i % dotCount == 0)
        {
            yield return new WaitForSeconds(_dotAnimationTime * 2);
        }

        // Using the % operator ensures that the index is always within the bounds of the list
        StartDotsAnimationCycle(i % dotCount);
    }

    IEnumerator SpinnerAnimationCoroutine()
    {
        if (_spinnerPivot != null)
        {
            while (true)
            {
                _spinnerPivot.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }
    }

    void SetImageAlpha(Image image, float targetAlpha, float t)
    {
        Color c = image.color;
        float alpha = c.a;

        c.a = Mathf.Lerp(alpha, targetAlpha, t);
        image.color = c;
    }

    void SetImageYPos(RectTransform dot, float targetYPos, float t)
    {
        Vector2 pos = dot.anchoredPosition;
        float y = pos.y;

        pos.y = Mathf.Lerp(y, targetYPos, t);
        dot.anchoredPosition = pos;
    }

    void StopDotsAnimation()
    {
        GameHelper.StopAndNullifyCoroutine(this, ref _dotsAnimationCycleCoroutine);
    }

    void StopSpinnerAnimation()
    {
        GameHelper.StopAndNullifyCoroutine(this, ref _spinnerAnimationCoroutine);
    }
}
