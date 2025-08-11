using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
namespace LocalizationSystem
{
    public class LocalizationFileLoaderRemote : ILocalizationFileLoad
    {
        public string baseURL = "";

        public void SetBaseURL()
        {
            // Base URL setup for different platforms
            //baseURL = "https://testing.praxilabs-lms.com/api/";
            baseURL = "https://dev.praxilabs-lms.com/Testing/PraxiLabsVirtual/NewLab_LocalizationFiles/";

#if UNITY_WEBGL && !UNITY_EDITOR
        //baseURL = GetBaseURL() ;
#endif
        }

        public IEnumerator LoadFile(string path, System.Action<string> onFileLoaded)
        {
            SetBaseURL();

            string url = baseURL + path;
            using UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError("Failed to fetch remote file: " + request.error);
                onFileLoaded(null);
                yield break;
            }

            onFileLoaded(request.downloadHandler.text);
        }
    }
}