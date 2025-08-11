using UnityEngine;

public static class TintColorApplier
{
    private static MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();

    public static void SetTint(Renderer renderer, Color tintColor, bool multiplyColor = false)
    {
        if(renderer == null)
            return;

        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();

        int materialCount = renderer.sharedMaterials.Length;

        for (int i = 0; i < materialCount; i++)
        {
            renderer.GetPropertyBlock(_propertyBlock, i);

            Color baseColor = renderer.sharedMaterials[i].GetColor("_BaseColor");
            Color tintedColor = multiplyColor ? (baseColor * tintColor) : (baseColor + tintColor) ;

            _propertyBlock.SetColor("_BaseColor", tintedColor);

            renderer.SetPropertyBlock(_propertyBlock, i);
        }
    }

    public static void ClearTint(Renderer renderer)
    {
        if(renderer == null)
            return;

        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();

        int materialCount = renderer.sharedMaterials.Length;

        for (int i = 0; i < materialCount; i++)
        {
            renderer.GetPropertyBlock(_propertyBlock, i);

            _propertyBlock.Clear();

            renderer.SetPropertyBlock(_propertyBlock, i);
        }
    }

}
