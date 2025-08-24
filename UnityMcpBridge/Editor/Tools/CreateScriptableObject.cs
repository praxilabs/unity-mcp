using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityMcpBridge.Editor.Helpers;

namespace UnityMcpBridge.Editor.Tools
{
    /// <summary>
    /// Handles creating ScriptableObject assets of various types.
    /// </summary>
    public static class CreateScriptableObject
    {
        /// <summary>
        /// Main handler for creating ScriptableObjects.
        /// </summary>
        public static object HandleCommand(JObject @params)
        {
            try
            {
                string scriptableObjectType = @params["scriptableObjectType"]?.ToString();
                string assetPath = @params["assetPath"]?.ToString();

                if (string.IsNullOrWhiteSpace(scriptableObjectType))
                {
                    return Response.Error("Required parameter 'scriptableObjectType' is missing or empty.");
                }

                if (string.IsNullOrWhiteSpace(assetPath))
                {
                    return Response.Error("Required parameter 'assetPath' is missing or empty.");
                }

                // Ensure directory exists
                string directory = Path.GetDirectoryName(assetPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create the ScriptableObject
                ScriptableObject asset = CreateScriptableObjectInstance(scriptableObjectType);
                if (asset == null)
                {
                    return Response.Error($"Failed to create ScriptableObject of type '{scriptableObjectType}'. Type may not exist or may not be a ScriptableObject.");
                }

                // Ensure unique filename
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                // Create the asset
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Select the created asset in the Project window
                EditorGUIUtility.PingObject(asset);

                return new
                {
                    success = true,
                    message = $"Successfully created {scriptableObjectType} at {assetPath}",
                    assetPath = assetPath,
                    timestamp = System.DateTime.Now.ToString()
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"[CreateScriptableObject] Failed to create ScriptableObject: {e}");
                return Response.Error($"Failed to create ScriptableObject: {e.Message}");
            }
        }

        /// <summary>
        /// Creates a ScriptableObject instance of the specified type.
        /// </summary>
        private static ScriptableObject CreateScriptableObjectInstance(string typeName)
        {
            try
            {
                // Get the type from the assembly
                Type type = Type.GetType(typeName);
                if (type == null)
                {
                    // Try to find the type in all loaded assemblies
                    foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                    {
                        type = assembly.GetType(typeName);
                        if (type != null) break;
                    }
                }

                if (type == null)
                {
                    Debug.LogError($"[CreateScriptableObject] Type '{typeName}' not found in any loaded assembly.");
                    return null;
                }

                if (!typeof(ScriptableObject).IsAssignableFrom(type))
                {
                    Debug.LogError($"[CreateScriptableObject] Type '{typeName}' is not a ScriptableObject.");
                    return null;
                }

                // Create instance
                return ScriptableObject.CreateInstance(type);
            }
            catch (Exception e)
            {
                Debug.LogError($"[CreateScriptableObject] Error creating instance of type '{typeName}': {e}");
                return null;
            }
        }
    }
}
