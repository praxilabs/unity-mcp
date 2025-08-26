using System;
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
            private set
            {
                EditorPrefs.SetBool(InstallationKey, value);
            }
        }
        
        public static void StartInstallation()
        {
            if (IsServerInstalled)
            {
                Debug.Log("Server is already installed.");
                return;
            }
            
            Debug.Log("Starting server installation...");
            
            try
            {
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
        
        public static void UninstallServer()
        {
            if (!IsServerInstalled)
            {
                Debug.Log("Server is not installed.");
                return;
            }
            
            Debug.Log("Uninstalling server...");
            
            try
            {
                string saveLocation = ServerInstaller.GetSaveLocation();
                if (System.IO.Directory.Exists(saveLocation))
                {
                    System.IO.Directory.Delete(saveLocation, true);
                }
                
            }
            catch
            {
            
            }
             
                IsServerInstalled = false;
                Debug.Log("Server uninstalled successfully.");
        }
        
        public static void ResetInstallationStatus()
        {
            IsServerInstalled = false;
            Debug.Log("Installation status reset.");
        }
    }
}
