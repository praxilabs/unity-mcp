using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// View assets in bundle at {bundlePath}
/// </summary>
public class AssetBundleViewer : MonoBehaviour
{
    // The path to the AssetBundle (set this manually or via inspector)
    public string bundlePath;
    public List<Object> loadedAssets = new List<Object>();
    public bool loadBundle;

    [HideInInspector] public AssetBundle loadedBundle;

    private void OnValidate()
    {
        if (loadBundle && !string.IsNullOrEmpty(bundlePath))
        {
            LoadAssetBundle();
            loadBundle = false;
        }
    }

    public void LoadAssetBundle()
    {
        if (loadedBundle != null)
            loadedBundle.Unload(true);

        loadedBundle = AssetBundle.LoadFromFile(bundlePath);

        if (loadedBundle == null)
        {
            Debug.Log($"<color=#E4003A>>Failed to load AssetBundle from path: {bundlePath}</color>");
            return;
        }

        loadedAssets.Clear();

        Object[] assets = loadedBundle.LoadAllAssets();

        foreach (var asset in assets)
            loadedAssets.Add(asset);

        Debug.Log($"Loaded {assets.Length} assets from AssetBundle at {bundlePath}");
    }

    private void OnDestroy()
    {
        if (loadedBundle != null)
            loadedBundle.Unload(true);
    }
}