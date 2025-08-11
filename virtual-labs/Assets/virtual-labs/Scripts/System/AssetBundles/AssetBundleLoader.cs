using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles the download of the asset bundles.
/// </summary>
public abstract class AssetBundleLoader
{
    protected virtual string Website
    {
        get { return ""; }
    }

    protected virtual string AssetBundlePath
    {
        get { return ""; }
    }

    public void LoadAssetBundle()
    {
        string expName = GetExpNameFromURL.GetExpFromURL(Application.absoluteURL);

        Debug.Log("Getting asset bundles with uri: " + this.Website + expName);
        AssetBundlesCoroutineManager.Instance.StartCoroutine(DownloadBundle(expName));
    }

    /// <summary>
    /// Download a bundle with the given uri.
    /// </summary>
    private IEnumerator DownloadBundle(string uri)
    {
        UnityWebRequest WebRequest = UnityWebRequestAssetBundle.GetAssetBundle(Website + uri);
        WebRequest.SendWebRequest();

        AssetBundle bundle = null;

        while (!WebRequest.downloadHandler.isDone)
        {
            yield return null;
        }

        if (WebRequest.result == UnityWebRequest.Result.ConnectionError ||
            WebRequest.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(WebRequest.error);
        else
            bundle = DownloadHandlerAssetBundle.GetContent(WebRequest);

        Debug.Log($"Downloaded bundle with name: {bundle.name}");
        if (bundle != null)
        {
            EventsManager.Instance.Invoke(AssetBundlesEvents.DownloadedAssetBundle, bundle);
            EventsManager.Instance.Invoke(AssetBundlesEvents.webRequestCompleted);
        }
        else
            Debug.Log(WebRequest.error);
    }
}