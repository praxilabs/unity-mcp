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
                // Validate required parameters using ToolUtils
                var (isValidType, scriptableObjectType, typeError) = ToolUtils.ValidateRequiredString(@params, "scriptableObjectType", "scriptableObjectType");
                if (!isValidType)
                {
                    return ToolUtils.CreateErrorResponse(typeError);
                }

                var (isValidPath, assetPath, pathError) = ToolUtils.ValidateRequiredString(@params, "assetPath", "assetPath");
                if (!isValidPath)
                {
                    return ToolUtils.CreateErrorResponse(pathError);
                }

                // Get optional asset name parameter
                string assetName = null;
                if (@params.ContainsKey("assetName"))
                {
                    assetName = @params["assetName"]?.ToString();
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
                    return ToolUtils.CreateErrorResponse($"Failed to create ScriptableObject of type '{scriptableObjectType}'. Type may not exist or may not be a ScriptableObject.");
                }

                // Generate asset path with custom name if provided
                if (!string.IsNullOrEmpty(assetName))
                {
                    // Ensure the asset name has the correct extension
                    string extension = Path.GetExtension(assetPath);
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = ".asset";
                    }
                    
                    // Create new path with custom name
                    string fileName = assetName.EndsWith(extension) ? assetName : assetName + extension;
                    assetPath = Path.Combine(directory, fileName);
                }

                // Ensure unique filename
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

                // Create the asset
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Select the created asset in the Project window
                EditorGUIUtility.PingObject(asset);

                return ToolUtils.CreateSuccessResponse(
                    $"Successfully created {scriptableObjectType} at {assetPath}",
                    new { assetPath = assetPath }
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"[CreateScriptableObject] Failed to create ScriptableObject: {e}");
                return ToolUtils.CreateErrorResponse($"Failed to create ScriptableObject: {e.Message}");
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
