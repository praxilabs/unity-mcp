using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class HintsJsonParser
{
    public HintsJsonData ParseHintsJson(string jsonString)
    {
        Dictionary<int, HintsList> hintsDictionary = JsonConvert.DeserializeObject<Dictionary<int, HintsList>>(jsonString);
        HintsJsonData hintsJson = new();
        hintsJson.MapDictionaryToList(hintsDictionary);

        return hintsJson;
    }
}
