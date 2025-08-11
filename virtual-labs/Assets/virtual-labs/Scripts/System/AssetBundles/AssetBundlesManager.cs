using UnityEngine;

/// <summary>
/// Responsible for Getting and loading the assets from the downloaded assetbundles.
/// </summary>
public class AssetBundlesManager : Singleton<AssetBundlesManager>
{
    public AssetBundleLoader assetBundleLoader;
    [Tooltip("Used to load experiment in editor")]

    [SerializeField] private ExperimentData _currentExperimentData;
    public ExperimentData CurrentExperimentData
    {
        get
        {
            if(_currentExperimentData == null)
                _currentExperimentData = LoadExperimentData();
            return _currentExperimentData;
        }
    }

    private bool _runInEditor = false;
    private AssetBundle _currentActiveAssetbundle;

    private void OnEnable()
    {
        EventsManager.Instance.AddListener(AssetBundlesEvents.DownloadedAssetBundle, AssetBundleDownloaded);
    }

    private void OnDisable()
    {
        if(EventsManager.Instance == null) return;
        EventsManager.Instance.RemoveListener(AssetBundlesEvents.DownloadedAssetBundle, AssetBundleDownloaded);
    }

    public void LoadExperimentBundle()
    {
#if UNITY_EDITOR
        _runInEditor = true;
        if (_runInEditor)
        {
            EventsManager.Instance.Invoke(AssetBundlesEvents.webRequestCompleted);
            return;
        }
#endif
        assetBundleLoader.LoadAssetBundle();
    }

    private ExperimentData LoadExperimentData()
    {
#if UNITY_EDITOR
        _runInEditor = true;
        if (_runInEditor)
            return _currentExperimentData;
#endif

        ExperimentData asset = _currentActiveAssetbundle.LoadAsset<ExperimentData>(_currentActiveAssetbundle.name);

        if (asset == null)
        {
            Debug.Log($"<color=#E4003A>Couldn't find {_currentActiveAssetbundle.name} in assetbundle {_currentActiveAssetbundle}</color>");
            return null;
        }

        return asset;
    }

    public void AssetBundleDownloaded(object downloadedBundle)
    {
        this._currentActiveAssetbundle = downloadedBundle as AssetBundle;
    }

    public void UnloadAssetBundle()
    {
        if (this._currentActiveAssetbundle != null)
            this._currentActiveAssetbundle.Unload(true);
    }
}