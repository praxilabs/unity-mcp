using System.Collections;
using UnityEngine;
namespace LocalizationSystem
{
    public class LocalizationFileLoaderDisk : ILocalizationFileLoad
    {
        public IEnumerator LoadFile(string fileName, System.Action<string> onFileLoaded)
        {
            string filePath = "LocalizationFiles/" + fileName;

            TextAsset file = Resources.Load<TextAsset>(filePath);

            if (file == null)
            {
                Debug.LogError("File not found in Resources: " + filePath);
                onFileLoaded?.Invoke(null);
                yield break;
            }

            onFileLoaded?.Invoke(file.text);
            yield return null;
        }
    }
}