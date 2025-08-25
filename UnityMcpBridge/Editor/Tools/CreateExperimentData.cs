using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public static class CreateExperimentData
{
    public static object HandleCommand(JObject args)
    {
        // Default asset name and folder
        string assetName = args?["assetName"]?.ToString() ?? "NewExperimentData";
        string folder = args?["folder"]?.ToString() ?? "Assets/Testing/ExperimentData/";

        // Ensure the folder exists
        if (!AssetDatabase.IsValidFolder(folder))
        {
            Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
        }

        // Create the ExperimentData ScriptableObject
        var experimentData = ScriptableObject.CreateInstance("ExperimentData");
        string assetPath = Path.Combine(folder, assetName + ".asset");
        AssetDatabase.CreateAsset(experimentData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return new
        {
            success = true,
            message = $"Experiment Data created at {assetPath}",
            assetPath,
            timestamp = System.DateTime.Now.ToString()
        };
    }
}
