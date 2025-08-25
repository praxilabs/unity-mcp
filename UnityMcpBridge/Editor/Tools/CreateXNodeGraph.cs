using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public static class CreateScriptableObject
{
    public static object HandleCommand(JObject args)
    {
        // Get the ScriptableObject class name and asset path from args
        string soType = args?["scriptableObjectType"]?.ToString();
        string assetPath = args?["assetPath"]?.ToString();

        if (string.IsNullOrEmpty(soType) || string.IsNullOrEmpty(assetPath))
        {
            return new
            {
                success = false,
                error = "Missing required arguments: scriptableObjectType and assetPath"
            };
        }

        // Ensure the folder exists
        string folder = Path.GetDirectoryName(assetPath);
        if (!AssetDatabase.IsValidFolder(folder))
        {
            Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
        }

        // Create the ScriptableObject
        var so = ScriptableObject.CreateInstance(soType);
        if (so == null)
        {
            return new
            {
                success = false,
                error = $"Could not create ScriptableObject of type '{soType}'"
            };
        }

        AssetDatabase.CreateAsset(so, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return new
        {
            success = true,
            message = $"ScriptableObject '{soType}' created at {assetPath}",
            assetPath,
            timestamp = System.DateTime.Now.ToString()
        };
    }
}

