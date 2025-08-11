using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    private Image fadeImage;
    private float _fadeProgress = 0f;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public IEnumerator StartFade(float duration)
    {
        _fadeProgress = 0f;

        while (_fadeProgress < duration)
        {
            _fadeProgress += Time.deltaTime;

            fadeImage.color = new Color(0f, 0f, 0f, _fadeProgress / duration);

            yield return null;
        }
    }

    public IEnumerator EndFade(float duration)
    {
        _fadeProgress = duration;

        while (_fadeProgress > 0)
        {
            _fadeProgress -= Time.deltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, _fadeProgress / duration);

            yield return null;
        }
    }
}