using System;
using System.Collections.Generic;
using UnityEngine;

public static class StyleHelperxNode
{
    [SerializeField] private static readonly Dictionary<int, GUIStyle> _styleCache = new();
    private static readonly Dictionary<int, Texture2D> _textureCache = new();
    private static Texture2D _cachedBoxTexture;
    private static Color _cachedBoxColor;

    public static void ClearCache()
    {
        foreach (var tex in _textureCache.Values)
            UnityEngine.Object.DestroyImmediate(tex);
        _styleCache.Clear();
        _textureCache.Clear();
    }

    public static GUIContent SetBackground(string imagePath)
    {
        return new GUIContent((Texture)Resources.Load(imagePath));
    }

    public static GUIStyle Style(float? width, float height, string backgroundHex, bool isHoverTransparent, string fontHex, RectOffset margin = null)
    {
        margin ??= new RectOffset(20, 0, 10, 0);
        int key = HashCode.Combine(
            HashCode.Combine(width, height, backgroundHex, isHoverTransparent),
            HashCode.Combine(fontHex, margin.left, margin.right, margin.top, margin.bottom)
        );

        if (_styleCache.TryGetValue(key, out var cachedStyle))
            return cachedStyle;

        var style = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fixedHeight = height,
            margin = margin,
            normal = { background = MakeTexHex(1, 1, backgroundHex), textColor = ConvertHexColor(fontHex) },
            hover = { background = isHoverTransparent ? MakeTexHex(1, 1, "#00000000") : MakeTexHex(1, 1, HoverColor(backgroundHex)), textColor = Color.white },
            active = { background = Texture2D.blackTexture, textColor = Color.white }
        };

        if (width.HasValue) style.fixedWidth = width.Value;
        else style.stretchWidth = true;

        _styleCache[key] = style;
        return style;
    }

    public static GUIStyle StyleNoBackGround(float? width, float height, string fontHex, RectOffset margin = null)
    {
        margin ??= new RectOffset(20, 0, 10, 0);
        int key = HashCode.Combine(width, height, fontHex, "noBG", margin.left, margin.right, margin.top, margin.bottom);

        if (_styleCache.TryGetValue(key, out var cachedStyle))
            return cachedStyle;

        var style = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fixedHeight = height,
            margin = margin,
            normal = { textColor = ConvertHexColor(fontHex) },
            hover = { textColor = Color.white },
            active = { textColor = Color.white }
        };

        if (width.HasValue) style.fixedWidth = width.Value;
        else style.stretchWidth = true;

        _styleCache[key] = style;
        return style;
    }

    public static GUIStyle Style(string backgroundHex, string fontHex)
    {
        int key = HashCode.Combine(backgroundHex, fontHex, "simple");

        if (_styleCache.TryGetValue(key, out var cachedStyle))
            return cachedStyle;

        GUIStyle style = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            margin = new RectOffset(20, 0, 20, 0),
            normal = { background = MakeTexHex(40, 35, backgroundHex), textColor = ConvertHexColor(fontHex) },
            hover = { background = MakeTexHex(40, 35, HoverColor(backgroundHex)), textColor = Color.white },
            active = { background = Texture2D.blackTexture, textColor = Color.white }
        };

        _styleCache[key] = style;
        return style;
    }

    public static Color ConvertHexColor(string hexColor)
    {
        return ColorUtility.TryParseHtmlString(hexColor, out var color) ? color : Color.white;
    }

    private static string HoverColor(string hexColor, float darkenFactor = 0.2f)
    {
        if (!ColorUtility.TryParseHtmlString(hexColor, out var color))
            return "#CCCCCC";

        color.r *= (1 - darkenFactor);
        color.g *= (1 - darkenFactor);
        color.b *= (1 - darkenFactor);

        return "#" + ColorUtility.ToHtmlStringRGB(color);
    }

    public static GUIStyle SetBoxStyle(GUIStyle boxStyle, Color color)
    {
        if (_cachedBoxTexture == null || _cachedBoxColor != color)
        {
            _cachedBoxColor = color;
            _cachedBoxTexture = MakeTexColor(1, 1, color);
        }

        boxStyle ??= new GUIStyle("box");
        boxStyle.normal.background = _cachedBoxTexture;
        boxStyle.padding = new RectOffset(10, 10, 10, 10);
        return boxStyle;
    }

    public static Texture2D MakeTexHex(int width, int height, string backgroundHex)
    {
        return MakeTexColor(width, height, ConvertHexColor(backgroundHex));
    }

    public static Texture2D MakeTexColor(int width, int height, Color color)
    {
        int key = HashCode.Combine(width, height, color.r, color.g, color.b, color.a);

        if (_textureCache.TryGetValue(key, out var cachedTex))
            return cachedTex;

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            hideFlags = HideFlags.HideAndDontSave
        };

        tex.SetPixel(0, 0, color);
        tex.Apply();

        _textureCache[key] = tex;

        return tex;
    }
}