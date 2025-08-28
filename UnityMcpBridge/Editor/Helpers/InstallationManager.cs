using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace UnityMcpBridge.Editor.Helpers
{
    public static class InstallationManager
    {
        private const string InstallationKey = "UnityMcpBridge_ServerInstalled";
        
        public static event Action OnInstallationCompleted;
        public static event Action OnInstallationStarted;

        public static bool IsServerInstalled
        {
            get
            {
                return EditorPrefs.GetBool(InstallationKey, false);
            }
            set
            {
                EditorPrefs.SetBool(InstallationKey, value);
            }
        }
        
        public static async void StartInstallation()
        {          
            try
            {
                OnInstallationStarted?.Invoke();
                
                // Wait for editor to stop compiling before proceeding
                await WaitForEditorCompilation();
                
                ServerInstaller.EnsureServerInstalled();
                IsServerInstalled = true;
                OnInstallationCompleted?.Invoke();
                Debug.Log("Server installation completed successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Server installation failed: {ex.Message}");
                IsServerInstalled = false;
            }
        }
        
        private static async Task WaitForEditorCompilation()
        {
            while (EditorApplication.isCompiling)
            {
                Debug.Log("Waiting for Unity editor to finish compiling...");
                await Task.Delay(1000); // Wait 1 second before checking again
            }
            Debug.Log("Unity editor compilation completed, proceeding with installation...");
        }
        
        public static void ResetInstallationStatus()
        {
            IsServerInstalled = false;
            Debug.Log("Installation status reset.");
        }
    }
}
