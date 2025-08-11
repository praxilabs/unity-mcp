public class AssetBundleLoaderMobile : AssetBundleLoader
{
    protected override string Website
    {
        get
        {
            var url = "http://52.38.63.223:8080/Testing/AssetBundlesAndroid/AssetBundlesPerExperiment/";
            return url;
        }
    }

    protected override string AssetBundlePath
    {
        get
        {
            return "/AssetBundlesAndroid/AssetBundlesPerExperiment/";
        }
    }
}