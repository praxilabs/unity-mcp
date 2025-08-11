using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace System.AssetBundles.Editor.xNode
{
    public class CreateXNodeAssetBundle
    {
        [MenuItem("Assets/Build AssetBundle")]
        static void BuildExperimentBundle()
        {
            ScriptableObject selectedObject = Selection.activeObject as ScriptableObject;

            BuildExperimentBundle(selectedObject, EditorUserBuildSettings.activeBuildTarget);
        }

        private static void BuildExperimentBundle(ScriptableObject selectedObject, BuildTarget buildTarget)
        {
            Debug.Log($"<color=#36C2CE>Building Asset for {selectedObject.name}</color>");

            if (selectedObject == null)
            {
                Debug.Log("<color=#E4003A> Selected object is not an ExperimentData.</color>");
                return;
            }

            string path = EditorUtility.SaveFolderPanel("Choose Location for AssetBundle", "", "");
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("No folder selected.");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(selectedObject);

            // Get all dependencies of the selected asset (excluding scripts)
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);

            // Use a HashSet to track unique assets and avoid duplicates
            HashSet<string> uniqueAssets = new HashSet<string>();

            // Add the selected asset itself first
            uniqueAssets.Add(assetPath);

            // Add all non-script dependencies, filtering out .cs files and avoiding duplicates
            foreach (var dependency in dependencies)
            {
                if (!dependency.EndsWith(".cs") && uniqueAssets.Add(dependency))
                {
                    // The asset is added only if it's not a script and wasn't already in the set
                }
            }

            // Convert the HashSet to an array for AssetBundle creation
            string[] allAssetPaths = new string[uniqueAssets.Count];
            uniqueAssets.CopyTo(allAssetPaths);

            // Define the build settings for the AssetBundle
            AssetBundleBuild[] bundleBuild = new AssetBundleBuild[1];
            bundleBuild[0].assetBundleName = selectedObject.name.ToLower(); // Use ScriptableObject name as the bundle name
            bundleBuild[0].assetNames = allAssetPaths; // Include the main asset and its unique dependencies

            // Build the AssetBundle
            BuildPipeline.BuildAssetBundles(path, bundleBuild, BuildAssetBundleOptions.None, buildTarget);

            Debug.Log($"AssetBundle '{bundleBuild[0].assetBundleName}' has been built successfully at {path}");
        }
    }
}