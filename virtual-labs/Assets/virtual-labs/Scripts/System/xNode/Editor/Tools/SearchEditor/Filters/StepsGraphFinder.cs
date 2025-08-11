using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class StepsGraphFinder
{
    public static List<StepsGraph> FindAllStepsGraphs(string rootFolderPath)
    {
        List<StepsGraph> foundGraphs = new List<StepsGraph>();

        // Find all asset GUIDs in the specified folder and its subfolders
        string[] assetGUIDs = AssetDatabase.FindAssets("t:StepsGraph", new[] { rootFolderPath });

        foreach (string guid in assetGUIDs)
        {
            // Get the asset path from the GUID
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Load the asset
            StepsGraph graph = AssetDatabase.LoadAssetAtPath<StepsGraph>(assetPath);

            if (graph != null)
            {
                Debug.Log($"Found StepsGraph: {graph.name} at {assetPath}");
                foundGraphs.Add(graph);
            }
        }

        if (foundGraphs.Count == 0)
        {
            Debug.LogWarning($"No StepsGraph assets found in {rootFolderPath}");
        }

        return foundGraphs;
    }

    public static string FilterGraphName(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }
}