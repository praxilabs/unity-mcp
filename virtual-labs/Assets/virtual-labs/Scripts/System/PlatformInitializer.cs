using Praxilabs.CameraSystem;

namespace Praxilabs
{
    /// <summary>Class responsible for setting instances of managers depending on current platform</summary>
    public class PlatformInitializer : Singleton<PlatformInitializer>
    {
        protected override void Awake()
        {
            _addToDontDestroyOnLoad = false;
            base.Awake();
        }

        public void CheckLabManagersPlatform()
        {
#if UNITY_ANDROID || UNITY_IOS
            CameraManager.Instance.cameraInstance = new CameraInstanceMobile();
#elif UNITY_WEBGL || UNITY_STANDALONE_WIN
            CameraManager.Instance.cameraInstance = new CameraInstanceWebGL();
#endif
        }

        private void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            AssetBundlesManager.Instance.assetBundleLoader = new AssetBundleLoaderMobile();
#elif UNITY_WEBGL || UNITY_STANDALONE_WIN
            AssetBundlesManager.Instance.assetBundleLoader = new AssetBundleLoaderWebGL();
#endif

            GetAssetBundle();
        }

        private void GetAssetBundle()
        {
            AssetBundlesManager.Instance.LoadExperimentBundle();
        }
    }
}
