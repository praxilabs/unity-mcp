using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hologram : MonoBehaviour
{
    [field: SerializeField] public float PulseDelay {get; set;} = 5f;
    [SerializeField] private float pulseSpeed;
    [SerializeField] private List<Renderer> renderers;

    private bool _pulsate;

    public bool IsFlashing => _pulsate;

    public void SetSetAlphaMultiplierWithTime(float transitionTime, float alphaMultiplier)
    {
        StopAllCoroutines();
        StartCoroutine(SetAlphaMultiplierCoroutine(transitionTime, alphaMultiplier));
    }

    public void SetAlphaMultiplier(float alphaMultiplier)
    {
        foreach (Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].SetFloat("_AlphaMultiplier", alphaMultiplier);
            }
        }
    }

    IEnumerator SetAlphaMultiplierCoroutine(float transitionTime, float targetAlpha)
    {
        float t = 0;
        float currentAlpha = renderers[0].material.GetFloat("_AlphaMultiplier");
        float alpha = 0;

        while (t < 1)
        {
            t += Time.deltaTime / transitionTime;
            alpha = Mathf.Lerp(currentAlpha, targetAlpha, t);
            SetAlphaMultiplier(alpha);
            yield return null;
        }

        SetAlphaMultiplier(targetAlpha);
    }

    IEnumerator PulseCoroutine()
    {
        float alpha = 0;
        yield return new WaitForSeconds(PulseDelay);

        while (_pulsate)
        {
            if (!ToolsFlashManager.Instance.canFlash)
            {
                SetAlphaMultiplier(0);
            }
            else
            {
                alpha = Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));
                SetAlphaMultiplier(alpha);
            }
            yield return null;
        }
    }

    public void StartPulsating()
    {
        _pulsate = true;
        StartCoroutine(PulseCoroutine());
    }
    public void StopPulsating()
    {
        _pulsate = false;
        SetAlphaMultiplier(0);
    }

    public void SetRendersColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].color = color;
            }
        }
    }

    [ContextMenu("Collect Renderers From Children")]
    public void CollectRenderersFromChildren()
    {
        CollectRenderersFromChildren(true);
    }

    public void CollectRenderersFromChildren(bool includeInactive)
    {
        Renderer[] found = GetComponentsInChildren<Renderer>(includeInactive);

        if (renderers == null)
        {
            renderers = new List<Renderer>(found.Length);
        }
        else
        {
            renderers.Clear();
        }

        renderers.AddRange(found);
    }
}