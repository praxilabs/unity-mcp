using System.Collections;

namespace LocalizationSystem
{
    public interface ILocalizationFileLoad
    {
        IEnumerator LoadFile(string path, System.Action<string> onFileLoaded);
    }
}