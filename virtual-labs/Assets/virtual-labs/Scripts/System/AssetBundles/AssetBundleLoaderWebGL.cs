using UnityEngine;

public class AssetBundleLoaderWebGL : AssetBundleLoader
{
    protected override string Website
    {
        get
        {
             var url = Application.absoluteURL;

            // Absolute url (with experiment name)
            url = url.Substring(0, url.LastIndexOf("/")); // Build Path
            url = url.Substring(0, url.LastIndexOf("/")); // Builds Path
            url = url.Substring(0, url.LastIndexOf("/")); // Root Path
            url = url + AssetBundlePath;
            return url;
        }
    }

    protected override string AssetBundlePath
    {
        get
        {
            return "/AssetBundles/AssetBundlesPerExperiment/";
        }
    }
}
