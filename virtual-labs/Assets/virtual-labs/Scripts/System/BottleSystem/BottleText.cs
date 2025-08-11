using System.Collections;
using TMPro;
using UnityEngine;

public class BottleText : MonoBehaviour
{
    [Header("Render Texture Settings")]
    public int textureWidth = 2048;
    public int textureHeight = 1024;

    [Header("UI Setup")]
    [SerializeField] private Camera _textRenderCamera;
    [SerializeField] private Canvas _textCanvas;
    [SerializeField] private TextMeshProUGUI _bottleTextUI;

    [Header("Label")]
    [SerializeField] private GameObject _bottleLabel;

    private RenderTexture _renderTexture;

    public void ConstructText(BottleTextInfo bottleText)
    {
        UpdateText(bottleText);
        SetupRenderTexture();
        _bottleTextUI.ForceMeshUpdate();
    }

    private void SetupRenderTexture()
    {
        if (_renderTexture != null)
        {
            _renderTexture.Release();
            DestroyImmediate(_renderTexture);
        }

        _renderTexture = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32);
        _renderTexture.filterMode = FilterMode.Point;
        _renderTexture.antiAliasing = 8;
        _renderTexture.Create();

        _textRenderCamera.targetTexture = _renderTexture;
        _textRenderCamera.forceIntoRenderTexture = true;

        if (_bottleLabel != null)
        {
            var renderer = _bottleLabel.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.mainTexture = _renderTexture;
                renderer.material.mainTextureScale = Vector2.one;
                renderer.material.mainTextureOffset = Vector2.zero;
                renderer.material.renderQueue += 1;
            }
        }
    }

    public void UpdateText(BottleTextInfo bottleText)
    {
        _bottleTextUI.text = bottleText.text;
        _bottleTextUI.font = bottleText.font;
        _bottleTextUI.fontSize = bottleText.fontSize;
        _bottleTextUI.color = bottleText.color;

        _bottleTextUI.fontStyle = bottleText.fontStyle;
        _bottleTextUI.enableAutoSizing = bottleText.enableAutoSizing;
        _bottleTextUI.fontSizeMin = bottleText.minFontSize;
        _bottleTextUI.fontSizeMax = bottleText.maxFontSize;

        _bottleTextUI.alignment = bottleText.alignment;
        _bottleTextUI.enableWordWrapping = bottleText.enableWordWrapping;
        _bottleTextUI.overflowMode = bottleText.overflowMode;

        _bottleTextUI.characterSpacing = bottleText.characterSpacing;
        _bottleTextUI.wordSpacing = bottleText.wordSpacing;
        _bottleTextUI.lineSpacing = bottleText.lineSpacing;
        _bottleTextUI.paragraphSpacing = bottleText.paragraphSpacing;

        _bottleTextUI.margin = bottleText.margin;
        _bottleTextUI.richText = bottleText.richText;
        _bottleTextUI.raycastTarget = bottleText.raycastTarget;
    }

    public IEnumerator ToggleExtraRenderingItemsCoroutine(bool toggle)
    {
        yield return new WaitForSeconds(1f);
        _textRenderCamera.gameObject.SetActive(toggle);
        _textCanvas.gameObject.SetActive(toggle);
    }

    private void OnDestroy()
    {
        if (_renderTexture != null)
            _renderTexture.Release();
        Destroy(_renderTexture);
    }
}