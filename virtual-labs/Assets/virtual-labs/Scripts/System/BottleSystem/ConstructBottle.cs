using System;
using TMPro;
using UnityEngine;

public class ConstructBottle : MonoBehaviour
{
    [SerializeField] private GameObject _bottlePrefab;
    [SerializeField] private BottleInfo _bottleInfo;
    [SerializeField] private BottleCap _bottleCap;

    private Bottle _bottle;
    private GameObject _bottleObject;
    private BottleText _bottleText;

    private void Start()
    {
        CreateBottle();
        Init();

        if (_bottleCap != null)
            _bottleCap.Initialize(_bottle.gameObject, _bottle.bottleCap, _bottle.bottleBody);
        UpdateBottleInfo();
        SetBottleText();
        StartCoroutine(_bottleText.ToggleExtraRenderingItemsCoroutine(false));
    }

    private void Init()
    {
        _bottleText = _bottleObject.GetComponent<BottleText>();
        _bottle = _bottleObject.GetComponent<Bottle>();
    }

    private void CreateBottle()
    {
        _bottleObject = Instantiate(_bottlePrefab);
        _bottleObject.transform.parent = this.transform;
        _bottleObject.transform.localPosition = Vector3.zero;
        _bottleObject.transform.localRotation = Quaternion.identity;
    }

    private void UpdateBottleInfo()
    {
        _bottleObject.GetComponent<BottleColors>().SetColors(_bottleInfo);
    }

    private void SetBottleText()
    {
        _bottleText.ConstructText(_bottleInfo.bottleText);
    }
}

[Serializable]
public class BottleInfo
{
    public Color capColor;
    public Color bottleColor;

    public Color labelBackgroundColor;
    public Color liquidColor;
    public BottleTextInfo bottleText;
}

[Serializable]
public class BottleTextInfo
{
    [Header("Basic Text")]
    [TextArea(3, 10)] public string text = "Sample Text";
    public TMP_FontAsset font;
    public float fontSize = 36f;
    public Color color = Color.white;

    [Header("Font Style")]
    public FontStyles fontStyle = FontStyles.Normal;
    public bool enableAutoSizing = false;
    public float minFontSize = 18f;
    public float maxFontSize = 72f;

    [Header("Alignment")]
    public TextAlignmentOptions alignment = TextAlignmentOptions.Center;

    [Header("Wrapping & Overflow")]
    public bool enableWordWrapping = true;
    public TextOverflowModes overflowMode = TextOverflowModes.Overflow;

    [Header("Spacing")]
    public float characterSpacing = 0f;
    public float wordSpacing = 0f;
    public float lineSpacing = 0f;
    public float paragraphSpacing = 0f;

    [Header("Margins")]
    public Vector4 margin = Vector4.zero;

    [Header("Extra")]
    public bool richText = true;
    public bool raycastTarget = false;
}